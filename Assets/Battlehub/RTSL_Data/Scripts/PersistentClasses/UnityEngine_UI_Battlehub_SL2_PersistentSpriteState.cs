using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using UnityEngine.UI;
using UnityEngine.UI.Battlehub.SL2;
using UnityEngine;
using System;

using UnityObject = UnityEngine.Object;
namespace UnityEngine.UI.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentSpriteState : PersistentSurrogate
    {
        [ProtoMember(256)]
        public long highlightedSprite;

        [ProtoMember(257)]
        public long pressedSprite;

        [ProtoMember(258)]
        public long disabledSprite;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            SpriteState uo = (SpriteState)obj;
            highlightedSprite = ToID(uo.highlightedSprite);
            pressedSprite = ToID(uo.pressedSprite);
            disabledSprite = ToID(uo.disabledSprite);
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            SpriteState uo = (SpriteState)obj;
            uo.highlightedSprite = FromID(highlightedSprite, uo.highlightedSprite);
            uo.pressedSprite = FromID(pressedSprite, uo.pressedSprite);
            uo.disabledSprite = FromID(disabledSprite, uo.disabledSprite);
            return uo;
        }

        protected override void GetDepsImpl(GetDepsContext context)
        {
            base.GetDepsImpl(context);
            AddDep(highlightedSprite, context);
            AddDep(pressedSprite, context);
            AddDep(disabledSprite, context);
        }

        protected override void GetDepsFromImpl(object obj, GetDepsFromContext context)
        {
            base.GetDepsFromImpl(obj, context);
            SpriteState uo = (SpriteState)obj;
            AddDep(uo.highlightedSprite, context);
            AddDep(uo.pressedSprite, context);
            AddDep(uo.disabledSprite, context);
        }

        public static implicit operator SpriteState(PersistentSpriteState surrogate)
        {
            if(surrogate == null) return default(SpriteState);
            return (SpriteState)surrogate.WriteTo(new SpriteState());
        }
        
        public static implicit operator PersistentSpriteState(SpriteState obj)
        {
            PersistentSpriteState surrogate = new PersistentSpriteState();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

