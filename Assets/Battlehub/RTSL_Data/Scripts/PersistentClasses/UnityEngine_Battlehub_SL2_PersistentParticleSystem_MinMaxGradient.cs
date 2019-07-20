using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using UnityEngine;
using UnityEngine.Battlehub.SL2;

using UnityObject = UnityEngine.Object;
namespace UnityEngine.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentParticleSystemNestedMinMaxGradient : PersistentSurrogate
    {
        [ProtoMember(256)]
        public ParticleSystemGradientMode mode;

        [ProtoMember(257)]
        public PersistentGradient gradientMax;

        [ProtoMember(258)]
        public PersistentGradient gradientMin;

        [ProtoMember(259)]
        public PersistentColor colorMax;

        [ProtoMember(260)]
        public PersistentColor colorMin;

        [ProtoMember(261)]
        public PersistentColor color;

        [ProtoMember(262)]
        public PersistentGradient gradient;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            ParticleSystem.MinMaxGradient uo = (ParticleSystem.MinMaxGradient)obj;
            mode = uo.mode;
            gradientMax = uo.gradientMax;
            gradientMin = uo.gradientMin;
            colorMax = uo.colorMax;
            colorMin = uo.colorMin;
            color = uo.color;
            gradient = uo.gradient;
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            ParticleSystem.MinMaxGradient uo = (ParticleSystem.MinMaxGradient)obj;
            uo.mode = mode;
            uo.gradientMax = gradientMax;
            uo.gradientMin = gradientMin;
            uo.colorMax = colorMax;
            uo.colorMin = colorMin;
            uo.color = color;
            uo.gradient = gradient;
            return uo;
        }

        public static implicit operator ParticleSystem.MinMaxGradient(PersistentParticleSystemNestedMinMaxGradient surrogate)
        {
            if(surrogate == null) return default(ParticleSystem.MinMaxGradient);
            return (ParticleSystem.MinMaxGradient)surrogate.WriteTo(new ParticleSystem.MinMaxGradient());
        }
        
        public static implicit operator PersistentParticleSystemNestedMinMaxGradient(ParticleSystem.MinMaxGradient obj)
        {
            PersistentParticleSystemNestedMinMaxGradient surrogate = new PersistentParticleSystemNestedMinMaxGradient();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

