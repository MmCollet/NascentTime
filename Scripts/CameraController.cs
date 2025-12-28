using UnityEngine;

public class CameraController : MonoBehaviour
{
    float angle = 15;
    float mapHeight;
    float zoomFactor = 1;
    float zoomSpeed = 0.05f;
    float minCameraHeight = 5;
    float maxCameraHeight = 20;
    float cameraMoveSpeed = 5; // Could be replaced by an exponential speed
    float dragSpeed = 0.02f;


    void Start()
    {
        transform.rotation = Quaternion.Euler(90-angle, 0, 0);
    }

    void LateUpdate()
    {
        if (InputActionsProvider.IsZooming)
        {
            zoomFactor += InputActionsProvider.ZoomDelta*zoomSpeed;
            if (zoomFactor > 1) zoomFactor = 1;
            if (zoomFactor < 0) zoomFactor = 0;

            float cameraHeight = minCameraHeight + (maxCameraHeight-minCameraHeight) * (1-zoomFactor);
            transform.position = new(transform.position.x, cameraHeight, transform.position.z);
        }

        if (InputActionsProvider.IsMainMovementActive)
        {
            Vector2 direction = InputActionsProvider.MainMoveDelta;
            Vector3 delta = cameraMoveSpeed * Time.deltaTime * new Vector3(direction.x, 0, direction.y);
            transform.position += delta;
        }
        if (InputActionsProvider.RightClickIsPressed)
        {
            Vector2 direction = InputActionsProvider.PointerDelta;
            Vector3 delta = dragSpeed * new Vector3(-direction.x, 0, -direction.y);
            transform.position += delta;
        }
    }
}
