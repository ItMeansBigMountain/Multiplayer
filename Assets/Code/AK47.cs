using UnityEngine;
using StarterAssets;

public class AK47 : MonoBehaviour
{
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;
    [SerializeField] private Transform playerHand; // Reference to the player's hand or a target transform on the armature
    [SerializeField] private Vector3 aimPositionOffset = new Vector3(0, 0, 0.5f); // Adjust these as needed
    [SerializeField] private Vector3 aimRotationOffset = new Vector3(0, 0, 0); // Adjust these as needed

    void Start()
    {
        gameObject.SetActive(true);
    }

    void Update()
    {
        // Show or hide based on aiming
        gameObject.SetActive(starterAssetsInputs.Aim);

        if (starterAssetsInputs.Aim)
        {
            PositionGunForAiming();
        }

    }

    private void PositionGunForAiming()
    {
        // Position the AK47 based on player's hand and offsets
        if (playerHand != null)
        {
            transform.position = playerHand.position + playerHand.TransformDirection(aimPositionOffset);
            transform.rotation = playerHand.rotation * Quaternion.Euler(aimRotationOffset);
        }
    }
}
