using System.Collections.Generic;
using UnityEngine;

namespace MobX.Platforms
{
    public class CreditsSectionConfiguration : ScriptableObject
    {
        [SerializeField] private string header;

        [SerializeField] private string title;

        [SerializeField] private List<CreditsPersonEntry> people;

        public string Header => header;
        public string Title => title;
        public List<CreditsPersonEntry> People => people;
    }
}
