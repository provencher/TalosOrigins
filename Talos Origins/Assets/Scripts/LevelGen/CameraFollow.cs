﻿using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    Transform mTarget;

    float kFollowSpeed = 5f;
    float stepOverThreshold = 0.05f;

    void FixedUpdate ()
    {
        if(mTarget != null)
        {
            Vector3 targetPosition = new Vector3(mTarget.transform.position.x, mTarget.transform.position.y, transform.position.z);
            Vector3 direction = targetPosition - transform.position;        

            if(direction.magnitude > stepOverThreshold)
            {
                // If too far, translate at kFollowSpeed
                transform.Translate (direction.normalized * kFollowSpeed * Time.fixedDeltaTime);
            }
            else
            {
                // If close enough, just step over
                transform.position = targetPosition;
            }
        }
    }

    void StartPos(Vector3 pos)
    {
        pos.z = transform.position.z;
        transform.position = pos;        
    }       
}
