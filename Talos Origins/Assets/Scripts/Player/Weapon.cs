using UnityEngine;
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

    [SerializeField]
    GameObject shotAudio1;

    int mGunDamage;

    // Use this for initialization
    void Start () {   
        mShoot = false; 
        mGunDamage = 5;
        lastShootTime = Time.time;
        mTalos = transform.parent.GetComponent<Player>();

		FireRateLevel = PlayerPrefs.GetInt ("Rate of Fire");
		shootIntervalWithLevel = mShootInterval - (0.033f * this.FireRateLevel); 
		
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

            Vector3 mBulletDirection = mousePosition- mTalos.transform.position;
            mBulletDirection.z = 0;        

            GameObject mBullet = (GameObject)Instantiate(mBulletPrefab, bulletPosition, Quaternion.identity);            
            mBullet.GetComponent<Bullet>().SetDirection(mBulletDirection.normalized, mTalos.PlayerVelocity());
            //mBullet.SendMessage("BulletDamage", mGunDamage);

            lastShootTime = Time.time;

            
            Instantiate(shotAudio1, transform.position, Quaternion.identity);         
          
        }
        
    }

	public void SetRateOfFireLevel(int Level)
	{
		this.FireRateLevel = Level;
		shootIntervalWithLevel = mShootInterval - (0.033f * this.FireRateLevel); 

	}
}
