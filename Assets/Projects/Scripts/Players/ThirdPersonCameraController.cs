using Unity.Cinemachine;
using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    public PlayerNameDisplay nameDisplay;
    public CinemachineCamera cam;
    public Transform cameraTarget;
    public float mouseSensitivity = 1.5f;
    public float minPitch = -30f;
    public float maxPitch = 60f;

    private float _yaw;
    private float _pitch;
    private PlayerControls playerControl;
    private Vector2 lookInput;

    private void Awake()
    {
        cam = ServiceLocator.Get<NetworkManager>().followCamera;
        playerControl = new PlayerControls();
        playerControl.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        playerControl.Player.Look.canceled += _ => lookInput = Vector2.zero;
    }

    private void OnEnable()
    {
        playerControl.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        playerControl.Disable();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cam != null)
            cam.Follow = cameraTarget;
    }

    void Update()
    {
        if (!cameraTarget)
        {
            return;
        }

        Vector2 delta = lookInput * mouseSensitivity;

        _yaw += delta.x;
        _pitch -= delta.y;
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

        cameraTarget.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
    }


    private bool IsLocalPlayer()
    {
        var movement = GetComponent<PlayerMovementNetwork>();
        return movement != null && movement.HasInputAuthority;
    }
}
