using UnityEngine;
using System.Collections;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField]
    public Font ArcadeFont;

    public int maxHealth = 100;
    public int curHealth = 100;
    public float healthBarLength;
    GUIStyle myCustomStyle;

    int healthBarScreenRatio = 10;

    Vector3 offset;

    bool isBoss = false;
    

    // Use this for initialization
    void Start()
    {
        Vector3 boxSize = gameObject.GetComponent<BoxCollider2D>().size;
        Vector3 scale = transform.localScale;        

        offset = new Vector3(0,  boxSize.y * scale.y / 1.4f, 0);
        healthBarLength = Screen.width / healthBarScreenRatio;
        isBoss = gameObject.GetComponent<Enemy>().isBoss;
        myCustomStyle = new GUIStyle();
        myCustomStyle.font = ArcadeFont;
        myCustomStyle.normal.textColor = Color.white;

    }    

    // Update is called once per frame
    void Update()
    {
        if(!isBoss && (curHealth < maxHealth))
        {          
            AddjustCurrentHealth(0);            
        }       
    }
    
    void OnGUI()
    {
        if (!isBoss && curHealth < maxHealth)
        {
            Vector2 targetPos = Camera.main.WorldToScreenPoint(transform.position + offset);
            GUI.Box(new Rect(targetPos.x, Screen.height - targetPos.y, 60, 20), curHealth + "/" + maxHealth, myCustomStyle);
            //GUI.Box.
        }        
    }

    public void AddjustCurrentHealth(int adj)
    {
        curHealth += adj;

        if (curHealth < 0)
            curHealth = 0;        

        if (curHealth > maxHealth)
            curHealth = maxHealth;        

        if (maxHealth < 1)
            maxHealth = 1;

        healthBarLength = (Screen.width / healthBarScreenRatio) * (curHealth / (float)maxHealth);
    }

}
