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
            // �ƶ���ɫA��B����һ�����͵�
            MovePlayersToIndex(currentSpawnIndex);
        }
    }

    // ����ɫA���봥����ʱ����
    public void GoToNext()
    {
        if (isServer)
        {
            currentSpawnIndex = (currentSpawnIndex + 1) % generatePos.Count; // �л�����һ����
            // �ƶ���ɫA��B����һ�����͵�
            MovePlayersToNextPoint(ChainManager.instance.aPlayer, ChainManager.instance.bPlayer);
        }
    }

    // ����ɫA���봥����ʱ����
    public void GoToScene(int index)
    {
        currentSpawnIndex = index;
        if (isServer)
        {
            // �ƶ���ɫA��B����һ�����͵�
            MovePlayersToIndex(currentSpawnIndex);
        }
    }



    // �ƶ���ɫA��B����һ�����͵�
    [Server]
    private void MovePlayersToIndex(int index)
    {

        // �ƶ���ɫA��B���µ�λ��
        RpcMovePlayers(ChainManager.instance.aPlayer, ChainManager.instance.bPlayer, generatePos[index].position,scenePos[index].position);
    }

    
    // �ƶ���ɫA��B����һ�����͵�
    [Server]
    private void MovePlayersToNextPoint(GameObject playerA, GameObject playerB)
    {
        // �ƶ���ɫA��B���µ�λ��
        RpcMovePlayers(playerA, playerB, generatePos[currentSpawnIndex].position, scenePos[currentSpawnIndex].position);
    }

    // ����ͬ�����ͻ���
    [ClientRpc]
    private void RpcMovePlayers(GameObject playerA, GameObject playerB, Vector3 newPosition,Vector3 camPos)
    {
        playerA.transform.position = newPosition;
        playerB.transform.position = newPosition;
        cam.transform.position = camPos + new Vector3(0, 0, -10);
    }
}
