using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class Player : MonoBehaviour
{
    //Vector2 velocity;

	//Pain Audio
    [SerializeField]
    GameObject PainAudio1;
    [SerializeField]
    GameObject PainAudio2;
    [SerializeField]
    GameObject PainAudio3;

	//Jump Audio
    [SerializeField]
    GameObject jumpAudio1;
    [SerializeField]
    GameObject jumpAudio2;
	
	[SerializeField]
	GameObject ConfirmAudio;

    [SerializeField]
    GameObject ExplosionPrefab;

    [SerializeField]
    float mMoveSpeed;

    [SerializeField]
    float mJumpForce;
	
	float jumpLevelIndex;
	int jumpLevel;

	[SerializeField]
    LayerMask mWhatIsGround;

    [SerializeField]
    Slider mHealthSlider;

	[SerializeField]
	GameObject mShopCanvas;

	[SerializeField]
	GameObject mCanvas;

	int[] orbTank;
	string Upgrade = "Upgrade";
	public bool mShopOn;
	public static string[] mUpgrades;

    float kGroundCheckRadius = 0.1f;

    // Animator booleans
    bool mRunning;
    bool mGrounded;
    bool mRising;

    // Invincibility timer
    float kInvincibilityDuration = 0.85f;
    public float mInvincibleTimer;
    bool mInvincible;

    // Damage effects
    float kDamagePushForce = 2.5f;

    public bool mUsedDoubleJump = false;

    // Wall kicking
    bool mAllowWallKick;
    public Vector2 mFacingDirection;

    // References to other components and game objects
    Animator mAnimator;
    Rigidbody2D mRigidBody2D;
    Weapon mWeapon;

    // Reference to audio sources
    AudioSource mLandingSound;
    AudioSource mWallKickSound;
    AudioSource mTakeDamageSound;

//    [SerializeField]
//    GameObject mDeathParticleEmitter;

    int mTotalExp;
    float mMeleeTimer;
    bool mMeleeTrigger;
    int mHealth;
    int mEnemiesKilled;

    Vector2 mShoveDirection;

    Vector3 mExitLocation;
    int mCurrentLevel;
    int mEnemiesRemaining;
	
    Text exitDistance, enemiesLeft, curLevel, talosHealth, experience, actionPts, invicibleTime;

	int healthPackLevel;
	int shieldLevel;
	float shieldUpgradeIndex;

	Vector3 lastInGamePosition;

    public bool RESETUPGRADES = true;


    public Attachment_WallWalker walkerScript;
	
    /*
    [SerializeField]
    LifeMeter life;

    List<GroundCheck> mGroundCheckList;
    */

    void Start()
    {
        bool reset = false;
        //Removes all upgrades
        if (PlayerPrefs.GetInt("resetGame") == 1)
        {
            reset = true;
            Draggable[] upgrades = FindObjectsOfType<Draggable>();
            foreach (Draggable upgrade in upgrades)
            {
                upgrade.resetStat = true;
            }

            // Disable so upgrades persist on death
            PlayerPrefs.SetInt("resetGame", 0);
        }

        // Get references to other components and game objects
        mRigidBody2D 	 = GetComponent<Rigidbody2D>();
        mAnimator 		 = GetComponent<Animator>();
        mWeapon 		 = transform.FindChild("Weapon").GetComponent<Weapon>();
        mFacingDirection = Vector2.right;
        mTotalExp 		 = 0;
        mMeleeTimer 	 = 0;
        mShoveDirection  = Vector2.zero;
        mInvincibleTimer = 0;
        InitOrbTank();
		mShopOn = false;
		mShopCanvas.SetActive (false);

        if (reset)
        {
            // Set the Upgrades manually to zero
            jumpLevel = 0;
            healthPackLevel = 0;
            shieldLevel = 0;
        }
        else
        {
            // Set the Upgrades to their saved values
            jumpLevel = PlayerPrefs.GetInt("Jump", 0);
            healthPackLevel = PlayerPrefs.GetInt("Health Pack", 0);
            shieldLevel = PlayerPrefs.GetInt("Shield", 0);           
        }
        jumpLevelIndex = 1f + (Mathf.Log10(jumpLevel) / Mathf.Log10(5));
        shieldUpgradeIndex = 1f + (shieldLevel * 0.1f);
        mHealth = Mathf.CeilToInt(5 * Mathf.Pow(2, healthPackLevel));

        UpdateHealthBar(mHealth);
    }


    void InitOrbTank()
    {
		//Blue Orb 	 = 0
		//Green Orb  = 1
		//Red Orb    = 2 
		//Yellow Orb = 2
        orbTank = new int[4];
		for (int i=0; i < orbTank.Length; i++) {
			orbTank[i] = 0;
		}
    }

    void FixedUpdate()
    {
        StartCoroutine(CheckDead());
        NotifyEnemiesOfPosition();


        FaceMouse();
        CheckGround();
        

        TranslateInDirection(CheckMove());
        //walkerScript.ApplyMovementDirection(CheckMove());

        //TriggerMelee();
       
        //FaceMouse();
        

        if ((Input.GetButtonDown("Shop")) && !GameObject.Find("PauseController").GetComponent<PauseControl>().paused)
        {

            if (mShopOn)
            {
                ShopCancelClick();
            }
            else
            {
                EnterShop();
            }
        }

        



        
    }

	void UpdateUIText()
	{
		try{
			//GameObject.Find("distance").GetComponent<Text>().text = "Exit Dist: " + ((int)(mExitLocation - transform.position).magnitude).ToString();
			//GameObject.Find("enemiesLeft").GetComponent<Text>().text = "Enemies:  " + mEnemiesRemaining.ToString();
			GameObject.Find("curLevel").GetComponent<Text>().text = "Current Level: " + mCurrentLevel.ToString();
            if (mHealth < 0)            
            {
                mHealth = 0;
            }
            GameObject.Find("health").GetComponent<Text>().text = mHealth.ToString() + "/" + mHealthSlider.maxValue.ToString();

            //GameObject.Find("experience").GetComponent<Text>().text = "Orb T0: " + orbTank[0].ToString() + " T1: " + orbTank[1].ToString() + " T2: " + orbTank[2].ToString() + " T3: " + orbTank[3].ToString();
            //GameObject.Find("invincibleTime").GetComponent<Text>().text = "Invincible Timer: " + mInvincibleTimer.ToString("F2");
            GameObject.Find("blueOrb").GetComponent<Text>().text = "" + orbTank[0];
            GameObject.Find("greenOrb").GetComponent<Text>().text = "" + orbTank[1];
            GameObject.Find("redOrb").GetComponent<Text>().text = "" + orbTank[2];
            GameObject.Find("yellowOrb").GetComponent<Text>().text = "" + orbTank[3];
        }
        catch(Exception e){
			//Do Nothing
		}
	}

    void Update()
    {
        CheckInvicible();
        CheckJump();     
        mRising = mRigidBody2D.velocity.y > 0.0f;
        UpdateAnimator();
        UpdateCameraVelocity();
        UpdateUIText();
        rechargeHealth();




    }

    public void InflictDamage(int damage)
    {        

        if (!mInvincible)
        {
            mInvincible = true;
            mInvincibleTimer = kInvincibilityDuration * shieldUpgradeIndex;   
            gameObject.GetComponentInChildren<Shield>().rechargeDeployed = true;

            mHealth -= damage;
            UpdateHealthBar(mHealth);

            if (mHealth > 50)
            {
                Instantiate(PainAudio1, transform.position, Quaternion.identity);               
            }

            if (mHealth > 25 && mHealth < 50)
            {
                Instantiate(PainAudio2, transform.position, Quaternion.identity);               
            }

            if (mHealth > 0 && mHealth < 25)
            {
                Instantiate(PainAudio3, transform.position, Quaternion.identity);
            }

        }       
    }

    IEnumerator CheckDead()
    {      
        if (mHealth <= 0)
        {      
            Instantiate(PainAudio3, transform.position, Quaternion.identity);           
            Instantiate(PainAudio2, transform.position, Quaternion.identity);            
            Instantiate(PainAudio1, transform.position, Quaternion.identity);       
            Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);

			PlayerPrefs.SetInt("Red Orbs", 0);
			PlayerPrefs.SetInt("Green Orbs", 0);
			PlayerPrefs.SetInt("Blue Orbs", 0);
			PlayerPrefs.SetInt("Yellow Orbs", 0);
			PlayerPrefs.SetInt ("Total Orbs",0);
            yield return new WaitForSeconds(0.2f);   
            GameObject.Find("MapGenerator").SendMessage("ResetGame");            
        }
        yield break;
    }

    void CheckJump()
    {
        if (Input.GetButtonDown("Jump") && (mGrounded || !mUsedDoubleJump))
        {           
			mRigidBody2D.AddForce(Vector2.up * mJumpForce * (1f + (PlayerPrefs.GetInt("Jump") * 0.1f)), ForceMode2D.Impulse);

            if(mGrounded)
            {
                Instantiate(jumpAudio1, transform.position, Quaternion.identity);
            }
            else
            {
                mUsedDoubleJump = true;
                Instantiate(jumpAudio2, transform.position, Quaternion.identity);
            }
        }
      
    }

    void CheckInvicible()
    {
        float time = 0;

        if (mInvincible)
        {
            if (mInvincibleTimer > 0)
            {
                mInvincibleTimer -= Time.deltaTime;       
            }
            else
            {
                mInvincibleTimer = 0;
                mInvincible = false;
                gameObject.GetComponentInChildren<Shield>().rechargeDeployed = false;
            }
            time = mInvincibleTimer;
        }    

    }

    Vector3 CheckMove()
    {       
        if (mWeapon.mMelee)
        {
            mMeleeTrigger = true;
            mMeleeTimer = 0;
            mRunning = false;            
            return Vector3.zero;
        }

        mRunning = false;
        /*
       Vector3 dir = new Vector3();
       dir.x = Input.GetAxis("Horizontal");
       dir.y = Input.GetAxis("Vertical");
       mRunning = true;
       return dir;
       */

       if (Input.GetButton("Left"))
       {
           //transform.Translate(-Vector3.right * mMoveSpeed * Time.deltaTime, Space.World);           
           //FaceDirection(Vector2.left);
           mRunning = true;
           return Vector3.left;
       }
       if (Input.GetButton("Right"))
       {
           //transform.Translate(Vector3.right * mMoveSpeed * Time.deltaTime, Space.World);
           //FaceDirection(Vector2.right);            
           mRunning = true;
           return Vector3.right;
       } 
       

        return Vector3.zero;
    }

    void TranslateInDirection(Vector3 direction)
    {      

        if (direction != null && (Mathf.Abs(mRigidBody2D.velocity.x) < 7.0f))
        {
            //Vector3 direction = target - transform.position;
            direction.z = 0;            
            mRigidBody2D.AddForce(13*direction);
            //transform.position += direction.normalized * mMoveSpeed * Time.deltaTime;
        }
    }

    public Vector3 PlayerVelocity()
    {
        return mRigidBody2D.velocity;
    }

    void FaceMouse()
    {
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x > transform.position.x)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.forward);
            mFacingDirection = Vector2.right;
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(-Vector3.forward);
            mFacingDirection = Vector2.left;
        }
    }

    public void FaceDirection(Vector2 faceD)
    {
        
        if (faceD == Vector2.left)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.back);
        }
        if(faceD == Vector2.right)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.forward);
        }
        mFacingDirection = faceD;
    }

    void CheckGround()
    {
        //temp
        if (Mathf.Abs(mRigidBody2D.velocity.y) > 0)
        {
            mGrounded = false;
        }
        else
        {
            mGrounded = true;
            mUsedDoubleJump = false;
        }
    }

    void TriggerMelee()
    {
        if (mMeleeTrigger)
        {
            if(mMeleeTimer > 0.2f)
            {
                mMeleeTimer = 0;
                mMeleeTrigger = false;
                GameObject enemy = EnemyInDirection(mFacingDirection);
                if (enemy != null)
                {
                    enemy.SendMessage("KilledBySword");
                }
            }
            else
            {
                mMeleeTimer += Time.deltaTime;
            }         
        }
    }
    
    void UpdateAnimator()
    {
        mAnimator.SetBool("isGrounded", mGrounded);
        mAnimator.SetBool("isRunning", mRunning);
        mAnimator.SetBool("isMelee", mWeapon.mMelee);
        mAnimator.SetBool("isShoot", mWeapon.mShoot);
    }
    
        
    GameObject EnemyInDirection(Vector2 direction)
    {        
        Vector2 StartPosition;
        Vector2 EndPosition;
        RaycastHit2D hit = new RaycastHit2D();
        float increment;
        float boxSizeX = GetComponent<BoxCollider2D>().size.x;
        float scaleX = transform.localScale.x;

        float boxSizeY = GetComponent<BoxCollider2D>().size.x;
        float scaleY = transform.localScale.x;

        float boxSize, scaleSize;

        for (int i = 0; i < 3; i++)
        {
            increment = 0;

            if (Mathf.Abs(direction.x) > 0)
            {
                boxSize = boxSizeX;
                scaleSize = scaleX;
            }
            else
            {
                boxSize = boxSizeY;
                scaleSize = scaleY;
            }

            switch (i)
            {
                case 1:
                    {
                        increment = boxSize * scaleSize / 2;
                        break;
                    }
                case 2:
                    {
                        increment = (-1) * boxSize * scaleSize / 2;
                        break;
                    }
                default:
                    break;
            }

            StartPosition = transform.position;
            if (Mathf.Abs(direction.x) > 0)
            {
                StartPosition.y += increment;
            }
            else
            {
                StartPosition.x += increment;
            }

            EndPosition = StartPosition + direction * 0.6f;

            //Check if clear
            hit = Physics2D.Linecast(StartPosition, EndPosition);
            Debug.DrawLine(StartPosition, EndPosition, Color.red, 2, false);

            if(hit.collider != null && hit.collider.gameObject.tag == "Enemy")
            {
                return hit.collider.gameObject;
            }          

        }

        return null;
    }

  



    // Notification management
    ///////////////////////////////////////////
    void NotifyEnemiesOfPosition()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            enemy.SendMessage("UpdatePlayerPosition", transform.position);
        }
    }

    void KilledEnemy(int exp)
    {
        mTotalExp += exp;
        mEnemiesRemaining--;
    }

   

    void ShovedByEnemy(Vector3 shoveInfo)
    {
        if(!mInvincible)
        {
            InflictDamage((int)shoveInfo.z);
            // Get Shoved
            //mRigidBody2D.AddForce(new Vector3(5 * shoveInfo.x, 3, 0), ForceMode2D.Impulse);      
        }
        else
        {
            mShoveDirection = Vector2.zero;
        }
    }
    
    void StartPos(Vector3 pos)
    {
        transform.position = pos;
        GameObject.Find("Main Camera").SendMessage("StartPos", pos);

        mInvincible = true;
        mInvincibleTimer = 1.5f;
    }

    void ExitPos(Vector3 pos)
    {
        mExitLocation = pos;
    }

    void CurrentLevel(int level)
    {
        mCurrentLevel = level;
    }

    void TotalEnemies(int numEnemies)
    {
        mEnemiesRemaining = numEnemies;
    }

    void UpdateCameraVelocity()
    {
		if (Application.loadedLevelName == "Proto") 
		{
			GameObject.Find ("Main Camera").SendMessage ("PlayerVelocity", mRigidBody2D.velocity);   		
		}
	}

    void HitByBullet(int damage)
    {
        InflictDamage(damage);        
    }

    void PickupOrb(int type)
    {
        //Debug.Log("Picked Orb of type " + type.ToString());
        orbTank[type] += Mathf.CeilToInt(Mathf.Pow(2,(mCurrentLevel-1)));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null)
        {
            if (other.gameObject.tag == "Orb")
            {
                //Pickup orb                
                PickupOrb(other.gameObject.GetComponent<Orb>().type);      
                //Destroy orb
                other.gameObject.GetComponent<Orb>().pickedUp = true;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll != null)
        {
            if(coll.gameObject.tag == "enemyBullet")
            {
                mHealth -= coll.gameObject.GetComponent<EnemyCoBullet>().mDamage;
                coll.gameObject.GetComponent<EnemyCoBullet>().mHit = true;
            }
            
        }
    }


    float heartbeat = 0;
    void rechargeHealth()
    {
        int healthUp = mHealth;
        
        // if not invincible, recharge health
        if (!mInvincible)
        {
            if(heartbeat < 0.33f * 1/(healthPackLevel+1))
            {
                heartbeat += Time.deltaTime;
            }
            else
            {
                healthUp = mHealth + 1;
                heartbeat = 0;
            }             
        
        }
        UpdateHealthBar(healthUp);    
    }


    void UpdateHealthBar(int health)
	{	
		float maxVal = mHealthSlider.maxValue = Mathf.CeilToInt(5 * Mathf.Pow(2, healthPackLevel));
        

        if (health > maxVal)
        {
            health = Mathf.RoundToInt(mHealthSlider.maxValue);
        }

        if (mHealth / maxVal * 100 > 60)
        {
            mHealthSlider.transform.FindChild("Fill Area").FindChild("Fill").GetComponent<Image>().color = new Color(0, 255, 0);
        }
        if (mHealth/maxVal * 100 < 60)
        {
            mHealthSlider.transform.FindChild("Fill Area").FindChild("Fill").GetComponent<Image>().color = new Color(255, 255, 0);
        }
        if (mHealth / maxVal * 100 < 30)
        {
            mHealthSlider.transform.FindChild("Fill Area").FindChild("Fill").GetComponent<Image>().color = new Color(255, 0, 0);
        }

        mHealthSlider.value = health;
        mHealth = health;
    }

	public void UpdatePlayer()
	{
		if(GameObject.Find ("Orbs").GetComponent<ShopOrbs>().totalOrbsCount >= 0){

			Instantiate(ConfirmAudio, transform.position, Quaternion.identity);
			mCanvas.SetActive(true);
			mShopCanvas.SetActive(false);
			mShopOn = false;

			gameObject.GetComponent<Grapple>().setGrappleLevel(PlayerPrefs.GetInt("Grapple", 0));
			gameObject.GetComponent<Trail>().SetTrailLevel(PlayerPrefs.GetInt("Breadcrumbs", 0));

            mWeapon.GetComponent<Weapon>().SetRateOfFireLevel(PlayerPrefs.GetInt("Rate of Fire", 0));
            mWeapon.GetComponent<Weapon>().mNumberBullets = PlayerPrefs.GetInt("Spray Bullets", 0) + 1;
            mWeapon.GetComponent<Weapon>().mDamageLevel = PlayerPrefs.GetInt("Big Bullets", 0);

            jumpLevel = PlayerPrefs.GetInt("Jump");
            jumpLevelIndex = 1f + (jumpLevel * 0.1f);

            healthPackLevel = PlayerPrefs.GetInt("Health Pack", 0);
			shieldLevel = PlayerPrefs.GetInt("Shield");
			shieldUpgradeIndex = 1f + (shieldLevel * 0.1f);
			UpdateHealthBar(mHealth);        
          

        }
	}

	 public void ShopCancelClick()
	{
		foreach (Transform child in GameObject.Find ("Viewport").transform)
		{
			child.GetComponent<Draggable>().CancelShop();
		}

		mCanvas.SetActive(true);
		mShopCanvas.SetActive(false);
		mShopOn = false;
	 }


	void EnterShop()
	{
		PlayerPrefs.SetInt ("Blue Orbs"	 , orbTank[0]);
		PlayerPrefs.SetInt ("Green Orbs" , orbTank[1]);
		PlayerPrefs.SetInt ("Red Orbs"	 , orbTank[2]);
		PlayerPrefs.SetInt ("Yellow Orbs", orbTank[3]);
		
		mCanvas.SetActive(false);
		mShopCanvas.SetActive(true);
		mShopOn = true;
	}

    public float GetExitDistance()
    {
        return (mExitLocation - transform.position).magnitude;
    }
}