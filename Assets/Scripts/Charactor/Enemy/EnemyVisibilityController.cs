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
        // �����ռ�
        if (renderersToToggle == null || renderersToToggle.Length == 0)
            renderersToToggle = GetComponentsInChildren<Renderer>(true);
        ani = gameObject.GetComponent<AniBase>();
       // if (collidersToToggle == null || collidersToToggle.Length == 0)
       //     collidersToToggle = GetComponentsInChildren<Collider2D>(true);
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

        // ���������Ϣ��ȷ�� connA �� connB �Ƿ���ȷ
        Debug.Log($"connA: {connA}, connB: {connB}");

        // ��ʼ���ԣ��� A �������ء��� B ��ʾ
        if (connA != null) TargetSetVisible(connA, false);
        if (connB != null) TargetSetVisible(connB, true);
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


    // ClientRpc: ���������ã����пͻ���ִ��
    [ClientRpc]
    void RpcUpdatebit(bool flag)
    {
        // �ͻ��˸���
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


    // Command: �ͻ��˵��ã�������ִ��
    [Command(requiresAuthority = false)]
    public void KillEnemy(Vector2 dir)
    {
        hitPos = dir;
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
