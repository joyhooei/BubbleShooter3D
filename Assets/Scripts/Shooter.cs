using UnityEngine;
using System.Collections.Generic;


public class Shooter : MonoBehaviour {
	public Transform projectile;
	public Transform smallProjectile;
	public Transform bomb;
	private ProjectileController controllerScript;
	private bool weaponReady = false;
	private float time = 0;
	private Transform[] balls;
	private Vector3[] ballPos;
	private int nBalls;
	private GameObject blackBall;
	private Transform smallBall = null;
	private List<Color> colorsAvailable;
	private Vector3 positionSelected = new Vector3(-1,-1,-1);
	private bool freeView = false;
	private bool freeViewUsedThisRound = false;
	private CameraController cameraController;

	
	// Use this for initialization
	void Start () {


		nBalls = 2+PlayerPrefs.GetInt("ExtraBallLevel");
		balls = new Transform[nBalls];
		//Set magazined balls positions
		ballPos = new Vector3[]{
			new Vector3(1.5f, -1.5f, 3),
			new Vector3(3.2f, -1.3f, 5),
			new Vector3(4.6f, -0.9f, 7),
		};
		EventManager.onFreeView += setFreeView;
		EventManager.onProjectileResolved += setWeaponReady;
	}
	
	void OnDisable(){
		EventManager.onFreeView -= setFreeView;
		EventManager.onProjectileResolved -= setWeaponReady;
		
	}

	
	void setFreeView(bool isActive){
		freeView  = isActive;
		freeViewUsedThisRound = true;
	}

	void setWeaponReady(){	
		loadBall(0);
		makeBall();
		weaponReady = true;
	}
	
    public void loadBallsAtStart(){	
		blackBall = GameObject.Find("BlackBall");

		makeBall();
		for(int i = nBalls-2;i>=0;i--){
			loadBall(i);
			makeBall();
		}

		weaponReady = true;
	}

    void Update() {	
		if(Input.GetKey(KeyCode.Keypad9)){
			freeView = true;
		}
		
		if(Input.GetKey(KeyCode.Keypad7)){
			freeView = false;
		}

		time += Time.deltaTime;
		if(!GameMaster.hasWon && !GameMaster.levelComplete && !GameMaster.hasLost &&  Time.timeScale == 1.0f){
						
			positionSelected = new Vector3(-1,-1,-1);
			
			foreach (Touch touch in Input.touches) {                
				if (touch.phase == TouchPhase.Began) {
					positionSelected = touch.position;					
				}	            
	        }
			if (Input.GetButtonDown("Fire1") && positionSelected==new Vector3(-1,-1,-1)) {		
				positionSelected = Input.mousePosition;	
			}
			if(freeView){
			}else{
				
				if(positionSelected != new Vector3(-1,-1,-1) &&  weaponReady){
					Ray ray = Camera.main.ScreenPointToRay (positionSelected);
					RaycastHit hit;
					if (Physics.Raycast(Camera.main.transform.position, ray.direction, out hit) && raycastMagazinedBalls(hit.transform)){
						swapBall();
					}else if(positionSelected[0] <130 && positionSelected[1] <260){
						//Stop from shooting when clicking bomb powerup icon. Values are by trial and error
					}else{
						//Shoots

						//Sets bomb model and bomb collider size 
						if(balls[0].name=="Bomb"){
							float radius = (float)GameMaster.upgrades["BombSize"]/20;
							(balls[0].gameObject.collider as SphereCollider).radius = radius;
							
							Transform bombModel = balls[0].FindChild("Bomb(Clone)");
							float size = (float)GameMaster.upgrades["BombSize"]/10;
							bombModel.transform.localScale = new Vector3(size,size,size);
						}

						//Count a freeView powerup used
						if(freeViewUsedThisRound){
							freeViewUsedThisRound = false;
							GameMaster.upgrades["FreeView"]--;
						}

						controllerScript = balls[0].GetComponent<ProjectileController>();
						balls[0].transform.localPosition = new Vector3(0,0,0);
						balls[0].transform.rotation = transform.rotation;
						balls[0].transform.parent = null;
						controllerScript.StartMove(ray.direction);
						balls[0].tag = "BigBall";
						
						weaponReady = false;
						time = 0;	

						EventManager.ObjectFired(balls[0].name);
						//Set balls[0] variable to null so a new ball can be loaded
						balls[0] = null;
						
						
						
					}
				}	
			}
		}
		
		
		
		/*if(Input.GetKey(KeyCode.Keypad2)){
			balls[0].renderer.material.color = Color.blue;
			magazinedBall.renderer.material.color = Color.blue;

		}*/
		


		
	}

	bool raycastMagazinedBalls(Transform hitTransform){
		foreach(Transform ball in balls){
			if(hitTransform == ball)	return true;
		}

		return false;
	}
	//Takes ballnr as argument. Will make ball and place it as ball number "ballnr"
	void makeBall(){
		int indexToMake = nBalls -1;
		//Make new magazined ball
		balls[indexToMake] = Instantiate(projectile, transform.position, transform.rotation) as Transform;
		balls[indexToMake].renderer.material.shader = Shader.Find("Parallax Specular");
		balls[indexToMake].tag = "MagazinedBall";
		balls[indexToMake].name = "Bubble";
		ColorManager.checkAvailableColor();
		balls[indexToMake].renderer.material.color = ColorManager.getRandColor();
		balls[indexToMake].transform.parent = this.transform;


		balls[indexToMake].transform.localPosition = ballPos[indexToMake];		

		ProjectileController script = balls[indexToMake].GetComponent<ProjectileController>();
		script.blackBall = blackBall;

	}
	
	void loadBall(int ballIndex){
		//Return if ballIndex is equal to last ball
		if(ballIndex >= nBalls -1)		return;
		//Load ball one step ahead
		balls[ballIndex] = balls[ballIndex+1];


		balls[ballIndex].transform.localPosition = ballPos[ballIndex];

		if(ballIndex ==0){
			//Make a small ball as core for the balls[0] ball. It is vital for collisions		
			smallBall = Instantiate(smallProjectile, transform.position, transform.rotation) as Transform;
			smallBall.transform.parent = balls[0];
			smallBall.transform.position = balls[0].transform.position;
			smallBall.tag = "SmallBall";
		}

		loadBall (++ballIndex);

	}
	
	void swapBall(){
		//Not allowed to swap bombs
		//if(balls[0].renderer.material.color!= Color.black){
		Color temp = balls[0].renderer.material.color;
		for(int i = 0;i<nBalls-1;i++){
			balls[i].renderer.material.color = balls[i+1].renderer.material.color;
		}

		balls[nBalls-1].renderer.material.color = temp ;
		//}
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
			foreach(Transform ball in balls){
				if(ball !=null)			checkForInvalidColor(ball);

			}
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
		if(Time.timeScale == 1.0f && balls[0] != null){
			//balls[0].renderer.material.color = Color.black;
			balls[0].name = "Bomb";
			balls[0].renderer.enabled = false;

			Transform bombModel;
			bombModel = Instantiate(bomb, balls[0].transform.position, Quaternion.LookRotation(new Vector3(0.0f, 0.0f, 1.0f),new Vector3(-0.2f, 0.5f, -0.1f))) as Transform;
			bombModel.transform.parent = balls[0].transform;

			//float radius = (float)GameMaster.upgrades["BombSize"]/10;
			//(balls[0].gameObject.collider as SphereCollider).radius = radius;

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
