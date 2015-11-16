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
	public int health; 					//Asteroid Health (how much hit can it take)
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
   

	// Use this for initialization
	void Start () 
	{
        rigidBody = GetComponent<Rigidbody2D>();
        //RandomVelocity(Vector3.right);
        //driftTarget = GameObject.Find("Talos");
        rigidBody.angularVelocity = Random.Range(minTumble, maxTumble);
    }

    void Update()
    {
        if (false)
        {
            if ((driftTarget.transform.position - transform.position).magnitude > 500)
            {
                DestroyAsteroid();
            }
            else
            {
                if (switchTime >= 0)
                {
                    //rigidBody.velocity = RandomVelocity(Vector3.right);
                    switchTime = Random.Range(5, 10);
                }
                else
                {
                    switchTime -= Time.deltaTime;
                }
            }
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
		//Excute if the object tag was equal to one of these
		if(coll.gameObject.tag == "Bullet")
		{
			Instantiate (LaserGreenHit, transform.position , transform.rotation); 		//Instantiate LaserGreenHit 
            coll.gameObject.GetComponent<Bullet>().mAlive = false;

			//Check the Health if greater than 0
			if(health > 0)
				health--; 																//Decrement Health by 1

			//Check the Health if less or equal 0
			if(health <= 0)
			{
                if(hookedGrapple)
                {
                    GameObject.Find("Talos").GetComponent<Grapple>().unHook();
                }

				Instantiate (Explosion, transform.position , transform.rotation);       //Instantiate Explosion                                                                                                        
                DestroyAsteroid();
			}
		}
	}

    public void IsHooked()
    {
        hookedGrapple = true;
    }

    void DestroyAsteroid()
    {
        GameObject.Find("MapGenerator").SendMessage("DestroyAsteroid", index);
    }
    
}