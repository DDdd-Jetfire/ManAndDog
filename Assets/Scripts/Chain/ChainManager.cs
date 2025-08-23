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

    // 添加一个最大距离，用来限制B与A之间的最大距离
    public float maxDistanceFromA = 5f; // 这里设置一个示例值，你可以根据需要调整

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

    // 更新方法，限制B的移动范围
    private void Update()
    {
        if (!isServer) return;  // 只有服务器执行
        if (aPlayer == null || bPlayer == null) return;

        // 获取A和B之间的距离
        float distance = Vector2.Distance(aPlayer.transform.position, bPlayer.transform.position);

        // 如果B距离A太远，限制B的位置
        if (distance > maxDistanceFromA)
        {
            //// 计算从A到B的方向
            //Vector2 direction = (bPlayer.transform.position - aPlayer.transform.position).normalized;
            //
            //// 计算B的新位置，确保B不会超过最大距离
            //Vector2 newPosition = (Vector2)aPlayer.transform.position + direction * maxDistanceFromA;
            //
            //// 移动B到新的位置
            //bPlayer.transform.position = newPosition;
        }

        // 更新链条环的位置，确保环跟随A和B的移动（这一部分保持不变）
        RepositionRingsServer(aPlayer.transform.position, bPlayer.transform.position);
    }
}
