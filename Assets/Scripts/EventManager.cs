﻿using UnityEngine;
using System.Collections;

public class EventManager : MonoBehaviour {

	public delegate void BubbleEventHandler(int combo,int unRooted,int exploded, int points, Vector3 position);
	public static event BubbleEventHandler onBubblesPopped;

	public delegate void FireEventHandler(string name);
	public static event FireEventHandler onObjectFired;

	public delegate void WinEventHandler();
	public static event WinEventHandler onWin;

	public delegate void FreeViewHandler(bool isActive);
	public static event FreeViewHandler onFreeView;

	public delegate void ProjectileResolvedHandler();
	public static event ProjectileResolvedHandler onProjectileResolved;



	public static void BubblesPopped(int combo,int unRooted,int exploded, int points, Vector3 position){
		if(onBubblesPopped != null){
			onBubblesPopped(combo,unRooted,exploded,points,position );
		}
	}

	public static void ObjectFired(string ObjectName){
		if(onObjectFired != null){
			onObjectFired(ObjectName);
		}
	}

	public static void Victory(){
		if(onWin != null){
			onWin();
		}
	}

	public static void SetFreeView(bool isActive){
		if(onFreeView != null){
			onFreeView(isActive);
		}
	}

	public static void ProjectileResolved(){
		if(onProjectileResolved != null){
			onProjectileResolved();
		}
	}

}
