using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextOutline : MonoBehaviour {


	private List<GUIText> guiCopies= new List<GUIText>{	};

	void Start() {
		GUIText original = this.gameObject.guiText;
		float borderWidth = 0.003f;

		for(int i = 0; i<3;i++){
			for(int j = 0; j<3;j++){
				//if(i!=1&& j!=1){					
					GameObject GUIObj = new GameObject("GUIText");
					GUIText scoreGUI = (GUIText)GUIObj.AddComponent(typeof(GUIText));
					scoreGUI.text = original.text;
					scoreGUI.fontSize= 50; 
					scoreGUI.material.color = Color.black;
					scoreGUI.transform.position = new Vector3(0.99f-borderWidth+(borderWidth*i) , 0.99f-borderWidth+(borderWidth*j), 0.0f);
					scoreGUI.anchor = TextAnchor.UpperRight;
					guiCopies.Add(scoreGUI);
				//}
			}
		}


	}
	void OnGUI () {
		foreach(GUIText item in guiCopies){
			item.text = this.gameObject.guiText.text;
		}
	}

}