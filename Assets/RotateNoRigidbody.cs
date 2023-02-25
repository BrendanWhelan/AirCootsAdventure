using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateNoRigidbody : MonoBehaviour
{
    [SerializeField]
    private Vector3 rotationDirection;
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.instance.GamePaused) return;
        transform.rotation = transform.rotation * Quaternion.Euler(rotationDirection*Time.fixedDeltaTime);
    }
}
