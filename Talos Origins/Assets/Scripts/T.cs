using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class T : MonoBehaviour {

    // Use this for initialization
    Image mImage;
    PortalOpen mPortal;

    void Start()
    {
        mPortal = GameObject.Find("Talos").GetComponent<PortalOpen>();
        mImage = GetComponent<Image>();
        mImage.color = Color.grey;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateColor();
    }

    void UpdateColor()
    {
        float mTime = mPortal.getCoolTime();
        if(mTime <= 0)
        {
            mImage.color = Color.yellow;
        }
        else
        {
            mImage.color = Color.grey;
        }

    }
}
