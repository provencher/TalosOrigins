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
    public int mNumBullets;

	int DamageLevel;
	int FireRateLevel;
    Vector2 mVelocity;
	
    // Use this for initialization
    void Awake () {
        mRigidbody2D = GetComponent<Rigidbody2D>();
        mRigidbody2D.velocity = (mSpeed * Vector2.right);
        mRigidbody2D.gravityScale = 0;
        mDestroyTime = Time.time + lifeTime;
		
        mNumBullets = 1;

        mVelocity = mRigidbody2D.velocity;
        mDamage = 2;

        //Debug.Log("bullet damage: " + mDamage.ToString());

    }

	// Update is called once per frame
	void FixedUpdate () {
        CheckDeath();
        mRigidbody2D.velocity = mVelocity;
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
        mVelocity = mRigidbody2D.velocity;
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
        

        /*
        if (coll.gameObject.tag == "Enemy")
        {          
            coll.gameObject.SendMessage("HitByBullet", mDamage);
            Destroy(gameObject);
        }
          
        */

    }

    public void SetDamage(int numBullets, int damageLevel)
    {
        if (DamageLevel == 0)
        {
            mDamage = 2;
        }    
        else if (mDamage == 1)
        {
            mDamage = 4;
        }    
        else
        {
            mDamage =  mDamage * damageLevel;
        }       
        if(numBullets > 1)
        {
            mDamage = Mathf.RoundToInt(mDamage * (1 - (numBullets / 10)));
        }        
    }
}
