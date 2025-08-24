using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    AniBase ani;
    void Start()
    {
        ani = GetComponent<AniBase>();
        ani.TransAction("idle");
    }

    void Update()
    {
        if (ChainManager.instance.isSmokeing)
        {
            ani.TransAction("show");
        }
    }
}
