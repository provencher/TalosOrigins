/// <summary>
/// 2D Space Shooter Example
/// By Bug Games www.Bug-Games.net
/// Programmer: Danar Kayfi - Twitter: @DanarKayfi
/// Special Thanks to Kenney for the CC0 Graphic Assets: www.kenney.nl
/// 
/// This is the Explosions Script:
/// - Play Explosion Sound
/// 
/// </summary>

using UnityEngine;
using System.Collections;




public class explosions_script : MonoBehaviour 
{

    public float timeToLive = 2.0f;
    // Use this for initialization
    void Start () 
	{
		GetComponent<AudioSource>().Play(); //Play Explosion Sound
	}



    // Update is called once per frame
    void Update()
    {
        if (timeToLive > 0)
        {
            timeToLive -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
