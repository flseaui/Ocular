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
    public partial class PersistentAnimator : PersistentBehaviour
    {
        [ProtoMember(256)]
        public PersistentVector3 rootPosition;

        [ProtoMember(257)]
        public PersistentQuaternion rootRotation;

        [ProtoMember(258)]
        public bool applyRootMotion;

        [ProtoMember(261)]
        public AnimatorUpdateMode updateMode;

        [ProtoMember(264)]
        public bool stabilizeFeet;

        [ProtoMember(265)]
        public float feetPivotActive;

        [ProtoMember(266)]
        public float speed;

        [ProtoMember(267)]
        public AnimatorCullingMode cullingMode;

        [ProtoMember(269)]
        public float recorderStartTime;

        [ProtoMember(270)]
        public float recorderStopTime;

        [ProtoMember(271)]
        public long runtimeAnimatorController;

        [ProtoMember(272)]
        public long avatar;

        [ProtoMember(273)]
        public bool layersAffectMassCenter;

        [ProtoMember(274)]
        public bool logWarnings;

        [ProtoMember(275)]
        public bool fireEvents;

        [ProtoMember(276)]
        public bool keepAnimatorControllerStateOnDisable;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            Animator uo = (Animator)obj;
            rootPosition = uo.rootPosition;
            rootRotation = uo.rootRotation;
            applyRootMotion = uo.applyRootMotion;
            updateMode = uo.updateMode;
            stabilizeFeet = uo.stabilizeFeet;
            feetPivotActive = uo.feetPivotActive;
            speed = uo.speed;
            cullingMode = uo.cullingMode;
            recorderStartTime = uo.recorderStartTime;
            recorderStopTime = uo.recorderStopTime;
            runtimeAnimatorController = ToID(uo.runtimeAnimatorController);
            avatar = ToID(uo.avatar);
            layersAffectMassCenter = uo.layersAffectMassCenter;
            logWarnings = uo.logWarnings;
            fireEvents = uo.fireEvents;
            keepAnimatorControllerStateOnDisable = uo.keepAnimatorControllerStateOnDisable;
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            Animator uo = (Animator)obj;
            uo.rootPosition = rootPosition;
            uo.rootRotation = rootRotation;
            uo.applyRootMotion = applyRootMotion;
            uo.updateMode = updateMode;
            uo.stabilizeFeet = stabilizeFeet;
            uo.feetPivotActive = feetPivotActive;
            uo.speed = speed;
            uo.cullingMode = cullingMode;
            uo.recorderStartTime = recorderStartTime;
            uo.recorderStopTime = recorderStopTime;
            uo.runtimeAnimatorController = FromID(runtimeAnimatorController, uo.runtimeAnimatorController);
            uo.avatar = FromID(avatar, uo.avatar);
            uo.layersAffectMassCenter = layersAffectMassCenter;
            uo.logWarnings = logWarnings;
            uo.fireEvents = fireEvents;
            uo.keepAnimatorControllerStateOnDisable = keepAnimatorControllerStateOnDisable;
            return uo;
        }

        protected override void GetDepsImpl(GetDepsContext context)
        {
            base.GetDepsImpl(context);
            AddDep(runtimeAnimatorController, context);
            AddDep(avatar, context);
        }

        protected override void GetDepsFromImpl(object obj, GetDepsFromContext context)
        {
            base.GetDepsFromImpl(obj, context);
            Animator uo = (Animator)obj;
            AddDep(uo.runtimeAnimatorController, context);
            AddDep(uo.avatar, context);
        }
    }
}

