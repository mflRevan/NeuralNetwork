using JetBrains.Annotations;
using MobX.Utilities.Callbacks;
using MobX.Utilities.Inspector;
using UnityEngine;

namespace MobX.Mediator
{
    [SearchField(Enabled = false)]
    public class MediatorAsset : RuntimeAsset
    {
#pragma warning disable
        [TextArea]
        [DrawLineAfter]
        [UsedImplicitly]
        [SerializeField] private string description;
#pragma warning restore
    }
}