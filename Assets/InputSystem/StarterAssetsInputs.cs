using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public Vector2 arrows;
		public bool jump;
		public bool sprint;
		public bool tab;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		// move input actions
		private InputAction _moveForward;
		private InputAction _moveRight;
		private InputAction _moveBackward;
		private InputAction _moveLeft;

		private void OnEnable()
        {
			// bind input updating
			_moveForward = InputSystem.actions.FindAction("MoveForward");
			_moveForward.started += UpdateMoveInput;
			_moveForward.canceled += UpdateMoveInput;
			_moveRight = InputSystem.actions.FindAction("MoveRight");
			_moveRight.started += UpdateMoveInput;
			_moveRight.canceled += UpdateMoveInput;
			_moveBackward = InputSystem.actions.FindAction("MoveBackward");
			_moveBackward.started += UpdateMoveInput;
			_moveBackward.canceled += UpdateMoveInput;
			_moveLeft = InputSystem.actions.FindAction("MoveLeft");
			_moveLeft.started += UpdateMoveInput;
			_moveLeft.canceled += UpdateMoveInput;
		}
        private void OnDisable()
        {
			// unbind input updating
			_moveForward.started += UpdateMoveInput;
			_moveForward.canceled += UpdateMoveInput;
			_moveRight.started += UpdateMoveInput;
			_moveRight.canceled += UpdateMoveInput;
			_moveBackward.started += UpdateMoveInput;
			_moveBackward.canceled += UpdateMoveInput;
			_moveLeft.started += UpdateMoveInput;
			_moveLeft.canceled += UpdateMoveInput;
		}

        private void UpdateMoveInput(InputAction.CallbackContext context)
        {
            // fetch inputs for move every frame
            // cannot use OnMove below since we are no longer using a composite
            int xInput = 0;
			int yInput = 0;
			if (_moveRight.ReadValue<float>() > 0.5f)
                xInput++;
			if (_moveLeft.ReadValue<float>() > 0.5f)
				xInput--;
			if (_moveForward.ReadValue<float>() > 0.5f)
				yInput++;
			if (_moveBackward.ReadValue<float>() > 0.5f)
				yInput--;

			// update the value actually used in the player controller
			MoveInput(new Vector2(xInput, yInput));
        }


#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnArrows(InputValue value)
		{
			ArrowInput(value.Get<Vector2>());
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnTab(InputValue value)
		{
			TabInput(value.isPressed);
		}
#endif
		
		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void ArrowInput(Vector2 newArrowDirection)
		{
			arrows = newArrowDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void TabInput(bool newTabState)
		{
			tab = newTabState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}