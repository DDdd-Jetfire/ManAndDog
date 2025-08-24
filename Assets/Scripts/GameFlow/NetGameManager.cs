//using Mirror;
//using UnityEngine;

//public class NetGameManager : NetworkBehaviour
//{
//    public static NetGameManager Instance;

//    [Header("Leash Settings")]
//    public float leashRadius = 6f;            // 拴链半径（单位：米）
//    [SyncVar(hook = nameof(OnLeashStateChanged))]
//    private bool leashActive = true;          // true=拴链；false=松链

//    [SyncVar] public NetworkIdentity aIdentity;
//    [SyncVar] public NetworkIdentity bIdentity;

//    private Rigidbody2D rbB;

//    void Awake()
//    {
//        if (Instance != null && Instance != this)
//        {
//            enabled = false;
//            return;
//        }
//        Instance = this;
//    }

//    // 注册 A/B 角色
//    [Command]
//    public void CmdRegisterA(NetworkIdentity player)
//    {
//        aIdentity = player;
//        TryWireChainServer();
//    }

//    [Command]
//    public void CmdRegisterB(NetworkIdentity player)
//    {
//        bIdentity = player;
//        TryWireChainServer();
//        // 给 B 角色添加 HideFromAVisibility 组件
//        var vis = player.GetComponent<HideFromAVisibility>();
//        if (vis == null) vis = player.gameObject.AddComponent<HideFromAVisibility>();
//        vis.Initialize(this);
//    }

//    // 获取拴链状态
//    public bool IsLeashActive() => leashActive;

//    // 设置拴链状态（由 A 调用）
//    public void SetLeashActive(bool active)
//    {
//        leashActive = active;
//        ApplyLeashServerSide();
//    }

//    // 注册 A/B 角色，并根据拴链状态显示链条
//    [Server]
//    private void TryWireChainServer()
//    {
//        if (aIdentity == null || bIdentity == null) return;

//        // 让 ChainManager 记住 A/B
//        if (ChainManager.instance != null)
//        {
//            ChainManager.instance.SetA(aIdentity.gameObject);
//            ChainManager.instance.SetB(bIdentity.gameObject);
//        }

//        rbB = bIdentity.GetComponent<Rigidbody2D>();
//        ApplyLeashServerSide();
//    }

//    // 拴链状态变化时（SyncVar hook）
//    private void OnLeashStateChanged(bool oldVal, bool newVal)
//    {
//        if (bIdentity != null)
//        {
//            var hideScript = bIdentity.GetComponent<HideFromAVisibility>();
//            hideScript.UpdateVisibility();
//        }
//    }

//    // 服务器端：根据拴链状态更新链条显示和 B 的可见性
//    [Server]
//    private void ApplyLeashServerSide()
//    {
//        // 链条显示/隐藏
//        if (ChainManager.instance != null)
//        {
//            if (leashActive)
//            {
//                // 重新显示链条
//                ChainManager.instance.SetA(aIdentity ? aIdentity.gameObject : null);
//                ChainManager.instance.SetB(bIdentity ? bIdentity.gameObject : null);
//            }
//            else
//            {
//                // 隐藏链条
//                ChainManager.instance.ClearChain();
//            }
//        }

//        // 观察者更新：松链时，A 不能看见 B
//        if (bIdentity != null)
//        {
//            NetworkServer.RebuildObservers(bIdentity, initialize: false);
//        }
//    }

//    // 每帧检查 B 角色是否超出拴链半径，若超出则拉回
//    void Update()
//    {
//        if (!isServer) return;
//        if (!leashActive) return;
//        if (aIdentity == null || bIdentity == null) return;

//        Vector2 aPos = aIdentity.transform.position;
//        Vector2 bPos = bIdentity.transform.position;

//        Vector2 delta = bPos - aPos;
//        float dist = delta.magnitude;

//        // 拉回 B 角色到拴链半径内
//        if (dist > leashRadius)
//        {
//            Vector2 clamped = aPos + delta.normalized * leashRadius;
//            bIdentity.transform.position = clamped;

//            // 清除外冲的速度，防止回弹
//            if (rbB == null) rbB = bIdentity.GetComponent<Rigidbody2D>();
//            if (rbB != null) rbB.velocity = Vector2.zero;
//        }
//    }

//    // 提供 A 连接的网络连接（用于 HideFromAVisibility）
//    public NetworkConnectionToClient GetAConnection() => aIdentity ? aIdentity.connectionToClient : null;
//}
