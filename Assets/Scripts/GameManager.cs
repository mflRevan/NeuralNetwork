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
        public PlayerBaseTower playerBaseTower;
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

        }

        public void Iterate()
        {
            playerController.RandomlyPlaceTowers();
            StartGame();
        }

        public void StartGame()
        {
            if (State == GameState.Running) { return; }

            AllAliveEnemies = new();
            AllAliveTowers = new();

            float[] towerPositions = new float[TowerSpots.Count];

            for (int i = 0; i < TowerSpots.Count; i++)
            {
                var spot = TowerSpots[i];

                towerPositions[i] = spot.ActiveTower == null ? 0f : (spot.ActiveTower.Type == TowerType.Impulse ? 1f : 2f);
            }

            var strategy = Brain.FeedForward(towerPositions);
            enemyController.StartIteration(strategy);

            AllAliveTowers.Add(playerBaseTower);
            playerBaseTower.Initialize(this);

            State = GameState.Running;

            gameTimer = 0f;
            Evaluation = 0f;
        }

        private void FinishGame()
        {
            if (State != GameState.Running) { return; }

            ApplyTimePenalty();
            ResetGame();

            State = GameState.Finished;
        }

        private void ApplyTimePenalty()
        {
            Evaluation += MotherNature.Instance.TimePenaltyPerSecond;
        }

        private void ResetGame()
        {
            foreach (var spot in TowerSpots)
            {
                spot.Reset();
            }

            foreach (var unit in AllAliveEnemies)
            {
                Destroy(unit.EnemyObject);
            }

            playerBaseTower.Reset();
            enemyController.Reset();
            playerController.Reset();

            gameTimer = 0f;
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
            AllAliveTowers.Remove(tower);
        }

        public void OnEnemyUnitKilled(IEnemyUnit enemyUnit)
        {
            Evaluation += enemyUnit.Type == EnemyType.Melee ? MotherNature.Instance.MeleeUnitKilledPenalty : MotherNature.Instance.ArcherUnitKilledPenalty;

            AllAliveEnemies.Remove(enemyUnit);

            if (AllAliveEnemies.Count <= 0)
            {
                FinishGame();
            }
        }

        private void Update()
        {
            if (State == GameState.Running)
            {
                gameTimer += Time.deltaTime;
            }
        }
    }
}
