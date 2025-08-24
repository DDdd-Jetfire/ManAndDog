using Mirror;
using UnityEngine;

public class TrainSpawner : NetworkBehaviour
{
    public GameObject trainPrefab; // ����Ҫʵ�����ĵ���Ԥ����
    public Transform spawnPoint;   // �������ɵ�λ��

    public GameObject generateObj;


    public int sceneIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        GenerateNetObj();
    }

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
        if (trainPrefab != null && spawnPoint != null)
        {
            // ʵ��������
            GameObject instance = Instantiate(trainPrefab, spawnPoint.position, spawnPoint.rotation);

            //enemyInstance.GetComponent<EnemyVisibilityController>().sceneIndex = sceneIndex;
            // ͨ�� NetworkServer.Spawn �����˶���ͬ�������пͻ���
            NetworkServer.Spawn(instance);
            generateObj = instance;
            Debug.Log("Enemy spawned and networked.");
        }
        else
        {
            Debug.LogError("Enemy prefab or spawn point is not set.");
        }
    }
}
