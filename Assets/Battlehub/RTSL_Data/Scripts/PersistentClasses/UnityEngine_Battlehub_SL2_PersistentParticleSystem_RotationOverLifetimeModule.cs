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
    public partial class PersistentParticleSystemNestedRotationOverLifetimeModule : PersistentSurrogate
    {
        [ProtoMember(256)]
        public bool enabled;

        [ProtoMember(257)]
        public PersistentParticleSystemNestedMinMaxCurve x;

        [ProtoMember(258)]
        public float xMultiplier;

        [ProtoMember(259)]
        public PersistentParticleSystemNestedMinMaxCurve y;

        [ProtoMember(260)]
        public float yMultiplier;

        [ProtoMember(261)]
        public PersistentParticleSystemNestedMinMaxCurve z;

        [ProtoMember(262)]
        public float zMultiplier;

        [ProtoMember(263)]
        public bool separateAxes;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            ParticleSystem.RotationOverLifetimeModule uo = (ParticleSystem.RotationOverLifetimeModule)obj;
            enabled = uo.enabled;
            x = uo.x;
            xMultiplier = uo.xMultiplier;
            y = uo.y;
            yMultiplier = uo.yMultiplier;
            z = uo.z;
            zMultiplier = uo.zMultiplier;
            separateAxes = uo.separateAxes;
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            ParticleSystem.RotationOverLifetimeModule uo = (ParticleSystem.RotationOverLifetimeModule)obj;
            uo.enabled = enabled;
            uo.x = x;
            uo.xMultiplier = xMultiplier;
            uo.y = y;
            uo.yMultiplier = yMultiplier;
            uo.z = z;
            uo.zMultiplier = zMultiplier;
            uo.separateAxes = separateAxes;
            return uo;
        }

        public static implicit operator ParticleSystem.RotationOverLifetimeModule(PersistentParticleSystemNestedRotationOverLifetimeModule surrogate)
        {
            if(surrogate == null) return default(ParticleSystem.RotationOverLifetimeModule);
            return (ParticleSystem.RotationOverLifetimeModule)surrogate.WriteTo(new ParticleSystem.RotationOverLifetimeModule());
        }
        
        public static implicit operator PersistentParticleSystemNestedRotationOverLifetimeModule(ParticleSystem.RotationOverLifetimeModule obj)
        {
            PersistentParticleSystemNestedRotationOverLifetimeModule surrogate = new PersistentParticleSystemNestedRotationOverLifetimeModule();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

