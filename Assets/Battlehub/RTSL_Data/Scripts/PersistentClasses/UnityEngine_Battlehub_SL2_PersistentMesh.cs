using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using UnityEngine;
using UnityEngine.Battlehub.SL2;

using UnityObject = UnityEngine.Object;
namespace UnityEngine.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentMesh : PersistentObject
    {
        [ProtoMember(257)]
        public PersistentBoneWeight[] boneWeights;

        [ProtoMember(258)]
        public PersistentMatrix4x4[] bindposes;

        [ProtoMember(259)]
        public PersistentBounds bounds;

        [ProtoMember(260)]
        public PersistentVector3[] normals;

        [ProtoMember(261)]
        public PersistentVector4[] tangents;

        [ProtoMember(262)]
        public PersistentVector2[] uv;

        [ProtoMember(263)]
        public PersistentVector2[] uv2;

        [ProtoMember(264)]
        public PersistentVector2[] uv3;

        [ProtoMember(265)]
        public PersistentVector2[] uv4;

        [ProtoMember(266)]
        public PersistentVector2[] uv5;

        [ProtoMember(267)]
        public PersistentVector2[] uv6;

        [ProtoMember(268)]
        public PersistentVector2[] uv7;

        [ProtoMember(269)]
        public PersistentVector2[] uv8;

        [ProtoMember(270)]
        public PersistentColor[] colors;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            Mesh uo = (Mesh)obj;
            boneWeights = Assign(uo.boneWeights, v_ => (PersistentBoneWeight)v_);
            bindposes = Assign(uo.bindposes, v_ => (PersistentMatrix4x4)v_);
            bounds = uo.bounds;
            normals = Assign(uo.normals, v_ => (PersistentVector3)v_);
            tangents = Assign(uo.tangents, v_ => (PersistentVector4)v_);
            uv = Assign(uo.uv, v_ => (PersistentVector2)v_);
            uv2 = Assign(uo.uv2, v_ => (PersistentVector2)v_);
            uv3 = Assign(uo.uv3, v_ => (PersistentVector2)v_);
            uv4 = Assign(uo.uv4, v_ => (PersistentVector2)v_);
            uv5 = Assign(uo.uv5, v_ => (PersistentVector2)v_);
            uv6 = Assign(uo.uv6, v_ => (PersistentVector2)v_);
            uv7 = Assign(uo.uv7, v_ => (PersistentVector2)v_);
            uv8 = Assign(uo.uv8, v_ => (PersistentVector2)v_);
            colors = Assign(uo.colors, v_ => (PersistentColor)v_);
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            Mesh uo = (Mesh)obj;
            uo.boneWeights = Assign(boneWeights, v_ => (BoneWeight)v_);
            uo.bindposes = Assign(bindposes, v_ => (Matrix4x4)v_);
            uo.bounds = bounds;
            uo.normals = Assign(normals, v_ => (Vector3)v_);
            uo.tangents = Assign(tangents, v_ => (Vector4)v_);
            uo.uv = Assign(uv, v_ => (Vector2)v_);
            uo.uv2 = Assign(uv2, v_ => (Vector2)v_);
            uo.uv3 = Assign(uv3, v_ => (Vector2)v_);
            uo.uv4 = Assign(uv4, v_ => (Vector2)v_);
            uo.uv5 = Assign(uv5, v_ => (Vector2)v_);
            uo.uv6 = Assign(uv6, v_ => (Vector2)v_);
            uo.uv7 = Assign(uv7, v_ => (Vector2)v_);
            uo.uv8 = Assign(uv8, v_ => (Vector2)v_);
            uo.colors = Assign(colors, v_ => (Color)v_);
            return uo;
        }
    }
}

