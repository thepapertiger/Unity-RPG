/* StatsManager.cs
 * AUTHOR: Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION: This is to be inherited to enforce the singleton pattern. Derived classes should
 * 				implement 'protected T () []' as an empty constructor to prevent the 'new'
 * 				keyword from being used.
 * REQUIREMENTS: None
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
	
	private static T instance; //the static variable for singleton pattern

	private static object lock_object = new object ();

	public static T Instance {
		get {
			if (applicationIsQuitting) { //do not create another after application quits
				Debug.LogWarning ("[Singleton] Instance '" + typeof(T) +
					"' already destroyed on aplication quit."); 
				return null;
			}
			lock (lock_object) { //use lock to ensure only one thread is accesing this critical code block
				if (instance == null) {
					
					instance = (T) FindObjectOfType (typeof(T));

					if (FindObjectsOfType (typeof(T)).Length > 1) { //if there are more than one, error msg
						Debug.LogError ("[Singleton] There are more than one singleton! " +
							"Try reopening the scene.");
						return instance;
					}

					if (instance == null) { //if there are none, create singleton
						GameObject singleton = new GameObject ();
						instance = singleton.AddComponent<T> ();
						singleton.name = "(singleton) " + typeof(T).ToString ();

						DontDestroyOnLoad (singleton);

					}
				}

				return instance;
			}
		}
	}

	private static bool applicationIsQuitting = false;

	/// <summary>
	/// This is to prevent a buggy ghost of the instance
	/// after the singleton is destroyed.
	/// </summary>
	public void OnDestroy() {
		applicationIsQuitting = true;
	}
}
