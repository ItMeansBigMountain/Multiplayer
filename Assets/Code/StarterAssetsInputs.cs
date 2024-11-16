using UnityEngine;
using Photon.Pun;  
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class StarterAssetsInputs : MonoBehaviourPunCallbacks
    {
        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;
        public bool Aim;
        public bool Shoot;

        [Header("Movement Settings")]
        public bool analogMovement;

        // [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;

        private PhotonView photonView;

        private void Awake()
        {
            photonView = transform.parent.GetComponent<PhotonView>();
        }

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
        {
            if (photonView.IsMine) MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            if (photonView.IsMine)
            {
                LookInput(value.Get<Vector2>());
            }
        }

        public void OnJump(InputValue value)
        {
            if (photonView.IsMine) JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            if (photonView.IsMine) SprintInput(value.isPressed);
        }

        public void OnAim(InputValue value)
        {
            if (photonView.IsMine) AimInput(value.isPressed);
        }

        public void OnShoot(InputValue value)
        {
            if (photonView.IsMine) ShootInput(value.isPressed);
        }
#endif

        // Input functions for assigning input states
        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }

        public void AimInput(bool newAimState)
        {
            Aim = newAimState;
        }

        public void ShootInput(bool newShootState)
        {
            Shoot = newShootState;
        }

        // Cursor lock state management
        private void OnApplicationFocus(bool hasFocus)
        {
            if (photonView.IsMine) SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }

        public void ResetInput()
        {
            if (photonView.IsMine)
            {
                jump = false;
                Shoot = false;
            }
        }
    }
}
