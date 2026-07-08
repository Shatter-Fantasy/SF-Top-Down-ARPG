using Unity.U2D.Physics;
using UnityEngine;

namespace SF.U2D.Physics
{
    /// <summary>
    /// Use to validate your Physics Core 2D Module code for different cases that could cuase bugs or issues.
    /// This should be used mostly during development building not in a production build.
    /// </summary>
    public static class SFPhysicsValidator
    {

#region Non-SFShapeComponent Validation
	    /* The SFShapeComponents have a built in validation logic in the DebugPhysics method
	     * and can have custom user defined logic using the DebugPhysicsExtra method.
	     *
	     * The below methods in this region code block is for general PhysicsShape and PhysicsBody structs.
	     * */

	    /// <summary>
	    /// Checks to make sure if a callback target is safe to use. Note just because something isn't safe doesn't mean it won't work.
	    /// The behavior could just be not what is expected and cause logical bugs dduring development and runtime.
	    /// </summary>
	    /// <param name="body"></param>
	    /// <returns></returns>
	    public static bool IsCallbackTargetSafe(this PhysicsBody body)
	    {
		    if (!body.isValid)
		    {
			    Debug.LogWarning($"PhysicsBody is not valid, called during {nameof(IsCallbackTargetSafe)} method.");
			    return false;
		    }


		    if (body.callbackTarget.GetType().IsValueType)
		    {
			    Debug.LogWarning($"PhysicsBody.callbackTarget is not a reference type, but a value type, and can possibly be cleaned up by the garbage collector causing an unexpected null value to be set as the new callback target value. " +
			                     $"This can, not always though, stop PhysicsEvents from happening, called during {nameof(IsCallbackTargetSafe)} method.");
			    return false;
		    }


		    return true;
	    }
#endregion
    }
}