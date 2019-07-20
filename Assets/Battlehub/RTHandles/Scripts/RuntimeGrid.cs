using Battlehub.RTCommon;
using UnityEngine;
using UnityEngine.Rendering;

namespace Battlehub.RTHandles
{
    /// <summary>
    /// Attach to camera
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class RuntimeGrid : RTEComponent
    {
        public RuntimeHandlesComponent Appearance;
        public float CamOffset = 0.0f;
        public bool AutoCamOffset = true;
        public Vector3 GridOffset;

        private Camera m_camera;

        protected override void AwakeOverride()
        {
            base.AwakeOverride();
            m_camera = GetComponent<Camera>();
            #if UNITY_2019_1_OR_NEWER
            RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
            #endif
        }

        protected override void OnDestroyOverride()
        {
            base.OnDestroyOverride();
            #if UNITY_2019_1_OR_NEWER
            RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
            #endif
        }

        #if UNITY_2019_1_OR_NEWER
        private void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if(camera == m_camera)
            {
                DrawGrid(camera);
            }
        }
        #endif

        protected virtual void Start()
        {
            RuntimeHandlesComponent.InitializeIfRequired(ref Appearance);
        }

        private void OnPostRender()
        {
            Camera camera = Camera.current;
            DrawGrid(camera);
        }

        private void DrawGrid(Camera camera)
        {
            if (AutoCamOffset)
            {
                Appearance.DrawGrid(camera, GridOffset, camera.transform.position.y);
            }
            else
            {
                Appearance.DrawGrid(camera, GridOffset, CamOffset);
            }
        }
    }
}

