using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class MobileDisableAutoSwitchControls : MonoBehaviour
{
#if ENABLE_INPUT_SYSTEM && (UNITY_IOS || UNITY_ANDROID)

    [Header("Target")]
    public PlayerInput playerInput;

    void Start()
    {
        FindArmaturePlayerInput();
        DisableAutoSwitchControls();
    }

    void FindArmaturePlayerInput()
    {
        Transform armatureTransform = transform.parent?.Find("ARMATURE");
        
        if (armatureTransform != null)
        {
            playerInput = armatureTransform.GetComponent<PlayerInput>();

            if (playerInput == null)
            {
                Debug.LogWarning("PlayerInput component not found on ARMATURE.");
            }
        }
        else
        {
            Debug.LogWarning("ARMATURE object not found in parent.");
        }
    }

    void DisableAutoSwitchControls()
    {
        if (playerInput != null)
        {
            playerInput.neverAutoSwitchControlSchemes = true;
        }
        else
        {
            Debug.LogWarning("PlayerInput component not assigned. Auto-switch control schemes not disabled.");
        }
    }

#endif
}
