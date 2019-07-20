#if !RTSL_MAINTENANCE
using Battlehub.RTSL;
using ProtoBuf;
using UnityEngine;

namespace UnityEngine.Battlehub.SL2
{
    [CustomImplementation]
    public partial class PersistentMesh
    {        
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
    }
}
#endif

