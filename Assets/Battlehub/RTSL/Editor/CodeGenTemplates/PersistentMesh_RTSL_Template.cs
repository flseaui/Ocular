//#define RTSL_COMPILE_TEMPLATES
#if RTSL_COMPILE_TEMPLATES
//<TEMPLATE_USINGS_START>
using ProtoBuf;
using UnityEngine;
//<TEMPLATE_USINGS_END>
#endif

namespace Battlehub.RTSL.Internal
{
    [PersistentTemplate("UnityEngine.Mesh", new[] { "vertices", "subMeshCount", "indexFormat", "triangles" })]
    public class PersistentMesh_RTSL_Template : PersistentSurrogateTemplate
    {
#if RTSL_COMPILE_TEMPLATES
        //<TEMPLATE_BODY_START>
        [ProtoMember(1)]
        public Vector3[] vertices;

        [ProtoMember(2)]
        public int subMeshCount;

        [ProtoMember(3)]
        public IntArray[] m_tris;

        [ProtoMember(4)]
        public UnityEngine.Rendering.IndexFormat indexFormat;

        public override object WriteTo(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            Mesh o = (Mesh)obj;
            o.indexFormat = indexFormat;
            o.vertices = vertices;
            o.subMeshCount = subMeshCount;
            if (m_tris != null)
            {
                for (int i = 0; i < subMeshCount; ++i)
                {
                    o.SetTriangles(m_tris[i].Array, i);
                }
            }
            return  base.WriteTo(obj); 
        }

        public override void ReadFrom(object obj)
        {
            base.ReadFrom(obj);
            if (obj == null)
            {
                return;
            }
            Mesh o = (Mesh)obj;
            indexFormat = o.indexFormat;
            subMeshCount = o.subMeshCount;
            vertices = o.vertices;
            m_tris = new IntArray[subMeshCount];
            for (int i = 0; i < subMeshCount; ++i)
            {
                m_tris[i] = new IntArray();
                m_tris[i].Array = o.GetTriangles(i);
            }
        }
        //<TEMPLATE_BODY_END>
#endif
    }
}


