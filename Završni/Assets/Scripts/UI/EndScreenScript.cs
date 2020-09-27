using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndScreenScript : MonoBehaviour
{
    public TextMeshProUGUI endScreenText;
    public string serverMessage;

    private void Awake()
    {
        endScreenText.GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        endScreenText.text = serverMessage;
    }
}
