using RedBjorn.ProtoTiles;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    float angle = 15;
    float mapHeight;
    float zoomFactor = 1;
    float zoomSpeed = 0.05f;
    float minCameraHeight = 5;
    float maxCameraHeight = 20;
    float cameraMoveSpeed = 1; // Could be replaced by an exponential speed
    float dragSpeed = 0.02f;
    Vector3 dragStartWorld;
    Vector3 dragStartCameraPos;
    Plane plane;


    void Start()
    {
        transform.rotation = Quaternion.Euler(90-angle, 0, 0);
    }

    public void Init(MapSettings settings)
    {
        plane = settings.Plane();
    }

    void LateUpdate()
    {
        HandleZooming();
        HandleMoving();
    }

    void HandleZooming()
    {
        if (InputActionsProvider.IsZooming)
        {
            zoomFactor += InputActionsProvider.ZoomDelta*zoomSpeed;
            if (zoomFactor > 1) zoomFactor = 1;
            if (zoomFactor < 0) zoomFactor = 0;

            float cameraHeight = minCameraHeight + (maxCameraHeight-minCameraHeight) * (1-zoomFactor);
            transform.position = new(transform.position.x, cameraHeight, transform.position.z);
        }
    }

    void HandleMoving()
    {
        // Move camera through main way (e.g. aswd, controller stick, ...)
        if (InputActionsProvider.IsMainMovementActive)
        {
            Vector2 direction = InputActionsProvider.MainMoveDelta;
            Vector3 delta = cameraMoveSpeed * transform.position.y * Time.deltaTime * new Vector3(direction.x, 0, direction.y);
            transform.position += delta;
        }

        // Move camera through secondary way (right click dragging)
        if (InputActionsProvider.RightClickPressedThisFrame)
        {
            dragStartWorld = GetWorldPoint();
            dragStartCameraPos = transform.position;
        }
        if (InputActionsProvider.RightClickIsPressed)
        {
            Vector3 currentWorld = GetWorldPoint();
            Vector3 worldDelta = dragStartWorld - currentWorld;

            transform.position = Vector3.Lerp(
                transform.position,
                dragStartCameraPos + worldDelta*4f,
                0.25f
            );
        }
    }

    Vector3 GetWorldPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(
            InputActionsProvider.PointerPosition
        );

        float enter;
        if (plane.Raycast(ray, out enter))
            return ray.GetPoint(enter);

        return Vector3.zero;
    }
}
