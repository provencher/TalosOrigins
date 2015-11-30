using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    [SerializeField]
    float lifeTime;

    [SerializeField]
    float mSpeed;

    [SerializeField]
    public GameObject mExplosion;
	
    float mDestroyTime;
    Rigidbody2D mRigidbody2D;

    public bool mAlive = true;

    public int mDamage;

	int DamageLevel;
	int FireRateLevel;
	
    // Use this for initialization
    void Awake () {
        mRigidbody2D = GetComponent<Rigidbody2D>();
        mRigidbody2D.velocity = (mSpeed * Vector2.right);
        mRigidbody2D.gravityScale = 0;
        mDestroyTime = Time.time + lifeTime;
		DamageLevel = PlayerPrefs.GetInt ("Big Bullets");

		if (DamageLevel == 0) {
			mDamage = 2;
		} else {
			mDamage = 3 + DamageLevel;
		}

        Debug.Log("bullet damage: " + mDamage.ToString());

	}

	// Update is called once per frame
	void Update () {
        CheckDeath();
		
	}

    void CheckDeath()
    {
        if (Time.time > mDestroyTime || !mAlive)
        {
            Explode();
        }
    }

    void Explode()
    {
        Instantiate(mExplosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
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
        if (coll.gameObject.tag == "enemyBullet")
        {
            coll.gameObject.GetComponent<EnemyCoBullet>().mHit = true;
            Explode();
        }
        else if (coll.gameObject.tag == "Cave" || coll.gameObject.tag == "Bullet" || coll.gameObject.tag == "Exit")
        {
            Explode();
        }
        else if (coll.gameObject.tag == "Enemy")
        {

        }

        /*
        if (coll.gameObject.tag == "Enemy")
        {          
            coll.gameObject.SendMessage("HitByBullet", mDamage);
            Destroy(gameObject);
        }
          
        */

    }
}
