/* NAME:            Singleton.cs
 * AUTHOR:          Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION:     This is to be inherited to enforce the singleton pattern. Derived classes should
 * 				    implement 'protected T () []' as an empty constructor to prevent the 'new'
 * 				    keyword from being used. It is abstract.
 * REQUIREMENTS:    Any reference to a Singleton Instance should be made in Start() instead of Awake()
 * 				    to allow all Awakes to finish establishing all Singletons
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
	
	private static T BackingInstance; //the backing variable for singleton pattern
	private static object LockObject = new object ();
    private static bool applicationIsQuitting = false;

    protected virtual void Awake()
    {
        if (BackingInstance == null) {
            BackingInstance = Instance;
            DontDestroyOnLoad(BackingInstance);
        }
        else
            Destroy(BackingInstance);
    }

	public static T Instance {
		get {
			if (applicationIsQuitting) { //do not create another after application quits
				Debug.LogWarning ("[Singleton] Instance '" + typeof(T) +
					"' already destroyed on aplication quit."); 
				return null;
			}
			lock (LockObject) { //use lock to ensure only one thread is accesing this critical code block
				if (BackingInstance == null) {
					
					BackingInstance = (T) FindObjectOfType (typeof(T));

					if (FindObjectsOfType (typeof(T)).Length > 1) { //if there are more than one, error msg
						Debug.LogError ("[Singleton] There are more than one singleton! " +
							"Try reopening the scene.");
						return BackingInstance;
					}

					if (BackingInstance == null) { //if there are none, create singleton
                        Debug.LogError("Singleton was not found");
					}
				}

				return BackingInstance;
			}
		}
	}

	/// <summary>
	/// This is to prevent a buggy ghost
	/// after the singleton is destroyed.
	/// </summary>
	public void OnApplicationQuit()
    {
		applicationIsQuitting = true;
	}
}
