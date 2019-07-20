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
    public partial class PersistentGradientColorKey : PersistentSurrogate
    {
        [ProtoMember(256)]
        public PersistentColor color;

        [ProtoMember(257)]
        public float time;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            GradientColorKey uo = (GradientColorKey)obj;
            color = uo.color;
            time = uo.time;
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            GradientColorKey uo = (GradientColorKey)obj;
            uo.color = color;
            uo.time = time;
            return uo;
        }

        public static implicit operator GradientColorKey(PersistentGradientColorKey surrogate)
        {
            if(surrogate == null) return default(GradientColorKey);
            return (GradientColorKey)surrogate.WriteTo(new GradientColorKey());
        }
        
        public static implicit operator PersistentGradientColorKey(GradientColorKey obj)
        {
            PersistentGradientColorKey surrogate = new PersistentGradientColorKey();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

