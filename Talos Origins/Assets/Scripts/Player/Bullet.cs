using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    [SerializeField]
    float lifeTime;

    [SerializeField]
    float mSpeed;

    float mDestroyTime;
    Rigidbody2D mRigidbody2D;

    int mDamage;

    // Use this for initialization
    void Awake () {
        mRigidbody2D = GetComponent<Rigidbody2D>();
        mRigidbody2D.velocity = (mSpeed * Vector2.right);
        mRigidbody2D.gravityScale = 0;
        mDestroyTime = Time.time + lifeTime;
        mDamage = 25;
	}
	
	// Update is called once per frame
	void Update () {
        CheckDeath();
	}

    void CheckDeath()
    {
        if (Time.time > mDestroyTime)
        {
            Destroy(gameObject);
        }
    }

    public void SetDirection( Vector3 direction, Vector3 playerVelocity)
    {
        playerVelocity.Normalize();

        if((direction.x > 0 && playerVelocity.x > 0)
            || (direction.x < 0 && playerVelocity.x < 0))
        {
            direction.x += playerVelocity.x;
        }

        if ((direction.y > 0 && playerVelocity.y > 0)
           || (direction.y < 0 && playerVelocity.y < 0))
        {
            direction.y += playerVelocity.y;
        }


        mRigidbody2D.velocity = (mSpeed * (Vector2)direction);
        transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);
    }

    void BulletDamage(int damage)
    {
        mDamage = damage;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {       
        if (coll.gameObject.tag == "Enemy")
        {          
            coll.gameObject.SendMessage("HitByBullet", mDamage);
            Destroy(gameObject);
        }
        else if (coll.gameObject.tag == "Cave")
        {
            Destroy(gameObject);
        }
        

    }
}
