using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DistanceCircle : MonoBehaviour {

	// Use this for initialization
    float max_distance=70;
    Player mPlayer;
    Image mImage;
    float max_size = 35.0f;
    float mDistanceRatio;
    float lastDistance = 0;

	void Start () {
        mImage = GetComponent<Image>();
        //mImage.color = Color.blue;
        mPlayer = GameObject.Find("Talos").GetComponent<Player>();
        lastDistance = mPlayer.GetExitDistance();

    }
	
	// Update is called once per frame
	void Update () {
        UpdateColor();
        Grow();        
	}

    void Grow()
    {
        float mSize = max_size * mDistanceRatio;
        //mSize = max_size * mDistanceRatio + max_size * mDistanceRatio / 3 * Mathf.Sin(2 * Mathf.PI * Time.time);
        GetComponent<RectTransform>().sizeDelta = new Vector2(mSize, mSize);
    }
     void UpdateColor()
    {
        float distance=mPlayer.GetExitDistance();

        mDistanceRatio = distance / max_distance;
        if (mDistanceRatio > 1)
        {
            mDistanceRatio = 1;
        }

        if(distance < lastDistance )
        {
            mImage.color = Color.red;
        }
        else
        {
            mImage.color = Color.blue;
        }    

        lastDistance = distance;

        //float colorcode = max_size * mDistanceRatio + max_size * mDistanceRatio / 3 * Mathf.Sin(2 * Mathf.PI * Time.time);
        //mImage.color = new Color((int)(255 * (1 - colorcode)), 0, (int)(255 * colorcode));

    }
}
