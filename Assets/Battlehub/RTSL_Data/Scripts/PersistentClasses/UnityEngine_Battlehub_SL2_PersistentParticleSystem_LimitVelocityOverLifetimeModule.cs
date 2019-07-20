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
    public partial class PersistentParticleSystemNestedLimitVelocityOverLifetimeModule : PersistentSurrogate
    {
        [ProtoMember(256)]
        public bool enabled;

        [ProtoMember(257)]
        public PersistentParticleSystemNestedMinMaxCurve limitX;

        [ProtoMember(258)]
        public float limitXMultiplier;

        [ProtoMember(259)]
        public PersistentParticleSystemNestedMinMaxCurve limitY;

        [ProtoMember(260)]
        public float limitYMultiplier;

        [ProtoMember(261)]
        public PersistentParticleSystemNestedMinMaxCurve limitZ;

        [ProtoMember(262)]
        public float limitZMultiplier;

        [ProtoMember(263)]
        public PersistentParticleSystemNestedMinMaxCurve limit;

        [ProtoMember(264)]
        public float limitMultiplier;

        [ProtoMember(265)]
        public float dampen;

        [ProtoMember(266)]
        public bool separateAxes;

        [ProtoMember(267)]
        public ParticleSystemSimulationSpace space;

        [ProtoMember(268)]
        public PersistentParticleSystemNestedMinMaxCurve drag;

        [ProtoMember(269)]
        public float dragMultiplier;

        [ProtoMember(270)]
        public bool multiplyDragByParticleSize;

        [ProtoMember(271)]
        public bool multiplyDragByParticleVelocity;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            ParticleSystem.LimitVelocityOverLifetimeModule uo = (ParticleSystem.LimitVelocityOverLifetimeModule)obj;
            enabled = uo.enabled;
            limitX = uo.limitX;
            limitXMultiplier = uo.limitXMultiplier;
            limitY = uo.limitY;
            limitYMultiplier = uo.limitYMultiplier;
            limitZ = uo.limitZ;
            limitZMultiplier = uo.limitZMultiplier;
            limit = uo.limit;
            limitMultiplier = uo.limitMultiplier;
            dampen = uo.dampen;
            separateAxes = uo.separateAxes;
            space = uo.space;
            drag = uo.drag;
            dragMultiplier = uo.dragMultiplier;
            multiplyDragByParticleSize = uo.multiplyDragByParticleSize;
            multiplyDragByParticleVelocity = uo.multiplyDragByParticleVelocity;
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            ParticleSystem.LimitVelocityOverLifetimeModule uo = (ParticleSystem.LimitVelocityOverLifetimeModule)obj;
            uo.enabled = enabled;
            uo.limitX = limitX;
            uo.limitXMultiplier = limitXMultiplier;
            uo.limitY = limitY;
            uo.limitYMultiplier = limitYMultiplier;
            uo.limitZ = limitZ;
            uo.limitZMultiplier = limitZMultiplier;
            uo.limit = limit;
            uo.limitMultiplier = limitMultiplier;
            uo.dampen = dampen;
            uo.separateAxes = separateAxes;
            uo.space = space;
            uo.drag = drag;
            uo.dragMultiplier = dragMultiplier;
            uo.multiplyDragByParticleSize = multiplyDragByParticleSize;
            uo.multiplyDragByParticleVelocity = multiplyDragByParticleVelocity;
            return uo;
        }

        public static implicit operator ParticleSystem.LimitVelocityOverLifetimeModule(PersistentParticleSystemNestedLimitVelocityOverLifetimeModule surrogate)
        {
            if(surrogate == null) return default(ParticleSystem.LimitVelocityOverLifetimeModule);
            return (ParticleSystem.LimitVelocityOverLifetimeModule)surrogate.WriteTo(new ParticleSystem.LimitVelocityOverLifetimeModule());
        }
        
        public static implicit operator PersistentParticleSystemNestedLimitVelocityOverLifetimeModule(ParticleSystem.LimitVelocityOverLifetimeModule obj)
        {
            PersistentParticleSystemNestedLimitVelocityOverLifetimeModule surrogate = new PersistentParticleSystemNestedLimitVelocityOverLifetimeModule();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

