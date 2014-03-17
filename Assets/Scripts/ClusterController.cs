using UnityEngine;
using System.Collections;

public class ClusterController : MonoBehaviour {
	private float rotSpeed = 0f;
	private Vector3 rotateAxis = Vector3.zero;
	private string mode = "play";
	void Start () {
	
	}
	
	public void setVectors(float rSpeed,Vector3 rAxis){
		rotSpeed = rSpeed;
		rotateAxis = rAxis;
	}

	void FixedUpdate () {
		if(rotSpeed !=0){
			spinCluster (rotSpeed);
		}
		if(mode == "play"){
			if(rotSpeed<0.2f)	rotSpeed =0;	
			else rotSpeed*= 0.95f;
		}

	}

	private void spinCluster(float speed){
		GameObject[] worldObjects = GameObject.FindGameObjectsWithTag("BigBall");
		foreach(GameObject gObject in worldObjects)
		{
			gObject.transform.RotateAround(Vector3.zero,  rotateAxis, rotSpeed*Time.deltaTime);
		}

	}

	public void previewSpin(float seconds, int degrees){
		mode = "preview";
		float degreesPerSec;


		if(seconds != 0){
			degreesPerSec = degrees/seconds; 
			Invoke("setModePlay",seconds);
		}
		else {
			//0 seconds equals endless rotation at degrees per second. Used when lost
			degreesPerSec = degrees;
		}
		setVectors (degreesPerSec, new Vector3(0,1,0));
	}





	private void setModePlay(){
		rotSpeed = 0f;
		mode = "play";
	}
}
