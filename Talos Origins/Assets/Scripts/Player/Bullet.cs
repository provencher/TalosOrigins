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
        //mRigidbody2D.gravityScale = 0;
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

    public void SetDirection( Vector3 dic)
    {
        mRigidbody2D.velocity = (mSpeed * new Vector2(dic.x, dic.y));
        transform.rotation = Quaternion.FromToRotation(Vector3.right, dic);
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
