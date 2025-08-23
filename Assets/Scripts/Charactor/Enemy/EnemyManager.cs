using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : NetworkBehaviour
{
    // ����ʵ��
    public static EnemyManager Instance;

    // ʹ�� SyncVar ��ͬ�� markedEnemy����ǵĵ��ˣ�
    [SyncVar]
    private GameObject markedEnemy;

    // ע�����
    public void MarkEnemy(GameObject enemy)
    {
        if (isServer)
        {
            markedEnemy = enemy;  // �����������ñ��
        }
        else
        {
            CmdMarkEnemy(enemy);  // �ͻ���ͨ�� Command ��������������ñ��
        }
    }

    // �����������ñ��
    [Command]
    private void CmdMarkEnemy(GameObject enemy)
    {
        markedEnemy = enemy;
    }

    // ��ȡ��ǵĵ���
    public GameObject GetMarkedEnemy()
    {
        return markedEnemy;
    }

    // ���ض��ͻ������õ��˵���ʾ״̬
    [TargetRpc]
    public void SetEnemyVisibility(NetworkConnection conn, bool visible)
    {
        GameObject enemy = GetMarkedEnemy();
        if (enemy != null)
        {
            Renderer[] renderers = enemy.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                renderer.enabled = visible; // ���Ƶ��˵���Ⱦ
            }
        }
    }

    // ȷ���ǵ���
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
