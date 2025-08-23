using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ChainManager : NetworkBehaviour
{
    public static ChainManager instance;

    [Header("Chain Settings")]
    public GameObject ringPrefab;     // 必须：NetworkIdentity + NetworkTransform
    public float ringDistance = 1f;

    [SyncVar(hook = nameof(OnAChanged))] public GameObject aPlayer;
    [SyncVar(hook = nameof(OnBChanged))] public GameObject bPlayer;

    // 服务器维护的环列表（只在服务器赋值/使用）
    private readonly List<GameObject> rings = new();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning("Duplicate ChainManager found. Disabling this component.");
            enabled = false; // 不要 Destroy 带 NetworkIdentity 的场景对象
            return;
        }
        instance = this;
    }

    // —— 只有服务器能写入 SyncVar ——
    [Server] public void SetA(GameObject go) { aPlayer = go; TryRebuildServer(); }
    [Server] public void SetB(GameObject go) { bPlayer = go; TryRebuildServer(); }

    // SyncVar hook（客户端触发，用于本地依赖刷新；这里无需做生成）
    private void OnAChanged(GameObject _, GameObject __) { /* 可选：做本地UI提示等 */ }
    private void OnBChanged(GameObject _, GameObject __) { /* 可选：做本地UI提示等 */ }

    // —— 服务器：当 A/B 到齐或距离变化较大时重建环 —— 
    [Server]
    private void TryRebuildServer()
    {
        if (aPlayer == null || bPlayer == null) return;

        // 计算需要的环数量（两端都放一个更美观）
        Vector2 start = aPlayer.transform.position;
        Vector2 end = bPlayer.transform.position;
        float dist = Vector2.Distance(start, end);
        //int count = Mathf.Max(2, Mathf.CeilToInt(dist / ringDistance) + 1);
        int count = 8;

        // 如果数量变化，重建
        if (rings.Count != count)
        {
            ClearRingsServer();          // 用 NetworkServer.Destroy 清理旧环
            BuildRingsServer(count);     // 生成新环并 Spawn
        }

        // 立即摆放到位（NetworkTransform 将把位置同步到各客户端）
        RepositionRingsServer(start, end);
    }

    [Server]
    private void BuildRingsServer(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var ring = Instantiate(ringPrefab, Vector3.zero, Quaternion.identity);
            NetworkServer.Spawn(ring);     // 前提：ringPrefab 已注册为 Spawnable
            rings.Add(ring);
        }
    }

    [Server]
    private void ClearRingsServer()
    {
        for (int i = 0; i < rings.Count; i++)
        {
            if (rings[i] != null)
                NetworkServer.Destroy(rings[i]); // 服务器销毁 → 客户端自动销毁
        }
        rings.Clear();
    }

    // 额外添加 ClearChain() 方法
    // 用于在松链或其他需要清理链条的场景中调用
    [Server]
    public void ClearChain()
    {
        // 销毁所有已生成的环
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
            float t = last == 0 ? 0f : (float)i / last;  // 端点也占位
            Vector2 pos = start + dir * t;
            if (rings[i] != null) rings[i].transform.position = pos;
        }
        // 有 NetworkTransform，位置会自动同步给所有客户端
    }

    // —— 服务器每帧驱动位置跟随（也可用定时器/阈值减少频率） —— 
    private void Update()
    {
        if (!isServer) return;
        if (aPlayer == null || bPlayer == null) return;

        // 如果 A/B 在动，这里每帧更新环位置（成本取决于环数量）
        RepositionRingsServer(aPlayer.transform.position, bPlayer.transform.position);
    }
}
