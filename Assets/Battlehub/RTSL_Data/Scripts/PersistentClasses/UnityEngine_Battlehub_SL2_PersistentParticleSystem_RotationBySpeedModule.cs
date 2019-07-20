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
    public partial class PersistentParticleSystemNestedRotationBySpeedModule : PersistentSurrogate
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

        [ProtoMember(264)]
        public PersistentVector2 range;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            ParticleSystem.RotationBySpeedModule uo = (ParticleSystem.RotationBySpeedModule)obj;
            enabled = uo.enabled;
            x = uo.x;
            xMultiplier = uo.xMultiplier;
            y = uo.y;
            yMultiplier = uo.yMultiplier;
            z = uo.z;
            zMultiplier = uo.zMultiplier;
            separateAxes = uo.separateAxes;
            range = uo.range;
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            ParticleSystem.RotationBySpeedModule uo = (ParticleSystem.RotationBySpeedModule)obj;
            uo.enabled = enabled;
            uo.x = x;
            uo.xMultiplier = xMultiplier;
            uo.y = y;
            uo.yMultiplier = yMultiplier;
            uo.z = z;
            uo.zMultiplier = zMultiplier;
            uo.separateAxes = separateAxes;
            uo.range = range;
            return uo;
        }

        public static implicit operator ParticleSystem.RotationBySpeedModule(PersistentParticleSystemNestedRotationBySpeedModule surrogate)
        {
            if(surrogate == null) return default(ParticleSystem.RotationBySpeedModule);
            return (ParticleSystem.RotationBySpeedModule)surrogate.WriteTo(new ParticleSystem.RotationBySpeedModule());
        }
        
        public static implicit operator PersistentParticleSystemNestedRotationBySpeedModule(ParticleSystem.RotationBySpeedModule obj)
        {
            PersistentParticleSystemNestedRotationBySpeedModule surrogate = new PersistentParticleSystemNestedRotationBySpeedModule();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

