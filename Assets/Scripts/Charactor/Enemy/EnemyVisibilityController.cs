using Mirror;
using UnityEngine;
using System.Collections;

public class EnemyVisibilityController : NetworkBehaviour
{
    [SerializeField] Renderer[] renderersToToggle;
    //[SerializeField] Collider2D[] collidersToToggle;

    public AniBase ani;

    [SyncVar]public bool bitFlag = false;
    [SyncVar] public bool showFlag = false;

    public HumanController hc;
    [SyncVar] public Vector2? hitPos;

    public float maxAlertTime = 2f;
    public float currentAlertTime = 0f;
    public bool isAlert = false;

    public float sceneIndex = 0;

    //public bool canAlert

    void Awake()
    {
        // 兜底收集
        if (renderersToToggle == null || renderersToToggle.Length == 0)
            renderersToToggle = GetComponentsInChildren<Renderer>(true);
        ani = gameObject.GetComponent<AniBase>();
        // if (collidersToToggle == null || collidersToToggle.Length == 0)
        //     collidersToToggle = GetComponentsInChildren<Collider2D>(true);
        currentAlertTime = maxAlertTime;
        //  sceneIndex = CameraManager.instance.currentSpawnIndex;
        ani.TransAction("idle");
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


    // Command: 客户端调用，服务器执行
    [Command(requiresAuthority = false)]
    public void CmdUpdateShow(bool flag)
    {
        showFlag = flag;

        // 使用 Rpc 将值同步到所有客户端
        //RpcUpdatebit(flag);
    }


    public void StopBit()
    {
        //CmdUpdateBit(false);
    }

    void Update()
    {
        //Debug.Log($"{gameObject.name} : this {sceneIndex} and {CameraManager.instance.currentSpawnIndex}");
        if (sceneIndex != CameraManager.instance.currentSpawnIndex) return;
        if (ChainManager.instance.isShooting != null && !bitFlag && hitPos == null)
        {
            if (!isAlert)
            {

                //gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                AudioManager.instance.CmdPlaySound(3, transform.position);
                isAlert = true;
            }
            currentAlertTime -= Time.deltaTime;
            gameObject.transform.eulerAngles = new Vector3(0, (ChainManager.instance.aPlayer.transform.position.x
                                                                - gameObject.transform.position.x) > 0 ? 0 : 180, 0);
            
            if (currentAlertTime < 0)
            {
                ChainManager.instance.ResetCurrentScene();
            }
        }

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
            if (showFlag)
            {
                gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "ShowEnemy";
            }
            else
            {

                gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "UnShowEnemy";
            }

            if (bitFlag)
            {
                if (showFlag)
                {
                    hc.evc = this;
                }
                else
                {
                    hc.evc = null;
                }
            }
        }
        
        if (bitFlag)
        {
            ani.TransAction("bit");
        }
        else
        {
            if (ChainManager.instance.isShooting != null)
            {
                ani.TransAction("alert");
            }
            else
            {
                ani.TransAction("idle");
            }
        }
    }


    // Command: 客户端调用，服务器执行
    [Command(requiresAuthority = false)]
    public void KillEnemy(Vector2 dir)
    {
        hitPos = dir;
    }

    [ClientRpc]
    public void RpcResetObj()
    {
        bitFlag = false;
        hitPos = null;
        showFlag = false;
        isAlert = false;
        currentAlertTime = maxAlertTime;
        gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
        gameObject.SetActive(true);
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
