using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

public class LevelStatRecorder : MonoBehaviour {
	public int score;
	public int shotsUsed;
	private int largestCombo;
	private int nPopped;
	private int nDetached;
	private int nExploded;
	private int nShotBubbles;
	private int nShotBombs;
	private int recordCount;
	private string levelPrefix;
	private Dictionary<string, int> tempDict;

	public TextAsset levelInfoXml;

	// Use this for initialization
	void Start () {


		score = 0;
		shotsUsed = 0;
		largestCombo = 0;

		nPopped = 0;
		nDetached = 0;
		nExploded = 0;
		nShotBubbles = 0;
		nShotBombs = 0;

		recordCount = 0;
		levelPrefix = GameMaster.world+"Level"+(GameMaster.levelNr);

		EventManager.onBubblesPopped += bubblesPopped;
		EventManager.onWin += recordStats;
		EventManager.onObjectFired += recordShot;
	}

	void OnDisable(){
		EventManager.onBubblesPopped -= bubblesPopped;
		EventManager.onWin -= recordStats;
		EventManager.onObjectFired -= recordShot;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	private void recordShot(string objectName){
		if(objectName == "Bomb")	nShotBombs++;
			
		else 						nShotBubbles++;

		shotsUsed ++;
	}

	private void recordStats(){
		tempDict = new Dictionary<string, int>();
		tempDict.Add("Score", score);
		tempDict.Add("LargestCombo", largestCombo);
		tempDict.Add("ShotsUsed", shotsUsed);
		tempDict.Add("TotalPopped", nPopped);
		tempDict.Add("TotalDetached", nDetached);
		tempDict.Add("TotalExploded", nExploded);

		isNewRecord("Score",score, true);
		isNewRecord("LargestCombo",largestCombo, true);
		isNewRecord("ShotsUsed",shotsUsed, false);

		int timesPlayed = PlayerPrefs.GetInt(levelPrefix+"TimesPlayed");
		timesPlayed++;
		PlayerPrefs.SetInt(levelPrefix+"TimesPlayed", timesPlayed);


		int rankingFact = 3;
		//int difficultyFact = 4;
		int worldDifficultyFact = 5;
		int recordFact = 2;
		int timesPlayedDivisiveFact = 2;
		int ranking = determineRanking();
		int difficulty = 1;

		tempDict.Add("Ranking", ranking);
		isNewRecord("Ranking",ranking, true);
		int xp = Mathf.FloorToInt (			(ranking *rankingFact+(worldDifficultyFact*GameMaster.worldNr +( GameMaster.levelNr-1))+recordCount * recordFact)/	((timesPlayed+1)/timesPlayedDivisiveFact));

		tempDict.Add("Xp", xp);
		PlayerPrefs.SetInt("Xp", (PlayerPrefs.GetInt("Xp")+ xp));



		GameGUI gameGUIScript = gameObject.GetComponent<GameGUI>();
		gameGUIScript.scoreSummarize = tempDict;

	}

	private void isNewRecord(string statString, int statToCheck,  bool largeIsBest = true){
		string stringToCheck = levelPrefix + statString;
		int isRecord = 0;
		if(largeIsBest){
			if(PlayerPrefs.GetInt(stringToCheck)<statToCheck)		isRecord = 1;
		}else{
			int recordedRecord = PlayerPrefs.GetInt(stringToCheck);
			Debug.Log ("Old record "+recordedRecord);
			if(recordedRecord==0 || recordedRecord>statToCheck)		isRecord = 1;
		}

		if(isRecord ==1){
			PlayerPrefs.SetInt(stringToCheck, statToCheck);
			recordCount++;
		}

		tempDict.Add(statString+"Record", isRecord);
	}

	private int determineRanking(){
		XmlDocument xmlDoc = new XmlDocument(); // xmlDoc is the new xml document.
		xmlDoc.LoadXml(levelInfoXml.text); // load the file.

		XmlNode world = xmlDoc.SelectSingleNode("/levelInfo/world[@name= '"+GameMaster.world+"']");
		XmlNode level = world.SelectSingleNode("level["+(GameMaster.levelNr-1)+"]");
		XmlNode rankingText = level.SelectSingleNode("rankingTarget/text()");
		int ranking =0;
		int highestRank;
		int.TryParse(rankingText.Value, out highestRank);

		int mediumRank = highestRank +(GameMaster.levelNr);
		int lowerstRank = highestRank +(2*GameMaster.levelNr);

		if(shotsUsed<=highestRank)	ranking = 3;
		else if(shotsUsed<=mediumRank)	ranking = 2;
		else if(shotsUsed<=lowerstRank)	ranking = 1;

		return ranking;
	}

	private void bubblesPopped(int combo,int unRooted,int exploded, int points, Vector3 position){

		score += points;

		nPopped += combo;
		nDetached += unRooted;
		nExploded += exploded;

		if(combo > largestCombo)	largestCombo = combo;
	}

}
