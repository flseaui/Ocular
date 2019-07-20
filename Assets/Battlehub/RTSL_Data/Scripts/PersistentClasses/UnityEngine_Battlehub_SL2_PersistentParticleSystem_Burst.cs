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
    public partial class PersistentParticleSystemNestedBurst : PersistentSurrogate
    {
        [ProtoMember(256)]
        public float time;

        [ProtoMember(257)]
        public PersistentParticleSystemNestedMinMaxCurve count;

        [ProtoMember(258)]
        public short minCount;

        [ProtoMember(259)]
        public short maxCount;

        [ProtoMember(260)]
        public int cycleCount;

        [ProtoMember(261)]
        public float repeatInterval;

        [ProtoMember(262)]
        public float probability;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            ParticleSystem.Burst uo = (ParticleSystem.Burst)obj;
            time = uo.time;
            count = uo.count;
            minCount = uo.minCount;
            maxCount = uo.maxCount;
            cycleCount = uo.cycleCount;
            repeatInterval = uo.repeatInterval;
            probability = uo.probability;
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            ParticleSystem.Burst uo = (ParticleSystem.Burst)obj;
            uo.time = time;
            uo.count = count;
            uo.minCount = minCount;
            uo.maxCount = maxCount;
            uo.cycleCount = cycleCount;
            uo.repeatInterval = repeatInterval;
            uo.probability = probability;
            return uo;
        }

        public static implicit operator ParticleSystem.Burst(PersistentParticleSystemNestedBurst surrogate)
        {
            if(surrogate == null) return default(ParticleSystem.Burst);
            return (ParticleSystem.Burst)surrogate.WriteTo(new ParticleSystem.Burst());
        }
        
        public static implicit operator PersistentParticleSystemNestedBurst(ParticleSystem.Burst obj)
        {
            PersistentParticleSystemNestedBurst surrogate = new PersistentParticleSystemNestedBurst();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

