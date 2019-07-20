using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using UnityEngine;
using UnityEngine.Battlehub.SL2;

using UnityObject = UnityEngine.Object;
namespace UnityEngine.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentBoxCollider : PersistentCollider
    {
        [ProtoMember(256)]
        public PersistentVector3 center;

        [ProtoMember(257)]
        public PersistentVector3 size;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            BoxCollider uo = (BoxCollider)obj;
            center = uo.center;
            size = uo.size;
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            BoxCollider uo = (BoxCollider)obj;
            uo.center = center;
            uo.size = size;
            return uo;
        }
    }
}

