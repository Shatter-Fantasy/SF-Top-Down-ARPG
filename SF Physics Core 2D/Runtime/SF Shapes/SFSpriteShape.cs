using System.Collections.Generic;
using Unity.Collections;
using Unity.U2D.Physics;
using UnityEngine;

namespace SF.U2D.Physics
{
    [DefaultExecutionOrder(PhysicsCore2DExecutionOrder.PhysicsBody)]
    public class SFSpriteShape : SFShapeComponent
    {
        [SerializeField, HideInInspector] private SpriteRenderer _spriteRenderer;
        [SerializeField, HideInInspector] private Sprite _sprite;
        private readonly List<Vector2> _physicsShapeVertex = new();
        
        protected override void PreEnabled()
        {
           _spriteRenderer ??= GetComponent<SpriteRenderer>();
           if (_spriteRenderer != null)
               _sprite = _spriteRenderer.sprite;
        }

        protected override void CreateBodyShapeGeometry()
        {
            if(_spriteRenderer == null || _sprite == null)
                return;
            
            var physicsShapeCount = _sprite.GetPhysicsShapeCount();
            if (physicsShapeCount == 0)
                return;
            
            var composer = PhysicsComposer.Create();
            composer.useDelaunay = _useDelaunay;

            using var   vertexPath        = new NativeList<Vector2>(Allocator.Temp);
            
            // Add all physic shape paths.
            for (var i = 0; i < physicsShapeCount; ++i)
            {
                if (_sprite.GetPhysicsShape(i, _physicsShapeVertex) > 0)
                {
                    // Add to something we can use.
                    for (int v = 0; v < _physicsShapeVertex.Count; v++)
                    {
                        
                            vertexPath.Add(_physicsShapeVertex[v]);
                    }
                }
                
                PhysicsTransform spriteTransform = PhysicsTransform.identity;
                spriteTransform.position += Offset;
                
                composer.AddLayer(vertexPath.AsArray(), spriteTransform);
            }
            
            vertexPath.Clear();
            
            if (!composer.isValid)
                return;
            
            using var polygons = composer.CreatePolygonGeometry(vertexScale: transform.lossyScale, Allocator.Temp);
            
            composer.Destroy();
            if (polygons.Length == 0)
                return;
            
            using var shapes = Body.CreateShapeBatch(polygons, ShapeDefinition);
            
            _shape = shapes[0];
        }
    }
}
