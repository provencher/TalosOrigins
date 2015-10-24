﻿using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {
    Animator mAnimator;
    public bool mMelee;
    public bool mShoot;
    int mWeapon;// 1: sword; 2:gun;
    Player mTalos;
    [SerializeField]
    float ShootInterval;
    float lastShootTime;
    [SerializeField]
    GameObject mBulletPrefab;

    int mGunDamage;

    // Use this for initialization
    void Start () {       
        mMelee = false;
        mShoot = false;
        mWeapon = 2;
        mGunDamage = 25;
        lastShootTime = Time.time;
        mTalos = transform.parent.GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () {
        UpdateWeapon();
        CheckFire();       
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
                mShoot = false;
            }
            if (mWeapon == 2)
            {
                mShoot = true;
                mMelee = false;
                Shoot();
            }
        }
        else
        {
            mMelee = false;
            mShoot = false;
        }
    }

    void Shoot()
    {
        if (Time.time - lastShootTime > ShootInterval)
        {
            GameObject mBullet = (GameObject)Instantiate(mBulletPrefab, transform.position+mTalos.mFacingDirection.x*Vector3.right * 0.4f, Quaternion.identity);
            mBullet.SendMessage("BulletDamage", mGunDamage);

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 mBulletDirection = mousePosition-mBullet.transform.position;
            mBulletDirection.z = 0;
            mBulletDirection.Normalize();
            mBullet.GetComponent<Bullet>().SetDirection(mBulletDirection);
            lastShootTime = Time.time;
        }
        
    }   
}
