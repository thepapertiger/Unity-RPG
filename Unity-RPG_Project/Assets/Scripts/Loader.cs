using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {

	public GameObject gameManager; //reference to the GameManager object

	void Awake () { //called when this script component starts
		//Note this is the static variable since a GameManager is a class, not a reference
		if (GameManager.instance == null) //if there is no GameManager currently,
			Instantiate(gameManager); //instantiate it
	}
}
