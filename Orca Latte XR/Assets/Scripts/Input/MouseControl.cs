using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControl : MonoBehaviour {

	public float mouseSpeed = .1f;

	private Vector3 mousePosition;
	private float mouseTimer = 0f;
	private float tapTime = .1f;

	void Start () {

	}

	void Update () {


		// Touch (Left mouse button)
		/*if (GyroControl.eyeView == true) {
			if (Input.GetMouseButtonDown (0)) {
				mouseTimer = 0f;
				mousePosition = Input.mousePosition;
			}
			if (Input.GetMouseButton (0)) {
				mouseTimer += Time.deltaTime;

				if (mouseTimer > tapTime) {
					GyroControl.RotateCamera (GetCameraRotation ());
				}

				mousePosition = Input.mousePosition;
			}
			if (Input.GetMouseButtonUp (0)) {
				if (mouseTimer < tapTime) {
					TeleportToMouseClick ();
				}
			}
		}*/

		// Right mouse button
		if (!GyroControl.gyroEnabled) {
			if (Input.GetMouseButtonDown (1)) {
				mousePosition = Input.mousePosition;
			}
			if (Input.GetMouseButton (1)) {
				mouseTimer += Time.deltaTime;
				GyroControl.RotateCamera (GetCameraRotation ());
				mousePosition = Input.mousePosition;
			}
		}
	}

	private Vector3 GetCameraRotation () {
		Vector3 cameraRotation = mouseSpeed * (Input.mousePosition - mousePosition);
        cameraRotation.z = -cameraRotation.x;
        cameraRotation.x = cameraRotation.y;
        cameraRotation.y = cameraRotation.z;
        cameraRotation.z = 0;
        if (GyroControl.gyroEnabled) {
            cameraRotation.y = 0;
        }
		return cameraRotation;
	}

	//Teleports the main camera to the collision point from the raycast to a collider
	//This is used for moving the camera around. Add colliders and possible masks to create specific teleporting points.
	private void TeleportToMouseClick()
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast (ray, out hit)) {
			if (hit.transform.gameObject.layer == 8) {
				GyroControl.cameraContainer.transform.position = new Vector3 (hit.point.x, hit.point.y + 1.8f, hit.point.z);
			}
		}
	}
}
