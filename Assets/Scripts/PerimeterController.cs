using UnityEngine;
using System.Collections;

public class PerimeterController : MonoBehaviour {
	private int nBlink = 0;
	private bool shrinking = false;
	public int targetScale = 15;
	private float shrinkSpeed = 0.8f;
  	static public int missLeft = GameMaster.missToShrinkPer;
	private bool increasingAlpha = true;
	
	float alphaIncSpeed = 0.0f;
	string dangerLevel = "";
	void Update(){
		if (shrinking) {
			gameObject.transform.localScale -= Vector3.one*Time.deltaTime*shrinkSpeed;
			
			if (gameObject.transform.localScale.x < targetScale){
				gameObject.transform.localScale = new Vector3(targetScale,targetScale,targetScale);
			    shrinking = false;
				targetScale--;
				//Check if the shrinking made balls come into danger
				checkIfDanger();
			}
		}
		
		Color c = gameObject.renderer.material.color;
		if(dangerLevel == "none"){			
			c.a = 0.4f;
			gameObject.renderer.material.color = c;
		}else{
			if(dangerLevel == "low")				alphaIncSpeed=0.003f;
			else if(dangerLevel == "medium")		alphaIncSpeed=0.005f;
			else if(dangerLevel == "high")			alphaIncSpeed=0.007f;
			
			
			if(c.a>0.55f)		increasingAlpha = false;
			else if(c.a<0.4f)	increasingAlpha = true;
			
			if(increasingAlpha) c.a += alphaIncSpeed;
			else c.a -= alphaIncSpeed;
			gameObject.renderer.material.color = c;
		}
	}
	
	public bool miss(){
		missLeft--;
		
		//Shrinks
		if(missLeft ==0){
			missLeft = GameMaster.missToShrinkPer;
			
			//Shrink the perimeter
			
			//Play shrinking sound
			//g.renderer.enabled = !g.renderer.enabled;
			//gameObject.audio.Play();
			blink();
			
			//gameObject.transform.localScale -= new Vector3(2,2,2);
			//Shrink the perimeter not in use
			//GameObject g2 = GameObject.FindWithTag("Perimeter2");
			//g2.transform.localScale -= new Vector3(1,1,1);
			
			
			//Check if any balls are outside
			/*GameObject[] worldObjects = GameObject.FindGameObjectsWithTag("BigBall");
			foreach(GameObject gObject in worldObjects)
			{
				ProjectileController objectScript = gObject.GetComponent<ProjectileController>();
				if(objectScript.moving==false)
					objectScript.isOutsidePerimeter();
				
			}*/
			return true;
		}
		return false;
	}
	private void blink(){
		nBlink++;
		gameObject.renderer.enabled = !gameObject.renderer.enabled;
		
		if(nBlink<6)	Invoke("blink",0.2f);
		else {
			nBlink = 0;
			shrinking = true;
			
			gameObject.audio.Play();
			//gameObject.transform.localScale -= new Vector3(1,1,1);
			
		}
	}
	
	public void dangerAlert(){
		Debug.Log("danger");
	}
	
	public void checkIfDanger(){

		GameObject[] balls = GameObject.FindGameObjectsWithTag("BigBall");
		float scale = targetScale/2-0.0f;
		float prod = scale*scale;
		float largestDiff = 0;
		foreach(GameObject gObject in balls){
			ProjectileController objectScript = gObject.GetComponent<ProjectileController>();
			if(!objectScript.moving && !objectScript.toBeDestroyed){
				float diff = gObject.transform.position.sqrMagnitude-prod;
				if(diff > largestDiff)	largestDiff = diff;
			}
		}
		if(largestDiff > 0){
			if(largestDiff < 3)			dangerLevel = "low";
			else if(largestDiff < 11)	dangerLevel = "medium";		
			else if(largestDiff < 50)	dangerLevel = "high";
			
		}else{		
			dangerLevel = "none";
		
		}
	}

}