using UnityEngine;
using System.Collections.Generic;

public class ColorManager : MonoBehaviour {
	private static List<Color> colorsAvailable= new List<Color>{
		Color.red,
		Color.blue,
		Color.yellow,
		Color.white, 
		Color.green,
		Color.cyan,
		Color.magenta
	};
	
	
	
	public static List<Color> colors= new List<Color>{
	};
		
	public static void setColors(int nColors){
		colors.Clear();
		for(int i = 0; i<nColors;i++){
			colors.Add(colorsAvailable[i]);
		}

	}
	/*//Should be rewritten to clear color list. Populate with colors found in worldobjects. Done.
	public static void checkAvailableColor(){
		Debug.Log ("checking available color");
		GameObject[] worldObjects = GameObject.FindGameObjectsWithTag("BigBall");
	
		List<Color> tempList = new List<Color>(colors);
		//Finner ut hvilke farger som mangler i verden i forhold til colors-list
		for(int i = 0; i<worldObjects.Length;i++){
			for(int j = 0; j<tempList.Count;j++){
				if(worldObjects[i].renderer.material.color == tempList[j]){
					tempList.RemoveAt(j);
					if(tempList.Count == 0)	break;
				}
			}
		}
		Debug.Log ("tempList"); 
			foreach(Color c in tempList){
			Debug.Log (c.ToString());
			}


		
		//Fjerner manglende farger fra colors-list
		for(int i = 0; i<tempList.Count;i++){
			for(int j = 0; j<colors.Count;j++){
				if(tempList[i]==colors[j])		colors.RemoveAt(j);
			}
		}

		Debug.Log ("colors"); 
		Debug.Log (colors.Count); 
		foreach(Color c in colors){
			Debug.Log (c.ToString());
		}
	
	}
	*/

	public static void checkAvailableColor(){
		GameObject[] worldObjects = GameObject.FindGameObjectsWithTag("BigBall");
		
		colors.Clear();

		bool colorInList;
		//Finner ut hvilke farger som mangler i verden i forhold til colors-list
		for(int i = 0; i<worldObjects.Length;i++){
			colorInList = false;
			Color ballColor = worldObjects[i].renderer.material.color;

			if(ballColor == Color.gray || ballColor == Color.black){

			}else{
				for(int j = 0; j<colors.Count;j++){
					if(ballColor == colors[j]){
						colorInList = true;
						break;
					}
				}
				if(!colorInList)	{
					colors.Add(ballColor);
					//Debug.Log ("adding " + ballColor);
				}
			}
		}

	
	}
	
	public static void removeColor(Color c){
		int index = -1;
		for (int i = 0; i < colors.Count; i++) {
			if(colors[i] == c)	index = i;
		}
		if(index!=-1)			colors.RemoveAt(index);
	}
	
	public static Color getRandColor(){
		if(colors.Count==0){
			return Color.black;
		}
		int numb = Random.Range(0, colors.Count);
		return colors[numb];		
	}
}
