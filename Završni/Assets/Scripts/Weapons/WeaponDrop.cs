using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDrop : MonoBehaviour
{
    //public WeaponPickup weaponPickup;
    public GameObject playerObject;

    public string playerTag;

    public void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.gameObject.tag == playerTag)
        {
            playerObject = collision.collider.gameObject;
            //weaponPickup.gameObject.GetComponent<IPickup>().Pickup();
            Destroy(gameObject);
        }
    }



}
