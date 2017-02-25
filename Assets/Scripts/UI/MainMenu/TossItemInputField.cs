using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TossItemInputField : MonoBehaviour {

    InputField MyInputField;

    private void OnDisable()
    {
        if (!MyInputField)
            MyInputField = GetComponent<InputField>();
        MyInputField.text = "1";
    }
}
