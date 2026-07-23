using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Collections;
using Unity.Scripting.LifecycleManagement;
using Unity.U2D.Physics;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace SF.U2D.Physics
{
    /// <summary>
    /// Keeps track of data and settings for custom SF Low Level Physics 2D
    /// API, classes, objects, and structs.
    /// </summary>
    public static partial class SFPhysicsManager
    {
        static SFPhysicsManager()
        {
            PhysicsCulling.CullingOperations.Add(new FarDistanceCullingOperation(50));
        }

        public static SFPhysicsCulling PhysicsCulling = new SFPhysicsCulling();

        /// <summary>
        /// Should the SF Low Level Physics API spit out debug information for the consoles. 
        /// </summary>
        public static bool UsingDebugMode = true;

        /// <summary>
        /// Should the SF Low Level Physics use the <see cref=""/>. 
        /// </summary>
        public static bool UsingDebugRendering = true;

#region PhysicsMask Layers
        public const int PlatformsLayer = 0;
        public const int OneWayPlatformsLayer = 1;
        public const int MovingPlatformsLayer = 2;

        public const int IgnoreQueriesLayer = 6;
        public const int VolumesLayer = 7;
        //public const int UILayer = 8;
        
        public const int PlayerLayer = 10;
        public const int PlayerBitmask = 1024;
        public const int EnemiesLayer = 11;
        public const int FriendliesLayer = 12;
        
        public const int WaterLayer = 14;
        public const int InteractableLayer = 17;
        public const int ProjectilesLayer = 18;
        public const int HitboxesLayer = 19;
#endregion
        [AutoStaticsCleanup] public static NativeList<PhysicsBody> ActiveBodies =
            new NativeList<PhysicsBody>(10,allocator: Allocator.Persistent);

        private static TransformHandle _centerHandle;
        public static TransformHandle CenterHandle
        {
            get => _centerHandle;
            set
            {
                if (!value.IsValid())
                    return;

                _centerHandle = value;
                SimulationCenter = _centerHandle.position;
            }
        }
        public static Vector2 SimulationCenter = new Vector2(0, 0);
        public const float FarDistanceDefault = 20;


        public static void CleanUpFarDistanceBodies(float farDistance = FarDistanceDefault)
        {
            CleanUpFarDistanceBodies(SimulationCenter, farDistance);
        }

        public static void CleanUpFarDistanceBodies(Vector2 center, float farDistance = FarDistanceDefault)
        {
            if(!ActiveBodies.IsCreated || ActiveBodies.Count < 1)
                return;
            NativeList<PhysicsBody> tempBodies = new NativeList<PhysicsBody>(0,Allocator.Temp);
            tempBodies.CopyFrom(ActiveBodies);
            foreach (var activeBody in tempBodies)
            {
                if (!activeBody.isValid)
                {
                    ActiveBodies.RemoveAt(ActiveBodies.IndexOf(activeBody));
                }
                else
                {
                    if (Vector2.Distance(activeBody.position, center) > farDistance)
                    {
                        ActiveBodies.RemoveAt(ActiveBodies.IndexOf(activeBody));
                        activeBody.Destroy();
                    }
                }
            }

            tempBodies.Dispose();
        }

        public static bool RunSFPhysicsPlayerLoop = false;


        /// <summary>
        /// Custom PlayerLoop injected update that runs certain <see cref="SFPhysicsManager"/> methods such as <see cref="CleanUpFarDistanceBodies"/>
        /// and others.
        /// </summary>
        private static void SFPhysicsUpdate()
        {
            // This is ready to be used and implemented.
            if (!RunSFPhysicsPlayerLoop || !Application.isPlaying)
                return;

            if (_centerHandle.IsValid())
                SimulationCenter = _centerHandle.position;

            PhysicsCulling.StartCulling();
        }

        [OnCodeInitializing]
        private static void OnCodeInitializing()
        {
            // Retrieve the default Player loop system. Get the current loop instead if the default was already modified previously.
            var defaultLoop = PlayerLoop.GetDefaultPlayerLoop();
            var fixedUpdateSubSystemLoop =
                defaultLoop.subSystemList.First(subsystem => subsystem.type == typeof(FixedUpdate));

            // Create a custom update system
            var myCustomUpdate = new PlayerLoopSystem
            {
                subSystemList = null,
                updateDelegate = SFPhysicsUpdate,
                type = typeof(SFPhysicsManager)
            };

            // Add the custom update system after the PreLateUpdate phase in the Player Loop
            var loopWithCustomUpdate = InsertSystemAfter<FixedUpdate.PhysicsCore2DFixedUpdate>(in fixedUpdateSubSystemLoop,myCustomUpdate);

            // Create a new list to populate with subsystems, including the custom system
            List<PlayerLoopSystem> newFullLoop = new();
            //Iterate through the subsystems in the existing loop we passed in and add them to the new list
            for (var i = 0; i < defaultLoop.subSystemList.Length; i++)
            {
                if(defaultLoop.subSystemList[i].type != typeof(FixedUpdate))
                    newFullLoop.Add(defaultLoop.subSystemList[i]);
                else
                    newFullLoop.Add(loopWithCustomUpdate);
            }

            defaultLoop.subSystemList = newFullLoop.ToArray();
            PlayerLoop.SetPlayerLoop(defaultLoop);

            /*
            // Print the current Player loop to verify the custom update was added
            StringBuilder sb = new();
            RecursivePlayerLoopPrint(PlayerLoop.GetCurrentPlayerLoop(), sb, 0);
            Debug.Log(sb.ToString());
            */
        }

        private static PlayerLoopSystem InsertSystemAfter<T>(in PlayerLoopSystem subplayerLoop, PlayerLoopSystem newSystem) where T : struct
        {
            // Create a new root PlayerLoopSystem
            PlayerLoopSystem newPlayerLoop = new()
            {
                loopConditionFunction = subplayerLoop.loopConditionFunction,
                type = subplayerLoop.type,
                updateDelegate = subplayerLoop.updateDelegate,
                updateFunction = subplayerLoop.updateFunction
            };
            // Create a new list to populate with subsystems, including the custom system
            List<PlayerLoopSystem> newSubSystemList = new();
            //Iterate through the subsystems in the existing loop we passed in and add them to the new list
            if (subplayerLoop.subSystemList != null)
            {
                for (var i = 0; i < subplayerLoop.subSystemList.Length; i++)
                {
                    newSubSystemList.Add(subplayerLoop.subSystemList[i]);
                    if (subplayerLoop.subSystemList[i].type == typeof(T))
                    {
                        // If the previously added subsystem is of the type to add after, add the custom system
                        newSubSystemList.Add(newSystem);
                    }
                }
            }
            newPlayerLoop.subSystemList = newSubSystemList.ToArray();
            return newPlayerLoop;
        }


        private static void RecursivePlayerLoopPrint(PlayerLoopSystem playerLoopSystem, StringBuilder sb, int depth)
        {
            if (depth == 0)
            {
                sb.AppendLine("ROOT NODE");
            }
            else if (playerLoopSystem.type != null)
            {
                for (int i = 0; i < depth; i++)
                {
                    sb.Append("\t");
                }
                sb.AppendLine(playerLoopSystem.type.Name);
            }
            if (playerLoopSystem.subSystemList != null)
            {
                depth++;
                foreach (var s in playerLoopSystem.subSystemList)
                {
                    RecursivePlayerLoopPrint(s, sb, depth);
                }
                depth--;
            }
        }
    }
}