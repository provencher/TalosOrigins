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
	bool mShopOn;
	public static string[] mUpgrades;

    float kGroundCheckRadius = 0.1f;

    // Animator booleans
    bool mRunning;
    bool mGrounded;
    bool mRising;

    // Invincibility timer
    float kInvincibilityDuration = 0.5f;
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


    public Attachment_WallWalker walkerScript;
	
    /*
    [SerializeField]
    LifeMeter life;

    List<GroundCheck> mGroundCheckList;
    */

    void Awake()
	{
		//TODO: Delete this once main menu has been created
		//PlayerPrefs.DeleteAll ();
	}

    void Start()
    {
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

		// Set the Upgrades
		jumpLevel 		   = PlayerPrefs.GetInt ("Jump");
		healthPackLevel    = PlayerPrefs.GetInt ("Health Pack");
		shieldLevel 	   = PlayerPrefs.GetInt ("Shield");
		jumpLevelIndex 	   = 1f + (jumpLevel * 0.1f);
		shieldUpgradeIndex = 1f + (shieldLevel * 0.1f);
		mHealth 		   = 100 + (healthPackLevel * 10);
		UpdateHealthBar(mHealth);


        //walkerScript = GetComponent<Attachment_WallWalker>();
        //walkerScript.USERINPUT = true;


        // UI Text
        /*exitDistance = GameObject.Find("DistanceExit").GetComponent<Text>();
        enemiesLeft = GameObject.Find("EnemiesRemaining").GetComponent<Text>();
        curLevel = GameObject.Find("CurrentLevel").GetComponent<Text>();
        talosHealth = GameObject.Find("TalosHealth").GetComponent<Text>();
        experience = GameObject.Find("Experience").GetComponent<Text>();
        actionPts = GameObject.Find("ActionPoints").GetComponent<Text>();
        invicibleTime = GameObject.Find("Invicible").GetComponent<Text>();*/


        /*
        // Obtain ground check components and store in list
        mGroundCheckList = new List<GroundCheck>();
        GroundCheck[] groundChecksArray = transform.GetComponentsInChildren<GroundCheck>();
        foreach (GroundCheck g in groundChecksArray)
        {
            mGroundCheckList.Add(g);
        }

        // Get audio references
        AudioSource[] audioSources = GetComponents<AudioSource>();
        mLandingSound = audioSources[0];
        mWallKickSound = audioSources[1];
        mTakeDamageSound = audioSources[2];
        */        

    }

    /*
    void Update()
    {
        velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * 5;
    }
    */

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
        UpdateCameraVelocity();
        UpdateUIText();
    }

	void UpdateUIText()
	{
		try{
			//GameObject.Find("distance").GetComponent<Text>().text = "Exit Dist: " + ((int)(mExitLocation - transform.position).magnitude).ToString();
			//GameObject.Find("enemiesLeft").GetComponent<Text>().text = "Enemies:  " + mEnemiesRemaining.ToString();
			GameObject.Find("curLevel").GetComponent<Text>().text = "Current Level: " + mCurrentLevel.ToString();
			//GameObject.Find("health").GetComponent<Text>().text = "Health: " + mHealth.ToString();
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
        //rigidbody.MovePosition(rigidbody.position + velocity * Time.fixedDeltaTime);       
        FaceMouse();
        CheckGround();
        CheckInvicible();

        TranslateInDirection(CheckMove());
        //walkerScript.ApplyMovementDirection(CheckMove());

        TriggerMelee();
        CheckJump();
        //FaceMouse();
        UpdateAnimator();

		if((Input.GetButtonDown("Shop"))){

			if(mShopOn)
			{
				UpdatePlayer();
			}
			else
			{
				EnterShop();
			}
		}

        /*
        bool grounded = CheckGrounded();
        if (!mGrounded && grounded)
        {
            mLandingSound.Play();
        }
        mGrounded = grounded;
        */


        /*

        mRising = mRigidBody2D.velocity.y > 0.0f;
        UpdateAnimator();

        if (mInvincible)
        {
            mInvincibleTimer += Time.deltaTime;
            if (mInvincibleTimer >= kInvincibilityDuration)
            {
                mInvincible = false;
                mInvincibleTimer = 0.0f;
            }
        }
        */

    }

	public IEnumerator InflictDamage(int damage)
    {        

        if (!mInvincible)
        {
            mInvincible = true;
            mInvincibleTimer = kInvincibilityDuration * shieldUpgradeIndex;           

            mHealth -= damage;
            UpdateHealthBar(mHealth);

            if (mHealth > 50)
            {
                Instantiate(PainAudio1, transform.position, Quaternion.identity);
                yield return new WaitForSeconds(Time.deltaTime);
            }

            if (mHealth > 25 && mHealth < 50)
            {
                Instantiate(PainAudio2, transform.position, Quaternion.identity);
                yield return new WaitForSeconds(Time.deltaTime);
            }

            if (mHealth > 0 && mHealth < 25)
            {
                Instantiate(PainAudio3, transform.position, Quaternion.identity);
            }

        }
        yield break;
    }

    IEnumerator CheckDead()
    {
        if(mHealth <= 0)
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

            yield return new WaitForSeconds(0.3f);
            GameObject.Find("MapGenerator").SendMessage("ResetGame");            
        }
        yield break;
    }

    void CheckJump()
    {
        if (Input.GetButtonDown("Jump") && (mGrounded || !mUsedDoubleJump))
        {           
			mRigidBody2D.AddForce(Vector2.up * mJumpForce * jumpLevelIndex, ForceMode2D.Impulse);

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
            mRigidBody2D.AddForce(10*direction);
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
            StartCoroutine(InflictDamage((int)shoveInfo.z));
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
        StartCoroutine(InflictDamage(damage));        
    }

    void PickupOrb(int type)
    {
        //Debug.Log("Picked Orb of type " + type.ToString());
        orbTank[type]++;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null)
        {
            if (other.gameObject.tag == "Orb")
            {
                //Pickup orb                
                PickupOrb(other.gameObject.GetComponent<Orb>().type);
                if(other.gameObject.GetComponent<Orb>().type == 0)
                {
                    mHealth += 5;
                }

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

    void UpdateHealthBar(int health)
	{	
		mHealthSlider.maxValue = 100 + (healthPackLevel * 10);

        mHealthSlider.value = health;
        if (health < 60)
        {
            mHealthSlider.transform.FindChild("Fill Area").FindChild("Fill").GetComponent<Image>().color = new Color(255, 255, 0);
        }
        if (health < 30)
        {
            mHealthSlider.transform.FindChild("Fill Area").FindChild("Fill").GetComponent<Image>().color = new Color(255, 0, 0);
        }
    }

	public void UpdatePlayer()
	{
		if(GameObject.Find ("Orbs").GetComponent<ShopOrbs>().totalOrbsCount >= 0){
			
			mCanvas.SetActive(true);
			mShopCanvas.SetActive(false);
			mShopOn = false;

			gameObject.GetComponent<Grapple>().setGrappleLevel(PlayerPrefs.GetInt("Grapple"));
			gameObject.GetComponent<Trail>().SetTrailLevel(PlayerPrefs.GetInt("Breadcrumbs"));
			mWeapon.GetComponent<Weapon>().SetRateOfFireLevel(PlayerPrefs.GetInt("Rate of Fire"));
			jumpLevel = PlayerPrefs.GetInt("Jump");
			healthPackLevel = PlayerPrefs.GetInt("Health Pack");
			shieldLevel = PlayerPrefs.GetInt("Shield");
			shieldUpgradeIndex = 1f + (shieldLevel * 0.1f);
			UpdateHealthBar(mHealth);

		}
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