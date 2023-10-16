using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;


namespace Default
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private GameObject meleeUnitPrefab;
        [SerializeField] private GameObject archerUnitPrefab;

        private List<float> delays;
        private List<float> positions;
        private List<EnemyType> types;

        private void Start()
        {
            Reset();
        }

        public void Reset()
        {
            delays = new();
            positions = new();
            types = new();
        }

        public void StartIteration(float[] brainOutput)
        {
            for (int i = 0; i < brainOutput.Length; i++)
            {
                if (i >= brainOutput.Length / 2)
                {
                    delays.Add((brainOutput[i] + 8f) / 8f); // i. e. max delay of 2 seconds
                }
                else
                {
                    types.Add(brainOutput[i] < 0 ? EnemyType.Melee : EnemyType.Archer);
                    positions.Add(Mathf.Abs(brainOutput[i]));
                }
            }

            SpawnWaves(delays.ToArray(), positions.ToArray(), types.ToArray());
        }

        private void TestWave()
        {
            float[] delays = { 2f, 7f, 4f, 12f, 5f, 1.5f, 9f, 8f, 3f, 10f, 10f, 10f };
            float[] positions = { 0.1f, 0.7f, 0.3f, 0.85f, 0.42f, 0.05f, 1.4f, 0.76f, 0.24f, 0.68f, 1.2f, 1.8f };
            EnemyType[] enemyTypes =
            {
                EnemyType.Melee,
                EnemyType.Archer,
                EnemyType.Melee,
                EnemyType.Melee,
                EnemyType.Archer,
                EnemyType.Melee,
                EnemyType.Archer,
                EnemyType.Melee,
                EnemyType.Archer,
                EnemyType.Melee,
                EnemyType.Melee,
                EnemyType.Melee
            };

            // Assuming enemyController is a reference to the EnemyController script
            EnemyController enemyController = GetComponent<EnemyController>();
            enemyController.SpawnWaves(delays, positions, enemyTypes);
        }

        public void SpawnWaves(float[] delays, float[] positions, EnemyType[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                var delay = delays[i];
                var positionLerpValue = positions[i] / gameManager.SpawnPoints.Count;
                var type = types[i];

                var task = SummonUnitDelayed(type, delay, positionLerpValue);
            }
        }

        private Vector2 GetPositionFromLerpValue(float lerpValue, List<Transform> spawnPoints)
        {
            if (spawnPoints.Count <= 1)
                return spawnPoints[0].position;

            // Calculate index
            var indexF = lerpValue * (spawnPoints.Count - 1);
            var indexA = Mathf.FloorToInt(indexF);
            var indexB = Mathf.CeilToInt(indexF);

            // Clamp to ensure we don't exceed array bounds
            indexA = Mathf.Clamp(indexA, 0, spawnPoints.Count - 1);
            indexB = Mathf.Clamp(indexB, 0, spawnPoints.Count - 1);

            // Calculate the new lerp value between the two points
            var newLerpValue = indexF - indexA;

            // Return the lerped position
            return Vector2.Lerp(spawnPoints[indexA].position, spawnPoints[indexB].position, newLerpValue);
        }

        private async UniTaskVoid SummonUnitDelayed(EnemyType type, float delay, float positionLerpValue)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: this.GetCancellationTokenOnDestroy()); // Convert delay to milliseconds

            if (this == null || gameManager.State != GameManager.GameState.Running) return;

            SummonUnit(type, positionLerpValue);
        }

        private void SummonUnit(EnemyType type, float positionLerpValue)
        {
            var lerpedPosition = GetPositionFromLerpValue(positionLerpValue, gameManager.SpawnPoints);

            var enemyInstance = Instantiate(
                type == EnemyType.Melee ? meleeUnitPrefab : archerUnitPrefab,
                lerpedPosition,
                Quaternion.identity
            ).GetComponent<IEnemyUnit>();

            enemyInstance.GameManager = gameManager;
        }
    }
}