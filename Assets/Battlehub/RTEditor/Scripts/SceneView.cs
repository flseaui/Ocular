using Battlehub.RTCommon;
using Battlehub.RTHandles;
using Battlehub.RTSL.Interface;
using Battlehub.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Battlehub.RTEditor
{
    public class SceneView : RuntimeWindow
    {
        private Plane m_dragPlane;
        private IProject m_project;
        private GameObject m_prefabInstance;
        private HashSet<Transform> m_prefabInstanceTransforms;
        private Vector3 m_point;
        private GameObject m_dropTarget;
        private AssetItem m_dragItem;
             
        protected override void AwakeOverride()
        {
            ActivateOnAnyKey = true;
            WindowType = RuntimeWindowType.Scene;
            base.AwakeOverride();
            m_project = IOC.Resolve<IProject>();

            if(!GetComponent<SceneViewInput>())
            {
                gameObject.AddComponent<SceneViewInput>();
            }    
        }

        public override void DragEnter(object[] dragObjects, PointerEventData eventData)
        {
            if (m_prefabInstance != null)
            {
                return;
            }

            if(Editor.DragDrop.DragObjects[0] is AssetItem)
            {
                AssetItem assetItem = (AssetItem)Editor.DragDrop.DragObjects[0];
                if (m_project.ToType(assetItem) == typeof(GameObject))
                {
                    Editor.DragDrop.SetCursor(KnownCursor.DropAllowed);
                    Editor.IsBusy = true;
                    m_project.Load(new[] { assetItem }, (error, obj) =>
                    {
                        Editor.IsBusy = false;
                        //if (IsPointerOver)
                        {
                            if (obj[0] is GameObject)
                            {
                                IScenePivot scenePivot = IOCContainer.Resolve<IScenePivot>();
                                Vector3 up = Vector3.up;
                                if (Mathf.Abs(Vector3.Dot(Camera.transform.up, Vector3.up)) > Mathf.Cos(Mathf.Deg2Rad))
                                {
                                    up = Vector3.Cross(Camera.transform.right, Vector3.up);
                                }
                                else
                                {
                                    up = Vector3.up;
                                }
                                m_dragPlane = new Plane(up, scenePivot.SecondaryPivot);

                                GameObject prefab = (GameObject)obj[0];
                                bool wasPrefabEnabled = prefab.activeSelf;
                                prefab.SetActive(false);

                                Vector3 point;
                                if (GetPointOnDragPlane(out point))
                                {
                                    m_prefabInstance = Instantiate(prefab, point, Quaternion.identity);
                                }
                                else
                                {
                                    m_prefabInstance = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                                }

                                m_prefabInstanceTransforms = new HashSet<Transform>(m_prefabInstance.GetComponentsInChildren<Transform>(true));

                                prefab.SetActive(wasPrefabEnabled);

                                ExposeToEditor exposeToEditor = m_prefabInstance.GetComponent<ExposeToEditor>();
                                if (exposeToEditor == null)
                                {
                                    exposeToEditor = m_prefabInstance.AddComponent<ExposeToEditor>();
                                }

                                exposeToEditor.SetName(obj[0].name);
                                m_prefabInstance.SetActive(true);
                            }
                        }
                    });
                    m_dragItem = null;
                }
                else if (m_project.ToType(assetItem) == typeof(Material))
                {
                    m_dragItem = assetItem;
                }
            }
        }

        public override void DragLeave(PointerEventData eventData)
        {
            base.DragLeave(eventData);
            
            if(!Editor.IsBusy)
            {
                Editor.DragDrop.SetCursor(KnownCursor.DropNowAllowed);
            }
            

            if (m_prefabInstance != null)
            {
                Destroy(m_prefabInstance);
                m_prefabInstance = null;
                m_prefabInstanceTransforms = null;
            }

            m_dragItem = null;
            m_dropTarget = null;
        }


        public override void Drag(object[] dragObjects, PointerEventData eventData)
        {
            base.Drag(dragObjects, eventData);

            Vector3 point;
            if (GetPointOnDragPlane(out point))
            {
                m_point = point;
                if (m_prefabInstance != null)
                {

                    m_prefabInstance.transform.position = m_point;

                    RaycastHit hit = Physics.RaycastAll(Pointer).Where(h => !m_prefabInstanceTransforms.Contains(h.transform)).FirstOrDefault();
                    if (hit.transform != null)
                    {
                        m_prefabInstance.transform.position = hit.point;
                    }
                }
            }
            
            if(m_dragItem != null)
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(Pointer, out hitInfo, float.MaxValue, Editor.CameraLayerSettings.RaycastMask))
                {
                    MeshRenderer renderer = hitInfo.collider.GetComponentInChildren<MeshRenderer>();
                    SkinnedMeshRenderer sRenderer = hitInfo.collider.GetComponentInChildren<SkinnedMeshRenderer>();

                    if (renderer != null || sRenderer != null)
                    {
                        Editor.DragDrop.SetCursor(KnownCursor.DropAllowed);
                        m_dropTarget = hitInfo.transform.gameObject;
                    }
                    else
                    {
                        Editor.DragDrop.SetCursor(KnownCursor.DropNowAllowed);
                        m_dropTarget = null;
                    }
                }
                else
                {
                    Editor.DragDrop.SetCursor(KnownCursor.DropNowAllowed);
                    m_dropTarget = null;
                }
            }
        }

        public override void Drop(object[] dragObjects, PointerEventData eventData)
        {
            if(m_prefabInstance != null)
            {
                RecordUndo();
                m_prefabInstance = null;
                m_prefabInstanceTransforms = null;
            }

            if (m_dropTarget != null)
            {
                MeshRenderer renderer = m_dropTarget.GetComponentInChildren<MeshRenderer>();
                SkinnedMeshRenderer sRenderer = m_dropTarget.GetComponentInChildren<SkinnedMeshRenderer>();

                if (renderer != null || sRenderer != null)
                {
                    AssetItem assetItem = (AssetItem)Editor.DragDrop.DragObjects[0];
                    Editor.IsBusy = true;
                    m_project.Load(new[] { assetItem }, (error, obj) =>
                    {
                        Editor.IsBusy = false;
                        
                        if(error.HasError)
                        {
                            IWindowManager wm = IOC.Resolve<IWindowManager>();
                            if(wm != null)
                            {
                                wm.MessageBox("Unable to load asset item ", error.ErrorText);
                            }
                            return;
                        }

                        if(obj[0] is Material)
                        {
                            if (renderer != null)
                            {
                                Editor.Undo.BeginRecordValue(renderer, Strong.PropertyInfo((MeshRenderer x) => x.sharedMaterials, "sharedMaterials"));
                                Material[] materials = renderer.sharedMaterials;
                                for (int i = 0; i < materials.Length; ++i)
                                {
                                    materials[i] = (Material)obj[0];
                                }
                                renderer.sharedMaterials = materials;
                            }

                            if (sRenderer != null)
                            {
                                Editor.Undo.BeginRecordValue(sRenderer, Strong.PropertyInfo((SkinnedMeshRenderer x) => x.sharedMaterials, "sharedMaterials"));
                                Material[] materials = sRenderer.sharedMaterials;
                                for (int i = 0; i < materials.Length; ++i)
                                {
                                    materials[i] = (Material)obj[0];
                                }
                                sRenderer.sharedMaterials = materials;
                            }

                            if (renderer != null || sRenderer != null)
                            {
                                Editor.Undo.BeginRecord();
                            }

                            if (renderer != null)
                            {
                                Editor.Undo.EndRecordValue(renderer, Strong.PropertyInfo((MeshRenderer x) => x.sharedMaterials, "sharedMaterials"));
                            }

                            if (sRenderer != null)
                            {
                                Editor.Undo.EndRecordValue(sRenderer, Strong.PropertyInfo((SkinnedMeshRenderer x) => x.sharedMaterials, "sharedMaterials"));
                            }

                            if (renderer != null || sRenderer != null)
                            {
                                Editor.Undo.EndRecord();
                            }
                        }
                    });
                }

                m_dropTarget = null;
                m_dragItem = null;
            }
        }

        private void RecordUndo()
        {
            ExposeToEditor exposeToEditor = m_prefabInstance.GetComponent<ExposeToEditor>();

            Editor.Undo.BeginRecord();
            Editor.Undo.RegisterCreatedObjects(new[] { exposeToEditor });
            Editor.Selection.activeGameObject = m_prefabInstance;
            Editor.Undo.EndRecord();
        }

    
        private bool GetPointOnDragPlane(out Vector3 point)
        {
            Ray ray = Pointer;
            float distance;
            if (m_dragPlane.Raycast(ray, out distance))
            {
                point = ray.GetPoint(distance);
                return true;
            }
            point = Vector3.zero;
            return false;
        }
    }
}
