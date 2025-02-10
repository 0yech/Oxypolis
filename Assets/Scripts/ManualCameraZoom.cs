using UnityEngine;

public class ManualCameraZoom : MonoBehaviour
{
    public Camera mainCamera;
    public float zoomSpeed = 1f;
    public float maxZoom = 20f;
    public float minZoom = 3f;

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        HandleZoom();
    }

    private void HandleZoom()
    {
        if (mainCamera == null) return;

        float currentZoom = mainCamera.orthographicSize;

        if (Input.GetKey(KeyCode.W))
        {
            float newZoom = currentZoom - zoomSpeed * Time.deltaTime;
            newZoom = Mathf.Clamp(newZoom, minZoom, maxZoom);
            mainCamera.orthographicSize = newZoom;
        }

        if (Input.GetKey(KeyCode.X))
        {
            float newZoom = currentZoom + zoomSpeed * Time.deltaTime;
            newZoom = Mathf.Clamp(newZoom, minZoom, maxZoom);
            mainCamera.orthographicSize = newZoom;
        }

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0f)
        {
            mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize - scrollInput * zoomSpeed, minZoom, maxZoom);
        }
    }
}