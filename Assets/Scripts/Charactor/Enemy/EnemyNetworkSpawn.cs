using Mirror;
using UnityEngine;

public class EnemyNetworkSpawn : NetworkBehaviour
{
    public GameObject enemyPrefab; // ����Ҫʵ�����ĵ���Ԥ����
    public Transform spawnPoint;   // �������ɵ�λ��

    // Start is called before the first frame update
    void Start()
    {
        // �����������ɵ���
        if (isServer) // ֻ�з������������ɶ���
        {
            if (spawnPoint == null) spawnPoint = gameObject.transform;
            SpawnEnemy();
        }
    }

    // �ڷ�������ʵ���������ɵ���
    void SpawnEnemy()
    {
        if (enemyPrefab != null && spawnPoint != null)
        {
            // ʵ��������
            GameObject enemyInstance = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            // ͨ�� NetworkServer.Spawn �����˶���ͬ�������пͻ���
            NetworkServer.Spawn(enemyInstance);

            Debug.Log("Enemy spawned and networked.");
        }
        else
        {
            Debug.LogError("Enemy prefab or spawn point is not set.");
        }
    }
}
