using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour {
    [SerializeField]
    GameObject talos;
    Vector3 reduction = new Vector3(0.15f, 0.15f, 0);

    [SerializeField]
    public GameObject rechargeAudio;
    bool rechargeDeployed = true;



	// Update is called once per frame
	void FixedUpdate () {
	    if (talos.GetComponent<Player>().mInvincibleTimer > 0)
        {
            transform.localScale = 5 * Vector3.one;
            Instantiate(rechargeAudio, transform.position, Quaternion.identity);
        }
        else
        {
            /*
            if(!rechargeDeployed)
            {
                Instantiate(rechargeAudio, transform.position, Quaternion.identity);
                rechargeDeployed = true;
            }
            */

            if (transform.localScale.x > 0)
            {
                transform.localScale -= reduction;
            }

        }
        transform.rotation = talos.transform.rotation;
        transform.position = talos.transform.position;
    }
}
