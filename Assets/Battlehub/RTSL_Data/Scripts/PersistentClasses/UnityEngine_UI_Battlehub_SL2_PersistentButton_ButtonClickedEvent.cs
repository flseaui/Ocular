using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using UnityEngine.UI;
using UnityEngine.UI.Battlehub.SL2;
using UnityEngine.Events.Battlehub.SL2;

using UnityObject = UnityEngine.Object;
namespace UnityEngine.UI.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentButtonNestedButtonClickedEvent : PersistentUnityEvent
    {
        
        public static implicit operator Button.ButtonClickedEvent(PersistentButtonNestedButtonClickedEvent surrogate)
        {
            if(surrogate == null) return default(Button.ButtonClickedEvent);
            return (Button.ButtonClickedEvent)surrogate.WriteTo(new Button.ButtonClickedEvent());
        }
        
        public static implicit operator PersistentButtonNestedButtonClickedEvent(Button.ButtonClickedEvent obj)
        {
            PersistentButtonNestedButtonClickedEvent surrogate = new PersistentButtonNestedButtonClickedEvent();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

