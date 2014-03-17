using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileController : MonoBehaviour {
	private int speed = 20;
	public bool moving = false;
	public List<GameObject> neighbours = new List<GameObject>();
	public int comboLimit;
	public bool beenChecked = false;
	public bool rooted = false;
	public bool collided = false;
	private bool insidePerimeter = false;
	private Vector3 direction = Vector3.forward;
	public Transform explosion;
	public Transform bombExplosion;
	public AudioClip soundPop;
	public bool toBeDestroyed = false;

	private PerimeterController perimeterScript;
	private GameGUI gameGUIScript;
	public GameObject blackBall;	
	
	void Start () {
		comboLimit = 4;
		
		GameObject perimeter = GameObject.FindWithTag("Perimeter");
		perimeterScript = perimeter.GetComponent<PerimeterController>();

		GameObject playMaster = GameObject.Find("PlayLevelMaster");
		gameGUIScript = playMaster.GetComponent<GameGUI>();


		//For when this script is attached to blackball
		if(this.transform.name == "BlackBall")	blackBall = this.gameObject;//blackBall = GameObject.Find("BlackBall");

	}
	
	//Updates every frame
	void Update () {
		checkIfCollided();
		
		//Move ball forward
		if(moving){
			transform.Translate(direction*speed * Time.deltaTime);
		}

		beenChecked = false;
		
		
		//Debug.Log (perimeterScript.targetScale*perimeterScript.targetScale - gameObject.transform.position.sqrMagnitude);
		/*if(updateCount%30 ==0 && gameObject.tag =="BigBall" && !moving){
			int scale = perimeterScript.targetScale/2-1;//Debug.Log(perimeterScript.targetScale*perimeterScript.targetScale - gameObject.transform.position.sqrMagnitude);
			float diff = scale*scale - gameObject.transform.position.sqrMagnitude;
			if(diff<0)
				//Debug.Log(gameObject.renderer.material.color);
				perimeterScript.dangerAlert();
		}*/
		
		
	}
	
	//Starts this ball's movement
	public void StartMove(Vector3 dir){
		moving = true;
		Invoke("DestroyRunAwayBall", 1.8f);
		direction = dir;
	}
	
	public void DestroyRunAwayBall() {
		//Destroy if ball has traveled for 100 frames
		if(!(collided || rooted)){
			Destroy(gameObject);
			perimeterScript.miss();
		}

	}
	
	//On collision with another ball
	void OnTriggerEnter (Collider other) {		
		//Adds colliding ball to neighbour list. Second condition is to prevent blackBall from having grey neighbours
		if(other.tag == "BigBall" && !(gameObject.name == "BlackBall" && other.gameObject.renderer.material.color == Color.grey)){

			neighbours.Add(other.gameObject);
		}
	
		
		//If other target is smallBall ->destroy the smallBall
		if(other.tag == "SmallBall" && !other.transform.IsChildOf(transform) && this.tag != "MagazinedBall"){
			//Rotate cluster according to impact
			Debug.Log ("blackball "+ blackBall+"  this name"+this.transform.name+"  other"+other+"  other name"+other.name);
			Vector3 deltaVec = other.transform.position - blackBall.transform.position;
			Vector3 rotateAxis = Vector3.Cross(deltaVec, direction);
			Vector3 sumVec = deltaVec + direction;
			
			Vector3 ortogVec = Vector3.Cross(rotateAxis,deltaVec);
			float rotSpeed = Vector3.Dot(ortogVec,sumVec);
			ClusterController clustCon = Camera.main.GetComponent<ClusterController>();
			clustCon.setVectors(rotSpeed*30,rotateAxis);

			//Destroy the projectile's smallball
			GameObject smallBall = other.gameObject;
			GameObject projectile = smallBall.transform.parent.gameObject;
			ProjectileController projectileScript = projectile.GetComponent<ProjectileController>();
			projectileScript.collided = true;
			Destroy (smallBall);
			
				
		}		
		
		if(other.tag == "Perimeter"){insidePerimeter = true;}
	}
	
	
	void OnTriggerExit (Collider other) {
		//Remove neighbour from neighbourslist if it looses connection
		if(other.tag == "BigBall"){
			neighbours.Remove(other.gameObject);                                 // add an item to the end of the List
		}
		
		//Set game to lost if a collided or not moving ball leaves the perimeter
		if(other.tag == "Perimeter"){
			if(!moving||collided)		{		GameMaster.lost();}
			else 						insidePerimeter = false;
		}
	}
	
	void checkIfCollided(){
		//Stuck to cluster
		if(collided){
			if(!insidePerimeter){		 GameMaster.lost();}
			/*
			foreach (Transform child in transform)
			{
				Destroy(child.gameObject);
			}*/
			moving = false;
			beenChecked = true;
			rooted = true;
			
			
			GameObject[] destroyList = new GameObject[neighbours.Count];
        	neighbours.CopyTo(destroyList);
			
			if(gameObject.name == "Bomb"){
				int nBubblesExploded = 0;
				foreach(GameObject gObject in destroyList){	
					if(gObject.name != "BlackBall"){
						nBubblesExploded ++;
						killObject(gObject);
					}
				}

				
				Transform boom = Instantiate(bombExplosion, gameObject.transform.position, gameObject.transform.rotation) as Transform;
				killObject(gameObject);
				
				//Sets all balls to be unrooted
				GameObject[] worldObjects = GameObject.FindGameObjectsWithTag("BigBall");
				foreach(GameObject gObject in worldObjects)
				{
					ProjectileController objectScript = gObject.GetComponent<ProjectileController>();
					objectScript.rooted= false;
				}

				ProjectileController blackballscript = blackBall.GetComponent<ProjectileController>();		
				blackballscript.rooted = true;
				blackballscript.beenChecked = true;
				checkRoots(blackBall);
				int nUnrooted = removeUnrooted();


				calcAndSendBubblePoints(0,nUnrooted,nBubblesExploded);
				//Swap loaded balls if explosion eliminated any colors
				Shooter shooterScript = Camera.main.GetComponent<Shooter>();
				shooterScript.swapColor();
				
				
				return;
			}
			
			
			//Combo
			List<GameObject> comboList = CountSameColors();			
			if(comboList.Count >= comboLimit){
				
				//Play sound
				AudioSource.PlayClipAtPoint(soundPop, transform.position);
				
				//Removes all balls in combo
				foreach(GameObject gobject in comboList){
					killObject(gobject);
				}
				
				
				//Sets all balls to be unrooted
				GameObject[] worldObjects = GameObject.FindGameObjectsWithTag("BigBall");
				foreach(GameObject gObject in worldObjects)
				{
					ProjectileController objectScript = gObject.GetComponent<ProjectileController>();
					objectScript.rooted= false;
				}

				ProjectileController blackballscript = blackBall.GetComponent<ProjectileController>();		
				blackballscript.rooted = true;
				blackballscript.beenChecked = true;
				checkRoots(blackBall);
				int nUnrooted = removeUnrooted();
				

				calcAndSendBubblePoints(comboList.Count,nUnrooted,0);		
				
				if(comboList.Count >12){
					//AudioSource.PlayClipAtPoint(Resources.Load("ultraCombo") as AudioClip, Camera.main.transform.position);	
				}else if(comboList.Count >9){
					//AudioSource.PlayClipAtPoint(Resources.Load("megaCombo") as AudioClip, Camera.main.transform.position);
				}else if(comboList.Count >7){				
					//AudioSource.PlayClipAtPoint(Resources.Load("superCombo") as AudioClip, Camera.main.transform.position);
				}
				
				//If combo eliminated a color that is loaded, swap the color
				Shooter shooterScript = Camera.main.GetComponent<Shooter>();
				shooterScript.swapColor();
				
			}else{
				perimeterScript.miss();
				
				
				
			}
			
			//Check if new ball is close to perimeter
			perimeterScript.checkIfDanger();
			collided = false;
		}
	}

	private void calcAndSendBubblePoints(int combo, int unRooted, int exploded){
		int score = 0;
		for(int i=1;i<=combo;i++){
			score += 100+20*i;
		}	
		score += unRooted*100;
		EventManager.BubblesPopped(combo,unRooted,exploded, score, this.transform.position);
		
			
	}

	//Checks if connected to root
	void checkRoots(GameObject node){
		ProjectileController nodeScript = node.GetComponent<ProjectileController>();
		
		foreach(GameObject gobject in nodeScript.neighbours)
        {
			ProjectileController objectScript = gobject.GetComponent<ProjectileController>();
			if(!objectScript.beenChecked){
				objectScript.beenChecked = true;
				objectScript.rooted = true;
				checkRoots(gobject);
			}
		}
	}
	
	
	//Removes balls that aren't rooted
	int removeUnrooted(){
		GameObject[] worldObjects = GameObject.FindGameObjectsWithTag("BigBall");
		int nUnrooted = 0;
		foreach(GameObject gobject in worldObjects)
        {
			ProjectileController objectScript = gobject.GetComponent<ProjectileController>();
			if(objectScript.rooted == false && objectScript.toBeDestroyed == false && !objectScript.moving){
				nUnrooted++;
				killObject(gobject);
			}
		}
		return nUnrooted;
	}
	
		
	//Removes ball from neighbours' neighbourslist and destroyes itself
	void killObject(GameObject victim){
		ProjectileController victimScript = victim.GetComponent<ProjectileController>();
		foreach(GameObject gobject in victimScript.neighbours)
        {
			ProjectileController objectScript = gobject.GetComponent<ProjectileController>();
			objectScript.neighbours.Remove(victim); 
		}
	
		Transform boom = Instantiate(explosion, victim.transform.position, victim.transform.rotation) as Transform;
		victimScript.toBeDestroyed = true;
		Destroy(victim);
	}
	
	
	//Uses reccursive function to go through neighbours in neighbourslist and count balls with same color
	public List<GameObject> CountSameColors(){
		List<GameObject> tempList = new List<GameObject>();
		tempList.Add(gameObject);
		foreach(GameObject gobject in neighbours)
        {
            ProjectileController objectScript = gobject.GetComponent<ProjectileController>();
			if(!objectScript.beenChecked && gobject.renderer.material.color == gameObject.renderer.material.color){
				objectScript.beenChecked = true;
				tempList.AddRange(objectScript.CountSameColors());
			}
		}
		return tempList;
	}
	
	
}


