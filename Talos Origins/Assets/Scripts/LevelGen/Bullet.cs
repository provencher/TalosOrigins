using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    [SerializeField]
    float lifeTime;

    [SerializeField]
    float mSpeed;

    Rigidbody2D mRigidbody2D;

    // Use this for initialization
    void Awake () {
        mRigidbody2D = GetComponent<Rigidbody2D>();
        mRigidbody2D.velocity = (mSpeed * Vector2.right);
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
    public void SetDirection( Vector2 dic)
    {
        mRigidbody2D.velocity = (mSpeed * dic);
    }
}
