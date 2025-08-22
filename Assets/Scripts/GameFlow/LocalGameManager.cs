using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LocalGameManager : MonoBehaviour
{


    public static LocalGameManager instance;

    public bool isAPlayer = false;

    public GameObject aTileMap;
    public GameObject bTileMap;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }


    public void InitSystem(bool isA)
    {
        isAPlayer = isA;
        if (isAPlayer)
        {
            bTileMap.SetActive(false);
        }
        else
        {
            aTileMap.SetActive(false);
        }
    }
}
