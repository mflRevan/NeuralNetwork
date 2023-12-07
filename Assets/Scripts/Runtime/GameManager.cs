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

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private LayerMask wallMask;
    [SerializeField] private NeuralNetworkData data;
    [SerializeField] private Button startEvolutionButton;
    [SerializeField] private TinyCarCamera activeCamera;
    [SerializeField] private TinyCarController playerController;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text fitnessText;
    public Transform target;
    public Transform spawn;

    [Header("Config - General"), Space]
    [SerializeField, Range(0.5f, 6f)] private float timeScale = 6f;
    [SerializeField] private int[] networkHiddenLayerStructure;

    [Header("Config - Backpropagation Training"), Space]
    [SerializeField] private bool startWithRandomizedNetworks;
    [SerializeField] private bool saveConvergedModels;
    [SerializeField] private bool saveEvaluationData;
    [SerializeField] private bool trainWithLocalBuffer;
    [SerializeField] private bool saveLocalBufferToData;
    [SerializeField] private int trainingRepetitions = 10;
    [Tooltip("Set the range for the learning rate which is spread among the active agents.\nIf only one instance exists, max will be used.")]
    [SerializeField] private Vector2 learningRateMinMax = new Vector2(0.2f, 0.6f);
    [SerializeField] private float weightDecay = 0.005f;

    [Header("Config - Genetic Engineering"), Space]
    [SerializeField] private bool startWithNewNeuralNetwork;
    [SerializeField] private bool saveFittestNetworks;
    [SerializeField] private int epochsCount;

    public bool TrainWithLocalBuffer => trainWithLocalBuffer;
    public bool SaveLocalBufferToData => saveLocalBufferToData;
    public LayerMask WallMask => wallMask;

    private List<AICarController> activeAgents;
    private bool isTraining;

    private const float FITNESS_SCALE = 100f;


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

    #region Training

    private void InitializeTraining()
    {
        for (int i = 0; i < activeAgents.Count; i++)
        {
            if (startWithRandomizedNetworks)
            {
                activeAgents[i].SetAI(networkHiddenLayerStructure);
            }
            else
            {
                activeAgents[i].SetAI(data.fittestNetworkData_Training);
            }
        }
    }

    public async UniTask TrainAgents(List<Dataset> trainingData)
    {
        if (isTraining) { return; } // if current training process is not finished 

        var currentLearningRates = new float[activeAgents.Count];
        var evalutationData = new EvaluationData();

        isTraining = true;

        InitializeTraining();

        for (int i = 0; i < activeAgents.Count; i++)
        {
            // set constant learning rates for each agent
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
            evalutationData.Evaluations.Add(new Evaluation(currentLearningRates[i]));
        }

        // training repetitions
        for (int i = 0; i < trainingRepetitions; i++)
        {
            statusText.text = $"Training {i}";

            for (int j = 0; j < activeAgents.Count; j++)
            {
                var agent = activeAgents[j];

                agent.SetPositionAndRotation(spawn.position, spawn.rotation);

                await agent.AI.Train(trainingData, currentLearningRates[j], weightDecay);
            }

            foreach (var agent in activeAgents)
            {
                agent.SetTarget(target.position).Forget();
                agent.EnableDrivingAI(true);
            }

            statusText.text = $"Evaluating {i}";

            await WaitUntilAllAgentsReset();

            for (int j = 0; j < activeAgents.Count; j++)
            {
                var agent = activeAgents[j];
                var completedPercentage = agent.GetCompletionPercentage();
                var fitness = completedPercentage * FITNESS_SCALE;

                evalutationData.Evaluations[j].FitnessConvergence.Add(fitness);

                agent.AI.SetFitness(fitness);
            }
        }

        // save evaluation data
        if (saveEvaluationData)
        {
            var contents = JsonConvert.SerializeObject(evalutationData);
            var path = $"{Application.dataPath}/Evaluation/BackPropEval.json";

            File.WriteAllText(path, contents);
        }

        isTraining = false;
    }

    #endregion

    #region Evolution

    private void InitializeGeneticEngineering()
    {
        statusText.text = "Initializing...";

        if (startWithNewNeuralNetwork)
        {
            foreach (var agent in activeAgents)
            {
                var layers = new List<int>();

                layers.Add(AICarController.INPUT_NEURONS);
                layers.AddRange(networkHiddenLayerStructure);
                layers.Add(AICarController.OUTPUT_NEURONS);

                agent.SetAI(new NeuralNetwork(layers.ToArray(), true));
            }
        }
        else
        {
            foreach (var agent in activeAgents)
            {
                agent.SetAI(new NeuralNetwork(data.fittestNetworkData_Evolution));
                agent.AI.SetFitness(0f);
            }
        }
    }

    public void StartEvolution()
    {
        Evolution(epochsCount).Forget();
    }

    private void GeneticEngineering(NeuralNetwork[] fittestNetworks, float fitnessDifference, int numberOfEpochsWithoutImprovement)
    {
        var j = 0;

        if (fitnessDifference < 0f) // => no improvement from last generation => almost everyone mutate
        {
            foreach (var agent in activeAgents)
            {
                if (j < (activeAgents.Count / 4))
                {
                    agent.SetAI(fittestNetworks[0]);
                }
                else
                {
                    agent.SetAI(fittestNetworks[Random.Range(0, fittestNetworks.Length)]);
                }
                agent.AI.Mutate();

                j++;
            }
        }
        else if (fitnessDifference < 4f) // => minor improvement => try to preserve most of the superior genes
        {
            foreach (var agent in activeAgents)
            {
                if (j < (activeAgents.Count / 3))
                {
                    // crossovers
                    agent.SetAI(fittestNetworks[0].Crossover(fittestNetworks[Random.Range(0, fittestNetworks.Length)], 0.1f));
                }
                else if (j < (activeAgents.Count / 3) * 2)
                {
                    // elitism
                    agent.SetAI(fittestNetworks[Random.Range(0, fittestNetworks.Length)]);
                    agent.AI.Mutate();
                }
                else
                {
                    // elitism
                    agent.SetAI(fittestNetworks[0]);
                }

                j++;
            }
        }
        else // => major improvement, spread the superior genes among all agents
        {
            foreach (var agent in activeAgents)
            {

                if (j < (activeAgents.Count / 4f))
                {
                    // crossover
                    agent.SetAI(fittestNetworks[0].Crossover(fittestNetworks[Random.Range(0, fittestNetworks.Length)], 0.1f));
                }
                else
                {
                    // set the basis for the next generation
                    agent.SetAI(fittestNetworks[0]);
                }

                j++;
            }
        }
    }

    private async UniTask Evolution(int epochs)
    {
        var highestFitness = -20f;
        var currentHighestFitness = 0f;
        var fitnessDifference = 0f;
        var numberOfEpochsWithoutImprovement = 0;

        NeuralNetwork[] fittestNetworks = new NeuralNetwork[4];

        InitializeGeneticEngineering();

        for (int i = 0; i < epochs; i++)
        {
            statusText.text = $"[Epoch {i}]Applying genetic engineering...";

            if (i > 0) // if not first epoch
            {
                GeneticEngineering(fittestNetworks, fitnessDifference, numberOfEpochsWithoutImprovement);
            }

            statusText.text = $"[Epoch {i}]Evolution...";

            foreach (var agent in activeAgents)
            {
                agent.SetPositionAndRotation(spawn.position, spawn.rotation);
                agent.SetTarget(target.position).Forget();
                agent.EnableDrivingAI(true);
            }

            var allReset = false;

            // wait till all the agents have crashed or finished
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

            statusText.text = $"[Epoch {i}]Evaluating...";

            // set the fitness based on how far each agent got
            foreach (var agent in activeAgents)
            {
                var completedPercentage = agent.GetCompletionPercentage();
                var fitness = completedPercentage * FITNESS_SCALE;

                agent.AI.SetFitness(fitness);
            }

            activeAgents.Sort((agent1, agent2) => agent2.AI.CompareTo(agent1.AI)); // sort by fitness descending

            currentHighestFitness = activeAgents[0].AI.GetFitness();

            fitnessDifference = currentHighestFitness - highestFitness;

            // if the current epoch hasnt brought forth stronger individuals, repeat with the previous generation (the generation with the highest fitness at this point)
            if (fitnessDifference > 0f)
            {
                highestFitness = currentHighestFitness;
                numberOfEpochsWithoutImprovement = 0;

                for (int j = 0; j < fittestNetworks.Length; j++)
                {
                    fittestNetworks[j] = new NeuralNetwork(activeAgents[j].AI);
                }

                if (saveFittestNetworks) { data.StoreNetwork(fittestNetworks[0], true); }
            }
            else
            {
                numberOfEpochsWithoutImprovement++;
            }

            fitnessText.text = $"[Epoch {i}]Highest Fitness: {highestFitness}";
        }

        statusText.text = "Finished!";
        startEvolutionButton.gameObject.SetActive(true);
    }

    #endregion
}
