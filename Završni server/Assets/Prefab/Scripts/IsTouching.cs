using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsTouching : MonoBehaviour
{
    public bool isTouching = false;
    public BoxCollider boxCollider;

    private void Awake()
    {
        boxCollider.GetComponent<BoxCollider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("TEST");
        isTouching = true;
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("TEST");
        isTouching = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isTouching = false;
    }
}
