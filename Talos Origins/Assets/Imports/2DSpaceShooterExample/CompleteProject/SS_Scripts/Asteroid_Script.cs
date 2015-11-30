/// <summary>
/// 2D Space Shooter Example
/// By Bug Games www.Bug-Games.net
/// Programmer: Danar Kayfi - Twitter: @DanarKayfi
/// Special Thanks to Kenney for the CC0 Graphic Assets: www.kenney.nl
/// 
/// This is the Asteroid Script:
/// - Normal & Angular Velocity
/// - Hit/Explosion on Trigger Enter
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class Asteroid_Script : MonoBehaviour 
{
	//Public Var
	public float maxTumble; 			//Maximum Speed of the angular velocity
	public float minTumble; 			//Minimum Speed of the angular velocity
	public float speed; 				//Asteroid Speed
	public int health = 2; 					//Asteroid Health (how much hit can it take)
	public GameObject LaserGreenHit; 	//LaserGreenHit Prefab
	public GameObject Explosion; 		//Explosion Prefab
	public int ScoreValue; 				//How much the Asteroid give score after explosion
    int currentLevel;
    int index;
    float switchTime = 5;

    Rigidbody2D rigidBody;

    Vector3 lastDirection;

    GameObject driftTarget;

    bool hookedGrapple = false;

    public float mScaleValue = 1;
    bool init = false;
   

	// Use this for initialization
	void Start () 
	{        
        // rigidBody = GetComponent<Rigidbody2D>();
        //RandomVelocity(Vector3.right);
        //driftTarget = GameObject.Find("Talos");
        //rigidBody.angularVelocity = Random.Range(minTumble, maxTumble);

        Vector3 angle = transform.eulerAngles;// = Random.Range(0, 360);
        angle.z = Random.Range(0, 360);
        transform.eulerAngles = angle;
    }

    void FixedUpdate()
    {
        if(!init)
        {
            health = Mathf.CeilToInt(2 * mScaleValue);
            init = true;
        }       


        if(health <=0)
        {
            DestroyAsteroid();
        }
    }

    Vector3 RandomVelocity(Vector3 direction)
    {
        direction.x *= Random.Range(0.51f, 1.0f);
        direction.y *= Random.Range(0.51f, 1.0f);

        Vector3 up = transform.up * Random.Range(0.51f, 1.0f);


        Vector3 v = Quaternion.AngleAxis(Random.Range(0.0f, 360), direction) * Vector3.Cross(up, transform.right);
        						//Negative Velocity to move down towards the player ship
        return v * speed;
    }   

    void UpdateAsteroidIndex(int ind)
    {
        index = ind;
    }

    void UpdateLevel(int lvl)
    {
        currentLevel = lvl;
    }

    //Called when the Trigger entered
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Enemy")
        {
            if (coll.gameObject.GetComponent<Enemy>().type == Enemy.eClass.flyer)
            {
                health -= Mathf.CeilToInt(coll.gameObject.GetComponent<Enemy>().mScaleValue);
                //Check the Health if less or equal 0
                if (health <= 0)
                {
                    DestroyAsteroid();
                }
            }


        }

        //Excute if the object tag was equal to one of these
        else if (coll.gameObject.tag == "Bullet")
		{
			Instantiate (LaserGreenHit, transform.position , transform.rotation); 		//Instantiate LaserGreenHit 
            coll.gameObject.GetComponent<Bullet>().mAlive = false;

			//Check the Health if greater than 0			
			health--; 																//Decrement Health by 1

			//Check the Health if less or equal 0
			if(health <= 0)
			{                                                                                                                    
                DestroyAsteroid();
			}
		}
        
	}

    void DropOrbs(int numOrbs)
    {
        for (int i = 0; i < numOrbs; i++)
        {
            gameObject.GetComponentInParent<OrbController>().SpawnOrb(Random.Range(2, 4));
        }

    }

    public void IsHooked()
    {
        hookedGrapple = true;
    }

    public void DestroyAsteroid()
    {
        if (hookedGrapple)
        {
            GameObject.Find("Talos").GetComponent<Grapple>().unHook();
        }

        DropOrbs(Random.Range(1, Mathf.CeilToInt(mScaleValue) + 1));

        Instantiate(Explosion, transform.position, transform.rotation);       //Instantiate Explosion    

        GameObject.Find("MapGenerator").SendMessage("DestroyAsteroid", index);
    }
    
}