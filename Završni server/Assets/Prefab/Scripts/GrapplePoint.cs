using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePoint : MonoBehaviour
{
    //Essentials
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    private float maxDistance = 100f;
    private SpringJoint joint;

    //Dependencies
    public Transform playerCam;
    public Player player;

    public float jointMaxDistance = 0.8f;
    public float jointMinDistance = 0.25f;

    public float jointSpring = 4.5f;
    public float jointDamper = 7f;
    public float jointMassScale = 4.5f;


    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    public void StartGrapple(Vector3 pos)
    {
        RaycastHit hit;
        if (Physics.Raycast(pos, pos.normalized, out hit, maxDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.transform.position, grapplePoint);

            //The distance grapple will try to keep from grapple point. 
            joint.maxDistance = distanceFromPoint * jointMaxDistance;
            joint.minDistance = distanceFromPoint * jointMinDistance;

            //Adjust these values to fit your game.
            joint.spring = jointSpring;
            joint.damper = jointDamper;
            joint.massScale = jointMassScale;

            currentGrapplePosition = gameObject.transform.position;

            Debug.Log("START");
            ServerSend.PlayerPosition(player);
            ServerSend.PlayerRotation(player);
        }
    }

    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    public void StopGrapple()
    {
        Debug.Log("STOP");
        Destroy(joint);
    }

    Vector3 currentGrapplePosition;
    public float timeMulti = 8f;


    public bool IsGrappling()
    {
        return joint != null;
    }
}
