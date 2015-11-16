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

    float portalCoolDown = 5.0f;
    float coolDownTime = 0;

  

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
        portalInstance.transform.position = transform.position;
        Instantiate(portalCloseAudio, transform.position, Quaternion.identity);
        if (leaving)
        {
            GameObject.Find("MapGenerator").GetComponent<MapGenerator>().cycleLevel = true;
            portalInstance.transform.position = transform.position;
            yield return new WaitForSeconds(Time.deltaTime);
        
        }
       
        Vector3 portalScale = portalInstance.transform.localScale;

        while (portalScale.x > 0)
        {
            portalInstance.transform.position = transform.position;

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
	    if(Input.GetButtonDown("MapGeneration") && coolDownTime <= 0)
        {
            coolDownTime = portalCoolDown;
            StartCoroutine(SpawnPortal(true));
        }
        else if (coolDownTime > 0)
        {
            coolDownTime -= Time.deltaTime;
        }
	}
}
