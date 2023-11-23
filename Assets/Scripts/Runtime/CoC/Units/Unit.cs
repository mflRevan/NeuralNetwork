using UnityEngine;
using UnityEngine.AI;

namespace Default
{
    public abstract class Unit : MonoBehaviour
    {
        [Header("Unit")]
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private UnitType unitType = 0;
        [SerializeField] private float value = 1f;
        [SerializeField] private float initialHealth = 65;
        [SerializeField] private float damage = 11f;
        [SerializeField] private float damageRate = 1f;
        [SerializeField] private float range = 0.5f;
        [SerializeField] private float speed = 0.5f;

        public NavMeshAgent Agent => agent;
        public UnitType Type => unitType;
        public float Value => value;
        public float Health { get; }
        public float Damage => damage;
        public float DamageRate => damageRate;
        public float Range => range;
        public float Speed => speed;

    }

    public enum UnitType
    {
        Null,
        Barbarian,
        Archer,
        Giant,
        Wallbreaker
    }
}