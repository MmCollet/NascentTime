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

    public static Vector2 MainMoveDelta => 
        Actions.Player.Move.ReadValue<Vector2>();

    public static Vector2 PointerPosition =>
        Actions.Player.Point.ReadValue<Vector2>();
    
    public static Vector2 PointerDelta =>
        Actions.Player.DragDelta.ReadValue<Vector2>();

    public static bool LeftClickPressedThisFrame =>
        Actions.Player.Click.WasPressedThisFrame();

    public static bool LeftClickReleasedThisFrame =>
        Actions.Player.Click.WasReleasedThisFrame();

    public static bool LeftClickIsPressed =>
        Actions.Player.Click.IsPressed();

    public static bool RightClickIsPressed =>
        Actions.Player.SecondaryClick.IsPressed();

    public static bool GridToggleReleased =>
        Actions.UI.ToggleGrid.WasReleasedThisFrame();
    
    /// <summary>
    /// Raw zoom delta for this frame
    /// </summary>
    public static float ZoomDelta =>
        Actions.Player.Zoom.ReadValue<float>();

    /// <summary>
    /// True if zoom was used this frame
    /// </summary>
    public static bool IsZooming =>
        Mathf.Abs(ZoomDelta) > 0.001f;

    public static bool IsMainMovementActive =>
        MainMoveDelta.magnitude > 0.01f;
}