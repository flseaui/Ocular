using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using UnityEngine;
using UnityEngine.Battlehub.SL2;
using System;

using UnityObject = UnityEngine.Object;
namespace UnityEngine.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentParticleSystemNestedEmissionModule : PersistentSurrogate
    {
        [ProtoMember(256)]
        public bool enabled;

        [ProtoMember(257)]
        public PersistentParticleSystemNestedMinMaxCurve rateOverTime;

        [ProtoMember(258)]
        public float rateOverTimeMultiplier;

        [ProtoMember(259)]
        public PersistentParticleSystemNestedMinMaxCurve rateOverDistance;

        [ProtoMember(260)]
        public float rateOverDistanceMultiplier;

        [ProtoMember(261)]
        public int burstCount;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            ParticleSystem.EmissionModule uo = (ParticleSystem.EmissionModule)obj;
            enabled = uo.enabled;
            rateOverTime = uo.rateOverTime;
            rateOverTimeMultiplier = uo.rateOverTimeMultiplier;
            rateOverDistance = uo.rateOverDistance;
            rateOverDistanceMultiplier = uo.rateOverDistanceMultiplier;
            burstCount = uo.burstCount;
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            ParticleSystem.EmissionModule uo = (ParticleSystem.EmissionModule)obj;
            uo.enabled = enabled;
            uo.rateOverTime = rateOverTime;
            uo.rateOverTimeMultiplier = rateOverTimeMultiplier;
            uo.rateOverDistance = rateOverDistance;
            uo.rateOverDistanceMultiplier = rateOverDistanceMultiplier;
            uo.burstCount = burstCount;
            return uo;
        }

        public static implicit operator ParticleSystem.EmissionModule(PersistentParticleSystemNestedEmissionModule surrogate)
        {
            if(surrogate == null) return default(ParticleSystem.EmissionModule);
            return (ParticleSystem.EmissionModule)surrogate.WriteTo(new ParticleSystem.EmissionModule());
        }
        
        public static implicit operator PersistentParticleSystemNestedEmissionModule(ParticleSystem.EmissionModule obj)
        {
            PersistentParticleSystemNestedEmissionModule surrogate = new PersistentParticleSystemNestedEmissionModule();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

