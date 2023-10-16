using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Default
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;

        [SerializeField] private Transform selectionMarker;
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

            if (!randomPlacements)
            {
                SelectSpot(currentSelectionIndex);
            }
        }

        private void Update()
        {
            // if (randomPlacements && gameManager.State != GameManager.GameState.Running)
            // {
            //     if (Input.GetKeyDown(KeyCode.Space))
            //     {
            //         RandomlyPlaceTowers();
            //         gameManager.StartGame();
            //     }
            // }
            // else if (gameManager.State == GameManager.GameState.Preperation) // pre iteration
            // {
            //     if (Input.GetKeyDown(KeyCode.Space))
            //     {
            //         currentSelection = null;
            //         selectionMarker.transform.position = Vector3.down * 1000f;

            //         gameManager.StartGame();
            //     }

            //     if (Input.GetKeyDown(KeyCode.RightArrow))
            //     {
            //         currentSelectionIndex = currentSelectionIndex >= towerSpotCount - 1 ? 0 : currentSelectionIndex + 1;
            //         SelectSpot(currentSelectionIndex);
            //     }

            //     if (Input.GetKeyDown(KeyCode.LeftArrow))
            //     {
            //         currentSelectionIndex = currentSelectionIndex <= 0 ? towerSpotCount - 1 : currentSelectionIndex - 1;
            //         SelectSpot(currentSelectionIndex);
            //     }

            //     if (Input.GetKeyDown(KeyCode.I))
            //     {
            //         TryPlaceTower(TowerType.Impulse);
            //     }

            //     if (Input.GetKeyDown(KeyCode.S))
            //     {
            //         TryPlaceTower(TowerType.Sniper);
            //     }
            // }
        }

        public void RandomlyPlaceTowers()
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

            return true;
        }
    }
}

