using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TAKEN FROM JOHN TORRANCE BEE GAME
public struct rotateValues
{
    [SerializeField]
    public Vector3 positionToTurnCamera;
    public Quaternion rotateToCamera;

    public rotateValues(Vector3 positionToTurn, Quaternion rotateTo)
    {
        positionToTurnCamera = positionToTurn;
        rotateToCamera = rotateTo;
    }
}

public class CameraControl : MonoBehaviour {

    public float moveFactor = 50.0f;
    public float scrollFactor = 5.0f;
    public float zoomMax = 85.0f;
    public float zoomMin = 35.0f;

    private bool cameraRotate = true;
    private rotateValues rotateCameraBack;
    private rotateValues rotateCameraFront;
    private GameObject cameraControl;

    private Vector3 ResetCamera;
    private Vector3 Origin;
    private Vector3 Diference;
    private bool Drag = false;

    // Use this for initialization
    void Start()
    {
        ResetCamera = Camera.main.transform.position;
        cameraControl = GameObject.FindGameObjectWithTag("CameraController");
        rotateCameraFront = new rotateValues(new Vector3(-30, 44, -30), Quaternion.Euler(30, 45, 0));
        rotateCameraBack = new rotateValues(new Vector3(0, 30, 90), Quaternion.Euler(30, 135, 0));
    }

    // Update is called once per frame
    void Update()
    {
        moveCamera();
        if ((Input.GetAxis("Mouse ScrollWheel") > 0) && (Camera.main.orthographicSize > zoomMin))
        {
            Camera.main.orthographicSize -= scrollFactor;
        }
        else if ((Input.GetAxis("Mouse ScrollWheel") < 0) && (Camera.main.orthographicSize < zoomMax))
        {
            Camera.main.orthographicSize += scrollFactor;
        }
    }

    private void moveCamera()
    {
        if (Input.GetMouseButton(0))
        {
            Diference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
            if (Drag == false)
            {
                Drag = true;
                Origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else
        {
            Drag = false;
        }
        if (Drag == true)
        {
            cameraControl.transform.position = Origin - Diference;
        }
        /*	
		//RESET CAMERA TO STARTING POSITION WITH RIGHT CLICK
		if (Input.GetMouseButton (0)) 
		{
			Camera.main.transform.position=ResetCamera;
		}
		*/
    }

    public void rotateScreen()
    {
        cameraRotate = !cameraRotate;
        if (cameraRotate)
        {
            cameraControl.transform.SetPositionAndRotation(rotateCameraFront.positionToTurnCamera, rotateCameraFront.rotateToCamera);
            //Camera.main.transform.position=ResetCamera;
        }
        else
        {
            cameraControl.transform.SetPositionAndRotation(rotateCameraBack.positionToTurnCamera, rotateCameraBack.rotateToCamera);
            //Camera.main.transform.position=ResetCamera;
        }
    }


}
