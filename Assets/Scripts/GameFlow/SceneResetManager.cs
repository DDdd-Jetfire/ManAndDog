using Mirror;
using UnityEngine;

public class SceneResetManager : NetworkBehaviour
{
    public static SceneResetManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }


    // 重置场景
    [Command(requiresAuthority = false)]
    public void CmdResetScene()
    {
        // 服务器端删除所有生成的物体
        //DestroyAllEnemies();
        // 服务器端重新生成物体
        SpawnNewEnemies();
    }

    // 在服务器端生成新的敌人
    private void SpawnNewEnemies()
    {
        foreach (var enemy in FindObjectsOfType<EnemyVisibilityController>(true))
        {
            // 只要在服务器端重置敌人
            if (isServer)
            {
                enemy.RpcResetObj(); // 调用 RpcResetObj 来同步到客户端
            }
        }
        Debug.Log("All enemies destroyed.");
    }
}
