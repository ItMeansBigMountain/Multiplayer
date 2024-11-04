using UnityEngine;
using Unity.Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;
using Photon.Pun;

public class ArenaShooter_PlayerControls : MonoBehaviourPunCallbacks
{
    [Header("Manual Configuration")]
    [SerializeField] private float Normal_Sensitivity = 125f;
    [SerializeField] private float Aim_Sensitivity = 100f;
    [SerializeField] private LayerMask mouseColliderLayerMask;

    [Header("Auto-Configured Components")]
    [SerializeField] private GameObject _mainCamera;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private CinemachineCamera followCamera;
    [SerializeField] private CinemachineCamera aimVirtualCamera;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject lazerPointer;
    [SerializeField] private GameObject bullet_Prefab;
    [SerializeField] private Transform shotPoint;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;
    [SerializeField] private ThirdPersonController thirdPersonController;
    [SerializeField] private float raycastMaxRange = 999f;
    [SerializeField] private InputActionReference lookInput;

    // private PhotonView photonView;

    void Awake()
    {
        if (photonView || photonView.IsMine)
        {
            // GET COMPONENTS 
            starterAssetsInputs = GetComponent<StarterAssetsInputs>();
            thirdPersonController = GetComponent<ThirdPersonController>();
            playerInput = GetComponent<PlayerInput>();
            anim = GetComponent<Animator>();
            followCamera = transform.parent.Find("PlayerFollowCamera")?.GetComponent<CinemachineCamera>();
            aimVirtualCamera = transform.parent.Find("PlayerAimCamera")?.GetComponent<CinemachineCamera>();

            // GET GAME OBJECTS
            _mainCamera = transform.parent.Find("MainCamera")?.gameObject;
            lazerPointer = transform.parent.Find("lazer dot")?.gameObject;
            bullet_Prefab = Resources.Load<GameObject>("laser_bullet");
            shotPoint = transform.Find("shotPoint");

            // Initialize LineRenderer
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
            lineRenderer.material.color = Color.red;
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;

            if (playerInput != null && lookInput == null)
            {
                InputAction lookAction = playerInput.actions["Look"];
                lookInput = ScriptableObject.CreateInstance<InputActionReference>();
                lookInput.Set(lookAction);
            }

        }

        if (photonView == null || !photonView.IsMine)
        {
            // Disable scripts and components for non-local players
            if (lookInput != null) lookInput.action.Disable();
            playerInput.DeactivateInput();
            followCamera.gameObject.SetActive(false);
            aimVirtualCamera.gameObject.SetActive(false);
            _mainCamera.SetActive(false);
            this.enabled = false;
            return;
        }

    }

    void Update()
    {
        if (!photonView.IsMine) return;
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
            // Enable aiming camera and disable follow camera
            aimVirtualCamera?.gameObject.SetActive(true);
            followCamera?.gameObject.SetActive(false);

            // Adjust aiming sensitivity
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
            // Enable follow camera and disable aim camera
            followCamera?.gameObject.SetActive(true);
            aimVirtualCamera?.gameObject.SetActive(false);

            // Adjust rotation for non-aiming state
            Vector2 lookInputValue = lookInput.action.ReadValue<Vector2>();
            lookInputValue.y = 0; // Side-to-side rotation only
            float rotationAmount = lookInputValue.x * Normal_Sensitivity * Time.deltaTime;
            transform.Rotate(Vector3.up, rotationAmount);

            // Reset camera and animation settings
            thirdPersonController.setRotationOnMove(true);
            thirdPersonController.setSensitivity(Normal_Sensitivity);
            anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
        }
    }

}
