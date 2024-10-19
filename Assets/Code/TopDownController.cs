using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // Import Photon PUN for networking
using StarterAssets;
using UnityEngine.UIElements;

public class TopDownController : MonoBehaviourPunCallbacks
{
    // Input Variables
    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;

    // Shooting
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject bullet_Prefab;
    [SerializeField] private Transform shotPoint;
    [SerializeField] private float shootCooldown = 0.5f; // Optional: cooldown to prevent spamming

    private float nextShootTime = 0f;

    // Assign Components
    void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();
    }

    void Update()
    {
        // Only allow input from the local player
        if (photonView.IsMine)
        {
            // Handle Shooting
            if (starterAssetsInputs.Shoot && Time.time >= nextShootTime)
            {
                photonView.RPC("Shoot", RpcTarget.All);
                nextShootTime = Time.time + shootCooldown; // Set next allowed shoot time
                starterAssetsInputs.Shoot = false; // Reset shoot input for single-shot
            }

            // Handle Jumping
            HandleJumping();
        }
    }

    [PunRPC]
    private void Shoot()
    {
        // Set the shooting direction to be directly in front of the player
        Vector3 shootDirection = transform.forward;

        // Instantiate the bullet at the shot point with the correct direction
        Instantiate(bullet_Prefab, shotPoint.position, Quaternion.LookRotation(shootDirection, Vector3.up));

        //// Play shooting animation if available
        //if (anim != null)
        //{
        //    anim.SetTrigger("Shoot");
        //}

        // Debugging information
        Debug.Log("Bullet Fired");
    }

    void HandleJumping()
    {
        // Handle Jumping
        if (starterAssetsInputs.jump && thirdPersonController.Grounded)
        {
            // Reset the jump input after handling the jump
            starterAssetsInputs.jump = false;
        }
    }
}
