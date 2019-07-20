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
    public partial class PersistentSelectable : PersistentUIBehaviour
    {
        [ProtoMember(262)]
        public PersistentNavigation navigation;

        [ProtoMember(263)]
        public Selectable.Transition transition;

        [ProtoMember(264)]
        public PersistentColorBlock colors;

        [ProtoMember(265)]
        public PersistentSpriteState spriteState;

        [ProtoMember(266)]
        public PersistentAnimationTriggers animationTriggers;

        [ProtoMember(267)]
        public long targetGraphic;

        [ProtoMember(268)]
        public bool interactable;

        [ProtoMember(269)]
        public long image;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            Selectable uo = (Selectable)obj;
            navigation = uo.navigation;
            transition = uo.transition;
            colors = uo.colors;
            spriteState = uo.spriteState;
            animationTriggers = uo.animationTriggers;
            targetGraphic = ToID(uo.targetGraphic);
            interactable = uo.interactable;
            image = ToID(uo.image);
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            Selectable uo = (Selectable)obj;
            uo.navigation = navigation;
            uo.transition = transition;
            uo.colors = colors;
            uo.spriteState = spriteState;
            uo.animationTriggers = animationTriggers;
            uo.targetGraphic = FromID(targetGraphic, uo.targetGraphic);
            uo.interactable = interactable;
            uo.image = FromID(image, uo.image);
            return uo;
        }

        protected override void GetDepsImpl(GetDepsContext context)
        {
            base.GetDepsImpl(context);
            AddSurrogateDeps(navigation, context);
            AddSurrogateDeps(spriteState, context);
            AddDep(targetGraphic, context);
            AddDep(image, context);
        }

        protected override void GetDepsFromImpl(object obj, GetDepsFromContext context)
        {
            base.GetDepsFromImpl(obj, context);
            Selectable uo = (Selectable)obj;
            AddSurrogateDeps(uo.navigation, v_ => (PersistentNavigation)v_, context);
            AddSurrogateDeps(uo.spriteState, v_ => (PersistentSpriteState)v_, context);
            AddDep(uo.targetGraphic, context);
            AddDep(uo.image, context);
        }
    }
}

