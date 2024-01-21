using UnityEngine;

namespace MobX.Mediator.Conditions
{
    public sealed class AndConditionAsset : ConditionAsset
    {
        [SerializeField] private ConditionAsset[] conditions;

        public override bool Check()
        {
            return conditions.All();
        }
    }
}