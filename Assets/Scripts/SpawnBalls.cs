using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;


public class SpawnBalls : MonoBehaviour {
	public Transform projectile;
	public Transform perimeter;
	public Texture greyTex;
	public TextAsset levelInfoXml;
	
	
	void Start () {
		
	}
	
	public void initiateLevel(){
		//Screen.showCursor = false;
		//Debug.Log(GameMaster.score+ " " +GameMaster.levelNr +" "+GameMaster.nColors+ " "+GameMaster.greyModulus+ " "+GameMaster.missToShrinkPer);
		Transform blackBall;
		blackBall = Instantiate(projectile, transform.position, transform.rotation) as Transform;
		blackBall.renderer.material.color = new Color(0.0F, 0.0F, 0.0F, 1.0F);
		blackBall.transform.position = new Vector3(0,0,0);
		blackBall.name = "BlackBall";
		
		Transform perimiterBall;
		perimiterBall = Instantiate(perimeter, transform.position, transform.rotation) as Transform;
		perimiterBall.renderer.material.color = new Color(0.5F, 0.0F, 0.0F, 0.4F);
		perimiterBall.transform.position = new Vector3(0,0,0);
		perimiterBall.name = "PerimiterBall";
		
		int nBalls=0;

		XmlDocument xmlDoc = new XmlDocument(); // xmlDoc is the new xml document.
		xmlDoc.LoadXml(levelInfoXml.text); // load the file.

		XmlNode world = xmlDoc.SelectSingleNode("/levelInfo/world[@name= '"+GameMaster.world+"']");

		XmlNode levelInfo = world.SelectSingleNode("level["+GameMaster.levelNr+"]");

		XmlElement levelInfoElement = (XmlElement)levelInfo;			
		XmlNodeList balls = levelInfoElement.GetElementsByTagName("ball");
			
		foreach (XmlNode ball in balls)
		{

			nBalls++;
			Transform bigBall;
			bigBall = Instantiate(projectile, transform.position, transform.rotation) as Transform;
			bigBall.tag = "BigBall";

			ProjectileController bigBallScript = bigBall.gameObject.GetComponent<ProjectileController>();
			bigBallScript.blackBall = blackBall.gameObject;
			string type = ball.Attributes["type"].Value;

			if(type=="stone"){
				bigBall.renderer.material.color =  Color.grey;
				bigBall.renderer.material.mainTexture = greyTex;
			}else
				bigBall.renderer.material.color =  ColorManager.getRandColor();							


			float x;
			float.TryParse(ball.Attributes["x"].Value, out x);
			float y;
			float.TryParse(ball.Attributes["y"].Value, out y);
			float z;
			float.TryParse(ball.Attributes["z"].Value, out z);

			bigBall.transform.position = new Vector3(x,y,z);

			
		}
			

		
		/*float dist = 1.5f;
		for(int i = 0; i <5; i++){
			for(int j = 0; j <5; j++){
				for(int k = 0;k <5; k++){
					if(!(i==2 && j==2 && k==2)){
						nBalls++;
						Transform bigBall;
						bigBall = Instantiate(projectile, transform.position, transform.rotation) as Transform;
						if(nBalls%GameMaster.greyModulus==0){
							bigBall.renderer.material.color =  Color.grey;
							bigBall.renderer.material.mainTexture = greyTex;
						}else
							bigBall.renderer.material.color =  ColorManager.getRandColor();						
						bigBall.tag = "BigBall";
						bigBall.transform.position = new Vector3(-3+(i*dist),-3+(j*dist),-3+(k*dist));

					}
				}
			}
		}
		int nInACircle = 7;
		float thisRad = 0f;
		float rad = 0f;

		float dist = 1.5f;
		float zdist = dist/2;
		float offset = 0.0f;
		int nRounds = 0;
		int shrinkage = 0;
		for(int k = 1; k <9; k++){
			thisRad = 0f;
			if(k%2==0){
				offset = 0f;
				zdist *= -1;

			}else {
				shrinkage += 2;
				offset =0f;
				zdist = k*dist/2;
				nRounds ++;
			}
			nInACircle = 6-shrinkage;

			for(int i = 1; i <5-nRounds; i++){

				rad = 2*Mathf.PI/nInACircle;
				Debug.Log("Start of  round");
				for(int j = 0; j <nInACircle; j++){

					thisRad = rad*j;

					nBalls++;
					Transform bigBall;
					bigBall = Instantiate(projectile, transform.position, transform.rotation) as Transform;

					bigBall.renderer.material.color =  ColorManager.getRandColor();						
					bigBall.tag = "BigBall";
					Debug.Log("This rad: " + thisRad +" Sin: " +Mathf.Sin (thisRad) + "  Cos: " + Mathf.Cos(thisRad));
					bigBall.transform.position = new Vector3(Mathf.Cos(thisRad)*(i*dist)-offset,Mathf.Sin (thisRad)*(i*dist)-offset,zdist);
				
				}
				nInACircle += 5;
			}
		}*/
		Debug.Log("Number of balls " +nBalls);
		//GetLevel();
	}
		
/*
	private void GetLevel()
	{
		XmlDocument xmlDoc = new XmlDocument(); // xmlDoc is the new xml document.
		xmlDoc.LoadXml(GameAsset.text); // load the file.
		XmlNodeList levelsList = xmlDoc.GetElementsByTagName("level"); // array of the level nodes.
		
		foreach (XmlNode levelInfo in levelsList)
		{
			XmlNodeList levelcontent = levelInfo.ChildNodes;
			obj = new Dictionary<string,string>(); // Create a object(Dictionary) to colect the both nodes inside the level node and then put into levels[] array.
			
			foreach (XmlNode levelsItens in levelcontent) // levels itens nodes.
			{
				if(levelsItens.Name == "name")
				{
					obj.Add("name",levelsItens.InnerText); // put this in the dictionary.
				}
				
				if(levelsItens.Name == "tutorial")
				{
					obj.Add("tutorial",levelsItens.InnerText); // put this in the dictionary.
				}
				
				if(levelsItens.Name == "object")
				{
					switch(levelsItens.Attributes["name"].Value)
					{
					case "Cube": obj.Add("Cube",levelsItens.InnerText);break; // put this in the dictionary.
					case "Cylinder":obj.Add("Cylinder",levelsItens.InnerText); break; // put this in the dictionary.
					case "Capsule":obj.Add("Capsule",levelsItens.InnerText); break; // put this in the dictionary.
					case "Sphere": obj.Add("Sphere",levelsItens.InnerText);break; // put this in the dictionary.
					}
				}
				
				if(levelsItens.Name == "finaltext")
				{
					obj.Add("finaltext",levelsItens.InnerText); // put this in the dictionary.
				}
			}
			levels.Add(obj); // add whole obj dictionary in the levels[].
		}
	}

*/
}
