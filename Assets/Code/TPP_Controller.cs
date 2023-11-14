using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;


public class TPP_Controller : MonoBehaviour
{


    //Camera
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;

    // Input Variables
    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;

    // Aim sensitivity
    [SerializeField] private float Normal_Sensitvity = 15f;
    [SerializeField] private float Aim_Sensitivity = 5f;

    // Shooting
    [SerializeField] private Animator anim;
    [SerializeField] private LayerMask mouseColliderLayerMask;

    [SerializeField] private GameObject lazerPointer;
    [SerializeField] private GameObject bullet_Prefab;
    [SerializeField] private Transform shotPoint;








    // Assign Components
    void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();
    }






    void Update()
    {
        Vector3 mouseWorldPosition = Vector3.zero;

        // RAYCAST INIT - laser pointer hit detection
        Vector2 center_of_screen = new Vector2(Screen.width / 2f, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(center_of_screen);

        Transform hit_transform = null;  // Physics.Raycast() outputs var of the object that the raycast hit
        RaycastHit raycastHitted_object;

        //  Create Ray HitPoint Output
        if (Physics.Raycast(ray, out raycastHitted_object, 999f, mouseColliderLayerMask))
        {
            lazerPointer.SetActive(true);
            lazerPointer.transform.position = raycastHitted_object.point;
            mouseWorldPosition = raycastHitted_object.point;
            hit_transform = raycastHitted_object.transform;
        }
        else
        {
            lazerPointer.SetActive(false);
        }

        /// Is aiming
        if (starterAssetsInputs.Aim)
        {
            //camera settings
            aimVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.setRotationOnMove(false);
            thirdPersonController.setSensitivity(Aim_Sensitivity);
            anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 1f, Time.deltaTime * 10f));

            //rotate player to aim position settings
            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;

            // lerp player to rotate towards raycast hit position
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }
        // Not aiming
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.setRotationOnMove(true);
            thirdPersonController.setSensitivity(Normal_Sensitvity);
            anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
        }

        // Is Shooting
        if (starterAssetsInputs.Shoot)
        {
            // // RAYCAST HIT (corner shot bullets)
            // if (hit_transform != null)
            //     // Hit our Target
            //     Instantiate(bullet_Prefab.GetComponent<BulletProjectile>().vfx_Hit_green, transform.position, Quaternion.identity);
            // else
            //     // Hit Something Else
            //     Instantiate(bullet_Prefab.GetComponent<BulletProjectile>().vfx_Hit_red, transform.position, Quaternion.identity);



            // ***** SHOOT BULLET ******
            Vector3 aimDir = (mouseWorldPosition - shotPoint.position).normalized;
            Instantiate(bullet_Prefab, shotPoint.position, Quaternion.LookRotation(aimDir, Vector3.up));

            // SINGLE SHOT
            starterAssetsInputs.Shoot = false;

            // //debug
            print(hit_transform);
        }
    }

}
