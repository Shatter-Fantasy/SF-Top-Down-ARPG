using Unity.Cinemachine;
using UnityEngine;

using SF.SpawnModule;
using Unity.Scripting.LifecycleManagement;

namespace SF.CameraModule
{
    /// <summary>
    /// The manager for the active main camera in playable levels.
    /// Contains helper methods for switching active cameras.
    /// </summary>
    public partial class CameraController : MonoBehaviour
    {
        /// <summary>
        /// This is the default priority that is set on the old virtual cameras that are being switched away from.
        /// </summary>
        public const int DeactivatedPriority = -1;

        /// <summary>
        /// This is the virtual camera priority value for the currently active player camera.
        /// </summary>
        public const int ActivePriority = 1;
        
        /// <summary>
        /// This is the virtual camera priority value for the cutscene virtual cameras when a cutscene is playing requiring camera overriding.
        /// </summary>
        public const int CutsceneCameraPriority = 6;

        /// <summary>
        /// How far away the virtual cameras camera is set 
        /// </summary>
        public const int CameraDistance = 10;
        
        [SerializeField] private CinemachineCamera _startingPlayerCamera;
        [SerializeField] private CinemachineRectangleConfiner2D _playerCamConfiner;
        public Transform CameraTarget;

        public static CameraController Instance
        {
            get 
            {
                if (_instance == null)
                    _instance = FindAnyObjectByType<CameraController>();

                if(_instance == null)
                    _instance = Camera.main?.gameObject.AddComponent<CameraController>();

                return _instance;
            }
            set { _instance = value; }
        }
        [AutoStaticsCleanup] private static CameraController _instance;
        
        [AutoStaticsCleanup] public static Camera MainCamera;
        [AutoStaticsCleanup] public static CinemachineBrain MainCameraBrain;
        [AutoStaticsCleanup] public static CinemachineCamera PlayerCamera;
        [AutoStaticsCleanup] public static CinemachineCamera ActiveCutsceneCamera;

        [SerializeField] private CinemachineRectangleConfiner2D _cameraConfiner; 

        private void Awake()
        {
            if (Instance != null && _instance != this)
            {
                Destroy(this);
                return; // We return because Destroy sometimes takes effect next frame.
            }
            
            Instance = this;

            MainCamera = GetComponent<Camera>();
            if (MainCamera != null)
                MainCamera.TryGetComponent(out MainCameraBrain);
            
            if (_startingPlayerCamera != null)
                PlayerCamera = _startingPlayerCamera;
            
            SpawnSystem.InitialPlayerSpawnHandler += SetInitialCameraTarget;
        }
        
        private void OnDestroy()
        { 
            _instance = null;

            CameraTarget = null;
            MainCamera = null;
            MainCameraBrain = null;
            PlayerCamera = null;
            ActiveCutsceneCamera = null;
            
            SpawnSystem.InitialPlayerSpawnHandler -= SetInitialCameraTarget;
        }
        
        /// <summary>
        /// Set's the <see cref="CameraTarget"/> of the CameraManager <see cref="Instance"/>.
        /// </summary>
        /// <param name="spawnedPlayer"></param>
        private void SetInitialCameraTarget(GameObject spawnedPlayer = null)
        {
            _instance.CameraTarget = spawnedPlayer?.transform;
            
            if(PlayerCamera != null
               && _instance.CameraTarget != null)
                SwitchPlayerCMCamera(PlayerCamera);
        }

        /// <summary>
        /// Used to teleport the camera to the player along side the player if they get teleported.
        /// </summary>
        public static void TeleportCameraToPlayer(Vector3 deltaPosition)
        {
            if (PlayerCamera == null || PlayerCamera.Target.TrackingTarget == null)
                return;

            PlayerCamera.OnTargetObjectWarped(PlayerCamera.Target.TrackingTarget,deltaPosition);
        }

        public static void UpdateActiveCameraBounds(Vector3 centerOfBounds,Vector3 sizeOfBounds, Vector2 offsetOfBounds )
        {
            if (_instance._cameraConfiner != null && sizeOfBounds != default)
            {
                _instance._cameraConfiner.ConfinerBounds = new Bounds(centerOfBounds,sizeOfBounds);
                _instance._cameraConfiner.OffsetOfBounds = offsetOfBounds;
            }
        }
        
        public static void UpdateRectangleConfiner(Bounds cameraBounds)
        {
            // Try to find a camera confiner if one is null and exit method if can't find one in scene.
            if (_instance._playerCamConfiner == null)
            {
                if (PlayerCamera != null)
                {
                    if(!PlayerCamera.TryGetComponent(out _instance._playerCamConfiner))
                        _instance._playerCamConfiner = PlayerCamera.gameObject.AddComponent<CinemachineRectangleConfiner2D>();
                }
                else
                    return;
            }
            
            _instance._playerCamConfiner.ConfinerBounds = cameraBounds;
        }
        
        /// <summary>
        /// Switches between the current <see cref="PlayerCamera"/> and makes a new room camera the <see cref="PlayerCamera"/>.
        /// </summary>
        /// <param name="cmCamera"></param>
        /// <param name="cameraBounds"></param>
        /// <param name="priority"></param>
        public static void SwitchPlayerCMCamera(CinemachineCamera cmCamera, Bounds cameraBounds = default, int priority = ActivePriority)
        {
            if(cmCamera == null)
                return;
            
            // If the Virtual Camera has a CinemachinePositionComposer on it set it's distance to our set default.
            if (cmCamera.TryGetComponent(out CinemachinePositionComposer positionComposer))
                positionComposer.CameraDistance = CameraDistance;

            if (PlayerCamera != null)
            {
                // Reset the previous/old virtual camera priority.
                // At this point Instance.PlayerCamera is still the old camera.
                // We also clear the old camera follow to prevent it from following the player while not the active camera.
                PlayerCamera.Follow = null;
                PlayerCamera.Priority = DeactivatedPriority;
            }
            
            PlayerCamera = cmCamera;        
            
            // From here Instance.PlayerCamera is the new camera.
            if(Instance.CameraTarget != null)
                PlayerCamera.transform.position = Instance.CameraTarget.position;

            if (_instance._cameraConfiner != null && cameraBounds != default)
                _instance._cameraConfiner.ConfinerBounds = cameraBounds;
            
            PlayerCamera.Priority = ActivePriority;
            
            // We don't add setting the PlayerCamera.Follow in the null check above for when we need to do cutscenes and not have a follow target
            PlayerCamera.Follow = Instance.CameraTarget;  
            PlayerCamera.Target.TrackingTarget = Instance.CameraTarget;  
            PlayerCamera.Target.LookAtTarget = Instance.CameraTarget;  
        }
        public static void ActivateCutsceneCMCamera(CinemachineCamera cmCamera)
        {
            if(cmCamera == null)
                return;

            /* Not an error if this check is null: This is an expected result in some cases.
                This can happen when loading the first room in an area,
                 loading a game file into a save room, or when doing certain types of RoomTransitions from scene to scene.
            */

            // If the Virtual Camera has a CinemachinePositionComposer on it set it's distance to our set default.
            if (cmCamera.TryGetComponent(out CinemachinePositionComposer positionComposer))
                positionComposer.CameraDistance = CameraDistance;

            if (ActiveCutsceneCamera != null)
            {
                // Reset the previous/old virtual camera priority.
                // At this point Instance.ActiveCutsceneCamera is still the old camera.
                // We also clear the old camera follow to prevent it from following the player while not the active camera.
                ActiveCutsceneCamera.Follow = null;
                ActiveCutsceneCamera.Priority = DeactivatedPriority;
            }
            
            ActiveCutsceneCamera = cmCamera;             
            // From here Instance.ActiveCutsceneCamera is the new camera.
            ActiveCutsceneCamera.Priority = CutsceneCameraPriority;
        }
        public static void SetCameraFollow(CinemachineCamera camera,Transform target)
        {
            if (camera == null || target == null)
                return;

            camera.Follow = target;
        }
    }
}
