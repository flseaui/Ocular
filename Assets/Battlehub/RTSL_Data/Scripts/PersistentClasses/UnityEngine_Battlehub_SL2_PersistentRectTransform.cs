using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using UnityEngine;
using UnityEngine.Battlehub.SL2;

using UnityObject = UnityEngine.Object;
namespace UnityEngine.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentRectTransform : PersistentTransform
    {
        [ProtoMember(256)]
        public PersistentVector2 anchorMin;

        [ProtoMember(257)]
        public PersistentVector2 anchorMax;

        [ProtoMember(258)]
        public PersistentVector2 anchoredPosition;

        [ProtoMember(259)]
        public PersistentVector2 sizeDelta;

        [ProtoMember(260)]
        public PersistentVector2 pivot;

        [ProtoMember(261)]
        public PersistentVector3 anchoredPosition3D;

        [ProtoMember(262)]
        public PersistentVector2 offsetMin;

        [ProtoMember(263)]
        public PersistentVector2 offsetMax;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            RectTransform uo = (RectTransform)obj;
            anchorMin = uo.anchorMin;
            anchorMax = uo.anchorMax;
            anchoredPosition = uo.anchoredPosition;
            sizeDelta = uo.sizeDelta;
            pivot = uo.pivot;
            anchoredPosition3D = uo.anchoredPosition3D;
            offsetMin = uo.offsetMin;
            offsetMax = uo.offsetMax;
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            RectTransform uo = (RectTransform)obj;
            uo.anchorMin = anchorMin;
            uo.anchorMax = anchorMax;
            uo.anchoredPosition = anchoredPosition;
            uo.sizeDelta = sizeDelta;
            uo.pivot = pivot;
            uo.anchoredPosition3D = anchoredPosition3D;
            uo.offsetMin = offsetMin;
            uo.offsetMax = offsetMax;
            return uo;
        }
    }
}

