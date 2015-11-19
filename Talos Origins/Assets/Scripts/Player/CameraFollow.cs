using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    Transform mTarget;

    float kFollowSpeed = 8f;
    float stepOverThreshold = 0.15f;

	void Start()
	{
		mTarget = GameObject.Find("Talos").transform;            
	}

    void Update ()
    {
        if(mTarget != null)
        {
            Vector3 targetPosition = new Vector3(mTarget.transform.position.x, mTarget.transform.position.y + 0.75f, transform.position.z);
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
    
    void PlayerVelocity(Vector2 velocity)
    {
        if (velocity.magnitude > 8.0f)
        {
            kFollowSpeed = velocity.magnitude;
        }
        else
        {
            kFollowSpeed = 8.0f;
        }
    }     
}
