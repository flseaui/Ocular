#if !RTSL_MAINTENANCE
using Battlehub.RTSL;
using ProtoBuf;
using UnityEngine;

namespace UnityEngine.Battlehub.SL2
{
    [CustomImplementation]
    public partial class PersistentParticleSystemNestedCustomDataModule
    {        

        [ProtoMember(1)]
        public ParticleSystemCustomDataMode m_mode1;

        [ProtoMember(2)]
        public ParticleSystemCustomDataMode m_mode2;

        [ProtoMember(3)]
        public ParticleSystem.MinMaxGradient m_color1;

        [ProtoMember(4)]
        public ParticleSystem.MinMaxGradient m_color2;

        [ProtoMember(5)]
        public int m_vectorComponentCount1;

        [ProtoMember(6)]
        public int m_vectorComponentCount2;

        [ProtoMember(7)]
        public ParticleSystem.MinMaxCurve[] m_vectors1;

        [ProtoMember(8)]
        public ParticleSystem.MinMaxCurve[] m_vectors2;

        public override object WriteTo(object obj)
        {
            obj = base.WriteTo(obj);
            if (obj == null)
            {
                return null;
            }

            ParticleSystem.CustomDataModule o = (ParticleSystem.CustomDataModule)obj;
            o.SetMode(ParticleSystemCustomData.Custom1, m_mode1);
            o.SetMode(ParticleSystemCustomData.Custom1, m_mode2);

            if(m_mode1 == ParticleSystemCustomDataMode.Color)
            {
                o.SetColor(ParticleSystemCustomData.Custom1, m_color1);
            }
            else if(m_mode1 == ParticleSystemCustomDataMode.Vector)
            {
                o.SetVectorComponentCount(ParticleSystemCustomData.Custom1, m_vectorComponentCount1);
                for (int i = 0; i < m_vectorComponentCount1; ++i)
                {
                    o.SetVector(ParticleSystemCustomData.Custom1, i, m_vectors1[i]);
                }
            }
            
            if(m_mode2 == ParticleSystemCustomDataMode.Color)
            {
                o.SetColor(ParticleSystemCustomData.Custom2, m_color2);
            }
            else if(m_mode2 == ParticleSystemCustomDataMode.Vector)
            {
                o.SetVectorComponentCount(ParticleSystemCustomData.Custom2, m_vectorComponentCount2);
                for (int i = 0; i < m_vectorComponentCount2; ++i)
                {
                    o.SetVector(ParticleSystemCustomData.Custom2, i, m_vectors2[i]);
                }
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

            ParticleSystem.CustomDataModule o = (ParticleSystem.CustomDataModule)obj;
            m_mode1 = o.GetMode(ParticleSystemCustomData.Custom1);
            m_mode2 = o.GetMode(ParticleSystemCustomData.Custom2);
            m_color1 = o.GetColor(ParticleSystemCustomData.Custom1);
            m_color2 = o.GetColor(ParticleSystemCustomData.Custom2);
            m_vectorComponentCount1 = o.GetVectorComponentCount(ParticleSystemCustomData.Custom1);
            m_vectorComponentCount2 = o.GetVectorComponentCount(ParticleSystemCustomData.Custom2);

            if(m_vectorComponentCount1 > 0)
            {
                m_vectors1 = new ParticleSystem.MinMaxCurve[m_vectorComponentCount1];
                for (int i = 0; i < m_vectors1.Length; ++i)
                {
                    m_vectors1[i] = o.GetVector(ParticleSystemCustomData.Custom1, i);
                }

            }

            if(m_vectorComponentCount2 > 0)
            {
                m_vectors2 = new ParticleSystem.MinMaxCurve[m_vectorComponentCount2];
                for (int i = 0; i < m_vectors2.Length; ++i)
                {
                    m_vectors2[i] = o.GetVector(ParticleSystemCustomData.Custom2, i);
                }
            }
        }

        public override void GetDeps(GetDepsContext context)
        {
            base.GetDeps(context);
            AddSurrogateDeps(m_vectors1, v_ => (PersistentParticleSystemNestedMinMaxCurve)v_, context);
            AddSurrogateDeps(m_vectors2, v_ => (PersistentParticleSystemNestedMinMaxCurve)v_, context);
        }

        public override void GetDepsFrom(object obj, GetDepsFromContext context)
        {
            base.GetDepsFrom(obj, context);
            ParticleSystem.CustomDataModule o = (ParticleSystem.CustomDataModule)obj;
           
            int count = o.GetVectorComponentCount(ParticleSystemCustomData.Custom1);
            for (int i = 0; i < count; ++i)
            {
                AddSurrogateDeps(o.GetVector(ParticleSystemCustomData.Custom1, i), v_ => (PersistentParticleSystemNestedMinMaxCurve)v_, context);
            }

            count = o.GetVectorComponentCount(ParticleSystemCustomData.Custom2);
            for (int i = 0; i < count; ++i)
            {
                AddSurrogateDeps(o.GetVector(ParticleSystemCustomData.Custom2, i), v_ => (PersistentParticleSystemNestedMinMaxCurve)v_, context);
            }
        }
    }
}
#endif

