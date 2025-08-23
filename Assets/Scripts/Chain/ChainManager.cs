using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ChainManager : NetworkBehaviour
{
    public static ChainManager instance;

    [Header("Chain Settings")]
    public GameObject ringPrefab;     // ���룺NetworkIdentity + NetworkTransform
    public float ringDistance = 1f;

    [SyncVar(hook = nameof(OnAChanged))] public GameObject aPlayer;
    [SyncVar(hook = nameof(OnBChanged))] public GameObject bPlayer;

    // ������ά���Ļ��б�ֻ�ڷ�������ֵ/ʹ�ã�
    private readonly List<GameObject> rings = new();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning("Duplicate ChainManager found. Disabling this component.");
            enabled = false; // ��Ҫ Destroy �� NetworkIdentity �ĳ�������
            return;
        }
        instance = this;
    }

    // ���� ֻ�з�������д�� SyncVar ����
    [Server] public void SetA(GameObject go) { aPlayer = go; TryRebuildServer(); }
    [Server] public void SetB(GameObject go) { bPlayer = go; TryRebuildServer(); }

    // SyncVar hook���ͻ��˴��������ڱ�������ˢ�£��������������ɣ�
    private void OnAChanged(GameObject _, GameObject __) { /* ��ѡ��������UI��ʾ�� */ }
    private void OnBChanged(GameObject _, GameObject __) { /* ��ѡ��������UI��ʾ�� */ }

    // ���� ���������� A/B ��������仯�ϴ�ʱ�ؽ��� ���� 
    [Server]
    private void TryRebuildServer()
    {
        if (aPlayer == null || bPlayer == null) return;

        // ������Ҫ�Ļ����������˶���һ�������ۣ�
        Vector2 start = aPlayer.transform.position;
        Vector2 end = bPlayer.transform.position;
        float dist = Vector2.Distance(start, end);
        //int count = Mathf.Max(2, Mathf.CeilToInt(dist / ringDistance) + 1);
        int count = 8;

        // ��������仯���ؽ�
        if (rings.Count != count)
        {
            ClearRingsServer();          // �� NetworkServer.Destroy ����ɻ�
            BuildRingsServer(count);     // �����»��� Spawn
        }

        // �����ڷŵ�λ��NetworkTransform ����λ��ͬ�������ͻ��ˣ�
        RepositionRingsServer(start, end);
    }

    [Server]
    private void BuildRingsServer(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var ring = Instantiate(ringPrefab, Vector3.zero, Quaternion.identity);
            NetworkServer.Spawn(ring);     // ǰ�᣺ringPrefab ��ע��Ϊ Spawnable
            rings.Add(ring);
        }
    }

    [Server]
    private void ClearRingsServer()
    {
        for (int i = 0; i < rings.Count; i++)
        {
            if (rings[i] != null)
                NetworkServer.Destroy(rings[i]); // ���������� �� �ͻ����Զ�����
        }
        rings.Clear();
    }

    // ������� ClearChain() ����
    // ������������������Ҫ���������ĳ����е���
    [Server]
    public void ClearChain()
    {
        // �������������ɵĻ�
        ClearRingsServer();
    }

    [Server]
    private void RepositionRingsServer(Vector2 start, Vector2 end)
    {
        if (rings.Count == 0) return;
        Vector2 dir = end - start;
        int last = rings.Count - 1;

        for (int i = 0; i < rings.Count; i++)
        {
            float t = last == 0 ? 0f : (float)i / last;  // �˵�Ҳռλ
            Vector2 pos = start + dir * t;
            if (rings[i] != null) rings[i].transform.position = pos;
        }
        // �� NetworkTransform��λ�û��Զ�ͬ�������пͻ���
    }

    // ���� ������ÿ֡����λ�ø��棨Ҳ���ö�ʱ��/��ֵ����Ƶ�ʣ� ���� 
    private void Update()
    {
        if (!isServer) return;
        if (aPlayer == null || bPlayer == null) return;

        // ��� A/B �ڶ�������ÿ֡���»�λ�ã��ɱ�ȡ���ڻ�������
        RepositionRingsServer(aPlayer.transform.position, bPlayer.transform.position);
    }
}
