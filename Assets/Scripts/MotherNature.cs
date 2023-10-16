using System.Linq;
using Cysharp.Threading.Tasks; // Import the UniTask namespace
using UnityEngine;
using UnityEngine.AI;

namespace Default
{
    public class MotherNature : MonoBehaviour
    {
        public static MotherNature Instance { get; private set; }

        public float PlayerBaseDestroyedReward { get => playerBaseDestroyedReward; set => playerBaseDestroyedReward = value; }
        public float TowerDestroyReward { get => towerDestroyReward; set => towerDestroyReward = value; }
        public float MeleeUnitKilledPenalty { get => meleeUnitKilledPenalty; set => meleeUnitKilledPenalty = value; }
        public float ArcherUnitKilledPenalty { get => archerUnitKilledPenalty; set => archerUnitKilledPenalty = value; }

        [SerializeField] private GameManager[] gameInstances;
        [SerializeField] private NeuralNetworkData fittestNetworkData;

        [Header("Config")]
        [SerializeField, Range(1f, 4f)] private float timeScale = 1f;
        [SerializeField] private bool startWithNewNetworkInstances = true;
        [SerializeField] private int numberOfGenerations;
        [SerializeField] private float playerBaseDestroyedReward = 7f;
        [SerializeField] private float towerDestroyReward = 1f;
        [SerializeField] private float meleeUnitKilledPenalty = -1f;
        [SerializeField] private float archerUnitKilledPenalty = -2.5f;

        private NeuralNetwork[] activeNetworks;


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

            EvolutionRoutine().Forget();
        }

        private async UniTask EvolutionRoutine()
        {
            for (int generation = 0; generation < numberOfGenerations; generation++)
            {
                for (int i = 0; i < gameInstances.Length; i++)
                {
                    var gameInstance = gameInstances[i];

                    await UniTask.WaitUntil(() => gameInstance.State == GameManager.GameState.Finished);

                    activeNetworks[i].SetFitness(gameInstance.Evaluation);
                }

                Debug.Log($"All game instances recorded for generation {generation}");

                // sort by fitness
                activeNetworks = activeNetworks.OrderByDescending(n => n.GetFitness()).ToArray();

                // evaluate fitness
                // Crossover and mutation logic...

                // for (int i = 2; i < activeNetworks.Length; i++)
                // {
                //     activeNetworks[i].Mutate();
                // }

                // store fittest model
                fittestNetworkData.StoreNetwork(activeNetworks[0]);

                // restart iteration
            }
        }

        public void Update()
        {
            Time.timeScale = timeScale;
        }
    }
}