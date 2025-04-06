using Fusion;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementNetwork : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float runMultiplier = 1.5f;
    public float speedChangeRate = 10f;

    [Header("Jump & Gravity")]
    public float jumpHeight = 1.2f;
    public float gravity = -15f;
    public float terminalVelocity = 53f;

    [Header("References")]
    public Animator animator;
    public Transform cameraTransform;
    public Transform modelVisual;

    // Physics
    private CharacterController controller;
    private Vector3 _moveDir;
    private Vector3 _velocity;

    // Runtime
    private float _animationBlend;

    [Networked] private Vector3 NetworkedPosition { get; set; }
    [Networked] private Vector3 NetworkedVelocity { get; set; }
    [Networked] private bool IsGrounded { get; set; }
    [Networked] private Vector3 NetworkedMoveDir { get; set; }
    [Networked] private bool NetworkedIsMoving { get; set; }
    [Networked] private bool NetworkedIsRunning { get; set; }

    public override void Spawned()
    {
        controller = GetComponent<CharacterController>();

        GetComponent<ThirdPersonCameraController>().enabled = HasInputAuthority;

        if (HasInputAuthority)
        {
            cameraTransform = Camera.main.transform;
            var cam = ServiceLocator.Get<NetworkManager>().followCamera;
            if (cam != null)
                cam.Follow = transform;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            if (GetInput(out NetworkInputData input))
            {
                _moveDir = new Vector3(input.Move.x, 0, input.Move.y);
                float moveMagnitude = _moveDir.magnitude;
                float currentSpeed = input.Buttons.IsSet(NetworkInputData.BUTTON_RUN) ? moveSpeed * runMultiplier : moveSpeed;

                if (moveMagnitude > 0.01f)
                {
                    _moveDir.Normalize();
                    NetworkedMoveDir = _moveDir;

                    if (modelVisual != null)
                        modelVisual.rotation = Quaternion.Slerp(modelVisual.rotation, Quaternion.LookRotation(_moveDir), 0.2f);
                }
                else
                {
                    NetworkedMoveDir = Vector3.zero;
                }

                Vector3 move = _moveDir * currentSpeed;

                IsGrounded = controller.isGrounded;
                if (IsGrounded && _velocity.y < 0)
                    _velocity.y = -2f;

                if (IsGrounded && input.Buttons.IsSet(NetworkInputData.BUTTON_JUMP))
                    _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

                _velocity.y += gravity * Runner.DeltaTime;
                Vector3 finalMove = move + Vector3.up * _velocity.y;
                controller.Move(finalMove * Runner.DeltaTime);

                NetworkedPosition = transform.position;
                NetworkedVelocity = _velocity;
                Vector3 inputDir = new Vector3(input.Move.x, 0, input.Move.y);
                NetworkedIsMoving = inputDir.magnitude > 0.1f;
                NetworkedIsRunning = input.Buttons.IsSet(NetworkInputData.BUTTON_RUN);
            }
        }
        else
        {
            Vector3 diff = NetworkedPosition - transform.position;

            // calculate lag
            if (diff.sqrMagnitude > 2f * 2f) // 2 units
            {
                controller.enabled = false;
                transform.position = NetworkedPosition;
                controller.enabled = true;
            }
            else
            {
                Vector3 target = Vector3.Lerp(transform.position, NetworkedPosition, Runner.DeltaTime * 2f);
                Vector3 moveDelta = target - transform.position;

                controller.Move(moveDelta);
            }

            transform.position = NetworkedPosition;

            _moveDir = NetworkedMoveDir;
            _velocity = NetworkedVelocity;
        }

    }

    public override void Render()
    {
        if (animator != null)
        {
            float animSpeed = 0f;

            if (NetworkedIsMoving)
                animSpeed = NetworkedIsRunning ? 5f : 2f;

            _animationBlend = Mathf.Lerp(_animationBlend, animSpeed, Time.deltaTime * speedChangeRate);
            animator.SetFloat(AnimatorParams.Speed, _animationBlend);


            animator.SetFloat(AnimatorParams.MotionSpeed, 1);
            animator.SetBool(AnimatorParams.Grounded, IsGrounded);

            if (!IsGrounded)
            {
                if (_velocity.y > 0)
                {
                    animator.SetBool(AnimatorParams.Jump, true);
                    animator.SetBool(AnimatorParams.FreeFall, false);
                }
                else
                {
                    animator.SetBool(AnimatorParams.Jump, false);
                    animator.SetBool(AnimatorParams.FreeFall, true);
                }
            }
            else
            {
                animator.SetBool(AnimatorParams.Jump, false);
                animator.SetBool(AnimatorParams.FreeFall, false);
            }
        }


        if (_moveDir.sqrMagnitude > 0.01f && modelVisual != null)
        {
            modelVisual.rotation = Quaternion.Slerp(
                modelVisual.rotation,
                Quaternion.LookRotation(_moveDir),
                Time.deltaTime * 15f
            );
        }
    }


    // === Key event for read only animation ===
    private void OnFootstep(AnimationEvent animationEvent)
    {

    }

    private void OnLand(AnimationEvent animationEvent)
    {

    }
}
