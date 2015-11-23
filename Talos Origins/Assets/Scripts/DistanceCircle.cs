using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DistanceCircle : MonoBehaviour {

	// Use this for initialization
    float max_distance=50;
    Player mPlayer;
    Image mImage;
    float max_size = 40.0f;
    
	void Start () {
        mImage = GetComponent<Image>();
        mImage.color = Color.blue;
        mPlayer = GameObject.Find("Talos").GetComponent<Player>();
    }
	
	// Update is called once per frame
	void Update () {
        Grow();
        UpdateColor();
	}

    void Grow()
    {
        float mSize = max_size*0.75f + max_size*0.25f * Mathf.Sin(2 * Mathf.PI * Time.time);
        GetComponent<RectTransform>().sizeDelta = new Vector2(mSize, mSize);
    }
     void UpdateColor()
    {
        float distance=mPlayer.GetExitDistance();
        float distanceRatio = distance / max_distance;
        if (distanceRatio > 1)
        {
            distanceRatio = 1;
        }
        mImage.color = new Color((int)(255 * (1 - distanceRatio)), 0, (int)(255 * distanceRatio));
    }
}
