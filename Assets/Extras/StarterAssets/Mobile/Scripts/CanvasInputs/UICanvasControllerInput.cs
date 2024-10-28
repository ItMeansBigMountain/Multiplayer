using UnityEngine;
using TMPro;

namespace StarterAssets
{
    public class UICanvasControllerInput : MonoBehaviour
    {
        [Header("Output")]
        public StarterAssetsInputs starterAssetsInputs;

        [Header("Shooting Elements")]
        public GameObject crossHair;
        //public GameObject ammo;


        [Header("Kill Feed Settings")]
        public TextMeshProUGUI killFeedText;
        public int maxKillFeedEntries = 8;
        private string killFeedLog = "";

        [Header("Mobile Controls")]
        public GameObject joystick;
        public GameObject fireButton;
        public GameObject aimButton;
        public GameObject jumpButton;
        public GameObject sprintButton;

        private void Start()
        {
            // Find the parent object called "ARMATURE" and get the StarterAssetsInputs component
            Transform armature = transform.parent.Find("ARMATURE");
            if (armature != null)
            {
                starterAssetsInputs = armature.GetComponent<StarterAssetsInputs>();
            }
            else
            {
                Debug.LogError("ARMATURE object not found or StarterAssetsInputs component missing");
            }


            // Find the crosshair in the hierarchy
            crossHair = transform.Find("CrossHair")?.gameObject;

            // Assign Joystick and Buttons within MobileControls
            joystick = transform.Find("Joystick_Move")?.gameObject;
            fireButton = transform.Find("FireButton")?.gameObject;
            aimButton = transform.Find("AimButton")?.gameObject;
            jumpButton = transform.Find("JumpButton")?.gameObject;
            sprintButton = transform.Find("SprintButton")?.gameObject;

            ConfigureDeviceSpecificUI();
        }

        private void Update()
        {
            // Show or hide crosshair based on aiming input
            if (starterAssetsInputs != null)
            {
                crossHair?.SetActive(starterAssetsInputs.Aim);
            }
        }

        private void ConfigureDeviceSpecificUI()
        {
#if UNITY_IOS || UNITY_ANDROID
            fireButton.SetActive(true);
            virtualJoystick.SetActive(true);
            jumpButton.SetActive(true);
            aimButton.SetActive(true);
            jumpButton.SetActive(true);
            sprintButton.SetActive(true);
#else
            fireButton.SetActive(false);
            joystick.SetActive(false);
            jumpButton.SetActive(false);
            aimButton.SetActive(false);
            jumpButton.SetActive(false);
            sprintButton.SetActive(false);
#endif
        }

        public void VirtualMoveInput(Vector2 virtualMoveDirection)
        {
            starterAssetsInputs.MoveInput(virtualMoveDirection);
        }

        public void VirtualLookInput(Vector2 virtualLookDirection)
        {
            starterAssetsInputs.LookInput(virtualLookDirection);
        }

        public void VirtualJumpInput(bool virtualJumpState)
        {
            starterAssetsInputs.JumpInput(virtualJumpState);
        }

        public void VirtualSprintInput(bool virtualSprintState)
        {
            starterAssetsInputs.SprintInput(virtualSprintState);
        }

        public void VirtualAimInput(bool virtualAimState)
        {
            starterAssetsInputs.AimInput(virtualAimState);
        }

        public void VirtualShootInput(bool virtualShootState)
        {
            starterAssetsInputs.ShootInput(virtualShootState);
        }
        // Kill Feed Functionality
        public void AddKillFeedEntry(string killer, string victim)
        {
            // Add new entry to the kill feed
            killFeedLog += $"{killer} killed {victim}\n";

            // Keep only the last 'maxKillFeedEntries' entries
            string[] killFeedLines = killFeedLog.Split('\n');
            if (killFeedLines.Length > maxKillFeedEntries)
            {
                killFeedLog = "";
                for (int i = killFeedLines.Length - maxKillFeedEntries - 1; i < killFeedLines.Length - 1; i++)
                {
                    killFeedLog += killFeedLines[i] + "\n";
                }
            }

            // Update kill feed UI text
            killFeedText.text = killFeedLog;
        }
    }
}
