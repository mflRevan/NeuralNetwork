using System.Collections.Generic;
using Default;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private LayerMask wallMask;
    [SerializeField] private NeuralNetworkData data;
    [SerializeField] private Button startEvolutionButton;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text fitnessText;
    public Transform target;
    public Transform spawn;

    [Header("Config")]
    [SerializeField] private bool startWithNewNeuralNetwork;
    [SerializeField] private int epochsCount;
    [SerializeField, Range(0.5f, 12f)] private float timeScale = 1f;
    [Space]
    [SerializeField] private int[] networkHiddenLayerStructure;

    public LayerMask WallMask => wallMask;

    private List<AICarController> activeAgents;
    private float highestFitness;

    private const float FITNESS_SCALE = 100f;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        activeAgents = FindObjectsByType<AICarController>(FindObjectsSortMode.None).ToList<AICarController>();
    }

    private void LateUpdate()
    {
        Time.timeScale = timeScale;
    }

    private void Initialize()
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
                agent.SetAI(new NeuralNetwork(data.FittestNetwork));
            }
        }
    }

    public async UniTask WaitUntilAllAgentsReset()
    {
        var active = true;

        while (active == true)
        {
            await UniTask.Delay(500, false, PlayerLoopTiming.FixedUpdate, gameObject.GetCancellationTokenOnDestroy()); // check every 0.5 seconds

            active = false;

            // if one agent is still driving
            foreach (var agent in activeAgents)
            {
                if (agent.AIDrivingEnabled)
                {
                    active = true;
                    continue;
                }
            }
        }
    }

    public void StartEvolution()
    {
        Evolution(epochsCount).Forget();
    }

    private void GeneticEngineering(NeuralNetwork[] fittestNetworks, float fitnessDifference)
    {
        var j = 0;

        if (fitnessDifference < 0f) // => no improvement from last generation => almost everyone mutate
        {
            foreach (var agent in activeAgents)
            {
                agent.SetAI(fittestNetworks[Random.Range(0, fittestNetworks.Length)]);
                agent.AI.Mutate();

                j++;
            }
        }
        else if (fitnessDifference < 6f) // => minor improvement => try to preserve most of the superior genes
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
                    agent.SetAI(fittestNetworks[Random.Range(0, 2)]);
                    agent.AI.Mutate();
                }
                else
                {
                    // elitism
                    agent.SetAI(fittestNetworks[0]);
                    agent.AI.Mutate();
                }

                j++;
            }
        }
        else // => major improvement, spread the superior genes among all agents
        {
            foreach (var agent in activeAgents)
            {
                agent.SetAI(fittestNetworks[0].Crossover(fittestNetworks[Random.Range(1, fittestNetworks.Length)], 0.085f));

                // if (j < (activeAgents.Count / 10f)) // mutate around 1/10th of the population
                // {
                //     agent.AI.Mutate();
                // }

                j++;
            }
        }

    }

    private async UniTask Evolution(int epochs)
    {
        highestFitness = -0.1f;
        var fitnessDifference = 0f;

        NeuralNetwork[] fittestNetworks = new NeuralNetwork[6];

        Initialize();

        for (int i = 0; i < epochs; i++)
        {
            statusText.text = $"[Epoch {i}]Applying genetic engineering...";

            if (i > 0) // if not first epoch
            {
                GeneticEngineering(fittestNetworks, fitnessDifference);
            }

            statusText.text = $"[Epoch {i}]Evolution...";

            foreach (var agent in activeAgents)
            {
                agent.SetPositionAndRotation(spawn.position, spawn.rotation);
                agent.SetTarget(target.position).Forget();
                agent.EnableDrivingAI(true);
            }

            await WaitUntilAllAgentsReset();

            statusText.text = $"[Epoch {i}]Evaluating...";

            // set the fitness based on how far each agent got
            foreach (var agent in activeAgents)
            {
                var completedPercentage = agent.GetCompletionPercentage();
                var fitness = completedPercentage * FITNESS_SCALE;

                agent.AI.SetFitness(fitness);
            }

            activeAgents.Sort((agent1, agent2) => agent2.AI.CompareTo(agent1.AI)); // sort by fitness descending

            var currentHighestFitness = activeAgents[0].AI.GetFitness();

            // if the current epoch hasnt brought forth stronger individuals, repeat with the previous generation (the generation with the highest fitness at this point)
            if (highestFitness < currentHighestFitness)
            {
                highestFitness = currentHighestFitness;

                for (int j = 0; j < fittestNetworks.Length; j++)
                {
                    fittestNetworks[j] = new NeuralNetwork(activeAgents[j].AI);
                }

                data.StoreNetwork(fittestNetworks[0]);
            }

            fitnessDifference = currentHighestFitness - highestFitness;

            fitnessText.text = $"[Epoch {i}]Highest Fitness: {highestFitness}";
        }

        statusText.text = "Finished!";
        startEvolutionButton.gameObject.SetActive(true);
    }
}
