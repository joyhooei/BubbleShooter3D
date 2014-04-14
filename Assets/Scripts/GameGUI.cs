using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

public class GameGUI : MonoBehaviour {

	//The game
	public Texture2D sphereTex;
	public Texture2D bombTex;
	public Texture2D freeViewTex;
	public Texture2D lostTex;
	public Texture2D wonTex;
	public Texture2D lvlCompTex;
	public Texture2D worldCompTex;
	public Texture2D starFull;
	public Texture2D starEmpty;
	public Texture2D newRecordTex;
	public GUISkin bombSkin;
	public GUISkin bombActiveSkin;
	public GUISkin freeViewSkin;
	public GUISkin menuSkin;
	public GUIText scoreGUI;
	public GUIText scorePopUpGUI;

	public Dictionary<string, int> scoreSummarize = new Dictionary<string, int>();

	private LevelStatRecorder statRecorder;
	private int bombCounter;
	private bool bombActive;
	private GameObject gameMaster;
		
	private bool popUpVisible = false;
	private Vector3 popUpPos;
	private bool escMenuActivated = false;
	private bool isFreeViewActive = false;
	
	void Start(){


		gameMaster = GameObject.FindWithTag("GameMaster");

		statRecorder = gameObject.GetComponent<LevelStatRecorder>();

		bombCounter = GameMaster.upgrades["BombFrequency"];
		bombActive  = false;

		scoreGUI.fontSize= 50; 
		scoreGUI.material.color = Color.yellow;
        scoreGUI.transform.position = new Vector3(0.99f, 0.99f, 0.0f);
		scoreGUI.anchor = TextAnchor.UpperRight;
		

		scorePopUpGUI.fontSize= 50; 
		scorePopUpGUI.material.color = Color.yellow;
		scorePopUpGUI.anchor = TextAnchor.MiddleCenter;
		scorePopUpGUI.text = "";

		EventManager.onBubblesPopped += setPopUp;
		EventManager.onObjectFired += setBomb;
		EventManager.onFreeView += setFreeView;

		
	}

	void OnDisable(){
		EventManager.onBubblesPopped -= setPopUp;
		EventManager.onObjectFired -= setBomb;
		EventManager.onFreeView -= setFreeView;

	}

	private void setBomb(string objectName){
		if(objectName != "Bomb"){
			bombCounter --;
			if(bombCounter <=0){
				bombActive = true;
			}
		}
	}

	private void setFreeView(bool freeViewActive){
		isFreeViewActive = freeViewActive;


	}

	public void initiateBomb(){
		bombCounter = GameMaster.upgrades["BombFrequency"];
	}
	
	private void setPopUp(int combo,int unRooted,int exploded, int points, Vector3 pos){
		popUpVisible = true;
		popUpPos = pos;
		transformPopUpToScreen();		
		if(points > 0) scorePopUpGUI.text = ""+points;
		scorePopUpGUI.material.color = new Color(scorePopUpGUI.material.color[0],scorePopUpGUI.material.color[1]-(float)combo/50,scorePopUpGUI.material.color[2], 1.0f);
		scorePopUpGUI.fontSize +=Mathf.FloorToInt(points /50);
		Invoke ("removePopUp", 2);
		
	}
	
	private void removePopUp(){
		popUpVisible = false;
		scorePopUpGUI.text = "";
		scorePopUpGUI.fontSize= 50; 
		scorePopUpGUI.material.color = Color.yellow;
	}
	
	private void transformPopUpToScreen(){
		Vector3 screenPos = Camera.main.WorldToScreenPoint(popUpPos);
		float screenX = screenPos.x/Screen.width;
		float screenY = screenPos.y/Screen.height;	
		scorePopUpGUI.transform.position = new Vector3(screenX, screenY, 0.0f);
	}
	
	void OnGUI () {

		if(!(GameMaster.hasWon || GameMaster.hasLost))
			//GUI.Label(new Rect(Screen.width/2-crosshairTex.width/2,Screen.height/2-crosshairTex.height/2, crosshairTex.width, crosshairTex.height), crosshairTex);
	
		for(int i=0; i<PerimeterController.missLeft;i++){
			GUI.Label(new Rect(5+i*2*sphereTex.width/3,5, sphereTex.width, sphereTex.height), sphereTex);
		}
			
		
		//Free view button
		if(GameMaster.upgrades["FreeView"]>0){
			GUI.skin =freeViewSkin;
			if (GUI.Button(new Rect(5,Screen.height-(freeViewTex.height+bombTex.height), freeViewTex.width, freeViewTex.height),""+GameMaster.upgrades["FreeView"])){
				EventManager.SetFreeView(!isFreeViewActive);

			}
			GUI.skin = null;
		}
		GUI.skin =menuSkin;
		if(isFreeViewActive){
			GUI.skin.label.fontSize = 38;
			GUI.Label(new Rect(Screen.width/2-350,50, 700, 150), "Free view active\nDrag to rotate\nPress icon again to deactivate.");


		}
		GUI.skin = null;

		//Bomb button
		if(PlayerPrefs.GetInt("BombFrequencyLevel") > 0){		
			if(bombActive){
				GUI.skin = bombActiveSkin;
				if (GUI.Button(new Rect(5,Screen.height-bombTex.height, bombTex.width, bombTex.height),"") && !isFreeViewActive){

					Shooter shooter = Camera.main.GetComponent<Shooter>();
					//Successfully loaded bomb
					if(shooter.loadBomb()){
						bombActive = false;
						bombCounter = GameMaster.upgrades["BombFrequency"];
					}
				}
			}else{
				GUI.skin = bombSkin;

				GUI.Label(new Rect(5,Screen.height-bombTex.height, bombTex.width, bombTex.height), ""+bombCounter);
			}
			GUI.skin = null;
		}
		
		scoreGUI.text = ""+statRecorder.score;
		
		//Move popup upwards while visible
		if(popUpVisible){
			popUpPos += new Vector3(0,0.01f,0);
			transformPopUpToScreen();
		}
		int butWidth = 300;
		int butHeight = 95;
		int verticalOffset = 59;
		if(GameMaster.hasLost){
			//Time.timeScale = 1.0f;
			
			GUI.Label(new Rect(Screen.width/2-lostTex.width/2,Screen.height/2-lostTex.height/2-2*verticalOffset, lostTex.width, lostTex.height), lostTex);
			GUI.skin = menuSkin;			
			if (GUI.Button(new Rect(Screen.width/2-butWidth/2, Screen.height/2-butHeight/2+verticalOffset, butWidth, butHeight), "Restart level")){			
				leaveThisScene(1, GameMaster.levelNr);

			}	

			
			if (GUI.Button(new Rect(Screen.width/2-butWidth/2, Screen.height/2-butHeight/2+3*verticalOffset, butWidth, butHeight), "Menu")){					
				MenuGUI.menuMode = "worldSelection";
				leaveThisScene(0);
			}	
			GUI.skin = null;
		}else if(GameMaster.hasWon){
			GUI.Label(new Rect(Screen.width/2-wonTex.width/2,Screen.height/2-wonTex.height/2, wonTex.width, wonTex.height), wonTex);
		}else if(GameMaster.levelComplete){
			//GUI.Label(new Rect(Screen.width/2-3*butWidth,butHeight, 3* butWidth, 2*butHeight), "Summarize This super level you just completed");

			//Summarize
			GUILayout.BeginArea(new Rect(Screen.width/2-5*butWidth/4,butHeight/2 ,5*butWidth/2,7*butHeight));//(buttonWidth/2, Screen.height/2 -levelButtonHeight/2, levelButtonWidth*10, levelButtonHeight));			
			GUILayout.BeginVertical("box");
			GUILayout.BeginHorizontal();
			GUI.skin = menuSkin;
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			GUI.skin.label.fontSize = 34;
			GUILayout.Label("   Score:               \n"+
			                "   Largest combo:       \n" +
			                "   Shots used:          \n" +
			                "   Total combo:         \n" +
			                "   Total detached:      \n" +
			                "   Total bombed:        \n" +
			                "         \n" +
			                "   Xp gained:           \n",GUILayout.Height(4*butHeight),GUILayout.Width(530));			
			
			GUI.skin.label.alignment = TextAnchor.MiddleRight;
			GUILayout.Label(scoreSummarize["Score"]+"\n"+
			                scoreSummarize["LargestCombo"]+"\n"+
			                scoreSummarize["ShotsUsed"]+"\n"+
			                scoreSummarize["TotalPopped"]+"\n"+
			                scoreSummarize["TotalDetached"]+"\n"+
			                scoreSummarize["TotalExploded"]+"\n"+
			                "         \n" +
			                scoreSummarize["Xp"]+"\n",GUILayout.Height(4*butHeight),GUILayout.Width(100));
			GUILayout.BeginVertical();
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			GUILayout.Space(20);
			if(scoreSummarize["ScoreRecord"] == 1)	GUILayout.Label(newRecordTex,GUILayout.Height(36));	
			else 	GUILayout.Space(36);
			if(scoreSummarize["LargestComboRecord"] == 1)	GUILayout.Label(newRecordTex,GUILayout.Height(36));	
			else 	GUILayout.Space(36);
			if(scoreSummarize["ShotsUsedRecord"] == 1)	GUILayout.Label(newRecordTex,GUILayout.Height(36));	
			else 	GUILayout.Space(36);
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();


			GUILayout.BeginHorizontal();
			int ranking = scoreSummarize["Ranking"];
			GUILayout.Space((5*butWidth/4)-starFull.width*ranking/2);

			for(int i = 0; i<ranking;i++){
				GUILayout.Label(starFull,GUILayout.Height(starFull.width),GUILayout.Width(starFull.height));
			}



			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.EndArea();

			//Do not show button if last level of world
			if(GameMaster.levelNr<=GameMaster.nLevelsInCurrentWorld){
				//Next level button
				if (GUI.Button(new Rect(Screen.width/2-7*butWidth/4, Screen.height-4*butHeight/3, butWidth, butHeight), "Next level")){					

					leaveThisScene(1,GameMaster.levelNr);
				}
			}
			//Replay level button
			if (GUI.Button(new Rect(Screen.width/2-butWidth/2, Screen.height-4*butHeight/3, butWidth, butHeight), "Replay level")){					
				leaveThisScene(1,--GameMaster.levelNr);
			}
			//World selection button
			if (GUI.Button(new Rect(Screen.width/2+3*butWidth/4, Screen.height-4*butHeight/3, butWidth, butHeight), "World Selection")){					

				MenuGUI.menuMode = "worldSelection";
				leaveThisScene(0);
			}
			GUI.skin = null;
		}
		/*else if(GameMaster.worldComplete){
			GUI.Label(new Rect(Screen.width/2-worldCompTex.width/2,Screen.height/2-worldCompTex.height/2, worldCompTex.width, worldCompTex.height), worldCompTex);
		}*/
		//Back button pressed. Send to menu
		else if (Input.GetKeyDown(KeyCode.Escape)) { 
			/*if(escMenuActivated){
				Time.timeScale = 1.0f;
				escMenuActivated = false;
				Debug.Log("Deactivating escmenu");
			}else{*/
				Time.timeScale = 0.0f;
				escMenuActivated = true;
				Debug.Log("activating escmenu");

			
		}
		if(escMenuActivated){
			GUI.skin = menuSkin;
			if (GUI.Button(new Rect(Screen.width/2-butWidth/2, Screen.height/2-2*butHeight/3, butWidth, butHeight), "Continue")){					
				escMenuActivated =false;
				Time.timeScale = 1.0f;
			}
			if (GUI.Button(new Rect(Screen.width/2-butWidth/2, Screen.height/2+2*butHeight/3, butWidth, butHeight), "Menu")){					

				escMenuActivated =false;
				Time.timeScale = 1.0f;
				MenuGUI.menuMode = "worldSelection";
				leaveThisScene(0);
			}
			
			/*if (GUI.Button(new Rect(Screen.width/2-butWidth/2, Screen.height/2+6*butHeight/3, butWidth, butHeight), "Record level")){					
				recordLevel();
			}
			*/
			GUI.skin = null;
		} 




	}

	private void leaveThisScene(int levelToLoad, int gameLevelToLoad=0){

		GameMaster script = gameMaster.GetComponent<GameMaster>();	
		GameMaster.resetLevel();

		if(gameLevelToLoad != 0){
			GameMaster.levelNr = gameLevelToLoad;
			script.startGame();
		}
		Application.LoadLevel(levelToLoad);

	}
	private void recordLevel(){
		Debug.Log ("making level");
		string filepath = "C:/Users/Eivind/Documents/BubbleShooter3D/Assets/Resources/leveldata.xml";
		XmlDocument xmlDoc = new XmlDocument();
		
		if(File.Exists (filepath))
		{
			xmlDoc.Load(filepath);
			
			XmlElement root = xmlDoc.DocumentElement;
			XmlElement level = xmlDoc.CreateElement("level"); 
			
			XmlElement name = xmlDoc.CreateElement("name");
			XmlElement shrinkInterval = xmlDoc.CreateElement("shrinkInterval"); 
			XmlElement nColors = xmlDoc.CreateElement("nColors");
			XmlElement ranking = xmlDoc.CreateElement("rankingTarget");
			
			name.InnerText = "LevelX";
			shrinkInterval.InnerText = "4";
			nColors.InnerText = "4";
			ranking.InnerText = "10";
			
			level.AppendChild(name);
			level.AppendChild(shrinkInterval);
			level.AppendChild(nColors);
			level.AppendChild(ranking);
			float dist = 0.75f;
			int nBalls = 0;

			GameObject[] clusterBalls = GameObject.FindGameObjectsWithTag("BigBall");
			foreach (GameObject go in clusterBalls) {
				if(go.renderer.material.color != Color.black){
					XmlElement ball = xmlDoc.CreateElement("ball");

					float x = go.transform.position.x;
					float y = go.transform.position.y;
					float z = go.transform.position.z;

					ball.SetAttribute("x",x.ToString()); 
					ball.SetAttribute("y",y.ToString()); 
					ball.SetAttribute("z",z.ToString());

					if(go.renderer.material.color ==  Color.grey){
						ball.SetAttribute("type","stone");
					}else {
						ball.SetAttribute("type","randColor");												
					}

					level.AppendChild(ball);
				}
			}


			
			root.AppendChild(level);			
			xmlDoc.Save(filepath); // save file.
		}
	}
	
}
