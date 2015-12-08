using UnityEngine;
using System.Collections;

public class PortalOpen : MonoBehaviour {
    
    [SerializeField]
    GameObject portal;

    [SerializeField]
    GameObject portalOpenAudio;

    [SerializeField]
    GameObject portalCloseAudio;

    GameObject portalInstance;

    float portalCoolDown = 12.0f;
    public float coolDownTime = 0;
	int portalCoolDownLevel;
    public bool portalOpen = false;

    //public bool useNewPosition = false;
    public Vector3 newPositionOffset = Vector3.zero;
  

    public IEnumerator SpawnPortal(bool leaving)
    {
        Instantiate(portalOpenAudio, transform.position, Quaternion.identity);
        portalInstance = (GameObject)Instantiate(portal, transform.position, Quaternion.identity);

		Vector3 portalScale = new Vector3(0, 0, 0);

        while(portalScale.x < 1.5f)
        {
            portalInstance.transform.position = transform.position;

            portalScale.x += 0.1f;
            portalScale.y += 0.1f;

            portalInstance.transform.localScale = portalScale;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        StartCoroutine(DestroyPortal(leaving));

        yield break;
    }

    public IEnumerator DestroyPortal(bool leaving)
    {       
        transform.position += newPositionOffset;
        newPositionOffset = Vector3.zero;
      
        portalInstance.transform.position = transform.position;

        Instantiate(portalCloseAudio, transform.position, Quaternion.identity);
        if (leaving)
        {
            GameObject.Find("MapGenerator").GetComponent<MapGenerator>().cycleLevel = true;            
            yield return new WaitForSeconds(Time.deltaTime);
        
        }
        
        Vector3 portalScale = portalInstance.transform.localScale;

        while (portalScale.x > 0)
        {           
            portalScale.x -= 0.1f;
            portalScale.y -= 0.1f;

            portalInstance.transform.localScale = portalScale;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        Destroy(portalInstance);
        portalInstance = null;                        
        

        yield break;
    }
	
	// Update is called once per frame
	void Update () {
	    if((Input.GetButtonDown("MapGeneration") || portalOpen) /*|| Input.GetKeyDown(KeyCode.Mouse3)*/ && coolDownTime <= 0 )
        {
			portalCoolDownLevel = PlayerPrefs.GetInt ("Portal Cooldown", 0);
            coolDownTime = portalCoolDown - (float)portalCoolDownLevel;
			//Debug.Log (coolDownTime);
            StartCoroutine(SpawnPortal(!portalOpen));
            portalOpen = false;
        }
        else if (coolDownTime > 0)
        {
            coolDownTime -= Time.deltaTime;
        }
	}

    public float getCoolTime()
    {
        return coolDownTime;
    }

}
