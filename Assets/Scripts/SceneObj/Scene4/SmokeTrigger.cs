using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.GetComponent<HumanController>() != null)
            {
                ChainManager.instance.CmdUpdateSmoke(true);
            }
        }
    }
}
