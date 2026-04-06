using SF.U2D.Physics;
using Unity.Collections;
using UnityEngine;
using Unity.U2D.Physics;

namespace SF.Interactables
{
    public class InteractionController : MonoBehaviour
    {
        [SerializeField] protected PhysicsQuery.QueryFilter _interactableFilter;
        [SerializeField] protected SFShapeComponent _collisionShape;
        [SerializeField] protected SFShapeComponent _hitShape;
        protected NativeArray<PhysicsShape> _hitShapes;
    }
}
