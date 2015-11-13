using UnityEngine;
using System.Collections;

public class EnemyCoBullet : MonoBehaviour
{

    // Use this for initialization
    [SerializeField]
    float mSpeed;
    [SerializeField]
    float lifeTime;
    float createTime;
    float deadTime;
    Animator mAnimator;

    public int mDamage;

    public bool mHit = false;

    [SerializeField]
    public GameObject mExplosion;



    Rigidbody2D mRigidbody2D;
    void Awake()
    {

    }

    void Start()
    {
        deadTime = Time.time + lifeTime;        
        mAnimator = GetComponent<Animator>();
        createTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        CheckDeath();
        UpdateAnimator();
    }
    public void setDirection(Vector3 mDirection)
    {       
        mDirection.Normalize();
        GetComponent<Rigidbody2D>().velocity = mSpeed * (Vector2)mDirection;
    }
    void CheckDeath()
    {
        if (Time.time > deadTime || mHit)
        {
            Instantiate(mExplosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    void UpdateAnimator()
    {    
        mAnimator.SetFloat("time", Time.time - createTime);
    }

    void SetDamage(int damage)
    {
        mDamage = 2 * damage;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {/*
        if (coll != null)
        {
            if (coll.gameObject.tag == "Player")
            {
                coll.gameObject.SendMessage("HitByBullet", mDamage);
                
                Vector3 direction = transform.position - coll.gameObject.transform.position;
                Vector3 force =  direction.normalized / Mathf.Pow(direction.magnitude, 2);

                coll.gameObject.GetComponent<Rigidbody2D>().AddForce(300 * force);
              
            }           
        }
          */
    }
}
        
