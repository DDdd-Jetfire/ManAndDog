using Mirror;
using UnityEngine;
using System.Collections;

public class EnemyNetworkSpawn : NetworkBehaviour
{
    public GameObject enemyPrefab; // 你需要实例化的敌人预制体
    public Transform spawnPoint;   // 敌人生成的位置

    public GameObject generateObj;

    public int sceneIndex = 0;
    private ChainManager chainManager;

    // Start is called before the first frame update
    void Start()
    {
        chainManager = FindObjectOfType<ChainManager>(); // 获取 ChainManager 组件
        if (chainManager != null)
        {
            // 启动协程，等待 playerA 和 playerB 初始化完成后再生成敌人
            StartCoroutine(WaitForPlayersAndSpawn());
        }
        else
        {
            Debug.LogError("ChainManager not found.");
        }
    }

    // 协程：等待 playerA 和 playerB 被正确赋值
    IEnumerator WaitForPlayersAndSpawn()
    {
        // 等待直到 playerA 和 playerB 被正确赋值
        while (true)
        {
            if(ChainManager.instance.aPlayer != null && ChainManager.instance.bPlayer != null && sceneIndex == CameraManager.instance.currentSpawnIndex)
            {
                break;
            }
                // 每帧等待 0.1 秒后继续检查
                yield return new WaitForSeconds(0.1f);
        }

        // 当 playerA 和 playerB 被赋值后，生成敌人
        GenerateNetObj();
    }

    // 生成敌人
    public void GenerateNetObj()
    {
        // 服务器上生成敌人
        if (isServer) // 只有服务器可以生成对象
        {
            if (spawnPoint == null) spawnPoint = gameObject.transform;
            SpawnEnemy();
        }
    }

    // 在服务器端实例化并生成敌人
    void SpawnEnemy()
    {
        if (enemyPrefab != null && spawnPoint != null)
        {
            // 实例化敌人
            GameObject enemyInstance = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            // 为敌人设置场景编号（可以不使用 SyncVar，因为是本地数据）
            //enemyInstance.GetComponent<EnemyVisibilityController>().sceneIndex = sceneIndex;

            // 通过 NetworkServer.Spawn 将敌人对象同步到所有客户端
            NetworkServer.Spawn(enemyInstance);
            generateObj = enemyInstance;
            Debug.Log("Enemy spawned and networked.");
        }
        else
        {
            Debug.LogError("Enemy prefab or spawn point is not set.");
        }
    }
}
