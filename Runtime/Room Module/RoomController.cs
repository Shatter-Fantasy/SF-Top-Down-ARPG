using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Unity.U2D.Physics;
using UnityEngine.Serialization;

namespace SF.RoomModule
{
    using CameraModule;
    using Managers;
    using SF.RoomModule.RegionModule;
    using U2D.Physics;
    
    public class RoomController : MonoBehaviour, 
        ITriggerShapeCallback
    {
        
        /* TODO List:
            Room Auto Align: Make a method that allows taking in two transforms.
            each transform is the floor of two connected rooms. 
            We can round the x/y values of the transform to make sure they align perfect.
            We might have to make one room round using ceiling and one round using floor depending on the values.         */
        [FormerlySerializedAs("_roomCameraBounds")] 
        public Bounds RoomCameraBounds;

        public Vector2 RoomTileSize = new Vector2(15, 11);
        
        /// <summary>
        /// The id for the room's spawned instance the RoomController is controlling.
        /// </summary>
        public int RoomID;
        [NonSerialized] public List<int> RoomIdsToLoadOnEnter = new();
        
        public Action OnRoomEnteredHandler;
        public Action OnRoomExitHandler;

        #region Room Extensions
        /// <summary>
        /// Unfiltered list of all <see cref="IRoomExtension"/> that are connected to this room. 
        /// </summary>
        private readonly List<IRoomExtension> _roomExtensions = new();
        
        private ReadOnlyCollection<IRoomExtension> _roomEnteredExtensions;
        private ReadOnlyCollection<IRoomExtension> _roomExitedExtensions;
        private ReadOnlyCollection<IRoomExtension> _roomClearedExtensions;
        #endregion
        
        [SerializeReference] private SFShapeComponent _physicsShapeComponent;
        private void Awake()
        {
            if (TryGetComponent(out _physicsShapeComponent))
            {
                _physicsShapeComponent.BodyDefinition.type      = PhysicsBody.BodyType.Static;
            }
            
            // Non-allocating version when used with read only List<T>
            gameObject.GetComponents(_roomExtensions);
            
            if (_roomExtensions.Count > 0)
            {
                var roomsEntered =
                    _roomExtensions.Where((room => room.RoomExtensionType == RoomExtensionType.OnRoomEntered));
                _roomEnteredExtensions = new ReadOnlyCollection<IRoomExtension>(roomsEntered.ToList());

                var roomsExited =
                    _roomExtensions.Where((room => room.RoomExtensionType == RoomExtensionType.OnRoomExit));
                _roomExitedExtensions = new ReadOnlyCollection<IRoomExtension>(roomsExited.ToList());
                
                var roomsCleared =
                    _roomExtensions.Where((room => room.RoomExtensionType == RoomExtensionType.OnRoomCleared));
                _roomClearedExtensions = new ReadOnlyCollection<IRoomExtension>(roomsCleared.ToList());
            }
        }

        private void Start()
        {
            if (_physicsShapeComponent != null)
                _physicsShapeComponent.AddTriggerCallbackTarget(this);

            if (!RegionSystem.UsingRegionDatabase()) 
                return;
            
            if (RoomSystem.LoadedRegion == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"There is no region data set in the {nameof(RoomSystem)}");
                return;
#endif
            }

            if (RoomSystem.LoadedRegion[RoomID] == null)
            {
                Debug.LogWarning(
                    $"A room with the RoomIDInLoadingRegion of {RoomID} was not found in the RoomDatabase. Check if there was a room with the id of {RoomID} set inside the RoomDatabase");
                return;
            }

            RoomIdsToLoadOnEnter = RoomSystem.LoadedRegion[RoomID].ConnectedRoomsIDs;
            RoomSystem.LoadRoom(RoomID, loadDynamically: false, spawnedInstance: gameObject);
        }

        private void OnDestroy()
        {
            RoomSystem.CleanUpRoom(RoomID);
        }
        
        /// <summary>
        /// Changes the current room and invokes all the required CameraSystem, RoomSystem, and GameManagers calls. 
        /// </summary>
        public void MakeCurrentRoom()
        {
            if (!RoomSystem.IsRoomLoaded(RoomID))
            {
                return;
            }
            
            RoomSystem.LoadRoom(RoomID);
            RoomSystem.SetCurrentRoom(RoomID);
        }

        public void OnRoomCleared()
        {
            if (_roomClearedExtensions == null) return;

            foreach (var roomExtension in _roomClearedExtensions)
            {
                roomExtension.Process();
            }
        }
        
        public void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent, SFShapeComponent callingShapeComponent)
        {
            if (GameManager.Instance.ControlState == GameControlState.Cutscenes)
                return;

            // Grab the body data.
            var objectData = beginEvent.visitorShape.body.userData.objectValue;

            // SFShapeComponents default set the GameObject they are attached to as the objectValue in userData
            if (objectData is not GameObject visitingGameobject)
                return;

            if (!visitingGameobject.TryGetComponent(out PlayerControllerBody2D body2D))
                return;

            if (!body2D.CollisionInfo.CollisionActivated)
                return;

            OnRoomEnteredHandler?.Invoke();

            if (_roomEnteredExtensions != null)
            {
                foreach (var roomExtension in _roomEnteredExtensions)
                {
                    roomExtension.Process();
                }
            }

            PhysicsAABB aabb   = callingShapeComponent.Body.GetAABB();
            RoomCameraBounds = new Bounds(aabb.center,(aabb.extents * 2) + new Vector2(2,2));
            CameraController.UpdateRectangleConfiner(RoomCameraBounds);
            MakeCurrentRoom();
            //CameraController.UpdateActiveCameraBounds(transform.position,RoomCameraBounds.size, RoomCameraBounds.center);
        }

        public void OnTriggerEnd2D(PhysicsEvents.TriggerEndEvent endEvent, SFShapeComponent callingShapeComponent)
        {
            
            if (_roomExitedExtensions != null)
            {
                foreach (var roomExtension in _roomExitedExtensions)
                {
                    roomExtension.Process();
                }
            }
            
            OnRoomExitHandler?.Invoke();
        }
    }
}