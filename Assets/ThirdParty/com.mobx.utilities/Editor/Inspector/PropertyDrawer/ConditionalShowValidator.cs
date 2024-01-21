using System;

namespace MobX.Utilities.Editor.Inspector.PropertyDrawer
{
    public static class ConditionalShowValidator
    {
        public static bool ValidateComparison(object lhs, Func<object> getRhs, bool negationCondition)
        {
            var convertedRhs = TryConvert(getRhs(), lhs);

            Type valueType = getRhs().GetType();

            return valueType.IsFlagsEnum()
                ? negationCondition
                    ? !convertedRhs.Cast<int>().HasFlagInt(lhs.Cast<int>())
                    : convertedRhs.Cast<int>().HasFlagInt(lhs.Cast<int>())
                : negationCondition
                    ? !convertedRhs.Equals(lhs)
                    : convertedRhs.Equals(lhs);
        }

        private static object TryConvert(object from, object to)
        {
            try
            {
                return Convert.ChangeType(from, to.GetType());
            }
            catch (Exception)
            {
                return from;
            }
        }
    }
}
