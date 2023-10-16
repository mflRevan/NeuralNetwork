using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Default
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;

        [SerializeField] private GameObject prepareScreen;
        [SerializeField] private Transform selectionMarker;
        [SerializeField] private TMP_Text remainingTowersCountText;
        [SerializeField] private GameObject impulseTowerPrefab;
        [SerializeField] private GameObject sniperTowerPrefab;

        [Header("Config")]
        [SerializeField] private int maxTowersToPlace = 8;
        [SerializeField] private bool randomPlacements = true;

        private int towerSpotCount;
        private int placedTowersCount;
        private int currentSelectionIndex;
        private TowerSpot currentSelection;


        private void Start()
        {
            Reset();
        }

        public void Reset()
        {
            towerSpotCount = gameManager.TowerSpots.Count;
            remainingTowersCountText.text = $"Remaining towers to place: {maxTowersToPlace - placedTowersCount}";

            if (!randomPlacements)
            {
                SelectSpot(currentSelectionIndex);
            }
        }

        private void StartGame()
        {
            var towerSpots = gameManager.TowerSpots;
            var towerPositions = new float[towerSpots.Count];

            for (int i = 0; i < towerSpots.Count; i++)
            {
                var spot = towerSpots[i];

                towerPositions[i] = spot.ActiveTower == null ? 0f : (spot.ActiveTower.Type == TowerType.Impulse ? 1f : 2f);
            }

            gameManager.StartGame(towerPositions);
        }

        private void Update()
        {
            if (randomPlacements && gameManager.State != GameManager.GameState.Running)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    RandomlyPlaceTowers();
                    StartGame();
                }
            }
            else if (gameManager.State == GameManager.GameState.Preperation) // pre iteration
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    currentSelection = null;
                    selectionMarker.transform.position = Vector3.down * 1000f;
                    prepareScreen.SetActive(false);
                    remainingTowersCountText.gameObject.SetActive(false);

                    StartGame();
                }

                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    currentSelectionIndex = currentSelectionIndex >= towerSpotCount - 1 ? 0 : currentSelectionIndex + 1;
                    SelectSpot(currentSelectionIndex);
                }

                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    currentSelectionIndex = currentSelectionIndex <= 0 ? towerSpotCount - 1 : currentSelectionIndex - 1;
                    SelectSpot(currentSelectionIndex);
                }

                if (Input.GetKeyDown(KeyCode.I))
                {
                    TryPlaceTower(TowerType.Impulse);
                }

                if (Input.GetKeyDown(KeyCode.S))
                {
                    TryPlaceTower(TowerType.Sniper);
                }
            }
        }

        private void RandomlyPlaceTowers()
        {
            var availableSpots = new List<TowerSpot>(gameManager.TowerSpots);
            availableSpots.Shuffle();

            for (int i = 0; i < maxTowersToPlace; i++)
            {
                if (availableSpots.Count == 0)
                {
                    break;
                }

                TowerType randomTowerType = (TowerType)Random.Range(0, 2);

                SelectSpot(availableSpots[i]);
                TryPlaceTower(randomTowerType);
            }

            prepareScreen.SetActive(false);
            remainingTowersCountText.gameObject.SetActive(false);
        }

        private void SelectSpot(TowerSpot spot)
        {
            currentSelection = spot;
            selectionMarker.position = currentSelection.transform.position + Vector3.back * 2f;
        }

        private void SelectSpot(int index)
        {
            currentSelection = gameManager.TowerSpots[currentSelectionIndex];
            selectionMarker.position = currentSelection.transform.position + Vector3.back * 2f;
        }

        private bool TryPlaceTower(TowerType type)
        {
            if (!randomPlacements && (currentSelection.Occupied || placedTowersCount >= maxTowersToPlace))
            {
                return false;
            }

            var towerInstance = Instantiate(
                type == TowerType.Impulse ? impulseTowerPrefab : sniperTowerPrefab,
                currentSelection.transform.position + Vector3.back,
                Quaternion.identity
            ).GetComponent<ITower>();

            towerInstance.GameManager = gameManager;

            currentSelection.SetTower(towerInstance);

            placedTowersCount++;
            remainingTowersCountText.text = $"Remaining towers to place: {maxTowersToPlace - placedTowersCount}";

            return true;
        }
    }
}

