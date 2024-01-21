using UnityEngine;

namespace MobX.Mediator.Conditions
{
    public sealed class NoneConditionAsset : ConditionAsset
    {
        [SerializeField] private ConditionAsset[] conditions;

        public override bool Check()
        {
            return conditions.None();
        }
    }
}