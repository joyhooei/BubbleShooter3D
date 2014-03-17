using UnityEngine;
using System.Collections.Generic;


public class Shooter : MonoBehaviour {
	public Transform projectile;
	public Transform smallProjectile;
	public Transform bomb;
	private ProjectileController controllerScript;
	private bool weaponReady = false;
	private float time = 0;
	private Transform magazinedBall = null;
	private Transform loadedBall = null;
	private Transform smallBall = null;
	private List<Color> colorsAvailable;
	private Vector3 positionSelected = new Vector3(-1,-1,-1);
	
	// Use this for initialization
	void Start () {

	}

	void onDisable(){

	}
	
    public void loadBallsAtStart(){	
        if(magazinedBall!=null){	Destroy(magazinedBall.gameObject);				}
		if(loadedBall!=null)		Destroy(loadedBall.gameObject);
		if(smallBall!=null)			Destroy(smallBall.gameObject);
        
		makeBall();
		loadBall();
		makeBall();

		weaponReady = true;
	}
	
    void Update() {		
		time += Time.deltaTime;
		if(!GameMaster.hasWon && !GameMaster.levelComplete && !GameMaster.hasLost &&  Time.timeScale == 1.0f){
			if(time>=1.8f && loadedBall == null && GameMaster.levelReady){

				loadBall();
				makeBall();
				weaponReady = true;

			}			
			positionSelected = new Vector3(-1,-1,-1);
			
			foreach (Touch touch in Input.touches) {                
				if (touch.phase == TouchPhase.Began) {
					positionSelected = touch.position;					
				}	            
	        }
			if (Input.GetButtonDown("Fire1") && positionSelected==new Vector3(-1,-1,-1)) {		
				positionSelected = Input.mousePosition;	
			}
			if(positionSelected != new Vector3(-1,-1,-1) && weaponReady){
				Ray ray = Camera.main.ScreenPointToRay (positionSelected);
				RaycastHit hit;
		        if (Physics.Raycast(Camera.main.transform.position, ray.direction, out hit) && (hit.transform == loadedBall ||hit.transform == magazinedBall)){
		        	swapBall();
				}else if(positionSelected[0] <130 && positionSelected[1] <130){
					//Stop from shooting when clicking bomb powerup icon. Values are by trial and error
				}else{
					//Shoots


					controllerScript = loadedBall.GetComponent<ProjectileController>();
					loadedBall.transform.localPosition = new Vector3(0,0,0);
					loadedBall.transform.rotation = transform.rotation;
					loadedBall.transform.parent = null;
					controllerScript.StartMove(ray.direction);
					loadedBall.tag = "BigBall";
					
					weaponReady = false;
					time = 0;	

					Debug.Log ("fIREOBJECT NAME "+loadedBall.name);
					EventManager.ObjectFired(loadedBall.name);
					//Set loadedBall variable to null so a new ball can be loaded
					loadedBall = null;


					
				}
			}			
		}
		
		
		
		/*if(Input.GetKey(KeyCode.Keypad2)){
			loadedBall.renderer.material.color = Color.blue;
			magazinedBall.renderer.material.color = Color.blue;

		}*/
		
		if(Input.GetKey(KeyCode.Keypad1)){
			loadedBall.renderer.material.color = Color.red;
			magazinedBall.renderer.material.color = Color.red;

		}
		if(Input.GetKey(KeyCode.Keypad2)){
			/*loadedBall.renderer.material.color = Color.black;
			loadedBall.name = "Bomb";*/
			PlayerPrefs.SetInt("BombFrequency",10);

		}
		
	}
	void makeBall(){
		//Make new magazined ball
		magazinedBall = Instantiate(projectile, transform.position, transform.rotation) as Transform;
		magazinedBall.renderer.material.shader = Shader.Find("Parallax Specular");
		magazinedBall.tag = "MagazinedBall";
		magazinedBall.name = "Bubble";
		ColorManager.checkAvailableColor();
		magazinedBall.renderer.material.color = ColorManager.getRandColor();
		magazinedBall.transform.parent = this.transform;
		magazinedBall.transform.localPosition = new Vector3(4.9f,-1.6f,7);		

		GameObject blackBall = GameObject.Find("BlackBall");
		ProjectileController script = magazinedBall.GetComponent<ProjectileController>();
		script.blackBall = blackBall;

	}
	
	void loadBall(){
		//Set magazined ball to loaded ball
		loadedBall = magazinedBall;
		loadedBall.transform.localPosition = new Vector3(1.5f,-1.5f,3);
		
		//Make a small ball as core for the loadedBall ball. It is vital for collisions		
		smallBall = Instantiate(smallProjectile, transform.position, transform.rotation) as Transform;
		smallBall.transform.parent = loadedBall;
		smallBall.transform.position = loadedBall.transform.position;
		smallBall.tag = "SmallBall";
	}
	
	void swapBall(){
		//Not allowed to swap bombs
		if(loadedBall.renderer.material.color!= Color.black){
			Color temp = loadedBall.renderer.material.color;
			loadedBall.renderer.material.color = magazinedBall.renderer.material.color;
			magazinedBall.renderer.material.color = temp ;
		}
	}

	
	//Swaps color of loaded and magazined ball if there is no such color present in cluster. Wait a certain amount of time to let combo destroy
	public void swapColor(){
		Invoke("swapColor2", .3f);
	}
	
	public void swapColor2(){
		ColorManager.checkAvailableColor();
		/*for(int i =0; i<ColorManager.colors.Count;i++){
			Debug.Log (ColorManager.colors[i]);
		}*/
		if(!GameMaster.levelComplete){
			if(loadedBall != null)	checkForInvalidColor(loadedBall);						
			if(magazinedBall != null)		checkForInvalidColor(magazinedBall);	
		}
	}
	
	void checkForInvalidColor(Transform ballToBeChecked){
		for(int i =0; i<ColorManager.colors.Count;i++){
			if(ballToBeChecked.renderer.material.color == ColorManager.colors[i])	return;
		}
		//Didn't find the color -> color is invalid and must be swapped
		ballToBeChecked.renderer.material.color = ColorManager.getRandColor();
	}
	
	public bool loadBomb(){
		if(Time.timeScale == 1.0f && loadedBall != null){
			//loadedBall.renderer.material.color = Color.black;
			loadedBall.name = "Bomb";
			loadedBall.renderer.enabled = false;

			Transform bombModel;
			bombModel = Instantiate(bomb, loadedBall.transform.position, Quaternion.LookRotation(new Vector3(0.0f, 0.0f, 1.0f),new Vector3(-0.2f, 0.5f, -0.1f))) as Transform;
			bombModel.transform.parent = loadedBall.transform;

			float radius = (float)GameMaster.upgrades["BombSize"]/10;
			(loadedBall.gameObject.collider as SphereCollider).radius = radius;

			/*float size = (float)GameMaster.upgrades["BombSize"]/10;
			size ++;
			size = 0.7f;
			bombModel.transform.localScale = new Vector3(size,size,size);
			*/return true;
		}
		else {
			return false;
		}
	}
	
	
}
