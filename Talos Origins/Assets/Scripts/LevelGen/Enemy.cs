using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    // Maybe change enemy types?
    public enum eClass { flyer, walker, runner };
    public eClass type;

    int currentLevel;

    int expGiven;
        
    void Start () {
        //Write Code for Modifying stats based on currentLevel


	    //Write logic for setting enemy type

        //Write logic for changing enemy sprite 

	}	
	
	void Update () {
	
	}


    void FlyerUpdate()
    { }

    void WalkerUpdate()
    { }

    void RunnerUpdate()
    { } 
    
}
