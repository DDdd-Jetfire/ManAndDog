using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : NetworkBehaviour
{
    // 单例实例
    public static EnemyManager Instance;

    // 使用 SyncVar 来同步 markedEnemy（标记的敌人）
    [SyncVar]
    private GameObject markedEnemy;

    // 注册敌人
    public void MarkEnemy(GameObject enemy)
    {
        if (isServer)
        {
            markedEnemy = enemy;  // 服务器端设置标记
        }
        else
        {
            CmdMarkEnemy(enemy);  // 客户端通过 Command 向服务器请求设置标记
        }
    }

    // 服务器端设置标记
    [Command]
    private void CmdMarkEnemy(GameObject enemy)
    {
        markedEnemy = enemy;
    }

    // 获取标记的敌人
    public GameObject GetMarkedEnemy()
    {
        return markedEnemy;
    }

    // 给特定客户端设置敌人的显示状态
    [TargetRpc]
    public void SetEnemyVisibility(NetworkConnection conn, bool visible)
    {
        GameObject enemy = GetMarkedEnemy();
        if (enemy != null)
        {
            Renderer[] renderers = enemy.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                renderer.enabled = visible; // 控制敌人的渲染
            }
        }
    }

    // 确保是单例
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
