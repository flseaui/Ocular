#if !RTSL_MAINTENANCE
using Battlehub.RTSL;
using ProtoBuf;
using UnityEngine;

namespace UnityEngine.Battlehub.SL2
{
    [CustomImplementation]
    public partial class PersistentParticleSystemNestedEmissionModule
    {        

        [ProtoMember(1)]
        public ParticleSystem.Burst[] m_bursts;

        public override object WriteTo(object obj)
        {
            obj = base.WriteTo(obj);
            if (obj == null)
            {
                return null;
            }

            ParticleSystem.EmissionModule o = (ParticleSystem.EmissionModule)obj;       
            
            if(m_bursts != null)
            {
                o.SetBursts(m_bursts);
            }
            else
            {
                o.SetBursts(new ParticleSystem.Burst[0]);
            }

            return obj;
        }

        public override void ReadFrom(object obj)
        {
            base.ReadFrom(obj);
            if (obj == null)
            {
                return;
            }

            ParticleSystem.EmissionModule o = (ParticleSystem.EmissionModule)obj;

            m_bursts = new ParticleSystem.Burst[o.burstCount];
            o.GetBursts(m_bursts);
        }

        public override void GetDeps(GetDepsContext context)
        {
            base.GetDeps(context);

            AddSurrogateDeps(m_bursts, v_ => (PersistentParticleSystemNestedBurst)v_, context);
        }

        public override void GetDepsFrom(object obj, GetDepsFromContext context)
        {
            base.GetDepsFrom(obj, context);
            ParticleSystem.EmissionModule o = (ParticleSystem.EmissionModule)obj;

            ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[o.burstCount];
            o.GetBursts(bursts);
            AddSurrogateDeps(bursts, v_ => (PersistentParticleSystemNestedBurst)v_, context);
        }
    }
}
#endif

