using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using Battlehub.RTCommon;
using Battlehub.RTCommon.Battlehub.SL2;
using UnityEngine.Events.Battlehub.SL2;

using UnityObject = UnityEngine.Object;
namespace Battlehub.RTCommon.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentExposeToEditorUnityEvent : PersistentUnityEventBase
    {
        
        public static implicit operator ExposeToEditorUnityEvent(PersistentExposeToEditorUnityEvent surrogate)
        {
            if(surrogate == null) return default(ExposeToEditorUnityEvent);
            return (ExposeToEditorUnityEvent)surrogate.WriteTo(new ExposeToEditorUnityEvent());
        }
        
        public static implicit operator PersistentExposeToEditorUnityEvent(ExposeToEditorUnityEvent obj)
        {
            PersistentExposeToEditorUnityEvent surrogate = new PersistentExposeToEditorUnityEvent();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

