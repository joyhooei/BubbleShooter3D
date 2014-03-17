using UnityEngine;
using System.Collections;


public class CameraController : MonoBehaviour {
	int cubeRotSpeed = 80;
	// Use this for initialization
	void Start () {
		//Screen.lockCursor = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.Keypad4) || Input.GetKey(KeyCode.Keypad5) ||
			Input.GetKey(KeyCode.Keypad6) || Input.GetKey(KeyCode.Keypad8)){
			GameObject[] worldObjects = GameObject.FindGameObjectsWithTag("BigBall");

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
