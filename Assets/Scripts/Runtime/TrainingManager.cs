using System.Collections.Generic;
using Default;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System.IO;
using Newtonsoft.Json;
using DavidJalbert;

public class TrainingManager : MonoBehaviour
{
    public static TrainingManager Instance;

    [SerializeField] private LayerMask roadMask;
    [SerializeField] private NeuralNetworkData data;
    [SerializeField] private TinyCarCamera activeCamera;
    [SerializeField] private TinyCarController playerController;

    [Header("UI")]
    [SerializeField] private Button startEvolutionButton;
    [SerializeField] private TMP_Text evolutionCycleText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text fitnessTextAllTime;
    [SerializeField] private TMP_Text fitnessTextCurrent;
    [SerializeField] private TMP_Text noImprovementCounterText;

    [Header("Race Track")]
    public RaceTrack activeRaceTrack;

    [Header("Config - General"), Space]
    [SerializeField, Range(0.5f, 4.5f)] private float timeScale = 4.5f;
    [SerializeField] private int[] networkHiddenLayerStructure_Evolution;
    [SerializeField] private int[] networkHiddenLayerStructure_Training;

    [Header("Config - AI General"), Space]
    [SerializeField, Tooltip("How much facing the wrong direction impacts the fitness.")] private float directionIndicatorFitnessPenaltyWeight = 5f;

    [Header("Config - Backpropagation Training (work in progress)"), Space]
    [SerializeField] private bool startWithRandomizedNetworks;
    [SerializeField] private bool saveConvergedModels;
    [SerializeField] private bool saveEvaluationDataTraining;
    [SerializeField] private bool trainWithLocalBuffer;
    [SerializeField] private bool saveLocalBufferToData;
    [SerializeField] private int trainingRepetitions = 10;
    [Tooltip("Set the range for the learning rate which is spread among the active agents.\nIf only one instance exists, max will be used.")]
    [SerializeField] private Vector2 learningRateMinMax = new(0.2f, 0.6f);
    [SerializeField] private float weightDecay = 0.005f;

    [Header("Config - Genetic Engineering"), Space]
    [SerializeField] private bool saveEvaluationDataEvolution;
    [SerializeField] private bool startWithNewNeuralNetworks;
    [SerializeField] private bool saveFittestNetworks;
    [SerializeField] private int epochsCount = 100;
    [SerializeField] private int evolutionCycles = 1;
    [SerializeField] private int layerLengthAdditionPerCycle = 1;
    [SerializeField] private int hidderLayerAdditionIndex = 0;
    [SerializeField, Tooltip("Mutation strength based on how near the agent is to the target.")] private AnimationCurve relativeMutationRate;
    [SerializeField] private bool greedy; // enables resetting all the agents to the fittest agent after a set of epochs with no improvement, also scaling the mutation strength for every no-improvement-epoch
    [SerializeField] private int numberOfEpochsWithoutImprovementUntilReset = 6;
    [SerializeField, TextArea, Tooltip("Text snippet added at the top of the eval file!")] private string additionalNotes;

    public bool IsTrainingActive { get; private set; }
    public bool IsEvolutionActive { get; private set; }
    public bool TrainWithLocalBuffer => trainWithLocalBuffer;
    public bool SaveLocalBufferToData => saveLocalBufferToData;
    public LayerMask RoadMask => roadMask;

    private List<AICarController> activeAgents;
    private EvolutionEvaluationData evolutionEvaluationData;

    private int numberOfEpochsWithoutImprovementCounter;
    private float evolutionTimer;

    private const string EVOLUTION_EVALTEXT_PATH = "/Evaluation";
    private const float CRASH_PENALTY = 0f;
    private const float FITNESS_DISTANCE_SCALE = 100f;
    private const float FITNESS_SPEED_SCALE = 3f;
    private const float FINISH_REWARD = 0;
    private const float MAX_MUTATION_STRENGTH_DIVIDER = 7f;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        var enumerated = FindObjectsByType<AICarController>(FindObjectsSortMode.None).Where(c => !c.isPlayerController);

        activeAgents = enumerated.ToList<AICarController>();
    }

    private void LateUpdate()
    {
        Time.timeScale = timeScale;

        if (IsEvolutionActive)
        {
            evolutionTimer += Time.deltaTime;
        }
    }

    public async UniTask WaitUntilAllAgentsReset()
    {
        var allReset = false;

        while (allReset == false)
        {
            await UniTask.Delay(500, false, PlayerLoopTiming.FixedUpdate, gameObject.GetCancellationTokenOnDestroy()); // check every 0.5 seconds

            allReset = AreAllAgentsReset();
        }
    }

    public bool AreAllAgentsReset()
    {
        var allReset = true;

        // if one agent is still driving, return false 
        foreach (var agent in activeAgents)
        {
            if (agent.AIDrivingEnabled)
            {
                allReset = false;
                continue;
            }
        }

        return allReset;
    }

    private float EvaluateFitness(AICarController agentToEvaluate, float completedPercentage, float finishTime, bool hasCrashed)
    {
        var fitness = 0f;
        var hasFinished = finishTime >= 0.5f;

        var rightIndicator = 1f - agentToEvaluate.NNInputBuffer[^1];
        var leftIndicator = 1f - agentToEvaluate.NNInputBuffer[^2];
        fitness -= (rightIndicator + leftIndicator) * directionIndicatorFitnessPenaltyWeight;

        fitness += completedPercentage * FITNESS_DISTANCE_SCALE;
        fitness += hasCrashed ? -CRASH_PENALTY : 0f;

        if (hasFinished)
        {
            fitness += FINISH_REWARD;
            fitness += (agentToEvaluate.InitialDistance / finishTime) * FITNESS_SPEED_SCALE; // evaluate average speed of the agent
        }

        return fitness;
    }

    #region Training

    private void InitializeTraining()
    {
        for (int i = 0; i < activeAgents.Count; i++)
        {
            if (startWithRandomizedNetworks)
            {
                activeAgents[i].SetAI(networkHiddenLayerStructure_Training);
            }
            else
            {
                activeAgents[i].SetAI(data.fittestNetworkData_Training);
            }
        }
    }

    public async UniTask TrainAgents(List<Dataset> trainingData)
    {
        if (IsTrainingActive) { return; } // if current training process is not finished 

        var currentLearningRates = new float[activeAgents.Count];
        var evalutationData = new TrainingEvaluationData();

        IsTrainingActive = true;

        InitializeTraining();

        // set consistent learning rates for each agent
        for (int i = 0; i < activeAgents.Count; i++)
        {
            if (learningRateMinMax.y - learningRateMinMax.x <= 0.05f) // avoid division by zero if min >= max
            {
                currentLearningRates[i] = learningRateMinMax.x;
            }
            else
            {
                currentLearningRates[i] = learningRateMinMax.x + ((learningRateMinMax.y - learningRateMinMax.x) / (float)activeAgents.Count) * (i + 1);
            }

            activeAgents[i].SetUIHeader($"{currentLearningRates[i]}");

            // initialize evaluationData with correct learning rates
            evalutationData.Evaluations.Add(new TrainingEvaluation(currentLearningRates[i]));
        }

        // training repetitions
        for (int i = 0; i < trainingRepetitions; i++)
        {
            statusText.text = $"Training {i}";

            for (int j = 0; j < activeAgents.Count; j++)
            {
                var agent = activeAgents[j];

                agent.SetPositionAndRotation(activeRaceTrack.spawn.position, activeRaceTrack.spawn.rotation).Forget();

                await agent.AI.Train(trainingData, currentLearningRates[j], weightDecay);
            }

            foreach (var agent in activeAgents)
            {
                agent.SetTarget(activeRaceTrack.GetRandomTargetAndActivateIt().position).Forget();
                agent.EnableDrivingAI(true);
            }

            statusText.text = $"Evaluating {i}";

            await WaitUntilAllAgentsReset();

            for (int j = 0; j < activeAgents.Count; j++)
            {
                var agent = activeAgents[j];
                var fitness = EvaluateFitness(agent, agent.GetCompletionPercentage(), agent.GetLastFinishTime(), agent.HasCrashedLastRun());

                evalutationData.Evaluations[j].FitnessConvergence.Add(fitness);

                agent.AI.SetFitness(fitness);
            }
        }

        // save evaluation data
        if (saveEvaluationDataTraining)
        {
            var contents = JsonConvert.SerializeObject(evalutationData);
            var path = $"{Application.dataPath}/Evaluation/BackPropEval.json";

            File.WriteAllText(path, contents);
        }

        IsTrainingActive = false;
    }

    #endregion

    #region Evolution

    private void InitializeEvolution(int evolutionCycle)
    {
        statusText.text = "Initializing...";

        if (startWithNewNeuralNetworks)
        {
            foreach (var agent in activeAgents)
            {
                var layers = new List<int>();

                layers.Add(AICarController.INPUT_NEURONS);

                for (int i = 0; i < networkHiddenLayerStructure_Evolution.Length; i++)
                {
                    var layer = networkHiddenLayerStructure_Evolution[i];
                    layer += i == hidderLayerAdditionIndex ? (evolutionCycle * layerLengthAdditionPerCycle) : 0;

                    layers.Add(layer);
                }

                layers.Add(AICarController.OUTPUT_NEURONS);

                agent.SetAI(new NeuralNetwork(layers.ToArray(), true));
            }
        }
        else
        {
            foreach (var agent in activeAgents)
            {
                agent.SetAI(new NeuralNetwork(data.fittestNetworkData_Evolution));
                // agent.AI.SetFitness(0f);
            }
        }
    }

    public async void StartEvolution()
    {
        evolutionEvaluationData = new();
        IsEvolutionActive = true;

        for (int i = 0; i < evolutionCycles; i++)
        {
            evolutionCycleText.text = $"Evolution Cycle: {i}";

            InitializeEvolution(i);

            await Evolution(epochsCount, i);
        }

        if (saveEvaluationDataEvolution)
        {
            evolutionEvaluationData._info = $"A total of {evolutionCycles} cycles, each trained for {epochsCount} epochs, with a total of {activeAgents.Count} agents, for a total of {evolutionTimer / 60f} minutes.";
            evolutionEvaluationData._additionalNotes = additionalNotes;

            var hiddenLayerStructureName = "";

            foreach (var neurons in networkHiddenLayerStructure_Evolution)
            {
                hiddenLayerStructureName += $"{neurons}_";
            }

            hiddenLayerStructureName += $"Add{layerLengthAdditionPerCycle}For{evolutionCycles}";

            File.AppendAllText($"{Application.dataPath}{EVOLUTION_EVALTEXT_PATH}/Eval_{hiddenLayerStructureName}.json", $"{JsonConvert.SerializeObject(evolutionEvaluationData)}");
        }

        startEvolutionButton.gameObject.SetActive(true);
        statusText.text = "Finished!";
        IsEvolutionActive = false;
    }

    private void GeneticEngineering(NeuralNetwork[] fittestNetworks, float fitnessDifference, float highestCompletionPercentage)
    {
        var j = 0;
        var highestCompletionPercentageInverted = relativeMutationRate.Evaluate(highestCompletionPercentage);
        var noImprovementMutationStrengthDivider = Mathf.Clamp(numberOfEpochsWithoutImprovementCounter, 1f, 1f + (highestCompletionPercentage * MAX_MUTATION_STRENGTH_DIVIDER)); // less mutation the further the agents get

        if (fitnessDifference < 0f) // => no improvement from last generation => mutate amount based on how many epochs went by without improvement
        {
            foreach (var agent in activeAgents)
            {
                if (j < (activeAgents.Count / 7))
                {
                    // elitism 
                    agent.SetAI(fittestNetworks[0]);
                    agent.SetUIHeader("Elite");
                }
                else if (j < (activeAgents.Count / 3))
                {
                    // mutation
                    agent.SetAI(fittestNetworks[0]);
                    agent.AI.Mutate(highestCompletionPercentageInverted);
                    agent.SetUIHeader("Mutation");
                }
                else
                {
                    // mutation
                    agent.SetAI(fittestNetworks[Random.Range(0, fittestNetworks.Length)]);
                    agent.AI.Mutate(highestCompletionPercentageInverted / noImprovementMutationStrengthDivider);
                    agent.SetUIHeader("Controlled Mutation");
                }

                j++;
            }
        }
        else if (fitnessDifference < 5f) // => minor improvement => try to preserve most of the superior genes
        {
            foreach (var agent in activeAgents)
            {
                if (j < (activeAgents.Count / 7))
                {
                    // elitism
                    agent.SetAI(fittestNetworks[0]);
                    agent.SetUIHeader("Elite");
                }
                else if (j < (activeAgents.Count / 2))
                {
                    // mutation
                    agent.SetAI(fittestNetworks[Random.Range(0, fittestNetworks.Length)]);
                    agent.AI.Mutate(highestCompletionPercentageInverted);
                    agent.SetUIHeader("Mutation");
                }
                else
                {
                    // elitism & mutation
                    agent.SetAI(fittestNetworks[0]);
                    agent.AI.Mutate(highestCompletionPercentageInverted); // minimal mutation
                    agent.SetUIHeader("Mutation");
                }

                j++;
            }
        }
        else // => major improvement, spread the superior genes among all agents
        {
            foreach (var agent in activeAgents)
            {

                if (j < (activeAgents.Count / 6))
                {
                    // preserve some diversity
                    agent.SetAI(fittestNetworks[Random.Range(0, fittestNetworks.Length)]);
                    agent.AI.Mutate(0.2f);
                    agent.SetUIHeader("Diversity");
                }
                else if (j < (activeAgents.Count / 3))
                {
                    // set the basis for the next generation
                    agent.SetAI(fittestNetworks[0]);
                    agent.AI.Mutate(highestCompletionPercentageInverted);
                    agent.SetUIHeader("Mutation");
                }
                else
                {
                    // elitism
                    agent.SetAI(fittestNetworks[0]);
                    agent.SetUIHeader("Elite");
                }

                j++;
            }
        }
    }

    private async UniTask Evolution(int epochs, int currentCycle)
    {
        var highestFitness = 0f;
        var currentHighestFitness = 0f;
        var fitnessDifference = 0f;
        var highestCompletionPercentage = 0f;
        numberOfEpochsWithoutImprovementCounter = 0;

        NeuralNetwork[] fittestNetworks = new NeuralNetwork[3];

        var eval = new EvolutionEvaluation
        {
            LayerStructure = activeAgents[0].AI.layers.ToList()
        };

        for (int i = 0; i < epochs; i++)
        {
            statusText.text = $"[Epoch {i}] Applying genetic engineering...";

            if (i > 0) // if not first epoch
            {
                GeneticEngineering(fittestNetworks, fitnessDifference, highestCompletionPercentage);
            }

            statusText.text = $"[Epoch {i}] Evolution with {activeAgents.Count} active agents...";

            foreach (var agent in activeAgents)
            {
                await agent.SetPositionAndRotation(activeRaceTrack.spawn.position, activeRaceTrack.spawn.rotation);
                agent.SetTarget(activeRaceTrack.GetRandomTargetAndActivateIt().position).Forget();
                agent.EnableDrivingAI(true);
            }

            var allReset = false;

            // wait until all the agents have crashed or finished
            while (!allReset)
            {
                var furthestDistance = 0f;
                Transform furthestAgent = playerController.transform;

                foreach (var agent in activeAgents)
                {
                    var agentDistance = agent.GetCompletionPercentage();

                    if (agentDistance > furthestDistance)
                    {
                        furthestDistance = agentDistance;
                        furthestAgent = agent.transform;
                    }
                }

                activeCamera.whatToFollow = furthestAgent;

                await UniTask.Delay(500, true, PlayerLoopTiming.FixedUpdate, gameObject.GetCancellationTokenOnDestroy());
                allReset = AreAllAgentsReset();
            }

            statusText.text = $"[Epoch {i}] Evaluating...";

            // set the fitness based game environment
            foreach (var agent in activeAgents)
            {
                var fitness = EvaluateFitness(agent, agent.GetCompletionPercentage(), agent.GetLastFinishTime(), agent.HasCrashedLastRun());

                agent.AI.SetFitness(fitness);
            }

            activeAgents.Sort((agent1, agent2) => agent2.AI.CompareTo(agent1.AI)); // sort by fitness descending

            currentHighestFitness = activeAgents[0].AI.GetFitness();
            highestCompletionPercentage = activeAgents[0].GetCompletionPercentage();

            fitnessDifference = currentHighestFitness - highestFitness;

            // if the current epoch has brought forth stronger individuals, save the evolved networks and repeat engineering and counting
            if (fitnessDifference > 0f)
            {
                highestFitness = currentHighestFitness;
                numberOfEpochsWithoutImprovementCounter = 0;

                for (int j = 0; j < fittestNetworks.Length; j++)
                {
                    fittestNetworks[j] = new NeuralNetwork(activeAgents[j].AI);
                }

                if (saveFittestNetworks) { data.StoreNetwork(fittestNetworks[0], true); }

                noImprovementCounterText.text = $"[Epoch {i}] Epochs without improvement: {numberOfEpochsWithoutImprovementCounter}";
                fitnessTextAllTime.text = $"[Epoch {i}] All-time highest fitness: {highestFitness}";
            }
            else // if not, repeat with the best generation up to this point (fittestNetworks), but if no improvement is made after a set of epochs, reset all the networks to the fittest network
            {
                numberOfEpochsWithoutImprovementCounter++;

                // reset logic
                if (numberOfEpochsWithoutImprovementCounter >= numberOfEpochsWithoutImprovementUntilReset && greedy)
                {
                    numberOfEpochsWithoutImprovementCounter = 0;
                    fitnessDifference = 50f; // influence the genetic engineering method to create an epoch of only fit agents, but leave highest fitness the same

                    for (int j = 1; j < fittestNetworks.Length; j++)
                    {
                        fittestNetworks[j] = new NeuralNetwork(fittestNetworks[0]);
                    }
                }

                noImprovementCounterText.text = $"[Epoch {i}] Epochs without improvement: {numberOfEpochsWithoutImprovementCounter}" + (numberOfEpochsWithoutImprovementCounter <= 0 ? ", has been Reset" : "");
            }

            eval.FitnessConvergence.Add(currentHighestFitness);

            fitnessTextCurrent.text = $"[Epoch {i}] Highest Fitness: {highestFitness} (Difference of {fitnessDifference})";
        }

        // save the fittest network of this evaluation löcycle
        eval.fittestNetwork = new SerializedNetworkData(fittestNetworks[0]);

        evolutionEvaluationData.Evaluations.Add(eval);
    }

    #endregion
}
