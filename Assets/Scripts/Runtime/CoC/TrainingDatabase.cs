using System;
using System.Collections.Generic;
using UnityEngine;


namespace Default
{
    [CreateAssetMenu(fileName = "TraniningDatabase", menuName = "AI/TrainingDatabase")]
    public class TrainingDatabase : ScriptableObject
    {
        public List<MapData> Maps;
    }

    [Serializable]
    public class MapData
    {
        [SerializeField] private List<int> grid;
        public List<int> Grid => grid;

        public readonly Texture2D mapTexture;

        public MapData(List<int> grid)
        {
            this.grid = grid;
            mapTexture = GenerateTexture(grid);
        }

        private Texture2D GenerateTexture(List<int> grid)
        {
            var gridSize = (int)Mathf.Sqrt(grid.Count);
            var texture = new Texture2D(gridSize, gridSize)
            {
                filterMode = FilterMode.Point
            };

            for (int i = 0; i < grid.Count; i++)
            {
                int x = i % gridSize;
                int y = i / gridSize;

                Color color = Color.black;

                switch (grid[i])
                {
                    case (int)BuildingType.Base:
                        color = Color.cyan;
                        break;

                    case (int)BuildingType.Cannon:
                        color = Color.white;
                        break;

                    case (int)BuildingType.Stomper:
                        color = Color.red;
                        break;

                    case (int)BuildingType.Mortar:
                        color = Color.magenta;
                        break;

                    case (int)BuildingType.Resource:
                        color = Color.yellow;
                        break;

                    case (int)BuildingType.Blank:
                        color = Color.grey;
                        break;
                }

                texture.SetPixel(x, y, color);
            }

            texture.Apply();

            return texture;
        }

        public List<int> GetCompressedData()
        {
            return null;
        }
    }
}