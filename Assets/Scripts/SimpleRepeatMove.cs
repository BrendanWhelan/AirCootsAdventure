using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRepeatMove : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 offset = new Vector3(50, 0, 0);
    private float timeToMove = 10;
    private float timeMoving = 0;

    private bool movingToStart = false;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (movingToStart)
        {
            transform.position = Vector3.Lerp(startPos + offset, startPos, timeMoving / timeToMove);
            timeMoving += Time.deltaTime;
            if (timeMoving >= timeToMove)
            {
                movingToStart = false;
                timeMoving = 0f;
            }
        }
        else
        {
            transform.position = Vector3.Lerp(startPos, startPos+offset, timeMoving / timeToMove);
            timeMoving += Time.deltaTime;
            if (timeMoving >= timeToMove)
            {
                movingToStart = true;
                timeMoving = 0f;
            }
        }
    }
}
