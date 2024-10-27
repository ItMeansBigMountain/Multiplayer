using UnityEngine;
using Unity.Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;
using Photon.Pun;

public class ArenaShooter_PlayerControls : MonoBehaviour
{
    [Header("Manual Configuration")]
    [SerializeField] private float Normal_Sensitvity = 125f;
    [SerializeField] private float Aim_Sensitivity = 100f;
    [SerializeField] private LayerMask mouseColliderLayerMask;
    [SerializeField] private InputActionReference lookInput;

    [Header("Auto-Configured Components")]
    [SerializeField] private CinemachineCamera aimVirtualCamera;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject lazerPointer;
    [SerializeField] private GameObject bullet_Prefab;
    [SerializeField] private Transform shotPoint;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;
    [SerializeField] private ThirdPersonController thirdPersonController;
    [SerializeField] private float raycastMaxRange = 999f;
    
    private PhotonView photonView;

    void Awake()
    {
        // Get PhotonView and check ownership
        photonView = GetComponentInParent<PhotonView>();
        if (photonView == null || !photonView.IsMine)
        {
            // Disable scripts and components for non-local players
            this.enabled = false;
            aimVirtualCamera.gameObject.SetActive(false);
            return;
        }

        // Get input and controller components
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();

        // Initialize components and settings
        aimVirtualCamera = transform.parent.Find("PlayerAimCamera")?.GetComponent<CinemachineCamera>();
        anim = GetComponent<Animator>();
        lazerPointer = transform.parent.Find("lazer dot")?.gameObject;
        bullet_Prefab = Resources.Load<GameObject>("laser_bullet");
        shotPoint = transform.Find("shotPoint");

        // Enable the input action for cross-platform input
        lookInput.action.Enable();

        // Initialize LineRenderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = Color.red;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    void Update()
    {
        Vector3 mouseWorldPosition = Vector3.zero;

        // Raycast to determine lazer pointer position if aiming
        if (starterAssetsInputs.Aim)
        {
            Vector2 center_of_screen = new Vector2(Screen.width / 2f, Screen.height / 2);
            Ray ray = Camera.main.ScreenPointToRay(center_of_screen);
            if (Physics.Raycast(ray, out RaycastHit raycastHitted_object, raycastMaxRange, mouseColliderLayerMask))
            {
                lazerPointer.SetActive(true);
                lazerPointer.transform.position = raycastHitted_object.point;
                mouseWorldPosition = raycastHitted_object.point;

                // Enable the laser beam and set positions
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, shotPoint.position);
                lineRenderer.SetPosition(1, lazerPointer.transform.position);
            }
            else
            {
                lazerPointer.SetActive(false);
                lineRenderer.enabled = false;
            }
        }
        else
        {
            lazerPointer.SetActive(false);
            lineRenderer.enabled = false;
        }

        // Process aiming input and rotation logic
        ProcessAiming(mouseWorldPosition);

        // Shooting logic
        if (starterAssetsInputs.Shoot && photonView.IsMine)
        {
            Vector3 aimDir = starterAssetsInputs.Aim ? 
                             (mouseWorldPosition - shotPoint.position).normalized : 
                             transform.forward;

            PhotonNetwork.Instantiate(bullet_Prefab.name, shotPoint.position, Quaternion.LookRotation(aimDir));
            starterAssetsInputs.Shoot = false; // Reset shooting input
        }
    }

    private void ProcessAiming(Vector3 mouseWorldPosition)
    {
        if (starterAssetsInputs.Aim)
        {
            // Enable aiming camera and aiming sensitivity
            aimVirtualCamera?.gameObject.SetActive(true);
            thirdPersonController.setRotationOnMove(false);
            thirdPersonController.setSensitivity(Aim_Sensitivity);

            // Set aiming animation layer weight
            anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 1f, Time.deltaTime * 10f));

            // Rotate player towards aim target on X-Z plane
            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }
        else
        {
            // Adjust rotation for non-aiming state
            Vector2 lookInputValue = lookInput.action.ReadValue<Vector2>();
            lookInputValue.y = 0; // Side-to-side rotation only
            float rotationAmount = lookInputValue.x * Normal_Sensitvity * Time.deltaTime;
            transform.Rotate(Vector3.up, rotationAmount);

            // Reset camera and animation settings
            aimVirtualCamera?.gameObject.SetActive(false);
            thirdPersonController.setRotationOnMove(true);
            thirdPersonController.setSensitivity(Normal_Sensitvity);
            anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
        }
    }

    private void OnDisable()
    {
        lookInput.action.Disable();
    }
}
