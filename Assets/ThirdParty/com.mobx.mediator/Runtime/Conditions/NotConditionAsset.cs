using UnityEngine;
using UnityEngine.Serialization;

namespace MobX.Mediator.Conditions
{
    public sealed class NotConditionAsset : ConditionAsset
    {
        [SerializeField] private ConditionAsset conditionAsset;

        public override bool Check()
        {
            return !conditionAsset.Check();
        }
    }
}