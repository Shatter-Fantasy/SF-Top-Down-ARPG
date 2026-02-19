using SF.PhysicsLowLevel;
using Unity.Collections;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace SF.Interactables
{
    public class InteractionController : MonoBehaviour
    {
        [SerializeField] protected PhysicsQuery.QueryFilter _interactableFilter;
        [SerializeField] protected SFShapeComponent _hitShape;
        protected NativeArray<PhysicsShape> _hitShapes;
    }
}
