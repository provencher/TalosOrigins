using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{   
    //Vector2 velocity;


    [SerializeField]
    float mMoveSpeed;
    [SerializeField]
    float mJumpForce;
    [SerializeField]
    LayerMask mWhatIsGround;
    float kGroundCheckRadius = 0.1f;

    // Animator booleans
    bool mRunning;
    bool mGrounded;
    bool mRising;

    // Invincibility timer
    float kInvincibilityDuration = 1.0f;
    float mInvincibleTimer;
    bool mInvincible;

    // Damage effects
    float kDamagePushForce = 2.5f;


    // Wall kicking
    bool mAllowWallKick;
    Vector2 mFacingDirection;

    // References to other components and game objects
    Animator mAnimator;
    Rigidbody2D mRigidBody2D;

    // Reference to audio sources
    AudioSource mLandingSound;
    AudioSource mWallKickSound;
    AudioSource mTakeDamageSound;

    [SerializeField]
    GameObject mDeathParticleEmitter;
    /*
    [SerializeField]
    LifeMeter life;

    List<GroundCheck> mGroundCheckList;
    */

    void Start()
    {
        // Get references to other components and game objects
        
        mRigidBody2D = GetComponent<Rigidbody2D>();


        /*
        mAnimator = GetComponent<Animator>();


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

    void FixedUpdate()
    {
        NotifyEnemiesOfPosition();
    }

    void NotifyEnemiesOfPosition()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            enemy.SendMessage("UpdatePlayerPosition", transform.position);
        }
    }

    void Update()
    {
        //rigidbody.MovePosition(rigidbody.position + velocity * Time.fixedDeltaTime);


        mRunning = false;
        if (Input.GetButton("Left"))
        {
            transform.Translate(-Vector3.right * mMoveSpeed * Time.deltaTime);
            //FaceDirection(-Vector2.right);
            mRunning = true;
        }
        if (Input.GetButton("Right"))
        {
            transform.Translate(Vector3.right * mMoveSpeed * Time.deltaTime);
            //FaceDirection(Vector2.right);
            mRunning = true;
        }

        /*
        bool grounded = CheckGrounded();
        if (!mGrounded && grounded)
        {
            mLandingSound.Play();
        }
        mGrounded = grounded;
        */


        if /*(mGrounded &&*/( Input.GetButtonDown("Jump"))
        {
            mRigidBody2D.AddForce(Vector2.up * mJumpForce, ForceMode2D.Impulse);
        }
        else if (mAllowWallKick && Input.GetButtonDown("Jump"))
        {
            mRigidBody2D.velocity = Vector2.zero;
            mRigidBody2D.AddForce(Vector2.up * mJumpForce, ForceMode2D.Impulse);
           // mWallKickSound.Play();
        }



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

    void StartPos(Vector3 pos)
    {
        transform.position = pos;
        GameObject.Find("Main Camera").SendMessage("StartPos", pos);
    }
}