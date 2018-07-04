using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroControl : MonoBehaviour
{
	public static bool gyroEnabled;
    private Gyroscope gyro;

    public static Transform cameraContainer;
    private Quaternion rot;
	private Vector3 mousePosition;

    //Counts the frames required for a smoother transition when in grey zone 
    //Change the values in the SwitchView() function in order to experiment with the delay.
    private uint downSwitchCounter = 0;
    private uint upSwitchCounter = 25;

    //Whether the user is in EyeView.
    //This is a static variable and you can access it from anywhere as "GyroControl.eyeView".
    public static bool eyeView = true;

    //private float direction = -0.05f;

    //private string stringToDisplay = " ";



    

    private void Start()
    {
        cameraContainer = transform.parent;
        LockDeviceOrientation();

        gyroEnabled = EnableGyro();

    }



    private void Update()
    {
        //MoveCamera();
		if (gyroEnabled) {
			transform.localRotation = gyro.attitude * rot;
		}

		float dot = Mathf.Abs (Vector3.Dot (Camera.main.transform.forward, Vector3.up));
		SwitchView (dot);
    }


	// Rotates the camera using euler angles
	public static void RotateCamera (Vector3 deltaRotation) {
		cameraContainer.localRotation = Quaternion.Euler (cameraContainer.eulerAngles + deltaRotation);
	}

    //Locks the orientation of the phone to always be in portrait mode.
    //This is a solution to having to rotate the phone canvas UI everytime the orientation of the phone changes. Runs at the start of the application.
    private void LockDeviceOrientation()
    {
        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.orientation = ScreenOrientation.AutoRotation;
    }



    //Enables the gyroscope of the phone device. Returns true if successful.
    //This runs once at the start of the application.
    private bool EnableGyro()
    {
		if (SystemInfo.supportsGyroscope || SystemInfo.supportsAccelerometer) {
			gyro = Input.gyro;
			gyro.enabled = true;

			cameraContainer.rotation = Quaternion.Euler (90f, 90f, 0f);
			rot = new Quaternion (0, 0, 1, 0);

			return true;
		}
        
        return false;
    }



    //Disables the gyroscope of the phone device.
    //Use this function whenever you want to manually control the camera.
    private void DisableGyro()
    {
        gyro = Input.gyro;
        gyro.enabled = false;
    }



    //Determines whether or not the user is in EyeView. Adds a delay in the grey zone for smoother transitions.
    //Run this function every frame to determine if the user is in EyeView or not.
    void SwitchView(float dot)
    {
        if (dot >= 0.95f)
        {
            downSwitchCounter = 10;
            upSwitchCounter = 0;
        }
        else if (dot <= 0.75f)
        {
            upSwitchCounter = 15;
            downSwitchCounter = 0;
        }
        else if (dot >= 0.85f && dot < 0.95f)
        {
            upSwitchCounter = 0;
            if (downSwitchCounter < 10)
            {
                downSwitchCounter++;
            }
        }
        else
        {
            downSwitchCounter = 0;
            if (upSwitchCounter < 15)
            {
                upSwitchCounter++;
            }
        }


        if (upSwitchCounter == 15)
        {
            eyeView = true;
        }
        else if (downSwitchCounter == 10)
        {
            eyeView = false;
        }
    }



    //Moves the camera back and forth.
    //Used to test how the gyroscope controls feels when there is movement.
    /*void MoveCamera()
    {
        cameraContainer.transform.position = new Vector3(cameraContainer.transform.position.x, cameraContainer.transform.position.y, cameraContainer.transform.position.z + direction);
        if (cameraContainer.transform.position.z < -10) direction = 0.05f;
        if (cameraContainer.transform.position.z > 10) direction = -0.05f;
    }*/


    //Displays a string on the top left of the screen
    //Use this to display things on screen for debugging.
    /*void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 1000, 20), stringToDisplay);
    }*/



}
 