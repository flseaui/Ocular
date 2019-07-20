using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Battlehub.RTCommon;

namespace Battlehub.RTEditor
{
    public interface IEditorsMap
    {
        void AddMapping(Type type, Type editorType, bool enabled, bool isPropertyEditor);
        void AddMapping(Type type, GameObject editor, bool enabled, bool isPropertyEditor);
        bool IsObjectEditorEnabled(Type type);
        bool IsPropertyEditorEnabled(Type type);
        bool IsMaterialEditorEnabled(Shader shader);
        GameObject GetObjectEditor(Type type, bool strict = false);
        GameObject GetPropertyEditor(Type type, bool strict = false);
        GameObject GetMaterialEditor(Shader shader, bool strict = false);
        Type[] GetEditableTypes();
    }

    public partial class EditorsMap : IEditorsMap
    {
        private class EditorDescriptor
        {
            public int Index;
            public bool Enabled;
            public bool IsPropertyEditor;

            public EditorDescriptor(int index, bool enabled, bool isPropertyEditor)
            {
                Index = index;
                Enabled = enabled;
                IsPropertyEditor = isPropertyEditor;
            }
        }

        private class MaterialEditorDescriptor
        {
            public GameObject Editor;
            public bool Enabled;

            public MaterialEditorDescriptor(GameObject editor, bool enabled)
            {
                Editor = editor;
                Enabled = enabled;
            }
        }

        private GameObject m_defaultMaterialEditor;
        private Dictionary<Shader, MaterialEditorDescriptor> m_materialMap = new Dictionary<Shader, MaterialEditorDescriptor>();
        private Dictionary<Type, EditorDescriptor> m_map = new Dictionary<Type, EditorDescriptor>();
        private GameObject[] m_editors = new GameObject[0];
        private bool m_isLoaded = false;

        public EditorsMap()
        {
            LoadMap();
        }

        public void Reset()
        {
            if(!m_isLoaded)
            {
                return;
            }
            m_materialMap = new Dictionary<Shader, MaterialEditorDescriptor>();
            m_map = new Dictionary<Type, EditorDescriptor>();
            m_editors = new GameObject[0];
            m_defaultMaterialEditor = null;
            m_isLoaded = false;
        }

        private void DefaultEditorsMap()
        {
            m_map = new Dictionary<Type, EditorDescriptor>
            {
                { typeof(GameObject), new EditorDescriptor(0, true, false) },
                { typeof(System.Object), new EditorDescriptor(1, true, true) },
                { typeof(UnityEngine.Object), new EditorDescriptor(2, true, true) },
                { typeof(System.Boolean), new EditorDescriptor(3, true, true) },
                { typeof(Enum), new EditorDescriptor(4, true, true) },
                { typeof(List<>), new EditorDescriptor(5, true, true) },
                { typeof(Array), new EditorDescriptor(6, true, true) },
                { typeof(System.String), new EditorDescriptor(7, true, true) },
                { typeof(System.Int32), new EditorDescriptor(8, true, true) },
                { typeof(System.Single), new EditorDescriptor(9, true, true) },
                { typeof(Range), new EditorDescriptor(10, true, true) },
                { typeof(Vector2), new EditorDescriptor(11, true, true) },
                { typeof(Vector3), new EditorDescriptor(12, true, true) },
                { typeof(Vector4), new EditorDescriptor(13, true, true) },
                { typeof(Quaternion), new EditorDescriptor(14, true, true) },
                { typeof(Color), new EditorDescriptor(15, true, true) },
                { typeof(Bounds), new EditorDescriptor(16, true, true) },
                { typeof(Component), new EditorDescriptor(17, true, false) },
                { typeof(BoxCollider), new EditorDescriptor(18, true, false) },
                { typeof(Camera), new EditorDescriptor(17, false, false) },
                { typeof(CapsuleCollider), new EditorDescriptor(18, true, false) },
                { typeof(FixedJoint), new EditorDescriptor(17, true, false) },
                { typeof(HingeJoint), new EditorDescriptor(17, true, false) },
                { typeof(Light), new EditorDescriptor(17, true, false) },
                { typeof(MeshCollider), new EditorDescriptor(17, true, false) },
                { typeof(MeshFilter), new EditorDescriptor(17, true, false) },
                { typeof(MeshRenderer), new EditorDescriptor(17, true, false) },
                { typeof(MonoBehaviour), new EditorDescriptor(17, false, false) },
                { typeof(Rigidbody), new EditorDescriptor(17, true, false) },
                { typeof(SkinnedMeshRenderer), new EditorDescriptor(17, true, false) },
                { typeof(Skybox), new EditorDescriptor(17, true, false) },
                { typeof(SphereCollider), new EditorDescriptor(18, true, false) },
                { typeof(SpringJoint), new EditorDescriptor(17, true, false) },
                { typeof(Transform), new EditorDescriptor(19, true, false) },
                { typeof(Cubeman.CubemanCharacter), new EditorDescriptor(17, true, false) },
                { typeof(Cubeman.CubemanUserControl), new EditorDescriptor(17, true, false) },
                { typeof(Cubeman.GameCameraFollow), new EditorDescriptor(17, true, false) },
                { typeof(Cubeman.GameCharacter), new EditorDescriptor(17, true, false) },
            };
        }

        partial void InitEditorsMap();

        public void LoadMap()
        {
            if(m_isLoaded)
            {
                return;
            }
            m_isLoaded = true;

            DefaultEditorsMap();
            InitEditorsMap();

            EditorsMapStorage editorsMap = Resources.Load<EditorsMapStorage>(EditorsMapStorage.EditorsMapPrefabName);
            if (editorsMap == null)
            {
                editorsMap = Resources.Load<EditorsMapStorage>(EditorsMapStorage.EditorsMapTemplateName);
            }
            if (editorsMap != null)
            {
                m_editors = editorsMap.Editors;
                
                for(int i = 0; i < editorsMap.MaterialEditors.Length; ++i)
                {
                    GameObject materialEditor = editorsMap.MaterialEditors[i];
                    Shader shader = editorsMap.Shaders[i];
                    bool enabled = editorsMap.IsMaterialEditorEnabled[i];
                    if(!m_materialMap.ContainsKey(shader))
                    {
                        m_materialMap.Add(shader, new MaterialEditorDescriptor(materialEditor, enabled));
                    }
                    m_defaultMaterialEditor = editorsMap.DefaultMaterialEditor;
                }
            }
            else
            {
                Debug.LogError("Editors map is null");
            }
        }

        public void AddMapping(Type type, Type editorType, bool enabled, bool isPropertyEditor)
        {
            GameObject editor = m_editors.Where(ed => ed.GetComponents<Component>().Any(c => c.GetType() == editorType)).FirstOrDefault();
            if (editor == null)
            {
                throw new ArgumentException("editorType");
            }

            AddMapping(type, editor, enabled, isPropertyEditor);
        }

        public void AddMapping(Type type, GameObject editor, bool enabled, bool isPropertyEditor)
        {
            int index = Array.IndexOf(m_editors, editor);
            if(index < 0)
            {
                Array.Resize(ref m_editors, m_editors.Length + 1);
                index = m_editors.Length - 1;
                m_editors[index] = editor;
            }
            m_map.Add(type, new EditorDescriptor(index, enabled, isPropertyEditor));
        }

        public bool IsObjectEditorEnabled(Type type)
        {
            return IsEditorEnabled(type, false, true);
        }

        public bool IsPropertyEditorEnabled(Type type)
        {
            return IsEditorEnabled(type, true, false);
        }

        private bool IsEditorEnabled(Type type, bool isPropertyEditor, bool strict)
        {
            EditorDescriptor descriptor = GetEditorDescriptor(type, isPropertyEditor, strict);
            if (descriptor != null)
            {
                return descriptor.Enabled;
            }
            return false;
        }

        public bool IsMaterialEditorEnabled(Shader shader)
        {
            MaterialEditorDescriptor descriptor = GetEditorDescriptor(shader);
            if (descriptor != null)
            {
                return descriptor.Enabled;
            }

            return false;
        }

        public GameObject GetObjectEditor(Type type, bool strict = false)
        {
            return GetEditor(type, false, strict);
        }

        public GameObject GetPropertyEditor(Type type, bool strict = false)
        {
            return GetEditor(type, true, strict);
        }

        private GameObject GetEditor(Type type, bool isPropertyEditor, bool strict = false)
        {
            EditorDescriptor descriptor = GetEditorDescriptor(type, isPropertyEditor, strict);
            if (descriptor != null)
            {
                return m_editors[descriptor.Index];
            }
            return null;
        }

        public GameObject GetMaterialEditor(Shader shader, bool strict = false)
        {
            MaterialEditorDescriptor descriptor = GetEditorDescriptor(shader);
            if(descriptor != null)
            {
                return descriptor.Editor;
            }

            if(strict)
            {
                return null;
            }

            return m_defaultMaterialEditor;
        }

        private MaterialEditorDescriptor GetEditorDescriptor(Shader shader)
        {
            MaterialEditorDescriptor descriptor;
            if(m_materialMap.TryGetValue(shader, out descriptor))
            {
                return m_materialMap[shader];
            }

            return null;
        }

        private EditorDescriptor GetEditorDescriptor(Type type, bool isPropertyEditor, bool strict)
        {
            do
            {
                EditorDescriptor descriptor;
                if (m_map.TryGetValue(type, out descriptor))
                {
                    if (descriptor.IsPropertyEditor == isPropertyEditor)
                    {
                        return descriptor;
                    }
                }
                else
                {
                    if (type.IsGenericType)
                    {
                        type = type.GetGenericTypeDefinition();
                        continue;
                    }
                }

                if (strict)
                {
                    break;
                }

                type = type.BaseType();
            }
            while (type != null);
            return null;
        }

        public Type[] GetEditableTypes()
        {
            return m_map.Where(kvp => kvp.Value != null && kvp.Value.Enabled).Select(kvp => kvp.Key).ToArray();
        }
    }
}
