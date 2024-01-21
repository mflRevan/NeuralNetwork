namespace MobX.Mediator.Conditions
{
    public abstract class ConditionAsset : MediatorAsset
    {
        public abstract bool Check();

        public static implicit operator bool(ConditionAsset conditionAsset)
        {
            return conditionAsset.Check();
        }
    }
}