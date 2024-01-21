using UnityEngine;

namespace MobX.Mediator.Pooling
{
    public class ParticleSystemPool : ComponentPool<ParticleSystem>
    {
        protected override void OnReleaseInstance(ParticleSystem instance)
        {
            instance.Stop(true);
            base.OnReleaseInstance(instance);
        }
    }
}
