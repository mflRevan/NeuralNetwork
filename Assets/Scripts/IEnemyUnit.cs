using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Default
{
    public interface IEnemyUnit
    {
        public GameManager GameManager { get; set; }

        public EnemyType Type { get; }
        public float Damage { get; }
        public float DamageRate { get; }
        public float Health { get; }
        public float Speed { get; }
        public float Range { get; }
        public GameObject EnemyObject { get; }

        public void TakeDamage(float damage);
    }
}