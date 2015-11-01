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
    float switchTime = 0;
   

	// Use this for initialization
	void Start () 
	{
        RandomVelocity();
    }

    void FixedUpdate()
    {
        if(switchTime == 0)
        {
            RandomVelocity();
            switchTime = Random.Range(5, 15);
        }
        else
        {
            switchTime -= Time.fixedDeltaTime;
        }
    }

    void RandomVelocity()
    {
        GetComponent<Rigidbody2D>().angularVelocity = Random.Range(minTumble, maxTumble);       //Angular movement based on random speed values

        Vector3 v = Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), Vector3.forward) * Vector3.up;
        GetComponent<Rigidbody2D>().velocity = v * speed; 						//Negative Velocity to move down towards the player ship
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
    void OnTriggerEnter2D(Collider2D other)
	{
		//Excute if the object tag was equal to one of these
		if(other.tag == "Bullet")
		{
			Instantiate (LaserGreenHit, transform.position , transform.rotation); 		//Instantiate LaserGreenHit 
			Destroy(other.gameObject);													//Destroy the Other (PlayerLaser)

			//Check the Health if greater than 0
			if(health > 0)
				health--; 																//Decrement Health by 1

			//Check the Health if less or equal 0
			if(health <= 0)
			{
				Instantiate (Explosion, transform.position , transform.rotation); 		//Instantiate Explosion
				//SharedValues_Script.score +=ScoreValue; 								//Increment score by ScoreValue
				//Destroy(gameObject); 													//Destroy the Asteroid
			}
		}
	}
}