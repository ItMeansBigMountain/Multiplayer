using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;
using Photon.Pun;


public class ArenaShooter_PlayerControls : MonoBehaviour
{
    // Manual Configuration
    [Header("Manual Configuration")]
    [Tooltip("Normal movement sensitivity when not aiming (manual configuration).")]
    [SerializeField] private float Normal_Sensitvity = 125f;

    [Tooltip("Movement sensitivity while aiming (manual configuration).")]
    [SerializeField] private float Aim_Sensitivity = 100f;

    [Tooltip("Layer mask for detecting mouse collisions with objects (manual configuration).")]
    [SerializeField] private LayerMask mouseColliderLayerMask;

    [Tooltip("Reference to an Input Action for Look/aiming - For Cross-Platform Input (manual configuration).")]
    [SerializeField] private InputActionReference lookInput;

    [Tooltip("Public because the guns may change into the future (manual configuration).")]
    [SerializeField] public GameObject bullet_Prefab;

    // Auto-Configured Components
    [Header("Auto-Configured Components")]
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject lazerPointer;
    [SerializeField] private Transform shotPoint;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;
    [SerializeField] private ThirdPersonController thirdPersonController;
    [SerializeField] private float raycastMaxRange = 999f;
    private PhotonView photonView;




    void Awake()
    {
        // Get input and controller components
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();

        // Find the virtual camera named 'PlayerAimCamera' within the parent
        aimVirtualCamera = transform.parent.Find("PlayerAimCamera")?.GetComponent<CinemachineVirtualCamera>();

        // Get the animator component on the same object
        anim = GetComponent<Animator>();

        // Find 'lazer dot' by navigating through children
        lazerPointer = transform.parent.Find("lazer dot")?.gameObject;

        // Find 'shotPoint' as a child within 'mech_skeleton'
        shotPoint = transform.Find("shotPoint");

        // Enable the input action for cross-platform input
        lookInput.action.Enable();

        // Initialize LineRenderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;  // Adjust thickness as desired
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Unlit/Color")); // Set a basic material
        lineRenderer.material.color = Color.red;  // Set laser color

        // Optional: Additional beam settings (e.g., glow)
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;

        // Debugging checks to ensure components are found
        if (aimVirtualCamera == null) Debug.LogWarning("PlayerAimCamera not found.");
        if (anim == null) Debug.LogWarning("Animator component not found on the object.");
        if (lazerPointer == null) Debug.LogWarning("Lazer pointer object not found.");
        if (bullet_Prefab == null) Debug.LogWarning("Bullet prefab not found.");
        if (shotPoint == null) Debug.LogWarning("Shot point not found.");
    }

    void Update()
    {

        // MULTIPLAYER CHECK 
        photonView = GetComponentInParent<PhotonView>();

        if (photonView == null || !photonView.IsMine)
        {
            // Disable scripts and components for non-local players
            this.enabled = false;
            aimVirtualCamera.gameObject.SetActive(false);
            return;
        }


        Vector3 mouseWorldPosition = Vector3.zero;

        // RAYCAST INIT - laser pointer hit detection
        Vector2 center_of_screen = new Vector2(Screen.width / 2f, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(center_of_screen);

        Transform hit_transform = null;
        RaycastHit raycastHitted_object;

        //  LAZER & SHOOTING LOGIC 
        if (Physics.Raycast(ray, out raycastHitted_object, raycastMaxRange, mouseColliderLayerMask) && starterAssetsInputs.Aim)
        {
            lazerPointer.SetActive(true);
            lazerPointer.transform.position = raycastHitted_object.point;
            mouseWorldPosition = raycastHitted_object.point;
            hit_transform = raycastHitted_object.transform;

            // Enable the laser beam and set the positions
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, shotPoint.position);
            lineRenderer.SetPosition(1, lazerPointer.transform.position);
        }
        else
        {
            lazerPointer.SetActive(false);
            lineRenderer.enabled = false;
        }

        // Process aiming input with cross-platform support
        ProcessAiming(mouseWorldPosition);

        // Is Shooting
        // if (starterAssetsInputs.Shoot && photonView.IsMine)
        if (starterAssetsInputs.Shoot)
        {
            Vector3 aimDir = starterAssetsInputs.Aim ? (mouseWorldPosition - shotPoint.position).normalized : transform.forward;

            // Networked instantiation for projectiles
            PhotonNetwork.Instantiate(bullet_Prefab.name, shotPoint.position, Quaternion.LookRotation(aimDir));
            starterAssetsInputs.Shoot = false;
        }
    }

    private void ProcessAiming(Vector3 mouseWorldPosition)
    {
        if (starterAssetsInputs.Aim)
        {
            // Enable aiming camera if available
            if (aimVirtualCamera != null) aimVirtualCamera.gameObject.SetActive(true);

            thirdPersonController.setRotationOnMove(false);
            thirdPersonController.setSensitivity(Aim_Sensitivity);

            anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 1f, Time.deltaTime * 10f));

            // Rotate player to aim position on the X-Z plane
            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;

            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }
        else
        {
            // Get the look input value (works for both joystick and touch screen swipes)
            Vector2 lookInputValue = lookInput.action.ReadValue<Vector2>();

            // Disable up/down input by setting Y-axis input to zero when not aiming
            lookInputValue.y = 0;

            // Update rotation based on filtered input (side-to-side only)
            float rotationAmount = lookInputValue.x * Normal_Sensitvity * Time.deltaTime;
            transform.Rotate(Vector3.up, rotationAmount);

            // Reset aiming settings
            if (aimVirtualCamera != null) aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.setRotationOnMove(true);
            thirdPersonController.setSensitivity(Normal_Sensitvity);

            anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
        }
    }

    private void OnDisable()
    {
        // Disable the input action when the script is disabled
        lookInput.action.Disable();
    }
}
