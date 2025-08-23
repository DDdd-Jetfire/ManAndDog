using Mirror;
using UnityEngine;

public class EnemyNetworkSpawn : NetworkBehaviour
{
    public GameObject enemyPrefab; // 你需要实例化的敌人预制体
    public Transform spawnPoint;   // 敌人生成的位置

    // Start is called before the first frame update
    void Start()
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

            // 通过 NetworkServer.Spawn 将敌人对象同步到所有客户端
            NetworkServer.Spawn(enemyInstance);

            Debug.Log("Enemy spawned and networked.");
        }
        else
        {
            Debug.LogError("Enemy prefab or spawn point is not set.");
        }
    }
}
