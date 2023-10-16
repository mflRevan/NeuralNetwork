using System;
using UnityEngine;

namespace Default
{
    public interface ITower
    {
        public GameManager GameManager { get; set; }

        public GameObject TowerObject { get; }
        public TowerType Type { get; }
        public float Health { get; }

        public Action Destroyed { get; set; }

        public void TakeDamage(float damage);
    }
}