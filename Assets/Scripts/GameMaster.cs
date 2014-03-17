using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

 
public class GameMaster : MonoBehaviour 
{
	private static GameMaster _Instance = null;
	public static GameMaster Instance { get { return _Instance; } }

	public static Dictionary<string, int> upgrades = new Dictionary<string, int>();


	public static string world;
	public static int worldNr = 1;
	public static int levelNr = 1;
	public static int nLevelsInCurrentWorld = 1;
	public static int nColors;
	public static int missToShrinkPer;

	
	public static bool hasLost = false;
	public static bool hasWon = false;
	public static bool levelComplete = false;
	public static bool worldComplete = false;
	public static bool levelReady = false;
	public static Texture backGroundTex;
	public static TextAsset levelInfoXml;
	

	/*States
	1 = options
	2 = game
	3 = lost
	*/
	void Start(){
		levelInfoXml = Resources.Load("leveldata") as TextAsset;

		float version = 0.007f;
		if(PlayerPrefs.GetFloat("GameVersion") != version){

			PlayerPrefs.DeleteAll();
			PlayerPrefs.SetFloat("GameVersion", version);

			//Create key for powerUps
			PlayerPrefs.SetInt ("BombFrequencyLevel", 1);
			PlayerPrefs.SetInt ("BombSizeLevel", 1);
		}

		//PlayerPrefs.SetInt ("BombFrequencyLevel", 1);
		//PlayerPrefs.SetInt ("BombSizeLevel", 1);

		upgrades.Add("BombFrequency", 0);
		upgrades.Add("BombFrequencyBase", 13);
		upgrades.Add("BombFrequencyCost", 100);
		upgrades.Add("BombFrequencyMaxLevel", 7);
		upgrades.Add("BombSize", 0);
		upgrades.Add("BombSizeBase", 5);
		upgrades.Add("BombSizeCost", 100);
		upgrades.Add("BombSizeMaxLevel", 5);
	}
	void Awake()
	{
		if (Instance == null || Instance == this){			
			_Instance = this;		
		}	
		else {		
			Destroy(gameObject);		
		}
		DontDestroyOnLoad(this);
	}
	public void startGame(){		
		Invoke("setUpLevel",0.5f);
	}

	
	public static void resetLevel(){
		nColors = 0;
		missToShrinkPer =0;
	
		hasLost = false;
		hasWon = false;
		levelComplete = false;
		levelReady = false;
		Debug.Log ("resetting level");
		
	}
    
	
	void Update(){
		GameObject blackBall = GameObject.Find("BlackBall");
		if(blackBall != null && GameMaster.levelReady){
			
			ProjectileController blackBallScript = blackBall.GetComponent<ProjectileController>();
			if(blackBallScript.neighbours.Count ==0){
				GameMaster.levelReady = false;
				stopBalls();
				changeLevel();

			}
		}
		

		//Debug to finish level
		if(Input.GetKey(KeyCode.Keypad3) && GameMaster.levelReady){
			GameMaster.levelReady = false;
			stopBalls();
			changeLevel();
		}
		

		//Debug colors
		if(Input.GetKey(KeyCode.Keypad2)){			
			ColorManager.checkAvailableColor();
		
		}

	}


	private void getLevelInfo(){
		//Get world and it's name
		XmlDocument xmlDoc = new XmlDocument(); // xmlDoc is the new xml document.
		xmlDoc.LoadXml(levelInfoXml.text); // load the file.
		XmlNode world = xmlDoc.SelectSingleNode("/levelInfo/world[@name='"+GameMaster.world+"']");
		XmlNode level = world.SelectSingleNode("level["+GameMaster.levelNr+"]");
		int.TryParse(level.SelectSingleNode("shrinkInterval/text()").Value, out GameMaster.missToShrinkPer);
		int.TryParse(level.SelectSingleNode("nColors/text()").Value, out GameMaster.nColors);
	}
	
	private void changeLevel(){
		
		GameMaster.levelNr++;
		GameMaster.levelComplete = true;
		EventManager.Victory();
		//Play sound
		Camera.main.audio.clip = Resources.Load("win") as AudioClip;
		Camera.main.audio.PlayOneShot(Camera.main.audio.clip);

		//Save level progress
		bool firstTimeCompleted = false;
		string prefix = GameMaster.world+"Level";
		if(PlayerPrefs.GetInt(prefix)<GameMaster.levelNr){
			PlayerPrefs.SetInt(prefix, GameMaster.levelNr);
			firstTimeCompleted = true;
			Debug.Log("Saving level progress");
		}


		if(GameMaster.levelNr <= GameMaster.nLevelsInCurrentWorld){
			//Invoke("setUpLevel",3);
			Debug.Log ("Setting up new level ");
		}
		else{
			//To avoid overlapping textures;
			//GameMaster.levelComplete = false;
			//if(GameMaster.world == "space")		GameMaster.hasWon = true;
			//else {
				if(firstTimeCompleted){

					GameMaster.worldComplete = true;
					PlayerPrefs.SetInt("isWorldUnlocked"+(GameMaster.worldNr+1), 1);
				
				}

				GameMaster.levelComplete = true;

			//}
		}
	}

		
	private void setUpLevel(){
		Debug.Log("Setting up level");
		GameMaster.levelComplete = false;

		//Setting power ups
		upgrades["BombFrequency"] = upgrades["BombFrequencyBase"] - (PlayerPrefs.GetInt("BombFrequencyLevel")-1);
		upgrades["BombSize"] = upgrades["BombSizeBase"] + (PlayerPrefs.GetInt("BombSizeLevel")-1);
		GameObject playMaster = GameObject.Find("PlayLevelMaster");
		GameGUI gameGUIScript = playMaster.GetComponent<GameGUI>();
		gameGUIScript.initiateBomb();

		getLevelInfo();
		
		PerimeterController.missLeft = GameMaster.missToShrinkPer;
		ColorManager.setColors(nColors);
		Camera.main.audio.clip = Resources.Load("DST-Destiny") as AudioClip;
		Camera.main.audio.PlayOneShot(Camera.main.audio.clip);
		Camera.main.audio.volume = 0.02f;
		
		//Set background		
		GameObject backGround = GameObject.Find("BackGroundPlane");
		backGround.renderer.material.mainTexture = backGroundTex;


		//Remove existing objects
		GameObject[] clusterBalls = GameObject.FindGameObjectsWithTag("BigBall");
		foreach (GameObject go in clusterBalls) {
            Destroy(go);
        }

		GameObject[] magazinedBalls = GameObject.FindGameObjectsWithTag("MagazinedBall");
		foreach (GameObject go in magazinedBalls) {
			Destroy(go);
		}

		//A comment
		
		//if(GameMaster.world == "space" && GameMaster.levelNr==GameMaster.nLevelsInWorld+1)	GameMaster.hasWon = true;	
		//else{
			GameObject perimeter = GameObject.FindWithTag("Perimeter");
			if(perimeter != null)	Destroy(perimeter);			
		
			GameObject playLevelMaster = GameObject.FindWithTag("PlayLevelMaster");
			if(playLevelMaster != null){
				SpawnBalls spawnBallsScript = playLevelMaster.GetComponent<SpawnBalls>();
				spawnBallsScript.initiateLevel();
			}

			//Make cluster preview rotate
			ClusterController clustCon = Camera.main.GetComponent<ClusterController>();
			clustCon.previewSpin(4.0f,360);
				
		Invoke("setLevelReady",4.5f);
		//Invoke("setLevelReady",0.5f);
		//}
	}	

	private void nonsense(){

	}

	private void setLevelReady(){
		GameMaster.levelReady = true;
		Shooter shooterScript = Camera.main.GetComponent<Shooter>();
		shooterScript.loadBallsAtStart();
	}
	
	public static void lost(){
		Debug.Log ("You just lost");
		hasLost = true;
		Camera.main.audio.clip = Resources.Load("lost") as AudioClip;
		Camera.main.audio.PlayOneShot(Camera.main.audio.clip);
		stopBalls();
		//Make cluster preview rotate
		ClusterController clustCon = Camera.main.GetComponent<ClusterController>();
		clustCon.previewSpin(0.0f,30);
		

		//AudioSource[] audios = Camera.main.GetComponents<AudioSource>();
		//audios[1].Play();
	}
	
	private static void stopBalls(){
		GameObject[] worldObjects = GameObject.FindGameObjectsWithTag("BigBall");
	
		for(int i = 0; i<worldObjects.Length;i++){
			ProjectileController projectileScript = worldObjects[i].GetComponent<ProjectileController>();
			projectileScript.moving = false;
		}
	}
}