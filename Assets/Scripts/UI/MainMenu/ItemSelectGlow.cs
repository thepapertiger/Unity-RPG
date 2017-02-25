/* NAME:            SetItemClickable.cs
 * AUTHOR:          Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION:     This allows the item glow to follow the image it surrounds as it scrolls.
 * REQUIREMENTS:    None.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSelectGlow : MonoBehaviour {

    public Transform MySelectedItem;

	void Update () {
        transform.position = MySelectedItem.position;
	}
}
