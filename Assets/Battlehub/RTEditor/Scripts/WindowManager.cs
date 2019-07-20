using Battlehub.RTCommon;
using Battlehub.RTHandles;
using Battlehub.UIControls;
using Battlehub.UIControls.Dialogs;
using Battlehub.UIControls.DockPanels;
using Battlehub.UIControls.MenuControl;
using Battlehub.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Battlehub.RTEditor
{
    public interface IWindowManager
    {
        bool IsDialogOpened
        {
            get;
        }

        event Action<IWindowManager> AfterLayout;
        void OverrideDefaultLayout(Func<IWindowManager, LayoutInfo> callback, string activateWindowOfType = null);
        void SetDefaultLayout();
        void SetLayout(Func<IWindowManager, LayoutInfo> callback, string activateWindowOfType = null);
        
        void OverrideTools(Transform contentPrefab);
        void SetTools(Transform content);

        bool RegisterWindow(CustomWindowDescriptor desc);

        Transform GetWindow(string windowTypeName);
        Transform[] GetWindows(string windowTypeName);
        Transform[] GetComponents(Transform content);

        bool Exists(string windowTypeName);
        bool IsActive(string windowType);
        bool IsActive(Transform content);

        bool ActivateWindow(string windowTypeName);
        bool ActivateWindow(Transform content);
        
        Transform CreateWindow(string windowTypeName, out WindowDescriptor wd, out GameObject content, out bool isDialog);
        Transform CreateWindow(string windowTypeName);
        Transform CreateDialogWindow(string windowTypeName, string header, DialogAction<DialogCancelArgs> okAction, DialogAction<DialogCancelArgs> cancelAction = null,
             float minWidth = 250,
             float minHeight = 250,
             float preferredWidth = 700,
             float preferredHeight = 400,
             bool canResize = true);

        void MessageBox(string header, string text, DialogAction<DialogCancelArgs> ok = null);
        void MessageBox(Sprite icon, string header, string text, DialogAction<DialogCancelArgs> ok = null);
        void Confirmation(string header, string text, DialogAction<DialogCancelArgs> ok, DialogAction<DialogCancelArgs> cancel, string okText = "OK", string cancelText = "Cancel");
        void Confirmation(Sprite icon, string header, string text, DialogAction<DialogCancelArgs> ok, DialogAction<DialogCancelArgs> cancel, string okText = "OK", string cancelText = "Cancel");
        
        void Dialog(string header, Transform content, DialogAction<DialogCancelArgs> ok, DialogAction<DialogCancelArgs> cancel, string okText = "OK", string cancelText = "Cancel",
             float minWidth = 150,
             float minHeight = 150,
             float preferredWidth = 700,
             float preferredHeight = 400);

        void Dialog(Sprite icon, string header, Transform content, DialogAction<DialogCancelArgs> ok, DialogAction<DialogCancelArgs> cancel, string okText = "OK", string cancelText = "Cancel",
            float minWidth = 150,
             float minHeight = 150,
             float preferredWidth = 700,
             float preferredHeight = 400);
    }

    [Serializable]
    public class WindowDescriptor
    {
        public Sprite Icon;
        public string Header;
        public GameObject ContentPrefab;
        
        public int MaxWindows = 1;
        [ReadOnly]
        public int Created = 0;
    }

    [Serializable]
    public class CustomWindowDescriptor
    {
        public string TypeName;
        public bool IsDialog;
        public WindowDescriptor Descriptor;
    }

    [DefaultExecutionOrder(-89)]
    public class WindowManager : MonoBehaviour, IWindowManager
    {
        public event Action<IWindowManager> AfterLayout;

        [SerializeField]
        private DialogManager m_dialogManager = null;

        [SerializeField]
        private WindowDescriptor m_sceneWindow = null;

        [SerializeField]
        private WindowDescriptor m_gameWindow = null;

        [SerializeField]
        private WindowDescriptor m_hierarchyWindow = null;

        [SerializeField]
        private WindowDescriptor m_inspectorWindow = null;

        [SerializeField]
        private WindowDescriptor m_projectWindow = null;

        [SerializeField]
        private WindowDescriptor m_consoleWindow = null;

        [SerializeField]
        private WindowDescriptor m_saveSceneDialog = null;

        [SerializeField]
        private WindowDescriptor m_openProjectDialog = null;

        [SerializeField]
        private WindowDescriptor m_selectAssetLibraryDialog = null;

        [SerializeField]
        private WindowDescriptor m_toolsWindow = null;

        [SerializeField]
        private WindowDescriptor m_importAssetsDialog = null;

        [SerializeField]
        private WindowDescriptor m_aboutDialog = null;

        [SerializeField]
        private WindowDescriptor m_selectObjectDialog = null;

        [SerializeField]
        private WindowDescriptor m_selectColorDialog = null;

        [SerializeField]
        private CustomWindowDescriptor[] m_customWindows = null;

        [SerializeField]
        private DockPanel m_dockPanels = null;

        [SerializeField]
        private Transform m_componentsRoot = null;

        [SerializeField]
        private RectTransform m_toolsRoot = null;
       
        private IRTE m_editor;
        private Func<IWindowManager, LayoutInfo> m_overrideLayoutCallback;
        private string m_activateWindowOfType;
        
        private readonly Dictionary<string, CustomWindowDescriptor> m_typeToCustomWindow = new Dictionary<string, CustomWindowDescriptor>();
        private readonly Dictionary<string, HashSet<Transform>> m_windows = new Dictionary<string, HashSet<Transform>>();
        private readonly Dictionary<Transform, List<Transform>> m_extraComponents = new Dictionary<Transform, List<Transform>>();
        
        private IInput Input
        {
            get { return m_editor.Input; }
        }

        private float m_zAxis;
        private bool m_isPointerOverActiveWindow = true;
        private bool m_skipUpdate;

        private RuntimeWindow ActiveWindow
        {
            get { return m_editor.ActiveWindow; }
        }

        private RuntimeWindow[] Windows
        {
            get { return m_editor.Windows; }
        }

        private GraphicRaycaster Raycaster
        {
            get { return m_editor.Raycaster; }
        }

        private TMP_InputField CurrentInputField
        {
            get { return m_editor.CurrentInputField; }
        }

        public bool IsDialogOpened
        {
            get { return m_dialogManager.IsDialogOpened; }
        }

        private void Start()
        {
            if (m_dockPanels == null)
            {
                m_dockPanels = FindObjectOfType<DockPanel>();
            }

            if(m_dialogManager == null)
            {
                m_dialogManager = FindObjectOfType<DialogManager>();
            }

            for(int i = 0; i < m_customWindows.Length; ++i)
            {
                CustomWindowDescriptor customWindow = m_customWindows[i];
                if(customWindow != null && customWindow.Descriptor != null && !m_typeToCustomWindow.ContainsKey(customWindow.TypeName))
                {
                    m_typeToCustomWindow.Add(customWindow.TypeName, customWindow);
                }
            }
            
            m_dockPanels.TabActivated += OnTabActivated;
            m_dockPanels.TabDeactivated += OnTabDeactivated;
            m_dockPanels.TabClosed += OnTabClosed;

            m_dockPanels.RegionBeforeDepthChanged += OnRegionBeforeDepthChanged;
            m_dockPanels.RegionDepthChanged += OnRegionDepthChanged;
            m_dockPanels.RegionSelected += OnRegionSelected;
            m_dockPanels.RegionUnselected += OnRegionUnselected;
            m_dockPanels.RegionEnabled += OnRegionEnabled;
            m_dockPanels.RegionDisabled += OnRegionDisabled;
            m_dockPanels.RegionMaximized += OnRegionMaximized;
            m_dockPanels.RegionBeforeBeginDrag += OnRegionBeforeBeginDrag;
            m_dockPanels.RegionBeginResize += OnBeginResize;
            m_dockPanels.RegionEndResize += OnRegionEndResize;

            m_dialogManager.DialogDestroyed += OnDialogDestroyed;

            if (m_componentsRoot == null)
            {
                m_componentsRoot = transform;
            }

            m_editor = IOC.Resolve<IRTE>();
            m_sceneWindow.MaxWindows = m_editor.CameraLayerSettings.MaxGraphicsLayers;

            SetDefaultLayout();

            WindowDescriptor wd;
            GameObject content;
            bool isDialog;

            Transform tools = CreateWindow(RuntimeWindowType.ToolsPanel.ToString().ToLower(), out wd, out content, out isDialog);
            if (tools != null)
            {
                SetTools(tools);
            }

            m_dockPanels.CursorHelper = m_editor.CursorHelper;
        }

        private RectTransform GetRegionTransform(RuntimeWindow window)
        {
            if (window == null)
            {
                return null;
            }

            Region region = window.GetComponentInParent<Region>();
            if (region == null)
            {
                return null;
            }

            return region.GetDragRegion() as RectTransform;
        }


        public bool IsOverlapped(RuntimeWindow testWindow)
        {
            for (int i = 0; i < Windows.Length; ++i)
            {
                RuntimeWindow window = Windows[i];
                if (window == testWindow)
                {
                    continue;
                }

                if (RectTransformUtility.RectangleContainsScreenPoint((RectTransform)window.transform, Input.GetPointerXY(0), Raycaster.eventCamera))
                {
                    if (testWindow.Depth < window.Depth)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void EnableOrDisableRaycasts()
        {
            if (ActiveWindow != null)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint((RectTransform)ActiveWindow.transform, Input.GetPointerXY(0), Raycaster.eventCamera) && !IsOverlapped(ActiveWindow))
                {
                    if (!m_isPointerOverActiveWindow)
                    {
                        m_isPointerOverActiveWindow = true;

                        RuntimeWindow[] windows = Windows;

                        for (int i = 0; i < windows.Length; ++i)
                        {
                            RuntimeWindow window = windows[i];
                            window.DisableRaycasts();
                        }
                    }
                }
                else
                {
                    if (m_isPointerOverActiveWindow)
                    {
                        m_isPointerOverActiveWindow = false;

                        RuntimeWindow[] windows = Windows;

                        for (int i = 0; i < windows.Length; ++i)
                        {
                            RuntimeWindow window = windows[i];
                            window.EnableRaycasts();
                        }
                    }
                }
            }
        }

        private void Update()
        {
            if (m_skipUpdate)
            {
                m_skipUpdate = false;
                return;
            }

            if (!m_editor.IsInputFieldActive)
            {
                if (m_dialogManager.IsDialogOpened)
                {
                    if (m_editor.Input.GetKeyDown(KeyCode.Escape))
                    {
                        m_dialogManager.CloseDialog();
                    }
                }
            }

            m_editor.UpdateCurrentInputField();
            EnableOrDisableRaycasts();

            bool mwheel = false;
            if (m_zAxis != Mathf.CeilToInt(Mathf.Abs(Input.GetAxis(InputAxis.Z))))
            {
                mwheel = m_zAxis == 0;
                m_zAxis = Mathf.CeilToInt(Mathf.Abs(Input.GetAxis(InputAxis.Z)));
            }

            bool pointerDownOrUp = Input.GetPointerDown(0) ||
              Input.GetPointerDown(1) ||
              Input.GetPointerDown(2) ||
              Input.GetPointerUp(0);

            bool canActivate = pointerDownOrUp ||
                mwheel ||
                Input.IsAnyKeyDown() && (CurrentInputField == null || !CurrentInputField.isFocused);

            if (canActivate)
            {
                PointerEventData pointerEventData = new PointerEventData(m_editor.EventSystem);
                pointerEventData.position = Input.GetPointerXY(0);

                List<RaycastResult> results = new List<RaycastResult>();
                Raycaster.Raycast(pointerEventData, results);

                RectTransform activeRectTransform = GetRegionTransform(ActiveWindow);
                bool activeWindowContainsScreenPoint = activeRectTransform != null && RectTransformUtility.RectangleContainsScreenPoint(activeRectTransform, Input.GetPointerXY(0), Raycaster.eventCamera);


                if (!results.Any(r => r.gameObject.GetComponent<Menu>()))
                {
                    foreach (Region region in results.Select(r => r.gameObject.GetComponentInParent<Region>()).Where(r => r != null).OrderBy(r => r.transform.localPosition.z))
                    {
                        RuntimeWindow window = region.ActiveContent != null ? region.ActiveContent.GetComponentInChildren<RuntimeWindow>() : region.ContentPanel.GetComponentInChildren<RuntimeWindow>();
                        if (window != null && (!activeWindowContainsScreenPoint || window.Depth >= ActiveWindow.Depth))
                        {
                            if (m_editor.Contains(window))
                            {
                                if (pointerDownOrUp || window.ActivateOnAnyKey)
                                {
                                    if (window != null && window.WindowType == RuntimeWindowType.Scene)
                                    {
                                        IEnumerable<Selectable> selectables = results.Select(r => r.gameObject.GetComponent<Selectable>()).Where(s => s != null);
                                        int count = selectables.Count();
                                        if (count >= 1)
                                        {
                                            RuntimeSelectionComponentUI selectionComponentUI = selectables.First() as RuntimeSelectionComponentUI;
                                            if (selectionComponentUI != null)
                                            {
                                                selectionComponentUI.Select();
                                            }
                                        }
                                    }

                                    if (window != ActiveWindow)
                                    {
                                        m_editor.ActivateWindow(window);
                                        region.MoveRegionToForeground();
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }


        private void OnDestroy()
        {
            if(m_dockPanels != null)
            {
                m_dockPanels.TabActivated -= OnTabActivated;
                m_dockPanels.TabDeactivated -= OnTabDeactivated;
                m_dockPanels.TabClosed -= OnTabClosed;
                
                m_dockPanels.RegionBeforeDepthChanged -= OnRegionBeforeDepthChanged;
                m_dockPanels.RegionDepthChanged -= OnRegionDepthChanged;
                m_dockPanels.RegionSelected -= OnRegionSelected;
                m_dockPanels.RegionUnselected -= OnRegionUnselected;
                m_dockPanels.RegionEnabled -= OnRegionEnabled;
                m_dockPanels.RegionDisabled -= OnRegionDisabled;
                m_dockPanels.RegionMaximized -= OnRegionMaximized;
                m_dockPanels.RegionBeforeBeginDrag -= OnRegionBeforeBeginDrag;
                m_dockPanels.RegionBeginResize -= OnBeginResize;
                m_dockPanels.RegionEndResize -= OnRegionEndResize;
            }

            if(m_dialogManager != null)
            {
                m_dialogManager.DialogDestroyed -= OnDialogDestroyed;
            }
        }

        private void OnDialogDestroyed(Dialog dialog)
        {
            OnContentDestroyed(dialog.Content);
        }

        private void OnRegionSelected(Region region)
        {
        }

        private void OnRegionUnselected(Region region)
        {

        }

        private void OnBeginResize(Resizer resizer, Region region)
        {
            
        }

        private void OnRegionEndResize(Resizer resizer, Region region)
        {
            m_skipUpdate = true;
        }


        private void OnTabActivated(Region region, Transform content)
        {
            List<Transform> extraComponents;
            if(m_extraComponents.TryGetValue(content, out extraComponents))
            {
                for(int i = 0; i < extraComponents.Count; ++i)
                {
                    Transform extraComponent = extraComponents[i];
                    extraComponent.gameObject.SetActive(true);
                }
            }

            RuntimeWindow window = region.ContentPanel.GetComponentInChildren<RuntimeWindow>();
            if(window != null)
            {
                window.Editor.ActivateWindow(window);
            }
        }

        private void OnTabDeactivated(Region region, Transform content)
        {
            List<Transform> extraComponents;
            if (m_extraComponents.TryGetValue(content, out extraComponents))
            {
                for (int i = 0; i < extraComponents.Count; ++i)
                {
                    Transform extraComponent = extraComponents[i];
                    if(extraComponent)
                    {
                        extraComponent.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void OnTabClosed(Region region, Transform content)
        {
            OnContentDestroyed(content);
        }

        private void OnRegionDisabled(Region region)
        {
            if(region.ActiveContent != null)
            {
                List<Transform> extraComponents;
                if (m_extraComponents.TryGetValue(region.ActiveContent, out extraComponents))
                {
                    for (int i = 0; i < extraComponents.Count; ++i)
                    {
                        Transform extraComponent = extraComponents[i];
                        if (extraComponent)
                        {
                            extraComponent.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }

        private void OnRegionEnabled(Region region)
        {
            if(region.ActiveContent != null)
            {
                List<Transform> extraComponents;
                if (m_extraComponents.TryGetValue(region.ActiveContent, out extraComponents))
                {
                    for (int i = 0; i < extraComponents.Count; ++i)
                    {
                        Transform extraComponent = extraComponents[i];
                        extraComponent.gameObject.SetActive(true);
                    }
                }
            }
        }

        private void OnRegionMaximized(Region region, bool maximized)
        {
            if(!maximized)
            {
                RuntimeWindow[] windows = m_dockPanels.RootRegion.GetComponentsInChildren<RuntimeWindow>();
                for(int i = 0; i < windows.Length; ++i)
                {
                    windows[i].HandleResize();
                }
            }
        }

        private void OnContentDestroyed(Transform content)
        {
            string windowTypeName = m_windows.Where(kvp => kvp.Value.Contains(content)).Select(kvp => kvp.Key).FirstOrDefault();
            if (!string.IsNullOrEmpty(windowTypeName))
            {
                HashSet<Transform> windowsOfType = m_windows[windowTypeName];
                windowsOfType.Remove(content);

                if (windowsOfType.Count == 0)
                {
                    m_windows.Remove(windowTypeName);
                }

                List<Transform> extraComponents = new List<Transform>();
                if (m_extraComponents.TryGetValue(content, out extraComponents))
                {
                    for (int i = 0; i < extraComponents.Count; ++i)
                    {
                        Destroy(extraComponents[i].gameObject);
                    }
                }

                WindowDescriptor wd = null;
                if (windowTypeName == RuntimeWindowType.Scene.ToString().ToLower())
                {
                    wd = m_sceneWindow;
                }
                else if (windowTypeName == RuntimeWindowType.Game.ToString().ToLower())
                {
                    wd = m_gameWindow;
                }
                else if (windowTypeName == RuntimeWindowType.Hierarchy.ToString().ToLower())
                {
                    wd = m_hierarchyWindow;
                }
                else if (windowTypeName == RuntimeWindowType.Inspector.ToString().ToLower())
                {
                    wd = m_inspectorWindow;
                }
                else if (windowTypeName == RuntimeWindowType.Project.ToString().ToLower())
                {
                    wd = m_projectWindow;
                }
                else if (windowTypeName == RuntimeWindowType.Console.ToString().ToLower())
                {
                    wd = m_consoleWindow;
                }
                else if(windowTypeName == RuntimeWindowType.SaveScene.ToString().ToLower())
                {
                    wd = m_saveSceneDialog;
                }
                else if(windowTypeName == RuntimeWindowType.OpenProject.ToString().ToLower())
                {
                    wd = m_openProjectDialog;
                }
                else if(windowTypeName == RuntimeWindowType.ToolsPanel.ToString().ToLower())
                {
                    wd = m_toolsWindow;
                }
                else if(windowTypeName == RuntimeWindowType.SelectAssetLibrary.ToString().ToLower())
                {
                    wd = m_selectAssetLibraryDialog;
                }
                else if(windowTypeName == RuntimeWindowType.ImportAssets.ToString().ToLower())
                {
                    wd = m_importAssetsDialog;
                }
                else if(windowTypeName == RuntimeWindowType.About.ToString().ToLower())
                {
                    wd = m_aboutDialog;
                }
                else if(windowTypeName == RuntimeWindowType.SelectObject.ToString().ToLower())
                {
                    wd = m_selectObjectDialog;
                }
                else if(windowTypeName == RuntimeWindowType.SelectColor.ToString().ToLower())
                {
                    wd = m_selectColorDialog;
                }
                else
                {
                    CustomWindowDescriptor cwd;
                    if(m_typeToCustomWindow.TryGetValue(windowTypeName, out cwd))
                    {
                        wd = cwd.Descriptor;
                    }
                }

                if(wd != null)
                {
                    wd.Created--;
                    Debug.Assert(wd.Created >= 0);
                } 
            }
        }

        private void CancelIfRegionIsNotActive(Region region, CancelArgs arg)
        {
            if (m_editor.ActiveWindow == null)
            {
                return;
            }

            Region activeRegion = m_editor.ActiveWindow.GetComponentInParent<Region>();
            if (activeRegion == null)
            {
                return;
            }

            if (activeRegion.GetDragRegion() != region.GetDragRegion())
            {
                arg.Cancel = true;
            }
        }

        private void OnRegionBeforeBeginDrag(Region region, CancelArgs arg)
        {
            CancelIfRegionIsNotActive(region, arg);
        }

        private void OnRegionBeforeDepthChanged(Region region, CancelArgs arg)
        {
            CancelIfRegionIsNotActive(region, arg);
        }

        private void OnRegionDepthChanged(Region region, int depth)
        {
            RuntimeWindow[] windows = region.GetComponentsInChildren<RuntimeWindow>(true);
            for (int i = 0; i < windows.Length; ++i)
            {
                RuntimeWindow window = windows[i];
                window.SetCameraDepth(10 + depth * 5);

                window.Depth = (region.IsModal() ? 2048 + depth : depth) * 5;
                if(window.GetComponentsInChildren<RuntimeWindow>().Length > 1)
                {
                    window.Depth -= 1;
                }
            }
        }

        public bool RegisterWindow(CustomWindowDescriptor desc)
        {
            if(m_typeToCustomWindow.ContainsKey(desc.TypeName.ToLower()))
            {
                return false;
            }

            m_typeToCustomWindow.Add(desc.TypeName.ToLower(), desc);
            return true;
        }

        public void OverrideDefaultLayout(Func<IWindowManager, LayoutInfo> buildLayoutCallback, string activateWindowOfType = null)
        {
            m_overrideLayoutCallback = buildLayoutCallback;
            m_activateWindowOfType = activateWindowOfType;
        }

        public void SetDefaultLayout()
        {
            if(m_overrideLayoutCallback != null)
            {
                SetLayout(m_overrideLayoutCallback, m_activateWindowOfType);
            }
            else
            {
                SetLayout(BuiltInDefaultLayout, RuntimeWindowType.Scene.ToString().ToLower());
            }
        }

        private static LayoutInfo BuiltInDefaultLayout(IWindowManager wm)
        {
            WindowDescriptor sceneWd;
            GameObject sceneContent;
            bool isDialog;
            wm.CreateWindow(RuntimeWindowType.Scene.ToString(), out sceneWd, out sceneContent, out isDialog);

            WindowDescriptor gameWd;
            GameObject gameContent;
            wm.CreateWindow(RuntimeWindowType.Game.ToString(), out gameWd, out gameContent, out isDialog);

            WindowDescriptor inspectorWd;
            GameObject inspectorContent;
            wm.CreateWindow(RuntimeWindowType.Inspector.ToString(), out inspectorWd, out inspectorContent, out isDialog);

            WindowDescriptor consoleWd;
            GameObject consoleContent;
            wm.CreateWindow(RuntimeWindowType.Console.ToString(), out consoleWd, out consoleContent, out isDialog);

            WindowDescriptor hierarchyWd;
            GameObject hierarchyContent;
            wm.CreateWindow(RuntimeWindowType.Hierarchy.ToString(), out hierarchyWd, out hierarchyContent, out isDialog);

            WindowDescriptor projectWd;
            GameObject projectContent;
            wm.CreateWindow(RuntimeWindowType.Project.ToString(), out projectWd, out projectContent, out isDialog);

            LayoutInfo layout = new LayoutInfo(false,
                new LayoutInfo(false,
                    new LayoutInfo(true,
                        new LayoutInfo(inspectorContent.transform, inspectorWd.Header, inspectorWd.Icon),
                        new LayoutInfo(consoleContent.transform, consoleWd.Header, consoleWd.Icon),
                        0.5f),
                    new LayoutInfo(true,
                        new LayoutInfo(sceneContent.transform, sceneWd.Header, sceneWd.Icon),
                        new LayoutInfo(gameContent.transform, gameWd.Header, gameWd.Icon),
                        0.75f),
                    0.25f),
                new LayoutInfo(true,
                    new LayoutInfo(hierarchyContent.transform, hierarchyWd.Header, hierarchyWd.Icon),
                    new LayoutInfo(projectContent.transform, projectWd.Header, projectWd.Icon),
                    0.5f),
                0.75f);

            return layout;
        }

        public void OverrideTools(Transform contentPrefab)
        {
            if(contentPrefab == null)
            {
                m_toolsWindow.ContentPrefab = null;
                return;
            }
            m_toolsWindow.ContentPrefab = contentPrefab.gameObject;
        }

        public void SetTools(Transform tools)
        {
            Transform window = GetWindow(RuntimeWindowType.ToolsPanel.ToString().ToLower());
            if(window != null)
            {
                OnContentDestroyed(window);
            }
            
            if (m_toolsRoot != null)
            {
                foreach(Transform child in m_toolsRoot)
                {
                    Destroy(child.gameObject);
                }
            }

            if(tools != null)
            {
                tools.SetParent(m_toolsRoot, false);

                RectTransform rt = tools as RectTransform;
                if (rt != null)
                {
                    rt.Stretch();
                }

                tools.gameObject.SetActive(true);
            }  
        }

        public void SetLayout(Func<IWindowManager, LayoutInfo> buildLayoutCallback, string activateWindowOfType = null)
        {
            Region rootRegion = m_dockPanels.RootRegion;
            if(rootRegion == null)
            {
                return;
            }
            if(m_editor == null)
            {
                return;
            }

            ClearRegion(rootRegion);
            foreach (Transform child in m_dockPanels.Free)
            {
                Region region = child.GetComponent<Region>();
                ClearRegion(region);
            }

            LayoutInfo layout = buildLayoutCallback(this);
            m_dockPanels.RootRegion.Build(layout);
            
            if(!string.IsNullOrEmpty(activateWindowOfType))
            {
                ActivateWindow(activateWindowOfType);
            }
            
            RuntimeWindow[] windows = Windows;
            for (int i = 0; i < windows.Length; ++i)
            {
                windows[i].EnableRaycasts();
                windows[i].HandleResize();
            }

            if(AfterLayout != null)
            {
                AfterLayout(this);
            }
        }

        private void ClearRegion(Region rootRegion)
        {
            Region[] regions = rootRegion.GetComponentsInChildren<Region>(true);
            for (int i = 0; i < regions.Length; ++i)
            {
                Region region = regions[i];
                foreach (Transform content in region.ContentPanel)
                {
                    OnContentDestroyed(content);
                }
            }
            rootRegion.Clear();
        }

        public bool Exists(string windowTypeName)
        {
            return GetWindow(windowTypeName) != null;
        }

        public Transform GetWindow(string windowTypeName)
        {
            HashSet<Transform> hs;
            if(m_windows.TryGetValue(windowTypeName.ToLower(), out hs))
            {
                return hs.FirstOrDefault();
            }
            return null;
        }

        public Transform[] GetWindows(string windowTypeName)
        {
            HashSet<Transform> hs;
            if (m_windows.TryGetValue(windowTypeName.ToLower(), out hs))
            {
                return hs.ToArray();
            }
            return new Transform[0];
        }

        public Transform[] GetComponents(Transform content)
        {
            List<Transform> extraComponents;
            if (m_extraComponents.TryGetValue(content, out extraComponents))
            {
                return extraComponents.ToArray();
            }
            return new Transform[0];
        }

        public bool IsActive(string windowTypeName)
        {
            HashSet<Transform> hs;
            if (m_windows.TryGetValue(windowTypeName.ToLower(), out hs))
            {
                foreach(Transform content in hs)
                {
                    Tab tab = Region.FindTab(content);
                    if(tab != null && tab.IsOn)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsActive(Transform content)
        {
            Tab tab = Region.FindTab(content);
            return tab != null && tab.IsOn;
        }
    
        public bool ActivateWindow(string windowTypeName)
        {
            Transform content = GetWindow(windowTypeName);
            if(content == null)
            {
                return false;
            }
            return ActivateWindow(content);
        }

        public bool ActivateWindow(Transform content)
        {
            Region region = content.GetComponentInParent<Region>();
            if (region != null)
            {
                region.MoveRegionToForeground();
                m_isPointerOverActiveWindow = m_editor != null && RectTransformUtility.RectangleContainsScreenPoint((RectTransform)region.transform, Input.GetPointerXY(0), Raycaster.eventCamera);
                if (m_isPointerOverActiveWindow)
                {
                    RuntimeWindow[] windows = Windows;
                    for (int i = 0; i < windows.Length; ++i)
                    {
                        windows[i].DisableRaycasts();
                    }
                }
            }

            Tab tab = Region.FindTab(content);
            if (tab == null)
            {
                return false;
            }

            tab.IsOn = true;
            return true;
        }

        public Transform CreateWindow(string windowTypeName)
        {
            WindowDescriptor wd;
            GameObject content;
            bool isDialog;

            Transform window = CreateWindow(windowTypeName, out wd, out content, out isDialog);
            if (!window)
            {
                return window;
            }

            if(isDialog)
            {
                Dialog dialog = m_dialogManager.ShowDialog(wd.Icon, wd.Header, content.transform);
                dialog.IsCancelVisible = false;
                dialog.IsOkVisible = false;
            }
            else
            {
                m_dockPanels.RootRegion.Add(wd.Icon, wd.Header, content.transform, true);
            }
            
            ActivateContent(wd, content);

            return window;
        }

        public Transform CreateDialogWindow(string windowTypeName, string header, DialogAction<DialogCancelArgs> okAction, DialogAction<DialogCancelArgs> cancelAction,
             float minWidth,
             float minHeight,
             float preferredWidth,
             float preferredHeight,
             bool canResize = true)
        {
            WindowDescriptor wd;
            GameObject content;
            bool isDialog;

            Transform window = CreateWindow(windowTypeName, out wd, out content, out isDialog);
            if (!window)
            {
                return window;
            }

            if (isDialog)
            {
                if(header == null)
                {
                    header = wd.Header;
                }
                Dialog dialog = m_dialogManager.ShowDialog(wd.Icon, header, content.transform, okAction, "OK", cancelAction, "Cancel", minWidth, minHeight, preferredWidth, preferredHeight, canResize);
                dialog.IsCancelVisible = false;
                dialog.IsOkVisible = false;
            }
            else
            {
                throw new ArgumentException(windowTypeName + " is not a dialog");
            }

            ActivateContent(wd, content);

            return window;
        }

        public Transform CreateWindow(string windowTypeName, out WindowDescriptor wd, out GameObject content, out bool isDialog)
        {
            if (m_dockPanels == null)
            {
                Debug.LogError("Unable to create window. m_dockPanels == null. Set DockPanels field");
            }

            windowTypeName = windowTypeName.ToLower();
            wd = null;
            content = null;
            isDialog = false;

            if (windowTypeName == RuntimeWindowType.Scene.ToString().ToLower())
            {
                wd = m_sceneWindow;
            }
            else if (windowTypeName == RuntimeWindowType.Game.ToString().ToLower())
            {
                wd = m_gameWindow;
            }
            else if (windowTypeName == RuntimeWindowType.Hierarchy.ToString().ToLower())
            {
                wd = m_hierarchyWindow;
            }
            else if (windowTypeName == RuntimeWindowType.Inspector.ToString().ToLower())
            {
                wd = m_inspectorWindow;
            }
            else if (windowTypeName == RuntimeWindowType.Project.ToString().ToLower())
            {
                wd = m_projectWindow;
            }
            else if (windowTypeName == RuntimeWindowType.Console.ToString().ToLower())
            {
                wd = m_consoleWindow;
            }
            else if(windowTypeName == RuntimeWindowType.ToolsPanel.ToString().ToLower())
            {
                wd = m_toolsWindow;
            }
            else if (windowTypeName == RuntimeWindowType.SaveScene.ToString().ToLower())
            {
                wd = m_saveSceneDialog;
                isDialog = true;
            }
            else if (windowTypeName == RuntimeWindowType.OpenProject.ToString().ToLower())
            {
                wd = m_openProjectDialog;
                isDialog = true;
            }
            else if(windowTypeName == RuntimeWindowType.SelectAssetLibrary.ToString().ToLower())
            {
                wd = m_selectAssetLibraryDialog;
                isDialog = true;
            }
            else if(windowTypeName == RuntimeWindowType.ImportAssets.ToString().ToLower())
            {
                wd = m_importAssetsDialog;
                isDialog = true;
            }
            else if(windowTypeName == RuntimeWindowType.About.ToString().ToLower())
            {
                wd = m_aboutDialog;
                isDialog = true;
            }
            else if(windowTypeName == RuntimeWindowType.SelectObject.ToString().ToLower())
            {
                wd = m_selectObjectDialog;
                isDialog = true;
            }
            else if(windowTypeName == RuntimeWindowType.SelectColor.ToString().ToLower())
            {
                wd = m_selectColorDialog;
                isDialog = true;
            }
            else
            {
                CustomWindowDescriptor cwd;
                if(m_typeToCustomWindow.TryGetValue(windowTypeName, out cwd))
                {
                    wd = cwd.Descriptor;
                    isDialog = cwd.IsDialog;
                }
            }

            if (wd == null)
            {
                Debug.LogWarningFormat("{0} window was not found", windowTypeName);
                return null;
            }

            if (wd.Created >= wd.MaxWindows)
            {
                return null;
            }
            wd.Created++;

            if (wd.ContentPrefab != null)
            {
                wd.ContentPrefab.SetActive(false);
                content = Instantiate(wd.ContentPrefab);
                content.name = windowTypeName;

                Transform[] children = content.transform.OfType<Transform>().ToArray();
                for(int i = 0; i < children.Length; ++i)
                {
                    Transform component = children[i];
                    if (!(component is RectTransform))
                    {
                        component.gameObject.SetActive(false);
                        component.transform.SetParent(m_componentsRoot, false);
                    }
                }

                List<Transform> extraComponents = new List<Transform>();
                for (int i = 0; i < children.Length; ++i)
                {
                    if (children[i].parent == m_componentsRoot)
                    {
                        extraComponents.Add(children[i]);
                    }
                }

                m_extraComponents.Add(content.transform, extraComponents);
            }
            else
            {
                Debug.LogWarningFormat("{0} WindowDescriptor.ContentPrefab is null", windowTypeName);

                content = new GameObject();
                content.AddComponent<RectTransform>();
                content.name = "Empty Content";

                m_extraComponents.Add(content.transform, new List<Transform>());
            }

            HashSet<Transform> windows;
            if (!m_windows.TryGetValue(windowTypeName, out windows))
            {
                windows = new HashSet<Transform>();
                m_windows.Add(windowTypeName, windows);
            }

            windows.Add(content.transform);
            return content.transform;
        }

        private void ActivateContent(WindowDescriptor wd, GameObject content)
        {
            List<Transform> extraComponentsList = new List<Transform>();
            m_extraComponents.TryGetValue(content.transform, out extraComponentsList);
            for (int i = 0; i < extraComponentsList.Count; ++i)
            {
                extraComponentsList[i].gameObject.SetActive(true);
            }

            wd.ContentPrefab.SetActive(true);
            content.SetActive(true);
        }

       
        public void MessageBox(string header, string text, DialogAction<DialogCancelArgs> ok = null)
        {
            m_dialogManager.ShowDialog(null, header, text, ok);
        }

        public void MessageBox(Sprite icon, string header, string text, DialogAction<DialogCancelArgs> ok = null)
        {
            m_dialogManager.ShowDialog(icon, header, text, ok);
        }

        public void Confirmation(string header, string text, DialogAction<DialogCancelArgs> ok, DialogAction<DialogCancelArgs> cancel, string okText = "OK", string cancelText = "Cancel")
        {
            m_dialogManager.ShowDialog(null, header, text, ok, okText, cancel, cancelText);

        }
        public void Confirmation(Sprite icon, string header, string text, DialogAction<DialogCancelArgs> ok, DialogAction<DialogCancelArgs> cancel, string okText = "OK", string cancelText = "Cancel")
        {
            m_dialogManager.ShowDialog(icon, header, text, ok, okText, cancel, cancelText);
        }

        public void Dialog(string header, Transform content, DialogAction<DialogCancelArgs> ok, DialogAction<DialogCancelArgs> cancel, string okText = "OK", string cancelText = "Cancel", 
             float minWidth = 150,
             float minHeight = 150,
             float preferredWidth = 700,
             float preferredHeight = 400)
        {
            m_dialogManager.ShowDialog(null, header, content, ok, okText, cancel, cancelText, minWidth, minHeight, preferredWidth, preferredHeight);
        }

        public void Dialog(Sprite icon, string header, Transform content, DialogAction<DialogCancelArgs> ok, DialogAction<DialogCancelArgs> cancel, string okText = "OK", string cancelText = "Cancel",
            float minWidth = 150,
             float minHeight = 150,
             float preferredWidth = 700,
             float preferredHeight = 400)
        {
            m_dialogManager.ShowDialog(icon, header, content, ok, okText, cancel, cancelText, minWidth, minHeight, preferredWidth, preferredHeight); 
        }
    }
}

