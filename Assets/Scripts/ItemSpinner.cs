using UnityEngine;

public class ItemSpinner : MonoBehaviour
{
    public float spinSpeed = 10f;

    [Header("Zoom Settings")]
    public Camera inspectCamera; // We gotta link the camera here!
    public float zoomSpeed = 20f;
    public float minZoom = 20f; // Closest zoom
    public float maxZoom = 60f; // Furthest zoom

    void Update()
    {
        // --- SPINNING ---
        if (Input.GetMouseButton(0))
        {
            float rotX = Input.GetAxis("Mouse X") * spinSpeed;
            float rotY = Input.GetAxis("Mouse Y") * spinSpeed;

            transform.Rotate(Vector3.down, rotX, Space.World);
            transform.Rotate(Vector3.right, rotY, Space.World);
        }

        // --- ZOOMING ---
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        
        if (scroll != 0f && inspectCamera != null)
        {
            // Subtracting because scrolling up (positive) should zoom IN (lower FOV)
            inspectCamera.fieldOfView -= scroll * zoomSpeed;
            
            // Lock it so we don't zoom inside the item or fly out into space 🪐
            inspectCamera.fieldOfView = Mathf.Clamp(inspectCamera.fieldOfView, minZoom, maxZoom);
        }
    }
}