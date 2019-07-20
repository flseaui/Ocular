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
    public partial class PersistentParticleSystemNestedSizeOverLifetimeModule : PersistentSurrogate
    {
        [ProtoMember(256)]
        public bool enabled;

        [ProtoMember(257)]
        public PersistentParticleSystemNestedMinMaxCurve size;

        [ProtoMember(258)]
        public float sizeMultiplier;

        [ProtoMember(259)]
        public PersistentParticleSystemNestedMinMaxCurve x;

        [ProtoMember(260)]
        public float xMultiplier;

        [ProtoMember(261)]
        public PersistentParticleSystemNestedMinMaxCurve y;

        [ProtoMember(262)]
        public float yMultiplier;

        [ProtoMember(263)]
        public PersistentParticleSystemNestedMinMaxCurve z;

        [ProtoMember(264)]
        public float zMultiplier;

        [ProtoMember(265)]
        public bool separateAxes;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            ParticleSystem.SizeOverLifetimeModule uo = (ParticleSystem.SizeOverLifetimeModule)obj;
            enabled = uo.enabled;
            size = uo.size;
            sizeMultiplier = uo.sizeMultiplier;
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
            ParticleSystem.SizeOverLifetimeModule uo = (ParticleSystem.SizeOverLifetimeModule)obj;
            uo.enabled = enabled;
            uo.size = size;
            uo.sizeMultiplier = sizeMultiplier;
            uo.x = x;
            uo.xMultiplier = xMultiplier;
            uo.y = y;
            uo.yMultiplier = yMultiplier;
            uo.z = z;
            uo.zMultiplier = zMultiplier;
            uo.separateAxes = separateAxes;
            return uo;
        }

        public static implicit operator ParticleSystem.SizeOverLifetimeModule(PersistentParticleSystemNestedSizeOverLifetimeModule surrogate)
        {
            if(surrogate == null) return default(ParticleSystem.SizeOverLifetimeModule);
            return (ParticleSystem.SizeOverLifetimeModule)surrogate.WriteTo(new ParticleSystem.SizeOverLifetimeModule());
        }
        
        public static implicit operator PersistentParticleSystemNestedSizeOverLifetimeModule(ParticleSystem.SizeOverLifetimeModule obj)
        {
            PersistentParticleSystemNestedSizeOverLifetimeModule surrogate = new PersistentParticleSystemNestedSizeOverLifetimeModule();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

