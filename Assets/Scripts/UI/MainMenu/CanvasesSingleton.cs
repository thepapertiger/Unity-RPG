/* NAME:            CanvasesSingleton.cs
 * AUTHOR:          Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION:     This enables all canvases to be a Singleton so that they are not destroyed on load.
 * REQUIREMENTS:    None
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasesSingleton : Singleton<CanvasesSingleton> {
    private void Awake()
    {
        base.Awake();
    }
}
