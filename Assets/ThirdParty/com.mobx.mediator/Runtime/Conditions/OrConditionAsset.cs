using UnityEngine;

namespace MobX.Mediator.Conditions
{
    public sealed class OrConditionAsset : ConditionAsset
    {
        [SerializeField] private ConditionAsset[] conditions;

        public override bool Check()
        {
            return conditions.Any();
        }
    }
}