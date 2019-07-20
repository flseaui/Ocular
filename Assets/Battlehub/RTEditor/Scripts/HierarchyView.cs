using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using Battlehub.UIControls;
using Battlehub.RTCommon;
using Battlehub.RTSL.Interface;
using Battlehub.Utils;
using UnityEngine.SceneManagement;
using TMPro;

namespace Battlehub.RTEditor
{
    public class HierarchyView : RuntimeWindow
    {
        public GameObject TreeViewPrefab;
        public Color DisabledItemColor = new Color(0.5f, 0.5f, 0.5f);
        public Color EnabledItemColor = new Color(0.2f, 0.2f, 0.2f);
        public UnityEvent ItemDoubleClick;

        private VirtualizingTreeView m_treeView;

        private bool m_lockSelection;
        private bool m_isSpawningPrefab;
        //private bool m_isStarted;

        private List<GameObject> m_rootGameObjects;

        private IProject m_project;
        private IRuntimeEditor m_editor;

        protected override void AwakeOverride()
        {
            WindowType = RuntimeWindowType.Hierarchy;
            base.AwakeOverride();
            if (!TreeViewPrefab)
            {
                Debug.LogError("Set TreeViewPrefab field");
                return;
            }

            m_project = IOC.Resolve<IProject>();
            m_editor = IOC.Resolve<IRuntimeEditor>();

            m_treeView = Instantiate(TreeViewPrefab, transform).GetComponent<VirtualizingTreeView>();
            m_treeView.name = "HierarchyTreeView";
            m_treeView.CanSelectAll = false;
            m_treeView.SelectOnPointerUp = true;

            RectTransform rt = (RectTransform)m_treeView.transform;
            rt.Stretch();

            m_treeView.ItemDataBinding += OnItemDataBinding;
            m_treeView.SelectionChanged += OnSelectionChanged;
            m_treeView.ItemsRemoved += OnItemsRemoved;
            m_treeView.ItemExpanding += OnItemExpanding;
            m_treeView.ItemBeginDrag += OnItemBeginDrag;
            m_treeView.ItemBeginDrop += OnItemBeginDrop;
            m_treeView.ItemDrag += OnItemDrag;
            m_treeView.ItemDrop += OnItemDrop;
            m_treeView.ItemEndDrag += OnItemEndDrag;
            m_treeView.ItemDragEnter += OnItemDragEnter;
            m_treeView.ItemDragExit += OnItemDragExit;
            m_treeView.ItemDoubleClick += OnItemDoubleClicked;
            m_treeView.ItemBeginEdit += OnItemBeginEdit;
            m_treeView.ItemEndEdit += OnItemEndEdit;
            m_treeView.PointerEnter += OnTreeViewPointerEnter;
            m_treeView.PointerExit += OnTreeViewPointerExit;

            if (!GetComponent<HierarchyViewInput>())
            {
                gameObject.AddComponent<HierarchyViewInput>();
            }
        }

        private void Start()
        {
            // m_isStarted = true;
        }

        private void OnEnable()
        {
            if (m_editor != null)
            {
                m_editor.SceneLoading += OnSceneLoading;
                m_editor.SceneLoaded += OnSceneLoaded;
            }

            EnableHierarchy();
        }

        private void OnDisable()
        {
            if (m_editor != null)
            {
                m_editor.SceneLoading -= OnSceneLoading;
                m_editor.SceneLoaded -= OnSceneLoaded;
            }

            DisableHierarchy();
        }

        protected override void OnDestroyOverride()
        {
            base.OnDestroyOverride();
            if (!m_treeView)
            {
                return;
            }
            m_treeView.ItemDataBinding -= OnItemDataBinding;
            m_treeView.SelectionChanged -= OnSelectionChanged;
            m_treeView.ItemsRemoved -= OnItemsRemoved;
            m_treeView.ItemExpanding -= OnItemExpanding;
            m_treeView.ItemBeginDrag -= OnItemBeginDrag;
            m_treeView.ItemBeginDrop -= OnItemBeginDrop;
            m_treeView.ItemDrag -= OnItemDrag;
            m_treeView.ItemDrop -= OnItemDrop;
            m_treeView.ItemEndDrag -= OnItemEndDrag;
            m_treeView.ItemDragEnter -= OnItemDragEnter;
            m_treeView.ItemDragExit -= OnItemDragExit;
            m_treeView.ItemDoubleClick -= OnItemDoubleClicked;
            m_treeView.ItemBeginEdit -= OnItemBeginEdit;
            m_treeView.ItemEndEdit -= OnItemEndEdit;
            m_treeView.PointerEnter -= OnTreeViewPointerEnter;
            m_treeView.PointerExit -= OnTreeViewPointerExit;
        }

        private void LateUpdate()
        {
            m_rootGameObjects = null;
        }

        public void SelectAll()
        {
            m_treeView.SelectedItems = m_treeView.Items;
        }

        private void EnableHierarchy()
        {
            BindGameObjects();
            m_lockSelection = true;
            m_treeView.SelectedItems = Editor.Selection.gameObjects != null ? Editor.Selection.gameObjects.Select(g => g.GetComponent<ExposeToEditor>()).Where(e => e != null).ToArray() : null;
            m_lockSelection = false;

            Editor.Selection.SelectionChanged += OnRuntimeSelectionChanged;

            Editor.Object.Awaked += OnObjectAwaked;
            Editor.Object.Started += OnObjectStarted;
            Editor.Object.Enabled += OnObjectEnabled;
            Editor.Object.Disabled += OnObjectDisabled;
            Editor.Object.Destroying += OnObjectDestroying;
            Editor.Object.Destroyed += OnObjectDestroyed;
            Editor.Object.MarkAsDestroyedChanging += OnObjectMarkAsDestoryedChanging;
            Editor.Object.MarkAsDestroyedChanged += OnObjectMarkAsDestroyedChanged;
            Editor.Object.ParentChanged += OnParentChanged;
            Editor.Object.NameChanged += OnNameChanged;

            Editor.PlaymodeStateChanged += OnPlaymodeStateChanged;
        }

        private void DisableHierarchy()
        {
            if (Editor != null)
            {
                if(Editor.Selection != null)
                {
                    Editor.Selection.SelectionChanged -= OnRuntimeSelectionChanged;
                }

                if(Editor.Object != null)
                {
                    Editor.Object.Awaked -= OnObjectAwaked;
                    Editor.Object.Started -= OnObjectStarted;
                    Editor.Object.Enabled -= OnObjectEnabled;
                    Editor.Object.Disabled -= OnObjectDisabled;
                    Editor.Object.Destroying -= OnObjectDestroying;
                    Editor.Object.Destroyed -= OnObjectDestroyed;
                    Editor.Object.MarkAsDestroyedChanging -= OnObjectMarkAsDestoryedChanging;
                    Editor.Object.MarkAsDestroyedChanged -= OnObjectMarkAsDestroyedChanged;
                    Editor.Object.ParentChanged -= OnParentChanged;
                    Editor.Object.NameChanged -= OnNameChanged;
                }

                Editor.PlaymodeStateChanged -= OnPlaymodeStateChanged;
            }
        }

        private void OnSceneLoading()
        {
            DisableHierarchy();
        }

        private void OnSceneLoaded()
        {
            EnableHierarchy();
        }

        private void BindGameObjects(bool forceUseCache = false, bool updateSelection = true)
        {
            bool useCache = Editor.IsPlaying;
            IEnumerable<ExposeToEditor> objects = Editor.Object.Get(true, useCache || forceUseCache);

            if (objects.Any())
            {
                Transform commonParent = objects.First().transform.parent;
                foreach (ExposeToEditor obj in objects)
                {
                    if (obj.transform.parent != commonParent)
                    {
                        Debug.LogWarning("ExposeToEditor objects have different parents, hierarchy may not work correctly.");
                        break;
                    }
                }
            }

            m_treeView.SetItems(objects.OrderBy(g => g.transform.GetSiblingIndex()), updateSelection);
        }

        private void OnPlaymodeStateChanged()
        {
            BindGameObjects();
        }

        private void OnItemExpanding(object sender, VirtualizingItemExpandingArgs e)
        {
            ExposeToEditor exposeToEditor = (ExposeToEditor)e.Item;

            if (exposeToEditor.HasChildren())
            {
                e.Children = exposeToEditor.GetChildren().Where(obj => !obj.MarkAsDestroyed);

                //This line is required to syncronize selection, runtime selection and treeview selection
                OnTreeViewSelectionChanged(m_treeView.SelectedItems, m_treeView.SelectedItems);
            }
            else
            {
                e.Children = new ExposeToEditor[0];
            }
        }

        private void OnRuntimeSelectionChanged(Object[] unselected)
        {
            if (m_lockSelection)
            {
                return;
            }
            m_lockSelection = true;

            if (Editor.Selection.gameObjects == null)
            {
                m_treeView.SelectedItems = new ExposeToEditor[0];
            }
            else
            {
                m_treeView.SelectedItems = Editor.Selection.gameObjects.Select(g => g.GetComponent<ExposeToEditor>()).Where(e => e != null && !e.gameObject.IsPrefab() && e.gameObject.hideFlags != HideFlags.HideAndDontSave).ToArray();
            }

            m_lockSelection = false;
        }

        private void OnSelectionChanged(object sender, SelectionChangedArgs e)
        {
            OnTreeViewSelectionChanged(e.OldItems, e.NewItems);
        }

        private void OnTreeViewSelectionChanged(IEnumerable oldItems, IEnumerable newItems)
        {
            if (m_lockSelection)
            {
                return;
            }

            m_lockSelection = true;

            if (newItems == null)
            {
                newItems = new ExposeToEditor[0];
            }
            ExposeToEditor[] selectableObjects = newItems.OfType<ExposeToEditor>().Where(o => o.CanSelect).ToArray();
            Editor.Selection.objects = selectableObjects.Select(o => o.gameObject).ToArray();

            //sync with RunitimeSelectiom.objects because of OnBeforeSelectionChanged event
            m_treeView.SelectedItems = selectableObjects;

            m_lockSelection = false;
        }

        private void OnItemsRemoved(object sender, ItemsRemovedArgs e)
        {
            if (Editor.ActiveWindow == this)
            {
                IRuntimeEditor editor = IOC.Resolve<IRuntimeEditor>();
                editor.Delete(e.Items.OfType<ExposeToEditor>().Select(exposed => exposed.gameObject).ToArray());
            }
            //Removal handled in RuntimeEditor class
        }

        private void OnItemDataBinding(object sender, VirtualizingTreeViewItemDataBindingArgs e)
        {
            ExposeToEditor dataItem = (ExposeToEditor)e.Item;
            if (dataItem != null)
            {
                TextMeshProUGUI text = e.ItemPresenter.GetComponentInChildren<TextMeshProUGUI>(true);
                text.text = dataItem.name;
                if (dataItem.gameObject.activeInHierarchy)
                {
                    text.color = EnabledItemColor;
                }
                else
                {
                    text.color = DisabledItemColor;
                }

                e.HasChildren = dataItem.HasChildren();
            }
        }

        private void OnItemDoubleClicked(object sender, ItemArgs e)
        {
            ExposeToEditor exposeToEditor = (ExposeToEditor)e.Items[0];
            if (exposeToEditor.CanSelect)
            {
                Editor.Selection.activeObject = exposeToEditor.gameObject;
                ItemDoubleClick.Invoke();
            }
        }

        private void OnItemBeginEdit(object sender, VirtualizingTreeViewItemDataBindingArgs e)
        {
            ExposeToEditor dataItem = (ExposeToEditor)e.Item;
            if (dataItem != null)
            {
                TMP_InputField inputField = e.EditorPresenter.GetComponentInChildren<TMP_InputField>(true);
                inputField.text = dataItem.name;
                inputField.ActivateInputField();
                inputField.Select();
                LayoutElement layout = inputField.GetComponent<LayoutElement>();

                TextMeshProUGUI text = e.ItemPresenter.GetComponentInChildren<TextMeshProUGUI>(true);
                text.text = dataItem.name;

                RectTransform rt = text.GetComponent<RectTransform>();
                layout.preferredWidth = rt.rect.width;
            }
        }

        private void OnItemEndEdit(object sender, VirtualizingTreeViewItemDataBindingArgs e)
        {
            ExposeToEditor dataItem = (ExposeToEditor)e.Item;
            if (dataItem != null)
            {

                TMP_InputField inputField = e.EditorPresenter.GetComponentInChildren<TMP_InputField>(true);
                if (!string.IsNullOrEmpty(inputField.text))
                {
                    dataItem.name = inputField.text;
                    TextMeshProUGUI text = e.ItemPresenter.GetComponentInChildren<TextMeshProUGUI>(true);
                    text.text = dataItem.name;
                }
                else
                {
                    inputField.text = dataItem.name;
                }
            }

            //Following code is required to unfocus inputfield if focused and release InputManager
            if (EventSystem.current != null && !EventSystem.current.alreadySelecting)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }

        private void OnItemDragEnter(object sender, ItemDropCancelArgs e)
        {
            Editor.DragDrop.SetCursor(KnownCursor.DropAllowed);
        }

        private void OnItemDragExit(object sender, System.EventArgs e)
        {
            
        }

        private void OnTreeViewPointerEnter(object sender, PointerEventArgs e)
        {
            if(m_treeView.DragItems != null)
            {
                Editor.DragDrop.SetCursor(KnownCursor.DropAllowed);
            }
        }

        private void OnTreeViewPointerExit(object sender, PointerEventArgs e)
        {
            if (m_treeView.DragItems != null)
            {
                Editor.DragDrop.SetCursor(KnownCursor.DropNowAllowed);
            }
        }

        private void OnItemBeginDrag(object sender, ItemArgs e)
        {
            Editor.DragDrop.RaiseBeginDrag(this, e.Items, e.PointerEventData);
        }

        private void OnItemDrag(object sender, ItemArgs e)
        {
            Editor.DragDrop.RaiseDrag(e.PointerEventData);
        }

        
        private void OnItemBeginDrop(object sender, ItemDropCancelArgs e)
        {
            if (e.IsExternal)
            {

            }
            else
            {
                Editor.Undo.BeginRecord();
                Editor.Undo.CreateRecord(null, null, false,
                    record => RefreshTree(record, true),
                    record => RefreshTree(record, false));

                IEnumerable<ExposeToEditor> dragItems = e.DragItems.OfType<ExposeToEditor>();

                if (e.Action == ItemDropAction.SetLastChild || dragItems.Any(d => (object)d.GetParent() != e.DropTarget))
                {
                    foreach (ExposeToEditor exposed in dragItems.Reverse())
                    {
                        Transform dragT = exposed.transform;
                        int siblingIndex = dragT.GetSiblingIndex();
                        Editor.Undo.BeginRecordTransform(dragT, dragT.parent, siblingIndex);
                    }   
                }
                else
                {
                    Transform dropT = ((ExposeToEditor)e.DropTarget).transform;
                    int dropTIndex = dropT.GetSiblingIndex();

                    foreach (ExposeToEditor exposed in dragItems
                        .Where(o => o.transform.GetSiblingIndex() > dropTIndex)
                        .OrderBy(o => o.transform.GetSiblingIndex())
                        .Union(dragItems
                            .Where(o => o.transform.GetSiblingIndex() < dropTIndex)
                            .OrderByDescending(o => o.transform.GetSiblingIndex())))
                    {
                        Transform dragT = exposed.transform;
                        int siblingIndex = dragT.GetSiblingIndex();
                        Editor.Undo.BeginRecordTransform(dragT, dragT.parent, siblingIndex);
                    }
                }

                Editor.Undo.EndRecord();
            }
        }

        private void OnItemDrop(object sender, ItemDropArgs e)
        {
            if (e.IsExternal)
            {

            }
            else
            {
                Transform dropT = ((ExposeToEditor)e.DropTarget).transform;
                if (e.Action == ItemDropAction.SetLastChild)
                {
                    Editor.Undo.BeginRecord();
                    for (int i = 0; i < e.DragItems.Length; ++i)
                    {
                        ExposeToEditor exposed = (ExposeToEditor)e.DragItems[i];
                        Transform dragT = exposed.transform;
                        dragT.SetParent(dropT, true);
                        dragT.SetAsLastSibling();

                        Editor.Undo.EndRecordTransform(dragT, dropT, dragT.GetSiblingIndex());
                    }
                    Editor.Undo.CreateRecord(null, null, true,
                       record => RefreshTree(record, true),
                       record => RefreshTree(record, false));
                    Editor.Undo.EndRecord();
                }
                else if (e.Action == ItemDropAction.SetNextSibling)
                {
                    Editor.Undo.BeginRecord();
                    
                    for (int i = e.DragItems.Length - 1; i >= 0; --i)
                    {
                        ExposeToEditor exposed = (ExposeToEditor)e.DragItems[i];
                        Transform dragT = exposed.transform;

                        int dropTIndex = dropT.GetSiblingIndex();
                        if (dragT.parent != dropT.parent)
                        {
                            dragT.SetParent(dropT.parent, true);
                            dragT.SetSiblingIndex(dropTIndex + 1);
                        }
                        else
                        {
                            int dragTIndex = dragT.GetSiblingIndex();
                            if (dropTIndex < dragTIndex)
                            {
                                dragT.SetSiblingIndex(dropTIndex + 1);
                            }
                            else
                            {
                                dragT.SetSiblingIndex(dropTIndex);
                            }
                        }
                        Editor.Undo.EndRecordTransform(dragT, dragT.parent, dragT.GetSiblingIndex());
                    }
                    Editor.Undo.CreateRecord(null, null, true,
                        record => RefreshTree(record, true),
                        record => RefreshTree(record, false));
                    Editor.Undo.EndRecord();

                }
                else if (e.Action == ItemDropAction.SetPrevSibling)
                {
                    Editor.Undo.BeginRecord();
                    for (int i = 0; i < e.DragItems.Length; ++i)
                    {
                        ExposeToEditor exposed = (ExposeToEditor)e.DragItems[i];
                        Transform dragT = exposed.transform;
                        if (dragT.parent != dropT.parent)
                        {
                            dragT.SetParent(dropT.parent, true);
                        }

                        int dropTIndex = dropT.GetSiblingIndex();
                        int dragTIndex = dragT.GetSiblingIndex();
                        if (dropTIndex > dragTIndex)
                        {
                            dragT.SetSiblingIndex(dropTIndex - 1);
                        }
                        else
                        {
                            dragT.SetSiblingIndex(dropTIndex);
                        }

                        Editor.Undo.EndRecordTransform(dragT, dragT.parent, dragT.GetSiblingIndex());
                    }
                    Editor.Undo.CreateRecord(null, null, true,
                        record => RefreshTree(record, true),
                        record => RefreshTree(record, false));
                    Editor.Undo.EndRecord();
                }

                Editor.DragDrop.RaiseDrop(e.PointerEventData);
            }
        }

        private bool RefreshTree(Record record, bool isRedo)
        {
            bool applyOnRedo = (bool)record.OldState;
            if (applyOnRedo != isRedo)
            {
                return false;
            }

            BindGameObjects(true, false);

            
            if (m_treeView.SelectedItems != null)
            {
                foreach (ExposeToEditor obj in m_treeView.SelectedItems.OfType<ExposeToEditor>().OrderBy(o => o.transform.GetSiblingIndex()))
                {
                    Expand(obj);
                }
            }

            return false;
        }


        private void OnItemEndDrag(object sender, ItemArgs e)
        {
            if(Editor.DragDrop.InProgress)
            {
                Editor.DragDrop.RaiseDrop(e.PointerEventData);
            }
        }

        private void OnObjectAwaked(ExposeToEditor obj)
        {
            if(!m_isSpawningPrefab)
            {
                if (!obj.MarkAsDestroyed && m_treeView.IndexOf(obj) == -1)
                {
                    ExposeToEditor parent = obj.GetParent();
                    m_treeView.AddChild(parent, obj);
                }
            }
        }

        private void OnObjectStarted(ExposeToEditor obj)
        {

        }

        private void OnObjectEnabled(ExposeToEditor obj)
        {
            VirtualizingTreeViewItem tvItem = m_treeView.GetTreeViewItem(obj);
            if (tvItem == null)
            {
                return;
            }
            TextMeshProUGUI text = tvItem.GetComponentInChildren<TextMeshProUGUI>();
            text.color = EnabledItemColor;
        }

        private void OnObjectDisabled(ExposeToEditor obj)
        {
            VirtualizingTreeViewItem tvItem = m_treeView.GetTreeViewItem(obj);
            if (tvItem == null)
            {
                return;
            }
            TextMeshProUGUI text = tvItem.GetComponentInChildren<TextMeshProUGUI>();
            text.color = DisabledItemColor;
        }

        private void OnObjectDestroying(ExposeToEditor o)
        {
            ExposeToEditor parent = o.GetParent();
            try
            {
                m_treeView.ItemsRemoved -= OnItemsRemoved;
                m_treeView.RemoveChild(parent, o);
            }
            finally
            {
                m_treeView.ItemsRemoved += OnItemsRemoved;
            }    
        }

        private void OnObjectDestroyed(ExposeToEditor o)
        {

        }

        private void OnObjectMarkAsDestoryedChanging(ExposeToEditor o)
        {
            if (o.MarkAsDestroyed)
            {

            }
            else
            {

            }
        }

        private void OnObjectMarkAsDestroyedChanged(ExposeToEditor o)
        {
            if (o.MarkAsDestroyed)
            {
                m_treeView.RemoveChild(o.GetParent(), o);
            }
            else
            {
                ExposeToEditor parent = o.GetParent();
                m_treeView.AddChild(parent, o); 
                SetSiblingIndex(o);
            }
        }

        private void SetSiblingIndex(ExposeToEditor o)
        {
            if (o.transform.parent == null && m_rootGameObjects == null)
            {
                m_rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects().OrderBy(g => g.transform.GetSiblingIndex()).ToList();
            }

            ExposeToEditor nextSibling = o.NextSibling(m_rootGameObjects);
            if (nextSibling != null)
            {
                m_treeView.SetPrevSibling(nextSibling, o);
            }
        }

        private void OnParentChanged(ExposeToEditor obj, ExposeToEditor oldParent, ExposeToEditor newParent)
        {
            if (Editor.IsPlaymodeStateChanging)
            {
                return;
            }

            bool isNewParentExpanded = true;
            bool isOldParentExpanded = true;
            bool isLastChild = false;
            if (newParent != null)
            {
                isNewParentExpanded = m_treeView.IsExpanded(newParent);
            }

            if (oldParent != null)
            {
                TreeViewItemContainerData itemContainerData = (TreeViewItemContainerData)m_treeView.GetItemContainerData(oldParent);

                isLastChild = !oldParent.HasChildren(); //!itemContainerData.HasChildren(m_treeView);
                
                isOldParentExpanded = m_treeView.IsExpanded(oldParent);
            }

            if (isNewParentExpanded)
            {
                m_treeView.ChangeParent(newParent, obj);

                if (!isOldParentExpanded)
                {
                    if (isLastChild)
                    {
                        VirtualizingTreeViewItem oldParentContainer = m_treeView.GetTreeViewItem(oldParent);
                        if (oldParentContainer)
                        {
                            oldParentContainer.CanExpand = false;
                        }
                    }
                }
            }
            else
            {
                if (newParent != null)
                {
                    VirtualizingTreeViewItem newParentTreeViewItem = m_treeView.GetTreeViewItem(newParent);
                    if (newParentTreeViewItem != null)
                    {
                        newParentTreeViewItem.CanExpand = true;
                    }
                }
                
                m_treeView.RemoveChild(oldParent, obj);  
            }
            
        }

        private void OnNameChanged(ExposeToEditor obj)
        {
            VirtualizingTreeViewItem tvItem = m_treeView.GetTreeViewItem(obj);
            if (tvItem == null)
            {
                return;
            }
            TextMeshProUGUI text = tvItem.GetComponentInChildren<TextMeshProUGUI>();
            text.text = obj.name;
        }

        private bool CanDrop(object[] dragObjects)
        {
            IEnumerable<AssetItem> assetItems = dragObjects.OfType<AssetItem>();
            return assetItems.Count() > 0 && assetItems.Any(assetItem => m_project.ToType(assetItem) == typeof(GameObject));
        }


        public override void DragEnter(object[] dragObjects, PointerEventData pointerEventData)
        {
            base.DragEnter(dragObjects, pointerEventData);
            m_treeView.ExternalBeginDrag(pointerEventData.position);
        }

        public override void DragLeave(PointerEventData pointerEventData)
        {
            base.DragLeave(pointerEventData);
            m_treeView.ExternalItemDrop();
            Editor.DragDrop.SetCursor(KnownCursor.DropNowAllowed);
        }

        public override void Drag(object[] dragObjects, PointerEventData pointerEventData)
        {
            base.Drag(dragObjects, pointerEventData);
            m_treeView.ExternalItemDrag(pointerEventData.position);
            if (CanDrop(dragObjects))
            {
                Editor.DragDrop.SetCursor(KnownCursor.DropAllowed);
            }
            else
            {
                Editor.DragDrop.SetCursor(KnownCursor.DropNowAllowed);
                m_treeView.ClearTarget();
            }
        }

        public override void Drop(object[] dragObjects, PointerEventData pointerEventData)
        {
            base.Drop(dragObjects, pointerEventData);

            if (CanDrop(dragObjects))
            {
                ExposeToEditor dropTarget = (ExposeToEditor)m_treeView.DropTarget;
                VirtualizingTreeViewItem treeViewItem = null;
                if (dropTarget != null)
                {
                    treeViewItem = m_treeView.GetTreeViewItem(m_treeView.DropTarget);
                }

                AssetItem[] loadAssetItems = dragObjects.Where(o => o is AssetItem && m_project.ToType((AssetItem)o) == typeof(GameObject)).Select(o => (AssetItem)o).ToArray();
                if (loadAssetItems.Length > 0)
                {
                    m_isSpawningPrefab = true;
                    Editor.IsBusy = true;
                    m_project.Load(loadAssetItems, (error, objects) =>
                    {
                        Editor.IsBusy = false;
                        if (error.HasError)
                        {
                            IWindowManager wm = IOC.Resolve<IWindowManager>();
                            wm.MessageBox("Unable to load asset items.", error.ErrorText);
                            return;
                        }

                        GameObject[] createdObjects = new GameObject[objects.Length];
                        for (int i = 0; i < objects.Length; ++i)
                        {
                            GameObject prefab = (GameObject)objects[i];
                            bool wasPrefabEnabled = prefab.activeSelf;
                            prefab.SetActive(false);
                            GameObject prefabInstance = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                            prefab.SetActive(wasPrefabEnabled);

                            ExposeToEditor exposeToEditor = prefabInstance.GetComponent<ExposeToEditor>();
                            if (exposeToEditor == null)
                            {
                                exposeToEditor = prefabInstance.AddComponent<ExposeToEditor>();
                            }

                            exposeToEditor.SetName(prefab.name);

                            if (dropTarget == null)
                            {
                                exposeToEditor.transform.SetParent(null);
                                m_treeView.Add(exposeToEditor);
                            }
                            else
                            {
                                if (m_treeView.DropAction == ItemDropAction.SetLastChild)
                                {
                                    exposeToEditor.transform.SetParent(dropTarget.transform);
                                    m_treeView.AddChild(dropTarget, exposeToEditor);
                                    treeViewItem.CanExpand = true;
                                    treeViewItem.IsExpanded = true;
                                }
                                else if (m_treeView.DropAction != ItemDropAction.None)
                                {
                                    int index;
                                    int siblingIndex;
                                    if (m_treeView.DropAction == ItemDropAction.SetNextSibling)
                                    {
                                        index = m_treeView.IndexOf(dropTarget) + 1;
                                        siblingIndex = dropTarget.transform.GetSiblingIndex() + 1;
                                    }
                                    else
                                    {
                                        index = m_treeView.IndexOf(dropTarget);
                                        siblingIndex = dropTarget.transform.GetSiblingIndex();
                                    }

                                    exposeToEditor.transform.SetParent(dropTarget.transform.parent != null ? dropTarget.transform.parent : null);
                                    exposeToEditor.transform.SetSiblingIndex(siblingIndex);

                                    TreeViewItemContainerData newTreeViewItemData = (TreeViewItemContainerData)m_treeView.Insert(index, exposeToEditor);
                                    VirtualizingTreeViewItem newTreeViewItem = m_treeView.GetTreeViewItem(exposeToEditor);
                                    if (newTreeViewItem != null)
                                    {
                                        newTreeViewItem.Parent = treeViewItem.Parent;
                                    }
                                    else
                                    {
                                        newTreeViewItemData.Parent = treeViewItem.Parent;
                                    }
                                }
                            }

                            prefabInstance.SetActive(true);
                            createdObjects[i] = prefabInstance;
                        }

                        if (createdObjects.Length > 0)
                        {
                            IRuntimeEditor editor = IOC.Resolve<IRuntimeEditor>();
                            editor.RegisterCreatedObjects(createdObjects);
                        }

                        m_treeView.ExternalItemDrop();
                        m_isSpawningPrefab = false;
                    });
                }
                else
                {
                    m_treeView.ExternalItemDrop();
                }
            }
            else
            {
                m_treeView.ExternalItemDrop();
            }
        }

        private void Expand(ExposeToEditor item)
        {
            if (item == null)
            {
                return;
            }
            ExposeToEditor parent = item.GetParent();
            if (parent != null && !m_treeView.IsExpanded(parent))
            {
                Expand(parent);
            }

            if(item.HasChildren())
            {
                m_treeView.Expand(item);
            }
        }
    }
}

