using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EllaAnimationCheapFix : MonoBehaviour {

    public Transform EllaModel;
	// Teporary Fix for animation shake
	void LateUpdate () {
        transform.position = EllaModel.position;
	}
}
