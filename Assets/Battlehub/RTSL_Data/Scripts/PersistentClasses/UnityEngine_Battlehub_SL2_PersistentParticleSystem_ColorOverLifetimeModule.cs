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
    public partial class PersistentParticleSystemNestedColorOverLifetimeModule : PersistentSurrogate
    {
        [ProtoMember(256)]
        public bool enabled;

        [ProtoMember(257)]
        public PersistentParticleSystemNestedMinMaxGradient color;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            ParticleSystem.ColorOverLifetimeModule uo = (ParticleSystem.ColorOverLifetimeModule)obj;
            enabled = uo.enabled;
            color = uo.color;
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            ParticleSystem.ColorOverLifetimeModule uo = (ParticleSystem.ColorOverLifetimeModule)obj;
            uo.enabled = enabled;
            uo.color = color;
            return uo;
        }

        public static implicit operator ParticleSystem.ColorOverLifetimeModule(PersistentParticleSystemNestedColorOverLifetimeModule surrogate)
        {
            if(surrogate == null) return default(ParticleSystem.ColorOverLifetimeModule);
            return (ParticleSystem.ColorOverLifetimeModule)surrogate.WriteTo(new ParticleSystem.ColorOverLifetimeModule());
        }
        
        public static implicit operator PersistentParticleSystemNestedColorOverLifetimeModule(ParticleSystem.ColorOverLifetimeModule obj)
        {
            PersistentParticleSystemNestedColorOverLifetimeModule surrogate = new PersistentParticleSystemNestedColorOverLifetimeModule();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

