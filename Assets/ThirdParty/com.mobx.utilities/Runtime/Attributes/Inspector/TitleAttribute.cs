using System;
using UnityEngine;

namespace MobX.Utilities.Inspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class TitleAttribute : HeaderAttribute
    {
        public TitleAttribute(string header) : base(header)
        {
        }
    }
}