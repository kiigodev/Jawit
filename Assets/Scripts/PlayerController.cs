// using UnityEngine;

// [RequireComponent(typeof(Rigidbody))]
// public class PlayerController : MonoBehaviour
// {
//     [Header("References")]
//     [SerializeField] private Transform cameraPivot;
//     [SerializeField] private Transform mainCamera;
//     [SerializeField] private Transform modelTransform;

//     [Header("Movement")]
//     [SerializeField] private float moveSpeed = 5f;
//     [SerializeField] private float acceleration = 12f;
//     [SerializeField] private float deceleration = 16f;

//     [Header("Look")]
//     [SerializeField] private float mouseSensitivity = 200f;
//     [SerializeField] private float minPitch = -35f;
//     [SerializeField] private float maxPitch = 60f;

//     [Header("Camera Zoom")]
//     [SerializeField] private float minCameraDistance = 1.5f;
//     [SerializeField] private float maxCameraDistance = 7f;
//     [SerializeField] private float zoomSpeed = 3f;
//     [SerializeField] private float zoomSmoothTime = 0.08f;

//     [Header("Model Rotation")]
//     [SerializeField] private float rotationSmoothTime = 0.12f;

//     private Rigidbody _rb;

//     private Vector2 _moveInput;
//     private Vector3 _currentVelocity;

//     private float _pitch;
//     private float _targetCameraDistance;
//     private float _currentCameraDistance;
//     private float _zoomVelocity;
//     private float _modelRotationVelocity;

//     public Vector2 MoveInput => _moveInput;
//     public bool IsMoving => _moveInput.sqrMagnitude > 0.01f;
//     public Vector3 PlanarVelocity => new Vector3(_currentVelocity.x, 0f, _currentVelocity.z);

//     private void Awake()
//     {
//         _rb = GetComponent<Rigidbody>();
//         _rb.freezeRotation = true;
//         _rb.interpolation = RigidbodyInterpolation.Interpolate;

//         Cursor.lockState = CursorLockMode.Locked;
//         Cursor.visible = false;

//         InitializeCameraDistance();
//     }

//     private void Update()
//     {
//         ReadMoveInput();
//         Look();
//         ZoomCamera();
//         HandleCursorToggle();
//     }

//     private void FixedUpdate()
//     {
//         Move();
//         RotateModel();
//     }

//     private void InitializeCameraDistance()
//     {
//         if (mainCamera == null)
//             return;

//         float startingDistance = Mathf.Abs(mainCamera.localPosition.z);
//         startingDistance = Mathf.Clamp(startingDistance, minCameraDistance, maxCameraDistance);

//         _targetCameraDistance = startingDistance;
//         _currentCameraDistance = startingDistance;

//         mainCamera.localPosition = new Vector3(
//             mainCamera.localPosition.x,
//             mainCamera.localPosition.y,
//             -_currentCameraDistance
//         );
//     }

//     private void ReadMoveInput()
//     {
//         _moveInput.x = Input.GetAxisRaw("Horizontal");
//         _moveInput.y = Input.GetAxisRaw("Vertical");

//         if (_moveInput.sqrMagnitude > 1f)
//             _moveInput.Normalize();
//     }

//     private void Move()
//     {
//         Vector3 moveDirection = transform.right * _moveInput.x + transform.forward * _moveInput.y;

//         if (moveDirection.sqrMagnitude > 1f)
//             moveDirection.Normalize();

//         Vector3 targetVelocity = moveDirection * moveSpeed;

//         float rate = IsMoving ? acceleration : deceleration;

//         _currentVelocity = Vector3.MoveTowards(
//             _currentVelocity,
//             targetVelocity,
//             rate * Time.fixedDeltaTime
//         );

//         _rb.linearVelocity = new Vector3(
//             _currentVelocity.x,
//             _rb.linearVelocity.y,
//             _currentVelocity.z
//         );
//     }

//     private void Look()
//     {
//         if (cameraPivot == null)
//             return;

//         float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
//         float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

//         _pitch -= mouseY;
//         _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

//         cameraPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);

//         transform.Rotate(Vector3.up * mouseX);
//     }

//     private void ZoomCamera()
//     {
//         if (mainCamera == null)
//             return;

//         float scroll = Input.GetAxis("Mouse ScrollWheel");

//         if (Mathf.Abs(scroll) > 0.001f)
//         {
//             _targetCameraDistance -= scroll * zoomSpeed;
//             _targetCameraDistance = Mathf.Clamp(
//                 _targetCameraDistance,
//                 minCameraDistance,
//                 maxCameraDistance
//             );
//         }

//         _currentCameraDistance = Mathf.SmoothDamp(
//             _currentCameraDistance,
//             _targetCameraDistance,
//             ref _zoomVelocity,
//             zoomSmoothTime
//         );

//         mainCamera.localPosition = new Vector3(
//             mainCamera.localPosition.x,
//             mainCamera.localPosition.y,
//             -_currentCameraDistance
//         );
//     }

//     private void RotateModel()
//     {
//         if (modelTransform == null)
//             return;

//         if (!IsMoving)
//             return;

//         float inputAngle = Mathf.Atan2(_moveInput.x, _moveInput.y) * Mathf.Rad2Deg;
//         float targetAngle = transform.eulerAngles.y + inputAngle;

//         float smoothedAngle = Mathf.SmoothDampAngle(
//             modelTransform.eulerAngles.y,
//             targetAngle,
//             ref _modelRotationVelocity,
//             rotationSmoothTime
//         );

//         modelTransform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);
//     }

//     private void HandleCursorToggle()
//     {
//         if (Input.GetKeyDown(KeyCode.Escape))
//         {
//             Cursor.lockState = CursorLockMode.None;
//             Cursor.visible = true;
//         }

//         if (Input.GetMouseButtonDown(0))
//         {
//             Cursor.lockState = CursorLockMode.Locked;
//             Cursor.visible = false;
//         }
//     }
// }