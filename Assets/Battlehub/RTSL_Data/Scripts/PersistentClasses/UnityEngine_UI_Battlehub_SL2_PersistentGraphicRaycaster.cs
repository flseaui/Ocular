using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using UnityEngine.UI;
using UnityEngine.UI.Battlehub.SL2;
using UnityEngine.EventSystems.Battlehub.SL2;
using System;

using UnityObject = UnityEngine.Object;
namespace UnityEngine.UI.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentGraphicRaycaster : PersistentBaseRaycaster
    {
        [ProtoMember(258)]
        public bool ignoreReversedGraphics;

        [ProtoMember(259)]
        public GraphicRaycaster.BlockingObjects blockingObjects;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            GraphicRaycaster uo = (GraphicRaycaster)obj;
            ignoreReversedGraphics = uo.ignoreReversedGraphics;
            blockingObjects = uo.blockingObjects;
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            GraphicRaycaster uo = (GraphicRaycaster)obj;
            uo.ignoreReversedGraphics = ignoreReversedGraphics;
            uo.blockingObjects = blockingObjects;
            return uo;
        }
    }
}

