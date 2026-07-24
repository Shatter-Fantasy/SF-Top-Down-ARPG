using UnityEngine;
using Unity.U2D.Physics;

namespace SF.RoomModule
{
    using SF.CameraModule;
    using SF.U2D.Physics;

    /// <summary>
    /// A component to update the camera boundaries when hitting a trigger.
    /// </summary>
    public class RoomPhysicsShape : MonoBehaviour, ITriggerShapeCallback
    {
        [Header("Shape Properties")]
        [SerializeField] SFShapeComponent _confinerShapeComponent;

        [SerializeField] private Bounds _cameraBounds;

        private void Awake()
        {
            if (_confinerShapeComponent == null)
                return;

            _confinerShapeComponent.AddTriggerCallbackTarget(this);
        }

        public void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent, SFShapeComponent callingShapeComponent)
        {
            PhysicsAABB aabb   = callingShapeComponent.Body.GetAABB();
            _cameraBounds = new Bounds(aabb.center,(aabb.extents * 2) + new Vector2(2,2));
            CameraController.UpdateRectangleConfiner(_cameraBounds);
        }

        public void OnTriggerEnd2D(PhysicsEvents.TriggerEndEvent endEvent, SFShapeComponent callingShapeComponent)
        {
            // no-op
        }
    }
}