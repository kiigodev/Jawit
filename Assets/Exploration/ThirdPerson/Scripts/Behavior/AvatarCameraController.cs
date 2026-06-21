using UnityEngine;

public class AvatarCameraController : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player;                                           // Referensi ke player
    public Vector3 pivotOffset = new Vector3(0.0f, 1.7f, 0.0f);        // Offset pivot kamera
    public Vector3 camOffset = new Vector3(0.0f, 0.0f, -3.0f);         // Offset kamera
    public float smooth = 10f; // Kehalusan perpindahan kamera
    public float rotationSmooth = 5f;   
    
    [Header("Zoom Settings")]
    public float zoomSpeed = 2f;
    public float minZoom = -1f; // Furthest away
    public float maxZoom = -1f; // Closest                               // Kehalusan rotasi kamera

    [Header("Value Settings")]
    public float horizontalAimingSpeed = 6f;                           // Sensitivitas horizontal
    public float verticalAimingSpeed = 6f;                             // Sensitivitas vertical
    public float maxVerticalAngle = 30f;                               // Batas atas vertikal
    public float minVerticalAngle = -60f;                              // Batas bawah vertikal
    public string XAxis = "Analog X";                                  // Input gamepad horizontal
    public string YAxis = "Analog Y";                                  // Input gamepad vertical

    private float angleH = 0;                                          // Angle horizontal kamera
    private float angleV = 0;                                          // Angle vertical kamera
    private float targetAngleH = 0;                                    // Target angle horizontal
    private float targetAngleV = 0;                                    // Target angle vertical
    private Transform cam;                                             // Transform kamera
    private Vector3 smoothPivotOffset;                                 // Offset pivot (smooth)
    private Vector3 smoothCamOffset;                                   // Offset kamera (smooth)
    private Vector3 targetPivotOffset;                                 // Target pivot
    private Vector3 targetCamOffset;                                   // Target offset
    private float defaultFOV;                                          // Default FOV
    private float targetFOV;                                           // Target FOV
    private float targetMaxVerticalAngle;                              // Target clamp vertical
    private bool isCustomOffset;                                       // Apakah pakai custom offset?

    // Get kamera horizontal
    private bool isFPSMode = false;
    public float GetH => angleH;

    public void SetFPSMode(bool active)
    {
        isFPSMode = active;
    }

    void Awake()
    {
        cam = transform;

        // Set posisi awal kamera
        cam.position = player.position + Quaternion.identity * pivotOffset + Quaternion.identity * camOffset;
        cam.rotation = Quaternion.identity;

        smoothPivotOffset = pivotOffset;
        smoothCamOffset = camOffset;
        defaultFOV = cam.GetComponent<Camera>().fieldOfView;
        angleH = player.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ResetTargetOffsets();
        ResetFOV();
        ResetMaxVerticalAngle();

    }

    void Update()
    {
        // Sinkronisasi runtime dari inspector kalau bukan custom offset
        if (!isCustomOffset)
        {
            targetPivotOffset = pivotOffset;
            targetCamOffset = camOffset;
        }

        // Grab mouse movement values
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // --- Mouse Orbit (Always active, 100% manual control!) ---
        targetAngleH += Mathf.Clamp(mouseX, -1, 1) * horizontalAimingSpeed;
        targetAngleV += Mathf.Clamp(mouseY, -1, 1) * verticalAimingSpeed;

        // --- Scroll Zoom ---
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f && !isCustomOffset)
        {
            camOffset.z += scroll * zoomSpeed;
            camOffset.z = Mathf.Clamp(camOffset.z, minZoom, maxZoom);
            targetCamOffset = camOffset; 
        }

        // Clamp vertical angle
        targetAngleV = Mathf.Clamp(targetAngleV, minVerticalAngle, targetMaxVerticalAngle);

        // Instant snap, no dizzy delay!
        angleH = targetAngleH;
        angleV = targetAngleV;

        // Hitung rotasi
        Quaternion camYRotation = Quaternion.Euler(0, angleH, 0);
        Quaternion aimRotation = Quaternion.Euler(-angleV, angleH, 0);

        // ===== Posisi Kamera =====
        Vector3 baseTempPosition = player.position + camYRotation * targetPivotOffset;
        Vector3 noCollisionOffset = targetCamOffset;
        while (noCollisionOffset.magnitude >= 0.2f)
        {
            if (DoubleViewingPosCheck(baseTempPosition + aimRotation * noCollisionOffset))
                break;
            noCollisionOffset -= noCollisionOffset.normalized * 0.2f;
        }
        if (noCollisionOffset.magnitude < 0.2f)
            noCollisionOffset = Vector3.zero;

        bool customOffsetCollision = isCustomOffset && noCollisionOffset.sqrMagnitude < targetCamOffset.sqrMagnitude;

        // Smooth posisi kamera
        smoothPivotOffset = Vector3.Lerp(smoothPivotOffset, customOffsetCollision ? pivotOffset : targetPivotOffset, smooth * Time.deltaTime);
        smoothCamOffset = Vector3.Lerp(smoothCamOffset, customOffsetCollision ? Vector3.zero : noCollisionOffset, smooth * Time.deltaTime);

        cam.position = player.position + camYRotation * smoothPivotOffset + aimRotation * smoothCamOffset;

        if (!isFPSMode)
        {
            // Third person: kamera lihat ke player
            cam.LookAt(player.position + camYRotation * smoothPivotOffset);
        }
        else
        {
            // First person: kamera ikut rotasi mouse, jangan lihat player lagi
            cam.rotation = Quaternion.Euler(-angleV, angleH, 0);
        }

        // Smooth FOV
        cam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(cam.GetComponent<Camera>().fieldOfView, targetFOV, Time.deltaTime);
    }

    // === Utility ===
    public void SetTargetOffsets(Vector3 newPivotOffset, Vector3 newCamOffset)
    {
        targetPivotOffset = newPivotOffset;
        targetCamOffset = newCamOffset;
        isCustomOffset = true;
    }

    public void ResetTargetOffsets()
    {
        targetPivotOffset = pivotOffset;
        targetCamOffset = camOffset;
        isCustomOffset = false;
    }

    public void ResetYCamOffset() => targetCamOffset.y = camOffset.y;
    public void SetYCamOffset(float y) => targetCamOffset.y = y;
    public void SetXCamOffset(float x) => targetCamOffset.x = x;

    public void SetFOV(float customFOV) => targetFOV = customFOV;
    public void ResetFOV() => targetFOV = defaultFOV;

    public void SetMaxVerticalAngle(float angle) => targetMaxVerticalAngle = angle;
    public void ResetMaxVerticalAngle() => targetMaxVerticalAngle = maxVerticalAngle;

    bool DoubleViewingPosCheck(Vector3 checkPos) => ViewingPosCheck(checkPos) && ReverseViewingPosCheck(checkPos);

    bool ViewingPosCheck(Vector3 checkPos)
    {
        Vector3 target = player.position + pivotOffset;
        Vector3 direction = target - checkPos;
        if (Physics.SphereCast(checkPos, 0.2f, direction, out RaycastHit hit, direction.magnitude))
        {
            if (hit.transform != player && !hit.transform.GetComponent<Collider>().isTrigger)
                return false;
        }
        return true;
    }

    bool ReverseViewingPosCheck(Vector3 checkPos)
    {
        Vector3 origin = player.position + pivotOffset;
        Vector3 direction = checkPos - origin;
        if (Physics.SphereCast(origin, 0.2f, direction, out RaycastHit hit, direction.magnitude))
        {
            if (hit.transform != player && hit.transform != transform && !hit.transform.GetComponent<Collider>().isTrigger)
                return false;
        }
        return true;
    }

    public float GetCurrentPivotMagnitude(Vector3 finalPivotOffset)
    {
        return Mathf.Abs((finalPivotOffset - smoothPivotOffset).magnitude);
    }

    public float GetCurrentFOV()
    {
        return GetComponent<Camera>().fieldOfView;
    }

    // === Tambahan untuk FPS mode ===
    public void FPSLook(float mouseX, float mouseY)
    {
        targetAngleH += mouseX;
        targetAngleV += -mouseY; // minus supaya gerak natural

        targetAngleV = Mathf.Clamp(targetAngleV, minVerticalAngle, maxVerticalAngle);

        angleH = targetAngleH;
        angleV = targetAngleV;
    }
}

