using UnityEngine;
using System.Collections.Generic;

public class MenuAnimation : MonoBehaviour {
	public Transform projectile;
	private Transform ball = null;
	private int spread;
	private int frameCounter;

	private static List<Color> colorsAvailable= new List<Color>{
		Color.red,
		Color.blue,
		Color.yellow,
		Color.white, 
		Color.green,
		Color.cyan,
		Color.magenta
	};
	//

	private static List<Transform> ballsInFlight= new List<Transform>{
	};
	// Use this for initialization
	void Start () {
		spread = 10;
		frameCounter = 0;
		ballsInFlight.Clear();
	}
	
	// Update is called once per frame
	void Update () {

		if(frameCounter%80 == 0 || frameCounter==0)	makeBall ();
		frameCounter++;
	}

	void makeBall(){
		//Make new magazined ball
		ball = Instantiate(projectile, transform.position, transform.rotation) as Transform;
		ball.renderer.material.shader = Shader.Find("Parallax Specular");
		int numb = Random.Range(0, colorsAvailable.Count);
		ball.renderer.material.color = colorsAvailable[numb];
		ball.transform.parent = this.transform;
		ball.transform.localPosition = randomPosition(-spread,spread,-spread,spread,-25);
		Vector3 target = randomPosition(-spread*4,spread*4,-spread*4,spread*4,60);

		Vector3 vel = target - ball.transform.localPosition;
		vel.Normalize();
		vel *=Random.Range(4, 6);
		ball.rigidbody.velocity = vel;

		ballsInFlight.Add(ball);
		if(ballsInFlight.Count > 50){
			GameObject temp = ballsInFlight[0].gameObject;
			ballsInFlight.RemoveAt (0);
			Destroy(temp);
		}
	}

	Vector3 randomPosition(int xFrom, int xTo, int yFrom, int yTo, int z){
		return new Vector3(Random.Range(xFrom, xTo),Random.Range(yFrom, yTo),z);
	}
}
