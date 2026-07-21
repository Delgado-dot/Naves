using UnityEngine.InputSystem;

/// <summary>Utility for retrieving actions from the InputActionAsset.</summary>
public static class InputHelper
{
    /// <summary>Gets a named action from the Player action map.</summary>
    /// <param name="asset">The InputActionAsset to search.</param>
    /// <param name="actionName">The name of the action to find.</param>
    /// <returns>The InputAction if found, otherwise null.</returns>
    public static InputAction GetPlayerAction(InputActionAsset asset, string actionName)
    {
        if (asset == null)
            return null;

        var map = asset.FindActionMap("Player", false);

        return map?.FindAction(actionName, false);
    }
}