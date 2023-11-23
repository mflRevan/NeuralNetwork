using UnityEngine;
using UnityEngine.Serialization;

namespace Default
{
    public enum BuildingType
    {
        Empty = 0,
        Cannon = 1,
        Stomper = 2,
        Mortar = 3,
        Resource = 4,
        Blank = 5,
        Base = 6
    }

    public abstract class Building : MonoBehaviour
    {
        [Header("Building")]
        [SerializeField] private BuildingType buildingType = 0;
        [SerializeField] private float value = 1f;
        [SerializeField, FormerlySerializedAs("health")] private float initialHealth = 380f;
        [SerializeField] private float damage = 15f;
        [SerializeField] private float damageRate = 1f;
        [SerializeField] private float range = 9f;

        public BuildingType Type => buildingType;
        public float Value => value;
        public float Health { get; }
        public float Damage => damage;
        public float DamageRate => damageRate;
        public float Range => range;


        public const int ENCODED_VALUES_COUNT = 4;

        /// <summary>
        /// Encodes the building ID into (Building.ENCODED_VALUES_COUNT) seperate normalized floats each representing the stats of the building (i.e. Value, Health, Damage, Range)
        /// </summary>
        public static float[] EncodeBuildingID(int buildingID)
        {
            switch (buildingID)
            {
                case (int)BuildingType.Base:
                    return new float[] { 1f, 1f, 0f, 0f };

                case (int)BuildingType.Cannon:
                    return new float[] { 0.5f, 0.25f, 0.5f, 0.8f };

                case (int)BuildingType.Stomper:
                    return new float[] { 0.5f, 0.25f, 0.9f, 0.2f };

                case (int)BuildingType.Mortar:
                    return new float[] { 0.6f, 0.17f, 0.7f, 1f };

                case (int)BuildingType.Resource:
                    return new float[] { 0.9f, 0.4f, 0f, 0f };

                case (int)BuildingType.Blank:
                    return new float[] { 0.15f, 0.25f, 0f, 0f };
            }

            return new float[] { 0f, 0f, 0f, 0f };
        }
    }
}