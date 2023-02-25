using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityControl : MonoBehaviour
{
    public GravityOrbit gravity;
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private Transform transformToRotate;

    [SerializeField]
    private float rotationSpeed = 20;

    private bool tryingToUseGravity = true;
    private void FixedUpdate()
    {
        if (gravity)
        {
            rb.useGravity = false;
            Vector3 gravityUp = gravity.transform.up;
            
            Vector3 newPlayerRight = Vector3.Cross(gravityUp, transformToRotate.forward);
            Vector3 newPlayerForward = Vector3.Cross(newPlayerRight, gravityUp);

            Quaternion newRotation = Quaternion.LookRotation(newPlayerForward, gravityUp);
            
            //var distanceToPlane = Vector3.Dot(transformToRotate.up, (gravity.transform.position - transformToRotate.position));
           // var planePoint = gravity.transform.position - (transformToRotate.up * distanceToPlane);
           //Quaternion fromToRotation = Quaternion.FromToRotation(transformToRotate.up, gravityUp) * transformToRotate.rotation;

            //Quaternion targetRotation = Quaternion.LookRotation(transformToRotate.forward,gravityUp);
            //Quaternion targetRotation = Quaternion.FromToRotation(transformToRotate.up, gravityUp) * transformToRotate.rotation;

            rb.MoveRotation(Quaternion.Slerp(transformToRotate.rotation, newRotation, rotationSpeed * Time.deltaTime));//Vector3.Lerp(transformToRotate.up, gravityUp, rotationSpeed * Time.deltaTime);
            
            rb.AddForce(-gravityUp * (gravity.Gravity * rb.mass));
        }
        else if(tryingToUseGravity)
        {
            rb.useGravity = true;
            Vector3 gravityUp = -Physics.gravity.normalized;

            Quaternion targetRotation = Quaternion.LookRotation(transformToRotate.forward, gravityUp);

            rb.MoveRotation(Quaternion.Slerp(transformToRotate.rotation, targetRotation, rotationSpeed * Time.deltaTime));//Vector3.Lerp(transformToRotate.up, gravityUp, rotationSpeed * Time.deltaTime);

        }
        else
        {
            rb.useGravity = false;
        }
    }

    public void LeftGravity(GravityOrbit gravityOrbit)
    {
        if (gravity == gravityOrbit)
        {
            gravity = null;
        }
    }

    public void UseGravity(bool useGravity)
    {
        tryingToUseGravity = useGravity;

        if (!useGravity) rb.useGravity = false;
    }
}
