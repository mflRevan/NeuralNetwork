using System.Collections.Generic;
using UnityEngine;

namespace Default
{
    public class GameManager : MonoBehaviour
    {
        public List<IEnemyUnit> AllAliveEnemies { get; set; }
        public List<ITower> AllAliveTowers { get; set; }

        public EnemyController enemyController;
        public PlayerController playerController;
        public List<TowerSpot> TowerSpots;
        public List<Transform> SpawnPoints;

        public NeuralNetwork Brain { get; set; }
        public float Evaluation { get; private set; }

        public enum GameState
        {
            Preperation,
            Running,
            Finished
        }

        public GameState State { get; private set; }

        private float gameTimer = 0f;


        private void Start()
        {
            AllAliveEnemies = new();
            AllAliveTowers = new();
        }

        public void StartGame(float[] towerPositions)
        {
            if (State == GameState.Running) { return; }

            var strategy = Brain.FeedForward(towerPositions);
            enemyController.StartIteration(strategy);

            State = GameState.Running;

            gameTimer = 0f;
            Evaluation = 0f;
        }

        private void FinishGame()
        {
            if (State != GameState.Running) { return; }

            ResetGame();

            State = GameState.Finished;
        }

        private void ResetGame()
        {
            foreach (var spot in TowerSpots)
            {
                spot.Reset();
            }

            enemyController.Reset();
            playerController.Reset();
        }

        public bool HasReachedState(GameState state)
        {
            return State == state;
        }

        public void OnPlayerBaseDestroyed()
        {
            Evaluation += MotherNature.Instance.PlayerBaseDestroyedReward;

            FinishGame();
        }

        public void OnTowerDestroyed(ITower tower)
        {
            Evaluation += MotherNature.Instance.TowerDestroyReward;
        }

        public void OnEnemyUnitKilled(IEnemyUnit enemyUnit)
        {
            Evaluation -= enemyUnit.Type == EnemyType.Melee ? MotherNature.Instance.MeleeUnitKilledPenalty : MotherNature.Instance.ArcherUnitKilledPenalty;

            if (AllAliveEnemies.Count <= 0)
            {
                FinishGame();
            }
        }

        private void Update()
        {
            if (State == GameState.Running)
            {

            }
        }
    }
}
