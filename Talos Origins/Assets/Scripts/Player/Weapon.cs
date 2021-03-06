﻿using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {
   
    public bool mMelee;
    public bool mShoot;
    int mWeapon;// 1: sword; 2:gun;
    Player mTalos;
    [SerializeField]
    float mShootInterval;
    float lastShootTime;
    [SerializeField]
    GameObject mBulletPrefab;

	float shootIntervalWithLevel;

	int FireRateLevel;
    public int mDamageLevel;

    [SerializeField]
    GameObject shotAudio1;
    int mGunDamage;

    public int mNumberBullets = 1;

    // Use this for initialization
    void Start () {   
        mShoot = false; 
        mGunDamage = 5;
        lastShootTime = Time.time;
        mTalos = transform.parent.GetComponent<Player>();

        //FireRateLevel = PlayerPrefs.GetInt ("Rate of Fire");

        SetRateOfFireLevel(PlayerPrefs.GetInt("Rate of Fire",0));
        shootIntervalWithLevel = mShootInterval - (0.033f * this.FireRateLevel);
		mNumberBullets = PlayerPrefs.GetInt("Spray Bullets", 0) + 1;
        mDamageLevel = PlayerPrefs.GetInt("Big Bullets", 0);
    }
	
	// Update is called once per frame
	void Update () {
		if (Application.loadedLevelName == "Proto") {			
			CheckFire ();       
		}
	}

    void CheckFire()
    {
		if(GameObject.Find ("Canvas") != null){
	        if (Input.GetAxis("Fire1")>0)
    	    {       	            
	            mShoot = true;	                
	            Shoot();	        
	        }
	        else
	        {	          
	            mShoot = false;
	        }
		}
    }

    void Shoot()
    {
        if (Time.time - lastShootTime > shootIntervalWithLevel)
        {
            Vector3 bulletPosition = transform.position + mTalos.mFacingDirection.x * Vector3.right * 0.4f;
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float talosHorizOffset = mTalos.GetComponent<BoxCollider2D>().size.x * mTalos.transform.localScale.x;

            //Offset mouse position to avoid problems when clicking on talos
            if ((mousePosition - mTalos.transform.position).magnitude < talosHorizOffset/8)
            {
                mousePosition.x += talosHorizOffset * mTalos.mFacingDirection.x;
            }


            Vector3 targetPos = mTalos.transform.position;
            //mNumberBullets = 10;

            for (int i = 0; i < mNumberBullets; i++)
            {
                Vector3 mBulletDirection = mousePosition - new Vector3(targetPos.x, targetPos.y + i * Mathf.Pow((-1), i)/mNumberBullets, 0);
                mBulletDirection.z = 0;

                GameObject mBullet = (GameObject)Instantiate(mBulletPrefab, bulletPosition, Quaternion.identity);
                if(false && i % 2 != 0)
                {
                    Destroy(mBullet.GetComponent<Light>());
                    Destroy(mBullet.GetComponent<ParticleSystem>());
                }
             

                mBullet.GetComponent<Bullet>().SetDirection(mBulletDirection.normalized, mTalos.PlayerVelocity());
                //mBullet.GetComponent<Bullet>().mNumBullets = mNumberBullets;
                mBullet.GetComponent<Bullet>().SetDamage(mNumberBullets, PlayerPrefs.GetInt("Big Bullets", 0));
            }
            
            //mBullet.SendMessage("BulletDamage", mGunDamage);

            lastShootTime = Time.time;

            
            Instantiate(shotAudio1, transform.position, Quaternion.identity);         
          
        }
        
    }

	public void SetRateOfFireLevel(int Level)
	{
		this.FireRateLevel = Level;
        
        if(Level <= 1)
        {
            shootIntervalWithLevel = mShootInterval - (0.033f * this.FireRateLevel);
        }
        else
        {
            shootIntervalWithLevel = mShootInterval - (0.033f + (0.033f * (this.FireRateLevel -1 )/1.3f));
        }		
	}
}
