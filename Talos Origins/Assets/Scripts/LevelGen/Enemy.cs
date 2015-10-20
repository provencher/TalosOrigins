using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {   
    public enum eClass { flyer, walker, runner };
    public eClass type;

    Vector2[] possibleDirections = { Vector2.left, Vector2.right, Vector2.up, Vector2.down };

    Animator animController;
    Rigidbody2D rb;

    int currentLevel;
    int expGiven;

    LayerMask ignoreLayer, defaultLayer;

    

    void Start () {
        //Write Code for Modifying stats based on currentLevel

        //Write logic for setting enemy type
        type = eClass.flyer;
        gameObject.tag = "flyer";

        //Write logic for changing enemy sprite 

        // Initialize variables
        defaultLayer = LayerMask.NameToLayer("Enemy");
        ignoreLayer = LayerMask.NameToLayer("Ignore");

        currentLevel = 1;
        expGiven = 50 * (1 + currentLevel/2);
    }

    void Update()
    {
        // Update according to enemy class
        switch(type)
        {
            case eClass.flyer:
                {
                    FlyerUpdate();
                    break;
                }
            case eClass.runner:
                {
                    WalkerUpdate();
                    break;
                }
            case eClass.walker:
                {
                    RunnerUpdate();
                    break;
                }
        }
    }


    void FlyerUpdate()
    {
        
    }

    void WalkerUpdate()
    { }

    void RunnerUpdate()
    { }

    void SetCollisionWithPlayer(bool enabled)
    {
        if (enabled)
        {
            gameObject.tag = "Enemy";
            gameObject.layer = defaultLayer;
        }
        else
        {
            gameObject.tag = "Ignore";
            gameObject.layer = ignoreLayer;
        }
    }

    // Message Receive function to communicate level and change enemy stats
    void UpdateLevel()
    {

    }    
}
