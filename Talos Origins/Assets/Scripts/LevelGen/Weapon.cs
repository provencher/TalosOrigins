using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {
    Animator mAnimator;
    bool mMelee;
    bool mShoot;
    int mWeapon;// 1: sword; 2:gun;

	// Use this for initialization
	void Start () {
        mAnimator = transform.parent.GetComponent<Animator>();
        mMelee = false;
        mShoot = false;
        mWeapon = 1;
	}
	
	// Update is called once per frame
	void Update () {
        UpdateWeapon();
        CheckFire();
        UpdateAnimator();
	}
    void UpdateWeapon()
    {
        if (Input.GetButtonDown("Sword"))
        {
            mWeapon = 1;
        }
        if (Input.GetButtonDown("Gun"))
        {
            mWeapon = 2;
        }
    }

    void CheckFire()
    {
        if (Input.GetAxis("Fire1")>0)
        {
            if (mWeapon == 1)
            {
                mMelee = true;
            }
            if (mWeapon == 2)
            {
                mShoot = true;
            }
        }
        else
        {
            mMelee = false;
            mShoot = false;
        }
    }

    void UpdateAnimator()
    {
        mAnimator.SetBool("isMelee", mMelee);
        mAnimator.SetBool("isShoot", mShoot);
    }
}
