using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

public class LevelXmlSanitizer : MonoBehaviour {
	public TextAsset levelInfoXml;
	private XmlDocument xmlDoc;
	// Use this for initialization
	void Start () {

		xmlDoc = new XmlDocument(); // xmlDoc is the new xml document.
		xmlDoc.LoadXml(levelInfoXml.text); // load the file.

		roundValues();
		removeDuplicateBubbles();
		
		xmlDoc.Save("C:/Users/Eivind/Documents/BubbleShooter3D/Assets/Resources/leveldata.xml");


	}

	void roundValues(){

		XmlNodeList balls = xmlDoc.SelectNodes("//ball");
		Debug.Log("length of list " + balls.Count);
		
		Debug.Log("Starting sanitizer");
		foreach (XmlNode ball in balls)
		{
			XmlNode parent = ball.ParentNode;
			Debug.Log ("Checking "+parent.SelectSingleNode("name").InnerText);
			XmlElement elBall = (XmlElement)ball;

			checkValue("x", elBall);
			checkValue("y", elBall);
			checkValue("z", elBall);

			
		}


	}

	void checkValue(string valueName, XmlElement ball){
		float tempValue;
		float.TryParse(ball.Attributes[valueName].Value, out tempValue);

		if(tempValue%0.05!=0){
			Debug.Log("Old value were "+tempValue);
			tempValue = Mathf.Round(tempValue * 20) / 20;
			ball.SetAttribute(valueName,tempValue.ToString()); 
			Debug.Log("New value is "+tempValue);
		}
	}
	void removeDuplicateBubbles(){
		XmlNodeList levels = xmlDoc.SelectNodes("//level");


		foreach (XmlNode level in levels)
		{
			XmlNodeList balls = level.SelectNodes("ball");
			Debug.Log ("Sanitizing level");
			
			float[,] ballsArray = new float[balls.Count,3];
			int counter = 0;
			foreach (XmlNode ball in balls)
			{
				XmlElement elBball = (XmlElement)ball;

				float x;
				float.TryParse(elBball.Attributes["x"].Value, out x);
				float y;
				float.TryParse(elBball.Attributes["y"].Value, out y);
				float z;
				float.TryParse(elBball.Attributes["z"].Value, out z);




				bool foundDuplicate = false;
				if(counter != 0){
					for(int i=counter-1; i>=0;i--){
						if(ballsArray[i,0] == x && ballsArray[i,1] == y && ballsArray[i,2] == z){
							Debug.Log ("Found duplicate at nr "+ i +" and "+ counter);
							XmlNode parent = ball.ParentNode;
							foundDuplicate = true;
							Debug.Log (parent.SelectSingleNode("name").InnerText);
							parent.RemoveChild(ball);
							break;

						}
					}
				}

				if(foundDuplicate){
					
				}else{
					ballsArray[counter,0] = x;
					ballsArray[counter,1] = y;
					ballsArray[counter,2] = z;
				}
				counter++;
			}


			
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
