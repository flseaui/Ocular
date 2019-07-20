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
    public partial class PersistentNavigation : PersistentSurrogate
    {
        [ProtoMember(256)]
        public Navigation.Mode mode;

        [ProtoMember(257)]
        public long selectOnUp;

        [ProtoMember(258)]
        public long selectOnDown;

        [ProtoMember(259)]
        public long selectOnLeft;

        [ProtoMember(260)]
        public long selectOnRight;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            Navigation uo = (Navigation)obj;
            mode = uo.mode;
            selectOnUp = ToID(uo.selectOnUp);
            selectOnDown = ToID(uo.selectOnDown);
            selectOnLeft = ToID(uo.selectOnLeft);
            selectOnRight = ToID(uo.selectOnRight);
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            Navigation uo = (Navigation)obj;
            uo.mode = mode;
            uo.selectOnUp = FromID(selectOnUp, uo.selectOnUp);
            uo.selectOnDown = FromID(selectOnDown, uo.selectOnDown);
            uo.selectOnLeft = FromID(selectOnLeft, uo.selectOnLeft);
            uo.selectOnRight = FromID(selectOnRight, uo.selectOnRight);
            return uo;
        }

        protected override void GetDepsImpl(GetDepsContext context)
        {
            base.GetDepsImpl(context);
            AddDep(selectOnUp, context);
            AddDep(selectOnDown, context);
            AddDep(selectOnLeft, context);
            AddDep(selectOnRight, context);
        }

        protected override void GetDepsFromImpl(object obj, GetDepsFromContext context)
        {
            base.GetDepsFromImpl(obj, context);
            Navigation uo = (Navigation)obj;
            AddDep(uo.selectOnUp, context);
            AddDep(uo.selectOnDown, context);
            AddDep(uo.selectOnLeft, context);
            AddDep(uo.selectOnRight, context);
        }

        public static implicit operator Navigation(PersistentNavigation surrogate)
        {
            if(surrogate == null) return default(Navigation);
            return (Navigation)surrogate.WriteTo(new Navigation());
        }
        
        public static implicit operator PersistentNavigation(Navigation obj)
        {
            PersistentNavigation surrogate = new PersistentNavigation();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

