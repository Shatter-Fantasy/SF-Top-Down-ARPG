using UnityEngine;
using UnityEngine.Events;

namespace SF.RoomModule
{
    /// <summary>
    /// Used to invoke events when all enemies in a room has been cleared.
    /// </summary>
    public class RoomClearedExtension : MonoBehaviour, IRoomExtension
    {
        public RoomExtensionType RoomExtensionType { get; } = RoomExtensionType.OnRoomCleared;

        [SerializeField] private UnityEvent _onRoomClearedUnityEvent;
        
        public void Process()
        {
            _onRoomClearedUnityEvent?.Invoke();
        }
    }
}
