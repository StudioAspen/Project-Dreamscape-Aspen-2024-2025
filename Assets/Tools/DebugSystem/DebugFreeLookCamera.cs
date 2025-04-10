using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GetMikyled.DebugSystem.FreeLookCamera
{
	//-/////////////////////////////////////////////////////////////////////////////////////////////////
	///
	public class DebugFreeLookCamera : MonoBehaviour
	{
		[SerializeField] private Camera _freeLookCamera;

		[Space]
		[SerializeField] private float sensitivityX = 1.0f;
		[SerializeField] private float sensitivityY = 1.0f;
		[SerializeField] private float moveSpeed = 2.0f;
		
		private bool _isActive = false;
		public bool isActive => _isActive;
		
		private bool _freeLookControlsEnabled = false;
		public bool freeLookControlsEnabled => _freeLookControlsEnabled;
		
		//-/////////////////////////////////////////////////////////////////////////////////////////////////
		///
		private void Update()
		{
			// Return if free look camera is not active
			if (_isActive == false)
			{
				return;
			}
			
			if (freeLookControlsEnabled && Input.GetMouseButton(1))
			{
				// -- HANDLE ROTATION
				
				// Get mouse movement direction
				float mouseX = Input.GetAxis("Mouse X");
				float mouseY = Input.GetAxis("Mouse Y");

				// Note: mouseMovement x swap with y when it comes to rotation (Intentional)
				Vector3 currentRotation = _freeLookCamera.transform.eulerAngles;
				float rotationX = currentRotation.x + -mouseY * sensitivityX;
				float rotationY = currentRotation.y + mouseX * sensitivityY;

				transform.eulerAngles = new Vector3(rotationX, rotationY, currentRotation.z);
				
				// -- HANDLE MOVEMENT
				
				// Get WASD movement direction
				float horizontalAxis = Input.GetAxisRaw("Horizontal");
				float verticalAxis = Input.GetAxisRaw("Vertical");

				Vector3 movementDirection = Vector3.zero;
				movementDirection += _freeLookCamera.transform.forward * verticalAxis;
				movementDirection += _freeLookCamera.transform.right * horizontalAxis;
				movementDirection.Normalize();
				
				_freeLookCamera.transform.position += movementDirection * moveSpeed;
			}
		}
		
		//-/////////////////////////////////////////////////////////////////////////////////////////////////
		///
		public void SetActive(bool argActive)
		{
			_freeLookCamera.enabled = argActive;
			_isActive = argActive;
			
			// If camera was just activated
			if (argActive)
			{
				// Set it to current position of the main camera
				Camera mainCamera = Camera.main;
				_freeLookCamera.transform.position = mainCamera.transform.position;
				_freeLookCamera.transform.rotation = mainCamera.transform.rotation;
			}
		}
		
		//-/////////////////////////////////////////////////////////////////////////////////////////////////
		///
		public void SetFreeLookControlsEnabled(bool argEnabled)
		{
			_freeLookControlsEnabled = argEnabled;
		}
	}
}