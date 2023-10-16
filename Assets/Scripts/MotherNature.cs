using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using TMPro;

namespace Default
{
    public class MotherNature : MonoBehaviour
    {
        public static MotherNature Instance { get; private set; }

        public float PlayerBaseDestroyedReward { get => playerBaseDestroyedReward; set => playerBaseDestroyedReward = value; }
        public float TowerDestroyReward { get => towerDestroyReward; set => towerDestroyReward = value; }
        public float MeleeUnitKilledPenalty { get => meleeUnitKilledPenalty; set => meleeUnitKilledPenalty = value; }
        public float ArcherUnitKilledPenalty { get => archerUnitKilledPenalty; set => archerUnitKilledPenalty = value; }
        public float TimePenaltyPerSecond { get => timePenaltyPerSecond; set => timePenaltyPerSecond = value; }

        [SerializeField] private GameManager[] gameInstances;
        [SerializeField] private NeuralNetworkData fittestNetworkData;
        [SerializeField] private TMP_Text generationOutputText;

        [Header("Config")]
        [SerializeField, Range(1f, 4f)] private float timeScale = 1f;
        [SerializeField] private bool startWithNewNetworkInstances = true;
        [SerializeField] private int numberOfGenerations;
        [SerializeField] private float playerBaseDestroyedReward = 7f;
        [SerializeField] private float towerDestroyReward = 1f;
        [SerializeField] private float meleeUnitKilledPenalty = -1f;
        [SerializeField] private float archerUnitKilledPenalty = -2.5f;
        [SerializeField] private float timePenaltyPerSecond = -0.3f;
        [SerializeField] private float optimalFitnessLevel = 14f;

        private NeuralNetwork[] activeNetworks;
        private bool isRunning;


        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (gameInstances.Length <= 0) { return; }

            // Initialize your active networks with random weights.
            activeNetworks = new NeuralNetwork[gameInstances.Length];

            for (int i = 0; i < gameInstances.Length; i++)
            {
                if (!startWithNewNetworkInstances && fittestNetworkData.Network != null)
                {
                    // Start as deep copies of the fittest network
                    activeNetworks[i] = new NeuralNetwork(fittestNetworkData.Network);
                }
                else
                {
                    activeNetworks[i] = new NeuralNetwork(fittestNetworkData.layers);
                }

                gameInstances[i].Brain = activeNetworks[i];
            }
        }

        private void Update()
        {
            if (!isRunning && Input.GetKeyDown(KeyCode.Space))
            {
                EvolutionRoutine().Forget();
                isRunning = true;
            }

            Time.timeScale = timeScale;
        }

        private async UniTask EvolutionRoutine()
        {
            for (int generation = 0; generation < numberOfGenerations; generation++)
            {

                // wait for all the game instances to finish their iterations
                for (int i = 0; i < gameInstances.Length; i++)
                {
                    var gameInstance = gameInstances[i];
                    gameInstance.Iterate();
                }

                for (int i = 0; i < gameInstances.Length; i++)
                {
                    var gameInstance = gameInstances[i];
                    // UI
                    generationOutputText.text = $"Current Generation: {generation}\nHighest Fitness: {activeNetworks[0].GetFitness()}\nWaiting for {gameInstances.Length - i} games to finish";

                    await UniTask.WaitUntil(() => gameInstance.HasReachedState(GameManager.GameState.Finished));

                    activeNetworks[i].SetFitness(gameInstance.Evaluation);
                }

                generationOutputText.text = $"Evaluating...";

                // sort by fitness
                activeNetworks = activeNetworks.OrderByDescending(n => n.GetFitness()).ToArray();

                Debug.Log($"All game instances recorded for generation {generation}, highest fitness: {activeNetworks[0].GetFitness()}");

                // store fittest model
                fittestNetworkData.StoreNetwork(activeNetworks[0]);

                // take top 2 NNs
                NeuralNetwork[] topNetworks = { activeNetworks[0], activeNetworks[1] };
                List<NeuralNetwork> children = new();

                // calculate mutation count for children based on how fit the networks were last generation, -> less mutation when nns are more evolved
                var highestFitnessFloored = activeNetworks[0].GetFitness() < 0f ? 0f : activeNetworks[0].GetFitness();
                var mutationPercentage = 1f - Mathf.Clamp01(highestFitnessFloored / optimalFitnessLevel);
                var mutateCount = (int)(mutationPercentage * activeNetworks.Length);

                // Generate children equal to the total population size.
                for (int i = 0; i < activeNetworks.Length; i++)
                {
                    var otherParentGeneticSuperiority = Random.Range(0.1f, 0.3f); // rather tend to take more genes from number 1 instead of number 2
                    var child = topNetworks[0].Crossover(topNetworks[1], otherParentGeneticSuperiority);

                    children.Add(child);
                }

                // Replace the active networks with the children
                activeNetworks = children.ToArray();

                // mutate
                for (int i = 0; i < mutateCount; i++)
                {
                    activeNetworks[i].Mutate();
                }

                for (int i = 0; i < activeNetworks.Length; i++)
                {
                    gameInstances[i].Brain = activeNetworks[i];
                }

                // restart iteration
            }
        }
    }
}