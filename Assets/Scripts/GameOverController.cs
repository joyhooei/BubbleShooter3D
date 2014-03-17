using UnityEngine;
using System.Collections;

public class GameOverController : MonoBehaviour {

	static public void lose() {
	Debug.Log("You lost");
	}
	
	static public void win() {
	Debug.Log("You won");
	}
}
