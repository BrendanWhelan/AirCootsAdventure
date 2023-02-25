using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RotateTransform : MonoBehaviour
{
    [SerializeField]
    private Vector3 rotationDirection;

    private Rigidbody rb;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.instance.GamePaused) return;

        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotationDirection*Time.fixedDeltaTime));
    }

    public Vector3 GetRotationDirection()
    {
        return rotationDirection.normalized;
    }
}
