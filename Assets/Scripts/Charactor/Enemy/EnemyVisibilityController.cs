using Mirror;
using UnityEngine;
using System.Collections;

public class EnemyVisibilityController : NetworkBehaviour
{
    [SerializeField] Renderer[] renderersToToggle;
    //[SerializeField] Collider2D[] collidersToToggle;

    public AniBase ani;

    [SyncVar]public bool bitFlag = false;

    public HumanController hc;
    [SyncVar] public Vector2? hitPos;

    void Awake()
    {
        // 兜底收集
        if (renderersToToggle == null || renderersToToggle.Length == 0)
            renderersToToggle = GetComponentsInChildren<Renderer>(true);
        ani = gameObject.GetComponent<AniBase>();
       // if (collidersToToggle == null || collidersToToggle.Length == 0)
       //     collidersToToggle = GetComponentsInChildren<Collider2D>(true);
    }

    private void OnEnable()
    {
        //LocalSetVisible(false); // 本地立即隐藏
    }

    // —— 关键 1：客户端一生成就先隐藏，避免“首帧闪现” —— //
    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    // —— 关键 2：服务器在合适时机，给 A 隐藏、给 B 显示 —— //
    public override void OnStartServer()
    {
        base.OnStartServer();
        // 等待玩家 ready（可能敌人先于玩家 spawn）
        //StartCoroutine(InitVisibilityForAB());
    }
    IEnumerator InitVisibilityForAB()
    {
        float t = 0f;
        NetworkConnectionToClient connA = null, connB = null;

        while (t < 2f && (connA == null || connB == null))
        {
            (connA, connB) = FindABConnections();
            //t += Time.unscaledDeltaTime;
            yield return null;
        }

        // 输出调试信息，确认 connA 和 connB 是否正确
        Debug.Log($"connA: {connA}, connB: {connB}");

        // 初始策略：对 A 继续隐藏、对 B 显示
        if (connA != null) TargetSetVisible(connA, false);
        if (connB != null) TargetSetVisible(connB, true);
    }


    public void GetBit()
    {
        CmdUpdateBit(true);
    }

    // Command: 客户端调用，服务器执行
    [Command(requiresAuthority = false)]
    public void CmdUpdateBit(bool flag)
    {
        bitFlag = flag;

        // 使用 Rpc 将值同步到所有客户端
        //RpcUpdatebit(flag);
    }


    // ClientRpc: 服务器调用，所有客户端执行
    [ClientRpc]
    void RpcUpdatebit(bool flag)
    {
        // 客户端更新
        bitFlag = flag;
    }

    public void StopBit()
    {
        //CmdUpdateBit(false);
    }

    void Update()
    {
        if (hitPos != null)
        {
            ani.PlayOneShoot("die");
        }
        if (hc == null)
        {
            hc = FindObjectOfType<HumanController>();
        }
        else
        {
            if (bitFlag)
            {
                gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "ShowEnemy";
            }
            else
            {

                gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "UnShowEnemy";
            }

            if (bitFlag)hc.evc = this;
        }

        if (bitFlag)
        {
            ani.TransAction("bit");
        }
        else
        {
            ani.TransAction("idle");
        }
    }


    // Command: 客户端调用，服务器执行
    [Command(requiresAuthority = false)]
    public void KillEnemy(Vector2 dir)
    {
        hitPos = dir;
    }


    public void Die()
    {
        gameObject.SetActive(false);
    }

    // ===== 可复用的查找逻辑 =====
    (NetworkConnectionToClient, NetworkConnectionToClient) FindABConnections()
    {
        NetworkConnectionToClient a = null, b = null;
        foreach (var conn in NetworkServer.connections.Values)
        {
            if (conn == null || !conn.isReady || conn.identity == null) continue;
            var pc = conn.identity.GetComponent<PlayerController>();
            if (pc == null) continue;
            if (pc.isPlayerA) a = conn; else b = conn;
        }
        return (a, b);
    }

    // ===== 本地立即生效的显示/隐藏（用于防闪帧） =====
    void LocalSetVisible(bool visible)
    {
        if (renderersToToggle != null)
            foreach (var r in renderersToToggle) if (r) r.enabled = visible;
       // if (collidersToToggle != null)
       //     foreach (var c in collidersToToggle) if (c) c.enabled = visible;
    }

    // ===== 定向给某个客户端切换可见性 =====
    [TargetRpc]
    void TargetSetVisible(NetworkConnection target, bool visible)
    {
        LocalSetVisible(visible);
    }
}
