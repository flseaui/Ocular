using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using UnityEngine;
using UnityEngine.Battlehub.SL2;

using UnityObject = UnityEngine.Object;
namespace UnityEngine.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentLightBakingOutput : PersistentSurrogate
    {
        
        public static implicit operator LightBakingOutput(PersistentLightBakingOutput surrogate)
        {
            if(surrogate == null) return default(LightBakingOutput);
            return (LightBakingOutput)surrogate.WriteTo(new LightBakingOutput());
        }
        
        public static implicit operator PersistentLightBakingOutput(LightBakingOutput obj)
        {
            PersistentLightBakingOutput surrogate = new PersistentLightBakingOutput();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

