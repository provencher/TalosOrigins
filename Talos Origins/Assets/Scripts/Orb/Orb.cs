using UnityEngine;
using System.Collections;

public class Orb : MonoBehaviour {

    public float timeToLive;
    float lifeTime;
    Transform player;

    public int type;
    public bool pickedUp = false;

    int audioTickCounter = 0;

    [SerializeField]
    public GameObject audioController;

    public int upgrade = -1;

	// Use this for initialization
	void Start () {
        lifeTime = Random.Range(5f,6f);
        timeToLive = lifeTime;
        player = GameObject.Find("Talos").transform;        
	}

    void SeekPlayer()
    {        
        if(pickedUp && audioTickCounter == 0)
        {
            GetComponent<AudioSource>().Play();
            audioTickCounter++;
        } 
        else if (pickedUp && audioTickCounter > 0)
        {
            Instantiate(audioController, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        Vector3 direction = player.position - transform.position;

        if (direction.magnitude < 6 && upgrade < 0) 
        {
            lifeTime = 10;
            transform.position += direction.normalized * 5.5f * Time.deltaTime;
        }
        else if (upgrade >= 0)
        {
            lifeTime = 999;
            transform.position += direction.normalized * Time.deltaTime;
        }
    }
	
	// Update is called once per frame
	void Update () {
        SeekPlayer();

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
