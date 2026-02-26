using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace ZTDREditor.CameraModule
{
    using SF.CameraModule;
    
    [EditorTool("Edit SF Shape Component", typeof(CinemachineRectangleConfiner2D))]
    public sealed class CinemachineRectangleConfiner2DTool : EditorTool
    {
        private CinemachineRectangleConfiner2D _confiner2D;
        
        private readonly Color GrabHandleColor = Color.green;
        public override void OnActivated()
        {
            _confiner2D = target as CinemachineRectangleConfiner2D;
        }
        
        
        // This is silly example that oscillates the scale of the selected objects as they are moved.
        public override void OnToolGUI(EditorWindow _)
        {
            if (_confiner2D == null)
                return;

            ToolGUIUtilities.DrawBoundsIn2D(_confiner2D.transform.position, ref _confiner2D.ConfinerBounds);
        }
    }

    public static class ToolGUIUtilities
    {
        public static readonly Color GrabHandleColor =  Color.green;
        
        
        public static void DrawBoundsIn2D(Vector3 drawingScopeOrigin, ref Bounds bounds, GameObject target = null)
        {
            
            EditorGUI.BeginChangeCheck();
            var snap       = EditorSnapSettings.move;
            
            // We use the _confiner2D.ConfinerBounds.center for handle size since the view will be focused mostly around the center of the bounding box.
            var handleSize = HandleUtility.GetHandleSize(bounds.center) * .1f;
            
            using (new Handles.DrawingScope(Matrix4x4.TRS(drawingScopeOrigin, Quaternion.identity, Vector3.one)))
            {
             
                Handles.color = GrabHandleColor;
               
                {  // Get the size
                    EditorGUI.BeginChangeCheck(); 
                    
                    // We use the ConfinerBounds.max to place the slider handle at the top right of the shape. 
                    var newCornerPos = Handles.Slider2D
                        (
                            (Vector2)bounds.max,
                            Vector3.forward, // Only used for rendering the handle
                            Vector3.right, 
                            Vector3.up, 
                            handleSize,
                            Handles.CubeHandleCap,
                            snap
                        );
                    
                    if (EditorGUI.EndChangeCheck())
                    {
                        if(target != null)
                            Undo.RecordObject(target, $"Changing bounds size on {target.name}");
                        
                        /* We minus the center from the calculated new top right corner pos to get the changed value of the extent.
                         * to get a bounds size you just times the bounds extents by 2
                         * For safety we turn any negative values to a positive when we drag the box and inverse it to keep the size value a positive number. */
                        var temp = (newCornerPos - bounds.center) * 2;
                        bounds.size = new Vector3(Mathf.Abs(temp.x), Mathf.Abs(temp.y),0);
                    }
                }
                
                { // Get the center
                    EditorGUI.BeginChangeCheck();
                    var newCenter  = Handles.PositionHandle(bounds.center, Tools.handleRotation);
                    if (EditorGUI.EndChangeCheck())
                    {
                        if(target != null)
                            Undo.RecordObject(target, $"Changing bounds center on {target.name}");
                        
                        bounds.center = newCenter;
                    }
                }
                Handles.DrawWireCube(bounds.center ,bounds.size );
            }
        }
    }
}
