using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Default
{
    public class Map : MonoBehaviour
    {
        public Camera playerCamera;
        public Tilemap tilemap;

        void Update()
        {
            if (Input.GetMouseButtonDown(0)) // If left mouse button clicked
            {
                var mouseWorldPos = playerCamera.ScreenToWorldPoint(Input.mousePosition);
                var coordinate = tilemap.WorldToCell(mouseWorldPos);
                var clickedTile = tilemap.GetTile(coordinate);

                if (clickedTile != null)
                {
                    Debug.Log("Clicked on tile " + clickedTile.name + " at " + coordinate);
                }
                else
                {
                    Debug.Log($"{mouseWorldPos} is the position of the mouse converted to coord. {coordinate}");
                }
            }
        }
    }
}