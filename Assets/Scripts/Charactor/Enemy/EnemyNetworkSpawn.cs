using Mirror;
using UnityEngine;
using System.Collections;

public class EnemyNetworkSpawn : NetworkBehaviour
{
    public GameObject enemyPrefab; // ����Ҫʵ�����ĵ���Ԥ����
    public Transform spawnPoint;   // �������ɵ�λ��

    public GameObject generateObj;

    public int sceneIndex = 0;
    private ChainManager chainManager;

    // Start is called before the first frame update
    void Start()
    {
        chainManager = FindObjectOfType<ChainManager>(); // ��ȡ ChainManager ���
        if (chainManager != null)
        {
            // ����Э�̣��ȴ� playerA �� playerB ��ʼ����ɺ������ɵ���
            StartCoroutine(WaitForPlayersAndSpawn());
        }
        else
        {
            Debug.LogError("ChainManager not found.");
        }
    }

    // Э�̣��ȴ� playerA �� playerB ����ȷ��ֵ
    IEnumerator WaitForPlayersAndSpawn()
    {
        // �ȴ�ֱ�� playerA �� playerB ����ȷ��ֵ
        while (true)
        {
            if(ChainManager.instance.aPlayer != null && ChainManager.instance.bPlayer != null && sceneIndex == CameraManager.instance.currentSpawnIndex)
            {
                break;
            }
                // ÿ֡�ȴ� 0.1 ���������
                yield return new WaitForSeconds(0.1f);
        }

        // �� playerA �� playerB ����ֵ�����ɵ���
        GenerateNetObj();
    }

    // ���ɵ���
    public void GenerateNetObj()
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

            // Ϊ�������ó�����ţ����Բ�ʹ�� SyncVar����Ϊ�Ǳ������ݣ�
            //enemyInstance.GetComponent<EnemyVisibilityController>().sceneIndex = sceneIndex;

            // ͨ�� NetworkServer.Spawn �����˶���ͬ�������пͻ���
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
