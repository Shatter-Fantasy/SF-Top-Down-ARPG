using System;

namespace SF.Interactables
{
	[Flags]
	public enum InteractableMode : int
	{
		TriggerBegin = 1, // Allows a TriggerBegin2D event to start the interaction.
		Input = 2,
		RayCast = 4, // Used to do interaction during ray cast checks only.
		ItemUse = 8,
	}

	/// <summary>
	/// Allows any object to interact with the component implementing this interface.
	/// </summary>
	public interface IInteractable
	{
		InteractableMode InteractableMode { get; set; }

		void Interact();
	}
	
	/// <summary>
	/// Allows any object to interact with the component implementing this interface,
	/// while also passing in any type of data that might need to be read during the interaction.
	/// See <see cref="SF.DataManagement.SaveStation.Interact(PlayerController)"/> for an example.
	/// </summary>
	/// <typeparam name="T">The data that needs to be passed in during an interaction.</typeparam>
	public interface IInteractable<in T> : IInteractable
	{
		void Interact(T interactingComponent);
	}
}