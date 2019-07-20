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
    public partial class PersistentMaterial : PersistentObject
    {
        [ProtoMember(256)]
        public int renderQueue;

        [ProtoMember(257)]
        public MaterialGlobalIlluminationFlags globalIlluminationFlags;

        [ProtoMember(258)]
        public bool doubleSidedGI;

        [ProtoMember(259)]
        public bool enableInstancing;

        [ProtoMember(260)]
        public string[] shaderKeywords;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            Material uo = (Material)obj;
            renderQueue = uo.renderQueue;
            globalIlluminationFlags = uo.globalIlluminationFlags;
            doubleSidedGI = uo.doubleSidedGI;
            enableInstancing = uo.enableInstancing;
            shaderKeywords = uo.shaderKeywords;
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            Material uo = (Material)obj;
            uo.renderQueue = renderQueue;
            uo.globalIlluminationFlags = globalIlluminationFlags;
            uo.doubleSidedGI = doubleSidedGI;
            uo.enableInstancing = enableInstancing;
            uo.shaderKeywords = shaderKeywords;
            return uo;
        }
    }
}

