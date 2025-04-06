using Fusion;
using UnityEngine;

public class FusionInput : MonoBehaviour
{
    [Header("Input System")]
    private PlayerControls playerControl;
    private Vector2 _moveVector;
    private Vector2 _lookVector;
    private bool _pressedJump;
    private bool _pressRun;

    void Awake()
    {
        playerControl = new PlayerControls();

        playerControl.Player.Movement.performed += ctx => _moveVector = ctx.ReadValue<Vector2>();
        playerControl.Player.Movement.canceled += _ => _moveVector = Vector2.zero;

        playerControl.Player.Look.performed += ctx => _lookVector = ctx.ReadValue<Vector2>();
        playerControl.Player.Look.canceled += _ => _lookVector = Vector2.zero;

        playerControl.Player.Jump.started += _ => _pressedJump = true;
        playerControl.Player.Jump.canceled += _ => _pressedJump = false;

        playerControl.Player.Run.started += _ => _pressRun = true;
        playerControl.Player.Run.canceled += _ => _pressRun = false;
    }

    private void OnEnable()
    {
        playerControl.Enable();
    }

    private void OnDisable()
    {
        playerControl.Disable();
    }

    public NetworkInputData GetNetworkInput()
    {
        Vector3 move = new Vector3(_moveVector.x, 0, _moveVector.y);

        if (Camera.main != null)
        {
            Quaternion camRotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
            move = camRotation * move;
        }

        var input = new NetworkInputData
        {
            Move = new Vector2(move.x, move.z),
            Look = _lookVector
        };

        if (_pressedJump)
            input.Buttons.Set(NetworkInputData.BUTTON_JUMP, true);

        if (_pressRun)
            input.Buttons.Set(NetworkInputData.BUTTON_RUN, true);

        return input;
    }

}
