using SF.DataManagement;
using UnityEngine;

namespace SF.SpawnModule
{
    public class CheckPoint : SavePoint
	{
		[SerializeField] public bool _doesActivateOnTriggerEnter2D = true;
		private void OnTriggerEnter2D(Collider2D collision)
		{
			if(!_doesActivateOnTriggerEnter2D)
				return;

			ActivateCheckPoint();
		}

        /// <summary>
        /// Invokes the checkpoint <see cref="CheckPointManager.ChangeCheckPoint"/> event to tell the checkpoint manager to set a new checkpoint.
        /// </summary>
        public virtual void ActivateCheckPoint()
		{
			CheckPointManager.ChangeCheckPoint(this);
		}
	}
}
