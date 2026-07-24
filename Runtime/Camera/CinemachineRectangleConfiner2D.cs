using Unity.Burst;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

namespace SF.CameraModule
{
    [AddComponentMenu("Cinemachine/Procedural/Extensions/Cinemachine Rectangle Confiner 2D")]
    [SaveDuringPlay]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [BurstCompile]
    public class CinemachineRectangleConfiner2D : CinemachineExtension
    {
        public Bounds ConfinerBounds;
        [FormerlySerializedAs("OriginsOfBounds")] public Vector3 OffsetOfBounds; 
        private Vector3 _correctedPosition;
        
        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage != CinemachineCore.Stage.Body)
                return;
            
            var camPosition = vcam.transform.position;
            var settings = state.Lens;
            float frustumHalfHeight = CalculateFrustumHalfHeight(settings.OrthographicSize,camPosition.z,settings.FieldOfView);
            float frustumHalfWidth = frustumHalfHeight * settings.Aspect;

            Vector3 pos = state.RawPosition;
            
            _correctedPosition.x = Mathf.Max(ConfinerBounds.min.x + OffsetOfBounds.x + frustumHalfWidth,
                Mathf.Min(ConfinerBounds.max.x - frustumHalfWidth + OffsetOfBounds.x, pos.x));
            
            _correctedPosition.y = Mathf.Max(ConfinerBounds.min.y + OffsetOfBounds.y + frustumHalfHeight,
                Mathf.Min(ConfinerBounds.max.y + OffsetOfBounds.y - frustumHalfHeight, pos.y));
            
            //Debug.Log($"YMin: {ConfinerBounds.min.y }, YMax:{ConfinerBounds.max.y }, X Corrected:{_correctedPosition.x}, Y Corrected:{_correctedPosition.y}");
            _correctedPosition.z = state.RawPosition.z;
            
            state.PositionCorrection.x += _correctedPosition.x - state.GetCorrectedPosition().x;
            state.PositionCorrection.y += _correctedPosition.y - state.GetCorrectedPosition().y;
        }
        
        /// <summary>
        /// Calculates half frustum height for orthographic or perspective camera.
        /// </summary>
        /// <param name="orthographicSize">Camera Lens Orthographic Size</param>
        /// <param name="cameraPosLocalZ"> Camera's z pos in local space</param>
        /// <param name="fieldOfView">Camera lens field of view</param>
        /// <param name="isOrthographic">Is camera lens orthographic</param>
        /// <returns>Frustum height of the camera</returns>
        /// <remarks>
        /// This method assumes the passed in Z has already been transformed from the 
        /// </remarks>
        [BurstCompile]
        public static float CalculateFrustumHalfHeight(
            in float orthographicSize, 
            in float cameraPosLocalZ,
            in float fieldOfView,
            bool isOrthographic = true)
        {
            float frustumHeight;
            
            // in Orthographic mode the half height of the frustum is literally just the lens.OrthographicSize
            if (isOrthographic) 
                frustumHeight = orthographicSize;
            else
            {
                // distance between the collider's plane and the camera
                float distance = cameraPosLocalZ;
                frustumHeight = distance * Mathf.Tan(fieldOfView * 0.5f * Mathf.Deg2Rad);
            }

            return Mathf.Abs(frustumHeight);
            
        }
    }
}