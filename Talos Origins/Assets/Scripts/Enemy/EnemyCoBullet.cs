using UnityEngine;
using System.Collections;

public class EnemyCoBullet : MonoBehaviour {

    // Use this for initialization
    [SerializeField]
    float mSpeed;
    [SerializeField]
    float lifeTime;
    float deadTime;



    Rigidbody2D mRigidbody2D;
	void Awake () {
        deadTime = Time.time + lifeTime;
        mRigidbody2D = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        CheckDeath();
	}
    public void setDirection(Vector3 mDirection)
    {
        mRigidbody2D = GetComponent<Rigidbody2D>();
        mDirection.Normalize();
        mRigidbody2D.velocity = 3.0f *Vector2.left;
    }
    void CheckDeath()
    {
        if (Time.time > deadTime)
        {
            Destroy(gameObject);
        }
    }
}
