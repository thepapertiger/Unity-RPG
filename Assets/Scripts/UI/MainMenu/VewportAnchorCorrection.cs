using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VewportAnchorCorrection : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
	}

}
