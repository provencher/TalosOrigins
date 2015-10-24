using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    [SerializeField]
    float lifeTime;

    [SerializeField]
    float mSpeed;

    float mDestroyTime;
    Rigidbody2D mRigidbody2D;

    // Use this for initialization
    void Awake () {
        mRigidbody2D = GetComponent<Rigidbody2D>();
        mRigidbody2D.velocity = (mSpeed * Vector2.right);
        mDestroyTime = Time.time + lifeTime;
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
}
