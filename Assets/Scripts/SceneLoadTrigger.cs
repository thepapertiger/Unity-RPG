using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadTrigger : MonoBehaviour {

    public string LoadName;

    private void OnTriggerEnter(Collider other)
    {
        if (LoadName != "")
            GameManager.Instance.LoadScene(LoadName);
    }
}
