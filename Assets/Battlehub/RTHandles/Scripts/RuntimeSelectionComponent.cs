using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Battlehub.Utils;
using Battlehub.RTCommon;

namespace Battlehub.RTHandles
{
    public delegate void UnityEditorToolChanged();
    public class UnityEditorToolsListener
    {
        public static event UnityEditorToolChanged ToolChanged;

        #if UNITY_EDITOR
        private static UnityEditor.Tool m_tool;
        static UnityEditorToolsListener()
        {
            m_tool = UnityEditor.Tools.current;
        }

        #else
        void Use()
        {
            ToolChanged();
        }
        #endif

        public static void Update()
        {
            #if UNITY_EDITOR
            if (m_tool != UnityEditor.Tools.current)
            {
                if (ToolChanged != null)
                {
                    ToolChanged();
                }
                m_tool = UnityEditor.Tools.current;
            }
            #endif
        }
    }

    public interface IScenePivot
    {
        Vector3 Pivot
        {
            get;
            set;
        }

        Vector3 SecondaryPivot
        {
            get;
            set;
        }

        Vector3 CameraPosition
        {
            get;
            set;
        }

        bool IsOrthographic
        {
            get;
            set;
        }

        void Focus();
    }

    public interface IRuntimeSelectionComponent : IScenePivot
    {
        PositionHandle PositionHandle
        {
            get;
        }

        RotationHandle RotationHandle
        {
            get;
        }

        ScaleHandle ScaleHandle
        {
            get;
        }

        BoxSelection BoxSelection
        {
            get;
        }

        bool IsPositionHandleEnabled
        {
            get;
            set;
        }

        bool IsRotationHandleEnabled
        {
            get;
            set;
        }

        bool IsScaleHandleEnabled
        {
            get;
            set;
        }

        bool IsBoxSelectionEnabled
        {
            get;
            set;
        }

        bool CanSelect
        {
            get;
            set;
        }
        
        bool CanSelectAll
        {
            get;
            set;
        }
    }

    [DefaultExecutionOrder(-55)]
    public class RuntimeSelectionComponent : RTEComponent, IRuntimeSelectionComponent
    {
        [SerializeField]
        private PositionHandle m_positionHandle = null;
        [SerializeField]
        private RotationHandle m_rotationHandle = null;
        [SerializeField]
        private ScaleHandle m_scaleHandle = null;
        [SerializeField]
        private BoxSelection m_boxSelection = null;
        [SerializeField]
        private RuntimeGrid m_grid = null;
        [SerializeField]
        private Transform m_pivot = null;
        [SerializeField]
        private Transform m_secondaryPivot = null;

        [SerializeField]
        private bool m_isPositionHandleEnabled = true;
        [SerializeField]
        private bool m_isRotationHandleEnabled = true;
        [SerializeField]
        private bool m_isScaleHandleEnabled = true;
        [SerializeField]
        private bool m_isBoxSelectionEnabled = true;
        [SerializeField]
        private bool m_canSelect = true;
        [SerializeField]
        private bool m_canSelectAll = true;
     
        protected Transform PivotTransform
        {
            get { return m_pivot; }
        }

        protected Transform SecondaryPivotTransform
        {
            get { return m_secondaryPivot; }
        }

        public bool IsOrthographic
        {
            get { return Window.Camera.orthographic; }
            set { Window.Camera.orthographic = value; }
        }

        public virtual Vector3 CameraPosition
        {
            get { return Window.Camera.transform.position; }
            set
            {
                Window.Camera.transform.position = value;
                Window.Camera.transform.LookAt(Pivot);
            }
        }

        public virtual Vector3 Pivot
        {
            get { return m_pivot.transform.position; }
            set
            {
                m_pivot.transform.position = value;
                Window.Camera.transform.LookAt(Pivot);
            }
        }

        public virtual Vector3 SecondaryPivot
        {
            get { return m_secondaryPivot.transform.position; }
            set { m_secondaryPivot.transform.position = value; }
        }

        public BoxSelection BoxSelection
        {
            get { return m_boxSelection; }
        }

        public PositionHandle PositionHandle
        {
            get { return m_positionHandle; }
        }

        public RotationHandle RotationHandle
        {
            get { return m_rotationHandle; }
        }

        public ScaleHandle ScaleHandle
        {
            get { return m_scaleHandle; }
        }

        public bool IsPositionHandleEnabled
        {
            get { return m_isPositionHandleEnabled && m_positionHandle != null; }
            set
            {
                m_isPositionHandleEnabled = value;
                if (m_positionHandle != null)
                {
                    m_positionHandle.gameObject.SetActive(value && Editor.Tools.Current == RuntimeTool.Move);
                }
            }
        }

        public bool IsRotationHandleEnabled
        {
            get { return m_isRotationHandleEnabled && m_rotationHandle != null; }
            set
            {
                m_isRotationHandleEnabled = value;
                if (m_rotationHandle != null)
                {
                    m_rotationHandle.gameObject.SetActive(value && Editor.Tools.Current == RuntimeTool.Rotate);
                }
            }
        }

        public bool IsScaleHandleEnabled
        {
            get { return m_isScaleHandleEnabled && m_scaleHandle != null; }
            set
            {
                m_isScaleHandleEnabled = value; 
                if(m_scaleHandle != null)
                {
                    m_scaleHandle.gameObject.SetActive(value && Editor.Tools.Current == RuntimeTool.Scale);
                }
            }
        }

        public bool IsBoxSelectionEnabled
        {
            get { return m_isBoxSelectionEnabled && m_boxSelection != null; }
            set
            {
                m_isBoxSelectionEnabled = value;
                if(m_boxSelection != null)
                {
                    m_boxSelection.enabled = value && Editor.ActiveWindow == Window;
                }
            }
        }

        public bool CanSelect
        {
            get { return m_canSelect; }
            set { m_canSelect = value; }
        }

        public bool CanSelectAll
        {
            get { return m_canSelectAll; }
            set { m_canSelectAll = value; }
        }

        protected override void AwakeOverride()
        {
            base.AwakeOverride();
            
            Window.IOCContainer.RegisterFallback<IScenePivot>(this);
            Window.IOCContainer.RegisterFallback<IRuntimeSelectionComponent>(this);

            if(m_boxSelection == null)
            {
                m_boxSelection = GetComponentInChildren<BoxSelection>(true);
            }
            if(m_positionHandle == null)
            {
                m_positionHandle = GetComponentInChildren<PositionHandle>(true);
            }
            if(m_rotationHandle == null)
            {
                m_rotationHandle = GetComponentInChildren<RotationHandle>(true);
            }
            if(m_scaleHandle == null)
            {
                m_scaleHandle = GetComponentInChildren<ScaleHandle>(true);
            }

            if (m_boxSelection != null)
            {
                if(m_boxSelection.Window == null)
                {
                    m_boxSelection.Window = Window;
                }

                m_boxSelection.Filtering += OnBoxSelectionFiltering;
            }

            if (m_positionHandle != null)
            {
                if(m_positionHandle.Window == null)
                {
                    m_positionHandle.Window = Window;
                }

                m_positionHandle.gameObject.SetActive(true);
                m_positionHandle.gameObject.SetActive(false);
            }

            if (m_rotationHandle != null)
            {
                if(m_rotationHandle.Window == null)
                {
                    m_rotationHandle.Window = Window;
                }

                m_rotationHandle.gameObject.SetActive(true);
                m_rotationHandle.gameObject.SetActive(false);
            }

            if (m_scaleHandle != null)
            {
                if(m_scaleHandle.Window == null)
                {
                    m_scaleHandle.Window = Window;
                }
                m_scaleHandle.gameObject.SetActive(true);
                m_scaleHandle.gameObject.SetActive(false);
            }

            if (m_grid != null)
            {
                if (m_grid.Window == null)
                {
                    m_grid.Window = Window;
                }
            }

            Editor.Selection.SelectionChanged += OnRuntimeSelectionChanged;
            Editor.Tools.ToolChanged += OnRuntimeToolChanged;

            if (m_pivot == null)
            {
                GameObject pivot = new GameObject("Pivot");
                pivot.transform.SetParent(transform, true);
                pivot.transform.position = Vector3.zero;
                m_pivot = pivot.transform;
            }

            if (m_secondaryPivot == null)
            {
                GameObject secondaryPivot = new GameObject("SecondaryPivot");
                secondaryPivot.transform.SetParent(transform, true);
                secondaryPivot.transform.position = Vector3.zero;
                m_secondaryPivot = secondaryPivot.transform;
            }

            Window.Camera.transform.LookAt(m_pivot);

            OnRuntimeSelectionChanged(null);
        }


        protected virtual void Start()
        {
            if (GetComponent<RuntimeSelectionInputBase>() == null)
            {
                gameObject.AddComponent<RuntimeSelectionInput>();
            }

            if (m_positionHandle != null && !m_positionHandle.gameObject.activeSelf)
            {
                m_positionHandle.gameObject.SetActive(true);
                m_positionHandle.gameObject.SetActive(false);
            }

            if (m_rotationHandle != null && !m_rotationHandle.gameObject.activeSelf)
            {
                m_rotationHandle.gameObject.SetActive(true);
                m_rotationHandle.gameObject.SetActive(false);
            }

            if (m_scaleHandle != null && !m_scaleHandle.gameObject.activeSelf)
            {
                m_scaleHandle.gameObject.SetActive(true);
                m_scaleHandle.gameObject.SetActive(false);
            }

            RuntimeTool tool = Editor.Tools.Current;
            Editor.Tools.Current = RuntimeTool.None;
            Editor.Tools.Current = tool;
        }

        protected override void OnDestroyOverride()
        {
            base.OnDestroyOverride();

            Window.IOCContainer.UnregisterFallback<IScenePivot>(this);
            Window.IOCContainer.UnregisterFallback<IRuntimeSelectionComponent>(this);

            if (m_boxSelection != null)
            {
                m_boxSelection.Filtering -= OnBoxSelectionFiltering;
            }

            Editor.Tools.ToolChanged -= OnRuntimeToolChanged;
            Editor.Selection.SelectionChanged -= OnRuntimeSelectionChanged;

            GameObject[] selectedObjects = Editor.Selection.gameObjects;
            if (selectedObjects != null)
            {
                for (int i = 0; i < selectedObjects.Length; ++i)
                {
                    GameObject go = selectedObjects[i];
                    if (go != null)
                    {
                        SelectionGizmo[] selectionGizmo = go.GetComponents<SelectionGizmo>();
                        for (int g = 0; g < selectionGizmo.Length; ++g)
                        {
                            if (selectionGizmo[g] != null && selectionGizmo[g].Window == Window)
                            {
                                Destroy(selectionGizmo[g]);
                            }
                        }
                    }
                }
            }
        }

        protected override void OnWindowActivated()
        {
            base.OnWindowActivated();
            if (m_boxSelection != null)
            {
                m_boxSelection.enabled = true && IsBoxSelectionEnabled;
            }
        }

        protected override void OnWindowDeactivated()
        {
            base.OnWindowDeactivated();
            if(m_boxSelection != null)
            {
                m_boxSelection.enabled = false;
            }
        }

        public virtual void SelectGO(bool multiselect, bool allowUnselect)
        {
            if(!CanSelect)
            {
                return;
            }

            Ray ray = Window.Pointer;
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, float.MaxValue))
            {
                GameObject hitGO = hitInfo.collider.gameObject;
                bool canSelect = CanSelectObject(hitGO);
                if (canSelect)
                {
                    hitGO = hitGO.GetComponentInParent<ExposeToEditor>().gameObject;
                    if (multiselect)
                    {
                        List<Object> selection;
                        if (Editor.Selection.objects != null)
                        {
                            selection = Editor.Selection.objects.ToList();
                        }
                        else
                        {
                            selection = new List<Object>();
                        }

                        if (selection.Contains(hitGO))
                        {
                            selection.Remove(hitGO);
                            if (!allowUnselect)
                            {
                                selection.Insert(0, hitGO);
                            }
                        }
                        else
                        {
                            selection.Insert(0, hitGO);
                        }
                        Editor.Undo.Select(selection.ToArray(), hitGO);
                    }
                    else
                    {
                        Editor.Selection.activeObject = hitGO;
                    }
                }
                else
                {
                    if (!multiselect)
                    {
                        Editor.Selection.activeObject = null;
                    }
                }
            }
            else
            {
                if (!multiselect)
                {
                    Editor.Selection.activeObject = null;
                }
            }
        }

        public virtual void SelectAll()
        {
            if(!CanSelect || !CanSelectAll)
            {
                return;
            }
            Editor.Selection.objects = Editor.Object.Get(false).Select(exposed => exposed.gameObject).ToArray();
        }

        private void OnRuntimeToolChanged()
        {
            if (Editor.Selection.activeTransform == null)
            {
                return;
            }

            if (m_positionHandle != null)
            {
                if (Editor.Tools.Current == RuntimeTool.Move && IsPositionHandleEnabled)
                {
                    m_positionHandle.transform.position = Editor.Selection.activeTransform.position;
                    m_positionHandle.Targets = GetTargets();
                    m_positionHandle.gameObject.SetActive(m_positionHandle.Targets.Length > 0);
                }
                else
                {
                    m_positionHandle.gameObject.SetActive(false);
                }
            }
            if (m_rotationHandle != null)
            {   
                if (Editor.Tools.Current == RuntimeTool.Rotate && IsRotationHandleEnabled)
                {
                    m_rotationHandle.transform.position = Editor.Selection.activeTransform.position;
                    m_rotationHandle.Targets = GetTargets();
                    m_rotationHandle.gameObject.SetActive(m_rotationHandle.Targets.Length > 0);
                }
                else
                {
                    m_rotationHandle.gameObject.SetActive(false);
                }
            }
            if (m_scaleHandle != null)
            {
                if (Editor.Tools.Current == RuntimeTool.Scale && IsScaleHandleEnabled)
                {
                    m_scaleHandle.transform.position = Editor.Selection.activeTransform.position;
                    m_scaleHandle.Targets = GetTargets();
                    m_scaleHandle.gameObject.SetActive(m_scaleHandle.Targets.Length > 0);
                }
                else
                {
                    m_scaleHandle.gameObject.SetActive(false);
                }
            }

#if UNITY_EDITOR
            switch (Editor.Tools.Current)
            {
                case RuntimeTool.None:
                    UnityEditor.Tools.current = UnityEditor.Tool.None;
                    break;
                case RuntimeTool.Move:
                    UnityEditor.Tools.current = UnityEditor.Tool.Move;
                    break;
                case RuntimeTool.Rotate:
                    UnityEditor.Tools.current = UnityEditor.Tool.Rotate;
                    break;
                case RuntimeTool.Scale:
                    UnityEditor.Tools.current = UnityEditor.Tool.Scale;
                    break;
                case RuntimeTool.View:
                    UnityEditor.Tools.current = UnityEditor.Tool.View;
                    break;
            }
#endif
        }

        private void OnBoxSelectionFiltering(object sender, FilteringArgs e)
        {
            if (e.Object == null || !CanSelect)
            {
                e.Cancel = true;
            }

            ExposeToEditor exposeToEditor = e.Object.GetComponent<ExposeToEditor>();
            if (!exposeToEditor || !exposeToEditor.CanSelect)
            {
                e.Cancel = true;
            }
        }

        private void OnRuntimeSelectionChanged(Object[] unselected)
        {
            if (unselected != null)
            {
                for (int i = 0; i < unselected.Length; ++i)
                {
                    GameObject unselectedObj = unselected[i] as GameObject;
                    if (unselectedObj != null)
                    {
                        SelectionGizmo[] selectionGizmo = unselectedObj.GetComponents<SelectionGizmo>();
                        for(int g = 0; g < selectionGizmo.Length; ++g)
                        {
                            if (selectionGizmo[g] != null && selectionGizmo[g].Window == Window)
                            {
                                //DestroyImmediate(selectionGizmo[g]);
                                selectionGizmo[g].Internal_Destroyed = true;
                                Destroy(selectionGizmo[g]);
                            }
                        }
                       
                        ExposeToEditor exposeToEditor = unselectedObj.GetComponent<ExposeToEditor>();
                        if (exposeToEditor)
                        {
                            if (exposeToEditor.Unselected != null)
                            {
                                exposeToEditor.Unselected.Invoke(exposeToEditor);
                            }
                        }
                    }
                }
            }

            GameObject[] selected = Editor.Selection.gameObjects;
            if (selected != null)
            {
                for (int i = 0; i < selected.Length; ++i)
                {
                    GameObject selectedObj = selected[i];
                    ExposeToEditor exposeToEditor = selectedObj.GetComponent<ExposeToEditor>();
                    if (exposeToEditor && exposeToEditor.CanSelect && !selectedObj.IsPrefab() && !selectedObj.isStatic)
                    {
                        SelectionGizmo selectionGizmo = selectedObj.GetComponent<SelectionGizmo>();
                        if (selectionGizmo == null || selectionGizmo.Internal_Destroyed || selectionGizmo.Window != Window)
                        {
                            if(!Editor.IsPlaymodeStateChanging || !Editor.IsPlaying)
                            {
                                selectionGizmo = selectedObj.AddComponent<SelectionGizmo>();
                            }
                            
                        }
                        if(selectionGizmo != null)
                        {
                            selectionGizmo.Window = Window;
                        }
                        
                        if (exposeToEditor.Selected != null)
                        {
                            exposeToEditor.Selected.Invoke(exposeToEditor);
                        }
                    }
                }
            }

            if (Editor.Selection.activeGameObject == null || Editor.Selection.activeGameObject.IsPrefab())
            {
                if (m_positionHandle != null)
                {
                    m_positionHandle.gameObject.SetActive(false);
                }
                if (m_rotationHandle != null)
                {
                    m_rotationHandle.gameObject.SetActive(false);
                }
                if (m_scaleHandle != null)
                {
                    m_scaleHandle.gameObject.SetActive(false);
                }
            }
            else
            {
                OnRuntimeToolChanged();
            }
        }

        protected virtual bool CanSelectObject(GameObject go)
        {
            return go.GetComponentInParent<ExposeToEditor>();
        }

        protected virtual Transform[] GetTargets()
        {
            return Editor.Selection.gameObjects.Select(g => g.transform).OrderByDescending(g => Editor.Selection.activeTransform == g).ToArray();
        }

        public virtual void Focus()
        {

        }
    }
}
