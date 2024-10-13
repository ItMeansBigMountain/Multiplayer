using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;

public class Top_Down_Camera_Follow : MonoBehaviour
{
    [Header("Camera Settings")]
    public Vector3 offset;
    public Transform Player;
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;

    private float zRot = 0, yRot = 0;
    private Vector3 startPos;
    private bool isFirstTouch = true, isHolding = false;

    [Header("Input & Shooting Settings")]
    private StarterAssetsInputs starterAssetsInputs;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shotPoint;
    [SerializeField] private Animator anim;
    [SerializeField] private float shootCooldown = 0.5f; // Cooldown between shots
    private float nextShootTime = 0f;

    [Header("Grounding & Jumping Settings")]
    public bool Grounded = true;
    public float GroundedOffset = -0.14f;
    public float GroundedRadius = 0.28f;
    public LayerMask GroundLayers;
    private float _verticalVelocity;
    private float Gravity = -15f;
    private float JumpHeight = 1.2f;

    private void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
    }

    private void Start()
    {
        offset = new Vector3(0f, 10f, -7.5f);
    }

    private void Update()
    {
        HandleCameraMovement();
        HandleShooting();
        JumpAndGravity();
    }

    private void LateUpdate()
    {
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        // Follow player with the given offset
        if (Player != null)
        {
            transform.position = Player.position + offset;
        }
    }

    private void HandleCameraMovement()
    {
        // Touch Input for Phones
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            if (Input.touchCount > 0)
            {
                if (isFirstTouch)
                {
                    isFirstTouch = false;
                    startPos = Input.GetTouch(0).position;
                }
                else
                {
                    zRot += (Input.GetTouch(0).position.y - startPos.y) * 0.09f;
                    zRot = Mathf.Clamp(zRot, -55, 55);
                    yRot += (Input.GetTouch(0).position.x - startPos.x) * 0.25f;
                    yRot = Mathf.Clamp(yRot, -75, 75);
                    startPos = Input.GetTouch(0).position;
                    isHolding = true;
                }
            }
            else
            {
                isFirstTouch = true;
                isHolding = false;
            }
        }
        // Mouse Input for PCs
        else
        {
            if (Input.GetMouseButton(1))
            {
                if (isFirstTouch)
                {
                    isFirstTouch = false;
                    startPos = Input.mousePosition;
                }
                else
                {
                    zRot += (Input.mousePosition.y - startPos.y) * 0.1f;
                    zRot = Mathf.Clamp(zRot, -55, 55);
                    yRot += (Input.mousePosition.x - startPos.x) * 0.3f;
                    yRot = Mathf.Clamp(yRot, -55, 55);
                    startPos = Input.mousePosition;
                    isHolding = true;
                }
            }
            else
            {
                isFirstTouch = true;
                isHolding = false;
            }
        }

        if (isHolding)
        {
            transform.eulerAngles = new Vector3(35, yRot, zRot);
        }
        else
        {
            // Snap back to the original direction smoothly
            zRot = Mathf.Lerp(zRot, 0, 0.2f);
            yRot = Mathf.Lerp(yRot, 0, 0.2f);
            transform.eulerAngles = new Vector3(35, yRot, zRot);
        }
    }

    private void HandleShooting()
    {
        if (starterAssetsInputs.Shoot && Time.time >= nextShootTime)
        {
            Shoot();
            nextShootTime = Time.time + shootCooldown; // Set next allowed shoot time
            starterAssetsInputs.Shoot = false; // Reset shoot input for single-shot
        }
    }

    private void Shoot()
    {
        // Instantiate the bullet at the shot point with forward direction
        Instantiate(bulletPrefab, shotPoint.position, Quaternion.LookRotation(transform.forward, Vector3.up));

        // Play shooting animation if available
        if (anim != null)
        {
            anim.SetTrigger("Shoot");
        }

        Debug.Log("Bullet Fired");
    }

    private void JumpAndGravity()
    {
        GroundedCheck();

        if (Grounded)
        {
            // Reset fall velocity
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // Jump when grounded
            if (starterAssetsInputs.jump)
            {
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                starterAssetsInputs.jump = false;
            }
        }
        else
        {
            // Apply gravity if not grounded
            if (_verticalVelocity > -53f)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
            starterAssetsInputs.jump = false;
        }

        // Apply vertical movement
        Vector3 move = new Vector3(0, _verticalVelocity, 0);
        transform.position += move * Time.deltaTime;
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
    }
}
