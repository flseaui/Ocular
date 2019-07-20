using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using UnityEngine.UI;
using UnityEngine.UI.Battlehub.SL2;
using UnityEngine;
using System;
using UnityEngine.Battlehub.SL2;

using UnityObject = UnityEngine.Object;
namespace UnityEngine.UI.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentText : PersistentMaskableGraphic
    {
        [ProtoMember(258)]
        public long font;

        [ProtoMember(259)]
        public string text;

        [ProtoMember(260)]
        public bool supportRichText;

        [ProtoMember(261)]
        public bool resizeTextForBestFit;

        [ProtoMember(262)]
        public int resizeTextMinSize;

        [ProtoMember(263)]
        public int resizeTextMaxSize;

        [ProtoMember(264)]
        public TextAnchor alignment;

        [ProtoMember(265)]
        public bool alignByGeometry;

        [ProtoMember(266)]
        public int fontSize;

        [ProtoMember(267)]
        public HorizontalWrapMode horizontalOverflow;

        [ProtoMember(268)]
        public VerticalWrapMode verticalOverflow;

        [ProtoMember(269)]
        public float lineSpacing;

        [ProtoMember(270)]
        public FontStyle fontStyle;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            Text uo = (Text)obj;
            font = ToID(uo.font);
            text = uo.text;
            supportRichText = uo.supportRichText;
            resizeTextForBestFit = uo.resizeTextForBestFit;
            resizeTextMinSize = uo.resizeTextMinSize;
            resizeTextMaxSize = uo.resizeTextMaxSize;
            alignment = uo.alignment;
            alignByGeometry = uo.alignByGeometry;
            fontSize = uo.fontSize;
            horizontalOverflow = uo.horizontalOverflow;
            verticalOverflow = uo.verticalOverflow;
            lineSpacing = uo.lineSpacing;
            fontStyle = uo.fontStyle;
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            Text uo = (Text)obj;
            uo.font = FromID(font, uo.font);
            uo.text = text;
            uo.supportRichText = supportRichText;
            uo.resizeTextForBestFit = resizeTextForBestFit;
            uo.resizeTextMinSize = resizeTextMinSize;
            uo.resizeTextMaxSize = resizeTextMaxSize;
            uo.alignment = alignment;
            uo.alignByGeometry = alignByGeometry;
            uo.fontSize = fontSize;
            uo.horizontalOverflow = horizontalOverflow;
            uo.verticalOverflow = verticalOverflow;
            uo.lineSpacing = lineSpacing;
            uo.fontStyle = fontStyle;
            return uo;
        }

        protected override void GetDepsImpl(GetDepsContext context)
        {
            base.GetDepsImpl(context);
            AddDep(font, context);
        }

        protected override void GetDepsFromImpl(object obj, GetDepsFromContext context)
        {
            base.GetDepsFromImpl(obj, context);
            Text uo = (Text)obj;
            AddDep(uo.font, context);
        }
    }
}

