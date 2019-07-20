using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using UnityEngine.EventSystems;
using UnityEngine.EventSystems.Battlehub.SL2;
using System;

using UnityObject = UnityEngine.Object;
namespace UnityEngine.EventSystems.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentStandaloneInputModule : PersistentPointerInputModule
    {
        [ProtoMember(264)]
        public bool forceModuleActive;

        [ProtoMember(265)]
        public float inputActionsPerSecond;

        [ProtoMember(266)]
        public float repeatDelay;

        [ProtoMember(267)]
        public string horizontalAxis;

        [ProtoMember(268)]
        public string verticalAxis;

        [ProtoMember(269)]
        public string submitButton;

        [ProtoMember(270)]
        public string cancelButton;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            StandaloneInputModule uo = (StandaloneInputModule)obj;
            forceModuleActive = uo.forceModuleActive;
            inputActionsPerSecond = uo.inputActionsPerSecond;
            repeatDelay = uo.repeatDelay;
            horizontalAxis = uo.horizontalAxis;
            verticalAxis = uo.verticalAxis;
            submitButton = uo.submitButton;
            cancelButton = uo.cancelButton;
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            StandaloneInputModule uo = (StandaloneInputModule)obj;
            uo.forceModuleActive = forceModuleActive;
            uo.inputActionsPerSecond = inputActionsPerSecond;
            uo.repeatDelay = repeatDelay;
            uo.horizontalAxis = horizontalAxis;
            uo.verticalAxis = verticalAxis;
            uo.submitButton = submitButton;
            uo.cancelButton = cancelButton;
            return uo;
        }
    }
}

