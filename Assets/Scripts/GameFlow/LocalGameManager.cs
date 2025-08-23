using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LocalGameManager : MonoBehaviour
{


    public static LocalGameManager instance;

    public bool isAPlayer = false;

    //public GameObject aTileMap;
    public GameObject bTileMap;
    public GameObject blackBoard;

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
            //bTileMap.SetActive(false);
            blackBoard.SetActive(true);
        }
        else
        {
            blackBoard.SetActive(false);
            //aTileMap.SetActive(false);
        }
    }
}
