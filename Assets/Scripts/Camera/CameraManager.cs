using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CameraManager : NetworkBehaviour
{
    public static CameraManager instance;

    public List<Transform> scenePos = new List<Transform>();

    public List<Transform> generatePos = new List<Transform>();
    [SyncVar] public int currentSpawnIndex = 0;

    public Camera cam;

    private void Awake()
    {
        if(instance == null)
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
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            scenePos.Add(gameObject.transform.GetChild(i));
            generatePos.Add(gameObject.transform.GetChild(i).GetChild(0));
        }
    }

    void Update()
    {

    }



    public void ResetScene()
    {
        if (isServer)
        {
            // 移动角色A和B到下一个传送点
            MovePlayersToIndex(currentSpawnIndex);
        }
    }

    // 当角色A进入触发器时调用
    public void GoToNext()
    {
        if (isServer)
        {
            currentSpawnIndex = (currentSpawnIndex + 1) % generatePos.Count; // 切换到下一个点
            // 移动角色A和B到下一个传送点
            MovePlayersToNextPoint(ChainManager.instance.aPlayer, ChainManager.instance.bPlayer);
        }
    }

    // 当角色A进入触发器时调用
    public void GoToScene(int index)
    {
        currentSpawnIndex = index;
        if (isServer)
        {
            // 移动角色A和B到下一个传送点
            MovePlayersToIndex(currentSpawnIndex);
        }
    }



    // 移动角色A和B到下一个传送点
    [Server]
    private void MovePlayersToIndex(int index)
    {

        // 移动角色A和B到新的位置
        RpcMovePlayers(ChainManager.instance.aPlayer, ChainManager.instance.bPlayer, generatePos[index].position,scenePos[index].position);
    }

    
    // 移动角色A和B到下一个传送点
    [Server]
    private void MovePlayersToNextPoint(GameObject playerA, GameObject playerB)
    {
        // 移动角色A和B到新的位置
        RpcMovePlayers(playerA, playerB, generatePos[currentSpawnIndex].position, scenePos[currentSpawnIndex].position);
    }

    // 用于同步到客户端
    [ClientRpc]
    private void RpcMovePlayers(GameObject playerA, GameObject playerB, Vector3 newPosition,Vector3 camPos)
    {
        playerA.transform.position = newPosition;
        playerB.transform.position = newPosition;
        cam.transform.position = camPos + new Vector3(0, 0, -10);
    }
}
