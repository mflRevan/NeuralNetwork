using System.Collections.Generic;
using UnityEngine;


namespace Default
{
    public class Map : MonoBehaviour
    {
        public static List<Cell> Grid { get; private set; }

        [SerializeField] private List<Cell> grid;
        [SerializeField] private TrainingDatabase database;

        [Header("Buildings")]
        [SerializeField] private Building cannonPrefab;
        [SerializeField] private Building stomperPrefab;
        [SerializeField] private Building mortarPrefab;
        [SerializeField] private Building resourcePrefab;
        [SerializeField] private Building blankPrefab;


        private void Awake()
        {
            Grid = grid;
        }

        public void InitializeMapFromData(MapData mapData)
        {
            for (int i = 0; i < grid.Count; i++)
            {
                switch (mapData.list[i])
                {
                    case (int)BuildingType.Empty:
                        // empty
                        break;

                    case (int)BuildingType.Base:
                        // dont do anything
                        break;

                    case (int)BuildingType.Cannon:
                        grid[i].PlaceBuilding(cannonPrefab);
                        break;

                    case (int)BuildingType.Stomper:
                        grid[i].PlaceBuilding(stomperPrefab);
                        break;

                    case (int)BuildingType.Mortar:
                        grid[i].PlaceBuilding(mortarPrefab);
                        break;

                    case (int)BuildingType.Resource:
                        grid[i].PlaceBuilding(resourcePrefab);
                        break;

                    case (int)BuildingType.Blank:
                        grid[i].PlaceBuilding(blankPrefab);
                        break;
                }
            }
        }

        public void InitializeRandomMapFromDatabase()
        {
            var random = Random.Range(0, database.Maps.Count);
            var mapData = database.Maps[random];

            InitializeMapFromData(mapData);
        }

        public void InitializeMapFromDatabase(int index)
        {
            if (index > database.Maps.Count - 1)
            {
                return;
            }

            var mapData = database.Maps[index];
            InitializeMapFromData(mapData);
        }

        public void SaveCurrentMapData()
        {
            MapData mapData = new();

            foreach (var cell in grid)
            {
                var b = cell.GetActiveBuilding();

                mapData.list.Add(b == null ? 0 : (int)b.Type);
            }

            database.Maps.Add(mapData);
        }

        public void Clear()
        {
            foreach (var cell in grid)
            {
                if (cell.ActiveBuilding != null && cell.ActiveBuilding.Type == BuildingType.Base)
                {
                    continue;
                }

                cell.Clear();
            }
        }
    }
}