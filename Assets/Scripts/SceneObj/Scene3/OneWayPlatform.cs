using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    private Collider2D platformCollider;
    public LayerMask playerLayer;

    private void Start()
    {
        platformCollider = GetComponent<Collider2D>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if ((playerLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            // 角色从下方进入，允许穿透
            if (collision.transform.position.y < transform.position.y)
            {
                platformCollider.isTrigger = true;
            }
            else
            {
                platformCollider.isTrigger = false;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if ((playerLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            platformCollider.isTrigger = false;
        }
    }


    private void OnTriggerStay2D(Collider2D collider)
    {
        if ((playerLayer.value & (1 << collider.gameObject.layer)) != 0)
        {
            // 角色从下方进入，允许穿透
            if (collider.transform.position.y < transform.position.y)
            {
                platformCollider.isTrigger = true;
            }
            else
            {
                platformCollider.isTrigger = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if ((playerLayer.value & (1 << collider.gameObject.layer)) != 0)
        {
            platformCollider.isTrigger = false;
        }
    }

}
