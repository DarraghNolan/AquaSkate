﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerMotion : MonoBehaviour
{
    [SerializeField] string horizontalAxis, verticalAxis, jumpAxis, accelerateAxis, brakeAxis;
    [SerializeField] Image accelerometerBar;

    Rigidbody pRB;
    [SerializeField] Vector3 appliedForce;
    [SerializeField] float maxSpeed, maxForce, accelerationForce, brakeForce, xZPlaneSpeed, jumpForce, turnSpeed,turnScaler;
    [SerializeField] bool isGrounded;
    [SerializeField] Transform groundTransform, slopeTransform;

    [SerializeField] Vector3 rampNormal, rampHitPoint;

    //Testing Vectors for visualisation
    [SerializeField] Vector3 testVector, playerPlane, hitNormaltoPlane, targetDir;

    Ray groundCheckRay;
    Ray rampRay;
    private void Awake()
    {
        pRB = this.gameObject.GetComponent<Rigidbody>();
        pRB.drag = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GroundCheck();
        //SlopeCheck();
        groundCheckRay = new Ray(groundTransform.position, -transform.up);
        rampRay = new Ray(slopeTransform.position, transform.forward);
        
        Motion();

        
    }
    
    void ScaleAccelerometer(float currentVelocity, float maxVelocity)
    {
        accelerometerBar.fillAmount = currentVelocity / maxVelocity;
    }

    void GroundCheck()
    {
        RaycastHit rH;
        if (Physics.Raycast(groundTransform.position, -transform.up, out rH, 0.5f))
        {
            if (rH.collider.gameObject.tag == "Ground")
            {
                 isGrounded =true;
            }
        }
        else
        {
            isGrounded = false;
        }
    }

    void SlopeCheck()
    {
        RaycastHit rH;
        if (Physics.Raycast(rampRay, out rH, 100f))
        {
            if (rH.collider.tag == "Ground")
            {
                rampNormal= rH.normal;
                rampHitPoint = rH.point;
                float angle = Vector3.Angle(Vector3.forward, rampNormal);

                playerPlane = new Vector3(slopeTransform.position.x, slopeTransform.position.y, slopeTransform.position.z);
                hitNormaltoPlane = new Vector3(rampNormal.x, 0, rampNormal.z) + rampHitPoint;
                targetDir = (hitNormaltoPlane-rampHitPoint) - slopeTransform.position;
                testVector = targetDir;
                float planeAngle = Vector3.Angle(transform.forward, targetDir);
            }
            
        }
    }

    void Motion()
    {
        pRB.drag = 0;
        
        var velocity = pRB.velocity;

        //turning
        float turning = Input.GetAxis(horizontalAxis);
        pRB.AddRelativeTorque(transform.up * turning * (turnSpeed*(maxSpeed/Mathf.Max(15f, velocity.magnitude*turnScaler))));


        xZPlaneSpeed = Mathf.Sqrt((velocity.x * velocity.x) + (velocity.z * velocity.z));

        Vector3 normalizedXZMovement = new Vector3(velocity.x / xZPlaneSpeed, velocity.y, velocity.z / xZPlaneSpeed);

        Vector3 xzSpeed = new Vector3(normalizedXZMovement.x * maxSpeed, velocity.y, normalizedXZMovement.z * maxSpeed);

        if (Input.GetButtonDown(accelerateAxis))
        {
            pRB.AddForce(transform.forward * accelerationForce * /*(accelerationForce *  Mathf.Max(0.5f, (velocity.magnitude / maxSpeed)))*/ Mathf.Max(0, Input.GetAxis(verticalAxis)), ForceMode.VelocityChange);
        }
        if (xZPlaneSpeed >= maxSpeed)
        {
            pRB.velocity = xzSpeed;
        }

        if (Input.GetButton(brakeAxis))
        {
            // pRB.velocity = normalizedXZMovement * (brakeForce*Time.deltaTime / 1);
            pRB.AddForce(-pRB.velocity.normalized / Mathf.Max(0.1f, brakeForce));
        }
        if (Input.GetAxis(jumpAxis)>0.9f&&isGrounded)
        {
            
            pRB.AddForce(transform.up * jumpForce,ForceMode.VelocityChange);
        }

        ScaleAccelerometer(xZPlaneSpeed, maxSpeed);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(slopeTransform.position, transform.forward+slopeTransform.position);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(rampHitPoint, /*rampNormal*/ new Vector3(rampNormal.x, 0, rampNormal.z) + rampHitPoint);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(slopeTransform.transform.position, targetDir.normalized);
        


        
        
    }
}
