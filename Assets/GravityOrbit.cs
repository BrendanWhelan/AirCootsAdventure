using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GravityOrbit : MonoBehaviour
{
    public float Gravity;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<GravityControl>())
        {
            other.GetComponent<GravityControl>().gravity = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<GravityControl>())
        {
            other.GetComponent<GravityControl>().LeftGravity(this);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<GravityControl>())
        {
            if (other.GetComponent<GravityControl>().gravity == null)
                other.GetComponent<GravityControl>().gravity = this;
        }
    }
}
