using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
        public GameObject Joystick_Move;
        public GameObject Joystick_Look;
        public UIVirtualButton fireButtonL;   
        public UIVirtualButton fireButtonR;   
        public UIVirtualButton aimButton;    
        public UIVirtualButton jumpButton;   
        public UIVirtualButton sprintButton; 
        public bool aiming = false;

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
            Joystick_Move = transform.Find("Joystick_Move")?.gameObject;
            Joystick_Look = transform.Find("Joystick_Look")?.gameObject;
            fireButtonL = transform.Find("FireButton L")?.GetComponent<UIVirtualButton>();
            fireButtonR = transform.Find("FireButton R")?.GetComponent<UIVirtualButton>();
            aimButton = transform.Find("AimButton")?.GetComponent<UIVirtualButton>();
            jumpButton = transform.Find("JumpButton")?.GetComponent<UIVirtualButton>();
            sprintButton = transform.Find("SprintButton")?.GetComponent<UIVirtualButton>();

            // ConfigureDeviceSpecificUI();
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
            fireButton.gameObject.SetActive(true);
            Joystick_Move.gameObject.SetActive(true);
            Joystick_Look.gameObject.SetActive(true);
            jumpButton.gameObject.SetActive(true);
            aimButton.gameObject.SetActive(true);
            jumpButton.gameObject.SetActive(true);
            sprintButton.gameObject.SetActive(true);
#else
            fireButtonL.gameObject.SetActive(false);
            fireButtonR.gameObject.SetActive(false);
            Joystick_Move.gameObject.SetActive(false);
            Joystick_Look.gameObject.SetActive(false);
            jumpButton.gameObject.SetActive(false);
            aimButton.gameObject.SetActive(false);
            jumpButton.gameObject.SetActive(false);
            sprintButton.gameObject.SetActive(false);
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
            aiming = !aiming;
            starterAssetsInputs.AimInput(aiming);
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
