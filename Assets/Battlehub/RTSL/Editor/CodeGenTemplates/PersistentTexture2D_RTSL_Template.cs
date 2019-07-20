//#define RTSL_COMPILE_TEMPLATES
#if RTSL_COMPILE_TEMPLATES
//<TEMPLATE_USINGS_START>
using Battlehub.RTCommon;
using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;
//<TEMPLATE_USINGS_END>
#else
using UnityEngine;
#endif

namespace Battlehub.RTSL.Internal
{
    [PersistentTemplate("UnityEngine.Texture2D")]
    public class PersistentTexture2D_RTSL_Template : PersistentSurrogateTemplate
    {
#if RTSL_COMPILE_TEMPLATES
        //<TEMPLATE_BODY_START>

        [ProtoMember(1)]
        public byte[] m_bytes;

        [ProtoMember(2)]
        public bool m_isReadable;

        public override void ReadFrom(object obj)
        {
            base.ReadFrom(obj);

            Texture2D texture = (Texture2D)obj;
            if (texture == null)
            {
                return;
            }

            try
            {
                m_bytes = texture.EncodeToPNG();
                m_isReadable = true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                m_bytes = new byte[0];
                m_isReadable = false;
            }
        }

        public override object WriteTo(object obj)
        {
            obj = base.WriteTo(obj);
            Texture2D texture = (Texture2D)obj;
            if (texture == null)
            {
                return null;
            }

            if (m_isReadable)
            {
                texture.LoadImage(m_bytes, false);
            }
            return texture;

        }

        public override void GetDeps(GetDepsContext context)
        {
            base.GetDeps(context);
        }

        public override void GetDepsFrom(object obj, GetDepsFromContext context)
        {
            base.GetDepsFrom(obj, context);
        }
        //<TEMPLATE_BODY_END>
#endif
    }
}


