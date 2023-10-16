using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Default
{
    public class TowerSpot : MonoBehaviour
    {
        public ITower ActiveTower { get; private set; }

        public bool Occupied;

        public void Reset()
        {
            if (ActiveTower.TowerObject != null)
            {
                ActiveTower.Destroyed -= OnTowerDestroyed;
            }

            Occupied = false;
            ActiveTower = null;
        }

        public void SetTower(ITower tower)
        {
            ActiveTower = tower;
            Occupied = true;

            tower.Destroyed += OnTowerDestroyed; // doesnt need to unsub
        }

        private void OnTowerDestroyed()
        {
            ActiveTower = null;
            Occupied = false;
        }
    }
}