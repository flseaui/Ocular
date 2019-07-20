using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using Battlehub.Cubeman;
using Battlehub.Cubeman.Battlehub.SL2;
using UnityEngine.Battlehub.SL2;
using UnityEngine.UI;
using System;
using UnityEngine;

using UnityObject = UnityEngine.Object;
namespace Battlehub.Cubeman.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentCubemenGame : PersistentMonoBehaviour
    {
        [ProtoMember(256)]
        public long TxtScore;

        [ProtoMember(257)]
        public long TxtCompleted;

        [ProtoMember(258)]
        public long TxtTip;

        [ProtoMember(259)]
        public long BtnReplay;

        [ProtoMember(260)]
        public long GameUI;

        [ProtoMember(262)]
        public int CurrentIndex;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            CubemenGame uo = (CubemenGame)obj;
            TxtScore = ToID(uo.TxtScore);
            TxtCompleted = ToID(uo.TxtCompleted);
            TxtTip = ToID(uo.TxtTip);
            BtnReplay = ToID(uo.BtnReplay);
            GameUI = ToID(uo.GameUI);
            CurrentIndex = uo.CurrentIndex;
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            CubemenGame uo = (CubemenGame)obj;
            uo.TxtScore = FromID(TxtScore, uo.TxtScore);
            uo.TxtCompleted = FromID(TxtCompleted, uo.TxtCompleted);
            uo.TxtTip = FromID(TxtTip, uo.TxtTip);
            uo.BtnReplay = FromID(BtnReplay, uo.BtnReplay);
            uo.GameUI = FromID(GameUI, uo.GameUI);
            uo.CurrentIndex = CurrentIndex;
            return uo;
        }

        protected override void GetDepsImpl(GetDepsContext context)
        {
            base.GetDepsImpl(context);
            AddDep(TxtScore, context);
            AddDep(TxtCompleted, context);
            AddDep(TxtTip, context);
            AddDep(BtnReplay, context);
            AddDep(GameUI, context);
        }

        protected override void GetDepsFromImpl(object obj, GetDepsFromContext context)
        {
            base.GetDepsFromImpl(obj, context);
            CubemenGame uo = (CubemenGame)obj;
            AddDep(uo.TxtScore, context);
            AddDep(uo.TxtCompleted, context);
            AddDep(uo.TxtTip, context);
            AddDep(uo.BtnReplay, context);
            AddDep(uo.GameUI, context);
        }
    }
}

