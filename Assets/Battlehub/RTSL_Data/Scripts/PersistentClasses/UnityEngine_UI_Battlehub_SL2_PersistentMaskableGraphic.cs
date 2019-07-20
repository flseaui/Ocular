using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using UnityEngine.UI;
using UnityEngine.UI.Battlehub.SL2;
using System;

using UnityObject = UnityEngine.Object;
namespace UnityEngine.UI.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentMaskableGraphic : PersistentGraphic
    {
        [ProtoMember(256)]
        public PersistentMaskableGraphicNestedCullStateChangedEvent onCullStateChanged;

        [ProtoMember(257)]
        public bool maskable;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            MaskableGraphic uo = (MaskableGraphic)obj;
            onCullStateChanged = uo.onCullStateChanged;
            maskable = uo.maskable;
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            MaskableGraphic uo = (MaskableGraphic)obj;
            uo.onCullStateChanged = onCullStateChanged;
            uo.maskable = maskable;
            return uo;
        }

        protected override void GetDepsImpl(GetDepsContext context)
        {
            base.GetDepsImpl(context);
            AddSurrogateDeps(onCullStateChanged, context);
        }

        protected override void GetDepsFromImpl(object obj, GetDepsFromContext context)
        {
            base.GetDepsFromImpl(obj, context);
            MaskableGraphic uo = (MaskableGraphic)obj;
            AddSurrogateDeps(uo.onCullStateChanged, v_ => (PersistentMaskableGraphicNestedCullStateChangedEvent)v_, context);
        }
    }
}

