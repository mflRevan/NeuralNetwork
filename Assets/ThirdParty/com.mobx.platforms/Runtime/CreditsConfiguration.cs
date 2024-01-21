using System.Collections.Generic;
using UnityEngine;

namespace MobX.Platforms
{
    /// <summary>
    ///     Type for creating a particular set of credits
    /// </summary>
    [CreateAssetMenu(menuName = "EVERSLAUGHT/Configs/Credits")]
    public class CreditsConfiguration : ScriptableObject
    {
        [SerializeField] private List<CreditsSectionConfiguration> sections;
        public List<CreditsSectionConfiguration> Sections => sections;
    }
}
