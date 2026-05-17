using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace ZTDREditor.CameraModule
{

    using SF.RoomModule;
    
    [EditorTool("Edit Room Controller Camera Bounds", typeof(RoomController))]
    public sealed class RoomControllerSceneTool : EditorTool
    {
        private RoomController _roomController ;
        
        private readonly Color GrabHandleColor = Color.green;
        public override void OnActivated()
        {
            _roomController = target as RoomController;
        }
        
        
        // This is silly example that oscillates the scale of the selected objects as they are moved.
        public override void OnToolGUI(EditorWindow _)
        {
            if (_roomController == null)
                return;

            ToolGUIUtilities.DrawBoundsIn2D(_roomController.transform.position, ref _roomController.RoomCameraBounds);
        }
    }
}
