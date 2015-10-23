using UnityEngine;
using System.Collections;

public class GroundCheck : MonoBehaviour {

    public bool CheckGrounded(float radius, LayerMask whatIsGround, GameObject ignoreObject = null)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, whatIsGround);
        foreach (Collider2D col in colliders)
        {
            if (col.gameObject != ignoreObject)
            {
                return true;
            }
        }
        return false;
    }
}
