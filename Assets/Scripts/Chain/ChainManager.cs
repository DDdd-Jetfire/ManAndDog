using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ChainManager : NetworkBehaviour
{
    public static ChainManager instance;

    [Header("Chain Settings")]
    public GameObject ringPrefab;
    public float ringDistance = 1f;
    [SyncVar(hook = nameof(OnChainChanged))] public bool isChain = false; // ��� hook

    [SyncVar(hook = nameof(OnAChanged))] public GameObject aPlayer;
    [SyncVar(hook = nameof(OnBChanged))] public GameObject bPlayer;

    private readonly List<GameObject> rings = new();
    public float minDistanceFromA = 1.2f;
    public float maxDistanceFromA = 5f;

    public bool init = false;

    [SyncVar(hook = nameof(OnNearChanged))] public bool isNear = false; // ��� hook
    [SyncVar] public Vector2? isShooting = null; // ��� hook



    [SyncVar(hook = nameof(OnNearChanged))] public bool isSmokeing = false; // ��� hook

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning("Duplicate ChainManager found. Disabling this component.");
            enabled = false;
            return;
        }
        instance = this;
    }

    // SyncVar hook for isChain
    private void OnChainChanged(bool oldValue, bool newValue)
    {
        // �ڿͻ��˴�����״̬�仯���������UI��Ч��
        Debug.Log($"Chain state changed to: {newValue}");
    }
    // SyncVar hook for isChain
    private void OnNearChanged(bool oldValue, bool newValue)
    {
        // �ڿͻ��˴�����״̬�仯���������UI��Ч��
        Debug.Log($"Chain state changed to: {newValue}");
    }
    // SyncVar hook for isChain
    private void OnSmokeChanged(bool oldValue, bool newValue)
    {
        // �ڿͻ��˴�����״̬�仯���������UI��Ч��
        Debug.Log($"smoke state changed to: {newValue}");
    }

    private void OnShootChanged(bool oldValue, Vector2? newValue)
    {
        // �ڿͻ��˴�����״̬�仯���������UI��Ч��
        Debug.Log($"Chain state changed to: {newValue}");
    }

    // Command: �ͻ��˵��ã�������ִ��
    [Command(requiresAuthority = false)]
    public void CmdUpdateShoot(Vector2? pos)
    {
        isShooting = pos;

        // ʹ�� Rpc ��ֵͬ�������пͻ���
        //RpcUpdatebit(flag);
    }

    [Command(requiresAuthority = false)]
    public void CmdUpdateSmoke(bool flag)
    {
        isSmokeing = flag;
    }

    [ClientRpc]
    public void RpcResetShooting()
    {
        isShooting = null;
    }

    [Command(requiresAuthority = false)]
    public void RedoAlertAndTo(int index)
    {
        // �ڷ������˵��� Rpc��֪ͨ�ͻ���������
        RpcResetShooting();

        // Ȼ���л�����
        CameraManager.instance.GoToScene(index);
    }

    [Command(requiresAuthority = false)]
    public void ResetCurrentScene()
    {
        isShooting = null;
        CameraManager.instance.ResetScene();
        SceneResetManager.instance.CmdResetScene();

        // ʹ�� Rpc ��ֵͬ�������пͻ���
        //RpcUpdatebit(flag);
    }

    [Server] public void SetA(GameObject go) { aPlayer = go; TryRebuildServer(); }
    [Server] public void SetB(GameObject go) { bPlayer = go; TryRebuildServer(); }

    private void OnAChanged(GameObject _, GameObject __) { /* ��ѡ */ }
    private void OnBChanged(GameObject _, GameObject __) { /* ��ѡ */ }

    [Server]
    private void TryRebuildServer()
    {
        if (aPlayer == null || bPlayer == null) return;
        if (!init)
        {
            AudioManager.instance.CmdPlayBackgroundMusic();
            CameraManager.instance.GoToScene(0);
            init = true;
        }
        rings.Clear();
        Vector2 start = aPlayer.transform.position;
        Vector2 end = bPlayer.transform.position;
        float dist = Vector2.Distance(start, end);
        int count = 8;

        if (rings.Count != count)
        {
            ClearRingsServer();
            BuildRingsServer(count);
            isChain = true; // ֱ���ڷ��������� SyncVar
        }

        RepositionRingsServer(start, end);
    }

    [Server]
    private void BuildRingsServer(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var ring = Instantiate(ringPrefab, Vector3.zero, Quaternion.identity);
            NetworkServer.Spawn(ring);
            rings.Add(ring);
        }
    }

    [Server]
    private void ClearRingsServer()
    {
        for (int i = 0; i < rings.Count; i++)
        {
            if (rings[i] != null)
                NetworkServer.Destroy(rings[i]);
        }
        rings.Clear();
    }

    [Server]
    public void ClearChain()
    {
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
            float t = last == 0 ? 0f : (float)i / last;
            Vector2 pos = start + dir * t;
            if (rings[i] != null) rings[i].transform.position = pos;
        }
    }

    private void Update()
    {
        if (!isServer) return;
        if (aPlayer == null || bPlayer == null) return;

        float distance = Vector2.Distance(aPlayer.transform.position, bPlayer.transform.position);
        if (distance < minDistanceFromA)
        {
            isNear = true;
        }
        else
        {
            isNear = false;
        }
        if (distance > maxDistanceFromA)
        {
            isChain = false; // ֱ���ڷ��������� SyncVar
            ClearChain();
            return;
        }

        if (rings.Count == 0) return;

        RepositionRingsServer(aPlayer.transform.position, bPlayer.transform.position);
    }
}