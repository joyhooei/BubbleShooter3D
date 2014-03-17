using UnityEngine;
using System.Collections;


public class CameraController : MonoBehaviour {
	int cubeRotSpeed = 80;
	bool freeView = true;
	private Vector2 lastPos;
	private bool holdingMouse = false;
	private Vector2 diff;

	private GameObject[] worldObjects;
	// Use this for initialization
	void Start () {
		//Screen.lockCursor = true;
		diff = Vector2.zero;
	}

	void FixedUpdate(){


	}

	private void moveCluster(Vector2 movement){
		float speedFactor = 10.0f;
		worldObjects = GameObject.FindGameObjectsWithTag("BigBall");
		foreach(GameObject gObject in worldObjects)
		{
			gObject.transform.RotateAround(Vector3.zero,  Vector3.up, -1*movement[0] *speedFactor* Time.deltaTime);
			gObject.transform.RotateAround(Vector3.zero,  Vector3.right, movement[1] *speedFactor* Time.deltaTime);
		}
		diff = Vector2.zero;
	}
	// Update is called once per frame
	void Update () {
		if(freeView){
			if (Input.GetButtonDown("Fire1") ) {		
				holdingMouse = true;	
				lastPos = Input.mousePosition;	
			}

			if (Input.GetButtonUp("Fire1")) {		
				holdingMouse = false;	
				lastPos = Vector2.zero;
			}

			if(holdingMouse){
				diff = new Vector2(Input.mousePosition[0]-lastPos[0],Input.mousePosition[1]-lastPos[1]);

				lastPos = Input.mousePosition;

			}
		}
		/*
		foreach (Touch touch in Input.touches) {                
			
			Debug.Log ("reading input");
			// Handle finger movements based on touch phase.
			switch (touch.phase) {
				// Record initial touch position.
			case TouchPhase.Began:
				lastPos = touch.position;
				break;
				
				// Determine direction by comparing the current touch
				// position with the initial one.
			case TouchPhase.Moved:
				diff = touch.position - lastPos;
				lastPos = touch.position;
				break;
				
				// Report that a direction has been chosen when the finger is lifted.
			case TouchPhase.Ended:
				lastPos = Vector2.zero;
				break;
			}
		}
		*/

		if(diff != Vector2.zero)	moveCluster(diff) ;

		if(Input.GetKey(KeyCode.Keypad9)){
			freeView = true;
			worldObjects = GameObject.FindGameObjectsWithTag("BigBall");
			Debug.Log ("Activating freeview");
		}

		if(Input.GetKey(KeyCode.Keypad7)){
			freeView = false;
			Debug.Log ("DeActivating freeview");
		}



		if(Input.GetKey(KeyCode.Keypad4) || Input.GetKey(KeyCode.Keypad5) ||
			Input.GetKey(KeyCode.Keypad6) || Input.GetKey(KeyCode.Keypad8)){
			worldObjects = GameObject.FindGameObjectsWithTag("BigBall");

			//Up
			if(Input.GetKey(KeyCode.Keypad8)){
				foreach(GameObject gObject in worldObjects)
				{
					gObject.transform.RotateAround(Vector3.zero,  -Vector3.right, cubeRotSpeed * Time.deltaTime);
				}
			}


			
			//Down
			if(Input.GetKey(KeyCode.Keypad5)){
				foreach(GameObject gObject in worldObjects)
				{
					gObject.transform.RotateAround(Vector3.zero,  Vector3.right, cubeRotSpeed * Time.deltaTime);
				}
			}
			
			//Right
			if(Input.GetKey(KeyCode.Keypad6)){
				foreach(GameObject gObject in worldObjects)
				{
					gObject.transform.RotateAround(Vector3.zero,  Vector3.up, cubeRotSpeed * Time.deltaTime);
				}
			}
			
			//Left
			if(Input.GetKey(KeyCode.Keypad4)){
				foreach(GameObject gObject in worldObjects)
				{
					gObject.transform.RotateAround(Vector3.zero, -Vector3.up, cubeRotSpeed * Time.deltaTime);
				}
			}
			
			//TestKey
			if(Input.GetKey(KeyCode.Keypad9)){
				
			}
		}
		
        //float h = horizontalSpeed * Input.GetAxis("Mouse X");
       // float v = -verticalSpeed * Input.GetAxis("Mouse Y");
		
        //transform.Rotate(v, h, 0f);
		//transform.rotation += new Quaternion.Euler(v, h, 0f);
		//transform.Rotate(v, h, transform.rotation.z);
		//transform.localRotation *= Quaternion.Euler(new Vector3(0f,0f,-transform.localRotation[2]));
		//transform.localRotation *= Quaternion.Euler(new Vector3(0f,  0f,1));
		
		/*yRotation -= Input.GetAxis("Mouse Y") * camRotSpeed * Time.deltaTime;
		yRotation = Mathf.Clamp(yRotation, -70, 70);
		xRotation += Input.GetAxis("Mouse X") * camRotSpeed * Time.deltaTime;
		xRotation = Mathf.Clamp(xRotation, -70, 70);
		//xRotation = xRotation % 360;
		transform.localEulerAngles = new Vector3(yRotation, xRotation, 0);
		*/
	}
	
}
