using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

public class MenuGUI : MonoBehaviour {
	//Game menu
	int buttonHeight = 150;
	int buttonWidth = 400;
	int spacing = 50;
	int worldButtonHeight = 150;
	int worldButtonWidth = 150;
	int worldSpacing = 40;
	int levelButtonHeight = 150;
	int levelButtonWidth = 150;
	int levelSpacing = 40;
	public Texture world1;
	public Texture world2;
	public Texture world3;
	public Texture world4;
	public Texture world5;
	public Texture locked;
	public GUISkin levelSelectBtn;
	public TextAsset levelInfoXml;
	public GameObject backGround;
	public Texture rank1;
	public Texture rank2;
	public Texture rank3;
	List<Texture> textures;

	GameObject gameMaster;
	GameMaster gameMasterScript;


	public static string menuMode = "mainMenu";
	private int worldNumberSelected = 0;
	private string worldSelected = "";
	private string worldDescription = "";
	private int levelNumberSelected = 0;
	private string levelInfo = "";
	private XmlDocument xmlDoc;
	private int nLevels = 0;
	private int levelFontSize = 0;
	
	
	void Start(){
		textures = new List<Texture>{
			world1,
			world2,
			world3,
			world4,
			world5
		};
		/*
		textures = new List<string>{
			"ocean",
			"woods",
			"mountain",
			"sky",
			"space"
		};*/
		levelFontSize = levelSelectBtn.button.fontSize;

		gameMaster = GameObject.FindWithTag("GameMaster");
		gameMasterScript = gameMaster.GetComponent<GameMaster>();


		//XML document
		xmlDoc = new XmlDocument(); // xmlDoc is the new xml document.
		xmlDoc.LoadXml(levelInfoXml.text); // load the file.

		Debug.Log ("Current level of XP : " + PlayerPrefs.GetInt ("Xp"));
		
		
	
	}
	void OnGUI () {
		if(menuMode == "mainMenu"){
			GUILayout.BeginArea(new Rect(Screen.width/2- buttonWidth/2, Screen.height/2 -(buttonHeight*3/2 + spacing), buttonWidth, 800));
				GUI.skin = levelSelectBtn;
				GUI.skin.label.alignment = TextAnchor.MiddleCenter;					
				if(GUILayout.Button("Start game", GUILayout.Height(buttonHeight))) {	
					menuMode = "worldSelection";
				}				
				GUILayout.Space(spacing);
							
				if(GUILayout.Button("Upgrade powerups", GUILayout.Height(buttonHeight))) {
					menuMode = "upgradeScreen";
				}
				GUILayout.Space(spacing);

				if(GUILayout.Button("Quit", GUILayout.Height(buttonHeight))){
					Application.Quit();
				}		
								
			GUILayout.EndArea();		
		}



		else if(menuMode == "worldSelection"){			
			GUILayout.BeginArea(new Rect(Screen.width/2-(5*worldButtonWidth+4*worldSpacing)/2,Screen.height/3 -worldButtonHeight/2,worldButtonWidth*5 + 4*(worldSpacing+5),worldButtonHeight + 10));		
				GUILayout.BeginHorizontal();
			
					for(int i=1;i<=5;i++){
						//Show locked symbol if not unlocked
						if(PlayerPrefs.GetInt("isWorldUnlocked"+i)!=1 && i!=1)	GUILayout.Button(locked, GUILayout.Height(worldButtonHeight),GUILayout.Width(worldButtonWidth));
									
						else if(GUILayout.Button(textures[i-1], GUILayout.Height(worldButtonHeight),GUILayout.Width(worldButtonWidth))){							 					
							worldNumberSelected = i;
							setupLevelSelection(worldNumberSelected);
							
						}
				GUILayout.Space(worldSpacing);				
					}
				
				GUILayout.EndHorizontal();
			GUILayout.EndArea();
			GUI.skin = levelSelectBtn;
			//If levelbutton active -> show levelInfo
			if(worldNumberSelected !=0){
				GUILayout.BeginArea(new Rect(Screen.width/2-3*worldButtonWidth/2,Screen.height/2 -40 ,worldButtonWidth*3,3*worldButtonHeight/2));//(buttonWidth/2, Screen.height/2 -levelButtonHeight/2, levelButtonWidth*10, levelButtonHeight));			
					GUILayout.BeginHorizontal("box");
					
					GUI.skin.label.alignment = TextAnchor.MiddleCenter;
				GUILayout.Label(worldDescription,GUILayout.Height(worldButtonHeight+20),GUILayout.Width(worldButtonWidth*3-10));			
											
					GUILayout.EndHorizontal();
				GUILayout.EndArea();

				GUILayout.BeginArea(new Rect(Screen.width/2-worldButtonWidth,Screen.height/2 + worldButtonHeight,worldButtonWidth*2,worldButtonHeight));//(buttonWidth/2, Screen.height/2 -levelButtonHeight/2, levelButtonWidth*10, levelButtonHeight));			
					GUILayout.BeginHorizontal();									
					if(GUILayout.Button("Enter world", GUILayout.Height(worldButtonHeight/2),GUILayout.Width(worldButtonWidth*2))){
						//Set background
						backGround.renderer.material.mainTexture = textures[worldNumberSelected-1];
						GameMaster.backGroundTex = 	textures[worldNumberSelected-1];
						menuMode = "levelSelection";
							
					}
					GUILayout.EndHorizontal();
				GUILayout.EndArea();

			}

			GUILayout.BeginArea(new Rect(Screen.width/2-worldButtonWidth,Screen.height/2 + 3*worldButtonHeight/2+20,worldButtonWidth*2,worldButtonHeight));//(buttonWidth/2, Screen.height/2 -levelButtonHeight/2, levelButtonWidth*10, levelButtonHeight));			
				GUILayout.BeginHorizontal();									
			if(GUILayout.Button("Main menu", GUILayout.Height(worldButtonHeight/2),GUILayout.Width(worldButtonWidth*2))) 			menuMode = "mainMenu";
				GUILayout.EndHorizontal();
			GUILayout.EndArea();
			GUI.skin = null;
		}

		else if(menuMode == "levelSelection"){	
			int areaWidth = levelButtonWidth*nLevels + (nLevels-1)*(levelSpacing+5);
			GUILayout.BeginArea(new Rect(Screen.width/2-areaWidth/2,Screen.height/3 -levelButtonHeight/2,areaWidth,levelButtonHeight + 10));		
			GUILayout.BeginHorizontal();
			GUI.skin = levelSelectBtn;
			GUI.skin.button.fontSize = levelFontSize*2;
			string prefix = worldSelected + "Level";
			
			GUIContent content = new GUIContent();
			for(int i=1;i<=nLevels;i++){
				content.image = null;
				content.text = null;
				int ranking = PlayerPrefs.GetInt (prefix+i+"Ranking");
				if(ranking == 3) content.image = rank3;
				else if(ranking == 2) content.image = rank2;
				else if(ranking == 1) content.image = rank1;
				else 				content.text = ""+ i;
				//content.image = (Texture2D)testImage;
				//Show locked symbol if not unlocked
				if(PlayerPrefs.GetInt(prefix)<i && i!=1)	GUILayout.Button(locked, GUILayout.Height(levelButtonHeight),GUILayout.Width(levelButtonWidth));

				else if(GUILayout.Button(content, GUILayout.Height(levelButtonHeight),GUILayout.Width(levelButtonWidth))){
					levelNumberSelected = i;
					setLevelInfo(i);					
				}


				GUILayout.Space(levelSpacing);
			}
			GUI.skin.button.fontSize = levelFontSize;
			//GUI.skin.button.fontSize = GUI.skin.button.fontSize/2;
			GUILayout.EndHorizontal();
			GUILayout.EndArea();

			//If levelbutton active -> show levelInfo
			if(levelNumberSelected !=0){
				GUILayout.BeginArea(new Rect(Screen.width/2-3*worldButtonWidth/2,Screen.height/2 -20 ,worldButtonWidth*3,3*worldButtonHeight/2));
				GUILayout.BeginHorizontal("box");
					
					GUI.skin.label.alignment = TextAnchor.MiddleLeft;
					GUILayout.Label(" Level nr:          \n"+
					                " Best score:        \n" +
					                " Least shots:       \n",GUILayout.Height(worldButtonHeight),GUILayout.Width(worldButtonWidth*3/2));			
					
					GUI.skin.label.alignment = TextAnchor.MiddleRight;
					GUILayout.Label(levelInfo,GUILayout.Height(worldButtonHeight),GUILayout.Width(worldButtonWidth*22/16));							
					GUILayout.EndHorizontal();
				GUILayout.EndArea();
				
				GUILayout.BeginArea(new Rect(Screen.width/2-worldButtonWidth,Screen.height/2 + worldButtonHeight,worldButtonWidth*2,worldButtonHeight));//(buttonWidth/2, Screen.height/2 -levelButtonHeight/2, levelButtonWidth*10, levelButtonHeight));			
				GUILayout.BeginHorizontal();									
				if(GUILayout.Button("Play level", GUILayout.Height(worldButtonHeight/2),GUILayout.Width(worldButtonWidth*2))){
					launchGame(levelNumberSelected);
					menuMode = "mainMenu";
				}
				GUILayout.EndHorizontal();
				GUILayout.EndArea();
				
			}
			GUILayout.BeginArea(new Rect(Screen.width/2-worldButtonWidth,Screen.height/2 + 3*worldButtonHeight/2+20,worldButtonWidth*2,worldButtonHeight));//(buttonWidth/2, Screen.height/2 -levelButtonHeight/2, levelButtonWidth*10, levelButtonHeight));			
				GUILayout.BeginHorizontal();									
					if(GUILayout.Button("Menu", GUILayout.Height(worldButtonHeight/2),GUILayout.Width(worldButtonWidth*2))) 			menuMode = "worldSelection";
				GUILayout.EndHorizontal();
			GUILayout.EndArea();
			GUI.skin = null;
		}
		else if(menuMode == "upgradeScreen"){	
			int areaWidth = 1040;
			int areaHeight = 600;
			GUI.skin.label.fontSize = 46;

			GUILayout.BeginArea(new Rect(Screen.width/2-areaWidth/2,30 ,areaWidth,areaHeight));		
				GUILayout.BeginVertical("box");
					//Categories
					GUILayout.BeginHorizontal();
						GUI.skin.label.normal.textColor = Color.yellow;
						GUILayout.Label("Research",GUILayout.Width(areaWidth*5/10));
						GUI.skin.label.alignment = TextAnchor.MiddleCenter;
						GUILayout.Label("Cost",GUILayout.Width(areaWidth*2/10));
									
						GUILayout.Label("Lvl",GUILayout.Width(areaWidth*1/10));
						GUI.skin.label.alignment = TextAnchor.MiddleLeft;
						GUI.skin.label.normal.textColor = Color.white;
					GUILayout.EndHorizontal();	
					
					
					insertUpgradeItem("Bomb frequency", "BombFrequency", areaWidth);
					if(PlayerPrefs.GetInt("BombFrequencyLevel") > 0)	insertUpgradeItem("Bomb size", "BombSize", areaWidth);
					insertUpgradeItem("Free view frequency", "FreeView", areaWidth);
					insertUpgradeItem("Magazined balls", "ExtraBall", areaWidth);
			GUILayout.Space(GUI.skin.label.fontSize);
			GUILayout.BeginHorizontal();

			GUILayout.Label("Xp to spend",GUILayout.Width(areaWidth*6/10));	
			GUILayout.Label(PlayerPrefs.GetInt("Xp").ToString(),GUILayout.Width(areaWidth*1/10));
			GUILayout.EndHorizontal();	
				
				GUILayout.EndVertical();
			GUILayout.EndArea();

			GUI.skin = levelSelectBtn;
			GUILayout.BeginArea(new Rect(Screen.width/2-worldButtonWidth,Screen.height/2 + 3*worldButtonHeight/2+20,worldButtonWidth*2,worldButtonHeight));			
				GUILayout.BeginHorizontal();									
					if(GUILayout.Button("Main menu", GUILayout.Height(worldButtonHeight/2),GUILayout.Width(worldButtonWidth*2))) 			menuMode = "mainMenu";
				GUILayout.EndHorizontal();
			GUILayout.EndArea();
			GUI.skin = null;
		}
	}
	
	void insertUpgradeItem(string nameOfUpgrade, string lookUpstring, int width){

		GUILayout.BeginHorizontal();
		GUILayout.Label(nameOfUpgrade,GUILayout.Width(width*5/10));	
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		int cost = GameMaster.upgrades[lookUpstring+"Cost"];

		int currentLevel = PlayerPrefs.GetInt(lookUpstring+"Level");
		int maxLevel = GameMaster.upgrades[lookUpstring+"MaxLevel"];


		if(currentLevel >= maxLevel){
			GUILayout.Label("",GUILayout.Width(width*2/10));
			GUILayout.Label("MAX",GUILayout.Width(width*1/10));
		}else{
			cost *= (currentLevel+1);
			GUILayout.Label(cost.ToString(),GUILayout.Width(width*2/10));
			GUILayout.Label(currentLevel.ToString(),GUILayout.Width(width*1/10));
			GUI.skin = levelSelectBtn;
			if(GUILayout.Button("Upgrade", GUILayout.Width(width*5/30),GUILayout.Height(GUI.skin.button.fontSize+12))){
				int currentXp = PlayerPrefs.GetInt("Xp");
				int saldo = currentXp-cost;
				if(saldo >=0){
					PlayerPrefs.SetInt(lookUpstring+"Level", ++currentLevel);
					PlayerPrefs.SetInt("Xp", saldo);
				}

			}
			GUI.skin = null;
		}
		GUI.skin.label.alignment = TextAnchor.MiddleLeft;
		GUILayout.EndHorizontal();

	}
	void launchGame(int level){
		GameMaster.world = worldSelected;	
		GameMaster.worldNr = worldNumberSelected;
		GameMaster.levelNr = level;
		GameMaster.nLevelsInCurrentWorld = nLevels;
		gameMasterScript.startGame();
		Application.LoadLevel(1);
	}

	private void setupLevelSelection(int worldNumber){
		//Get world and it's name
		XmlNode world = xmlDoc.SelectSingleNode("/levelInfo/world["+worldNumber+"]");

		worldSelected = world.Attributes["name"].Value;

		//Count number of levels in selected world
		XmlNodeList levels = world.SelectNodes("level");
		nLevels = levels.Count;

		worldDescription = world.SelectSingleNode ("description").InnerText;

		//Formula = 500 for total space. Spacing are one third of each button
		levelButtonWidth = 1000/(nLevels+((nLevels-1)/8));

		if(levelButtonWidth > worldButtonHeight)	levelButtonWidth = worldButtonHeight;
		levelButtonHeight = levelButtonWidth;
		levelSpacing = levelButtonWidth/8;

	}

	private void setLevelInfo(int levelNr){
		levelInfo = 
			levelNr+"\n" +
				PlayerPrefs.GetInt(worldSelected +"Level"+levelNr+"Score")+"\n"; 
		int shotsUsed = PlayerPrefs.GetInt(worldSelected +"Level"+levelNr+"ShotsUsed");
		if(shotsUsed ==0)
					levelInfo+="N/A\n";
		else 		levelInfo+=shotsUsed+"\n" ;
	}
}
