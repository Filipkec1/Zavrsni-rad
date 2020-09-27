using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePoint : MonoBehaviour
{
    //Essentials
    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    private float maxDistance = 100f;
    private SpringJoint joint;

    //Dependencies
    public Transform playerCam;
    public Transform player;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (Input.GetButton("Grapple"))
        {
            Debug.Log("TEST");

            StartGrapple();
            ClientSend.PlayerGrappleStart(playerCam.position);
        }
        else if (Input.GetButtonUp("Grapple"))
        {
            Debug.Log("STOP");

            StopGrapple();
            ClientSend.PlayerGrappleEnd();
        }
    }

    //Called after Update
    void LateUpdate()
    {
        DrawRope();
    }

    public float jointMaxDistance = 0.8f;
    public float jointMinDistance = 0.25f;

    public float jointSpring = 4.5f;
    public float jointDamper = 7f;
    public float jointMassScale = 4.5f;


    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCam.position, playerCam.forward, out hit, maxDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            //The distance grapple will try to keep from grapple point. 
            joint.maxDistance = distanceFromPoint * jointMaxDistance;
            joint.minDistance = distanceFromPoint * jointMinDistance;

            //Adjust these values to fit your game.
            joint.spring = jointSpring;
            joint.damper = jointDamper;
            joint.massScale = jointMassScale;

            lr.positionCount = 2;
            currentGrapplePosition = gameObject.transform.position;
        }
    }

    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    void StopGrapple()
    {
        lr.positionCount = 0;
        Destroy(joint);
    }

    Vector3 currentGrapplePosition;
    public float timeMulti = 8f;

    void DrawRope()
    {
        //If not grappling, don't draw rope
        if (!joint)
        {
            return;
        }

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * timeMulti);

        lr.SetPosition(0, gameObject.transform.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    public bool IsGrappling()
    {
        return joint != null;
    }
}
