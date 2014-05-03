using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileController : MonoBehaviour {
	private float speed = 20f;
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
	public AudioClip soundBomb;
	public bool toBeDestroyed = false;
	private float floatingSpeed = 0;
	private float floatingDirection = 0;

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
			if(this.tag == "Floater")			transform.Translate(direction*speed * Time.deltaTime, Space.World);
			else 								transform.Translate(direction*speed * Time.deltaTime);
		}

		beenChecked = false;

		//Check if ball is too far away.
		destroyRunAwayBall();

		if(this.tag == "Floater"){
			Color c = this.renderer.material.color;
			float greyConst = 0.5f;
			float fadingConst = 0.3f;
			float fadingSpeed = fadingConst *Time.deltaTime;

			if(c.r > greyConst)c.r-= fadingSpeed;
			else if(c.r < greyConst) c.r += fadingSpeed;

			if(c.g > greyConst)c.g-= fadingSpeed;
			else if(c.g < greyConst) c.g += fadingSpeed;

			if(c.b > greyConst)c.b-= fadingSpeed;
			else if(c.b < greyConst) c.b+= fadingSpeed;

			renderer.material.color=  c;
	
		}
	
	}

	//Starts this ball's movement
	public void StartMove(Vector3 dir){
		moving = true;
		direction = dir;
	}

	public void destroyRunAwayBall() {

		//Destroy if squared of position vector is bigger then tested value. Square root is not calculated for effeciency. 
		//Camera is at sqrMagnitude 400. Blackball is at 0.Check if bigball to differentiate from floaters.
		if(moving){
			if(this.tag == "BigBall" && this.transform.position.sqrMagnitude>401){
				Destroy(gameObject);
				perimeterScript.miss();
				EventManager.ProjectileResolved();
			}else if(this.tag == "Floater" && this.transform.position.sqrMagnitude>160){
				killObject(this.gameObject);
				//AudioSource.PlayClipAtPoint(soundPop, transform.position);
			}

		}
	}
	
	//On collision with another ball
	void OnTriggerEnter (Collider other) {		
		//Adds colliding ball to neighbour list. Second condition is to prevent blackBall from having grey neighbours
		if(other.tag == "BigBall" && !(gameObject.name == "BlackBall" && other.gameObject.renderer.material.color == Color.grey)){

			neighbours.Add(other.gameObject);
		}
	
		
		//If other target is smallBall ->destroy the smallBall
		if(other.tag == "SmallBall" && !other.transform.IsChildOf(transform) && this.tag != "MagazinedBall" && this.tag !="Floater"){
			//Rotate cluster according to impact
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
			if(!moving||collided)		{		GameMaster.lost(this.gameObject);showThisAsCauseOfLoss();}
			else 						insidePerimeter = false;
		}
	}
	void showThisAsCauseOfLoss(){

		ParticleSystem particleSystem =  gameObject.GetComponent<ParticleSystem>();
		particleSystem.Play();
	}
	void checkIfCollided(){
		//Stuck to cluster
		if(collided){


			moving = false;
			beenChecked = true;
			rooted = true;
			
			
			//Declare and instantiate comboList. If bomb comboList.Count will be 0
			List<GameObject> comboList = new List<GameObject>();
			if(gameObject.name != "Bomb") 	comboList = CountSameColors();							


			//Combo			
			if(comboList.Count >= comboLimit || gameObject.name == "Bomb"){
				if(comboList.Count >= comboLimit){
				
					//Play sound
					AudioSource.PlayClipAtPoint(soundPop, transform.position);
					
					//Removes all balls in combo
					foreach(GameObject gobject in comboList){
						killObject(gobject);
					}
					
					

				
				}else if(gameObject.name == "Bomb"){
					GameObject[] destroyList = new GameObject[neighbours.Count];
					neighbours.CopyTo(destroyList);

					int nBubblesExploded = 0;
					foreach(GameObject gObject in destroyList){	
						if(gObject.name != "BlackBall"){
							nBubblesExploded ++;
							killObject(gObject);
						}
					}


					Transform boom = Instantiate(bombExplosion, gameObject.transform.position, gameObject.transform.rotation) as Transform;
					AudioSource.PlayClipAtPoint(soundBomb, transform.position);
					killObject(gameObject);

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

				//Check if new ball is still close to perimeter
				perimeterScript.checkIfDanger();
			}else{
				perimeterScript.miss();

				//Check if new ball is close to perimeter
				perimeterScript.checkIfDanger();

				//Lose if ball collides outside perimeter	
				if(!insidePerimeter){										 
					GameMaster.lost(this.gameObject);
					showThisAsCauseOfLoss();
				}
			}
			

			collided = false;
			EventManager.ProjectileResolved();
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
	/*int removeUnrooted(){
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
	}*/
	int removeUnrooted(){
		GameObject[] worldObjects = GameObject.FindGameObjectsWithTag("BigBall");
		int nUnrooted = 0;
		foreach(GameObject gobject in worldObjects)
		{
			ProjectileController objectScript = gobject.GetComponent<ProjectileController>();
			if(objectScript.rooted == false && objectScript.toBeDestroyed == false && !objectScript.moving){
				nUnrooted++;
				objectScript.startFloating(Random.Range(2,3));
			}
		}
		return nUnrooted;
	}

	void startFloating(float seconds){
		this.tag = "Floater";
		moving = true;
		direction = transform.position;
		//direction = new Vector3(5,0,0);
		speed = 0.7f;
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


