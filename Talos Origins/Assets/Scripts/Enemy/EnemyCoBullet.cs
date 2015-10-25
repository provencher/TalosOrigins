using UnityEngine;
using System.Collections;

public class EnemyCoBullet : MonoBehaviour {

    // Use this for initialization
    [SerializeField]
    float mSpeed;
    [SerializeField]
    float lifeTime;
    float createTime;
    float deadTime;
    Animator mAnimator;



    Rigidbody2D mRigidbody2D;
	void Awake () {
        deadTime = Time.time + lifeTime;
        mRigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        mAnimator = GetComponent<Animator>();
        createTime = Time.time;
    }
	
	// Update is called once per frame
	void Update () {
        CheckDeath();
        UpdateAnimator();
	}
    public void setDirection(Vector3 mDirection)
    {
        mRigidbody2D = GetComponent<Rigidbody2D>();
        mDirection.Normalize();
        mRigidbody2D.velocity = mSpeed * (Vector2)mDirection;
    }
    void CheckDeath()
    {
        if (Time.time > deadTime)
        {
            Destroy(gameObject);
        }
    }
    void UpdateAnimator()
    {
        mAnimator.SetFloat("time", Time.time - createTime);
    }
}
