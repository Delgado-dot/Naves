using UnityEngine.InputSystem;

public static class InputHelper
{
    public static InputAction GetPlayerAction(InputActionAsset asset, string actionName)
    {
        if (asset == null)
            return null;

        var map = asset.FindActionMap("Player", false);

        return map?.FindAction(actionName, false);
    }
}
