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


    // ���ó���
    [Command(requiresAuthority = false)]
    public void CmdResetScene()
    {
        // ��������ɾ���������ɵ�����
        //DestroyAllEnemies();
        // ��������������������
        SpawnNewEnemies();
    }

    // �ڷ������������µĵ���
    private void SpawnNewEnemies()
    {
        foreach (var enemy in FindObjectsOfType<EnemyVisibilityController>(true))
        {
            // ֻҪ�ڷ����������õ���
            if (isServer)
            {
                enemy.RpcResetObj(); // ���� RpcResetObj ��ͬ�����ͻ���
            }
        }
        Debug.Log("All enemies destroyed.");
    }
}
