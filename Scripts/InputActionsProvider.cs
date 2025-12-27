using UnityEngine;
using UnityEngine.InputSystem;

public class InputActionsProvider : MonoBehaviour
{
    public static InputSystem_Actions Actions { get; private set; }

    static InputActionsProvider()
    {
        Actions = new InputSystem_Actions();
        Actions.Enable();
    }

    public static Vector2 PointerPosition =>
        Actions.Player.Point.ReadValue<Vector2>();

    public static bool LeftClickPressedThisFrame =>
        Actions.Player.Click.WasPressedThisFrame();

    public static bool LeftClickReleasedThisFrame =>
        Actions.Player.Click.WasReleasedThisFrame();

    public static bool LeftClickIsPressed =>
        Actions.Player.Click.IsPressed();

    public static bool GridToggleReleased =>
        Actions.UI.ToggleGrid.WasReleasedThisFrame();
}