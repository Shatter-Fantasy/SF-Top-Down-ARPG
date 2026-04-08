using System;
using SF.U2D.Physics;
using Unity.U2D.Physics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SF.LevelModule
{
    public class LevelTransition : MonoBehaviour, ITriggerShapeCallback
    {
        [SerializeField] private string _nextSceneName;
        private int _nextSceneIndex;
        [SerializeField] private SFShapeComponent _shapeComponent;

        private void Awake()
        {
            _shapeComponent ??= GetComponent<SFShapeComponent>();
            
            if(_shapeComponent != null)
                _shapeComponent.AddTriggerCallbackTarget(this);
        }

        private void Start()
        {

            if (!string.IsNullOrEmpty(_nextSceneName))
                _nextSceneIndex = SceneUtility.GetBuildIndexByScenePath(_nextSceneName);
        }

        public void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent, SFShapeComponent callingShapeComponent)
        {
            SceneManager.LoadSceneAsync(_nextSceneIndex);
        }

        public void OnTriggerEnd2D(PhysicsEvents.TriggerEndEvent endEvent, SFShapeComponent callingShapeComponent)
        {
            
        }
    }
}
