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
        // �����ռ�
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
        //LocalSetVisible(false); // ������������
    }

    // ���� �ؼ� 1���ͻ���һ���ɾ������أ����⡰��֡���֡� ���� //
    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    // ���� �ؼ� 2���������ں���ʱ������ A ���ء��� B ��ʾ ���� //
    public override void OnStartServer()
    {
        base.OnStartServer();
        // �ȴ���� ready�����ܵ���������� spawn��
        //StartCoroutine(InitVisibilityForAB());
    }

    public void GetBit()
    {
        CmdUpdateBit(true);
    }

    // Command: �ͻ��˵��ã�������ִ��
    [Command(requiresAuthority = false)]
    public void CmdUpdateBit(bool flag)
    {
        bitFlag = flag;

        // ʹ�� Rpc ��ֵͬ�������пͻ���
        //RpcUpdatebit(flag);
    }


    // Command: �ͻ��˵��ã�������ִ��
    [Command(requiresAuthority = false)]
    public void CmdUpdateShow(bool flag)
    {
        showFlag = flag;

        // ʹ�� Rpc ��ֵͬ�������пͻ���
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


    // Command: �ͻ��˵��ã�������ִ��
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

    // ===== �ɸ��õĲ����߼� =====
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

    // ===== ����������Ч����ʾ/���أ����ڷ���֡�� =====
    void LocalSetVisible(bool visible)
    {
        if (renderersToToggle != null)
            foreach (var r in renderersToToggle) if (r) r.enabled = visible;
       // if (collidersToToggle != null)
       //     foreach (var c in collidersToToggle) if (c) c.enabled = visible;
    }

    // ===== �����ĳ���ͻ����л��ɼ��� =====
    [TargetRpc]
    void TargetSetVisible(NetworkConnection target, bool visible)
    {
        LocalSetVisible(visible);
    }
}
