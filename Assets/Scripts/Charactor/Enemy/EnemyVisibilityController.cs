using Mirror;
using UnityEngine;
using System.Collections;

public class EnemyVisibilityController : NetworkBehaviour
{
    [SerializeField] Renderer[] renderersToToggle;
    //[SerializeField] Collider2D[] collidersToToggle;

    void Awake()
    {
        // �����ռ�
        if (renderersToToggle == null || renderersToToggle.Length == 0)
            renderersToToggle = GetComponentsInChildren<Renderer>(true);
       // if (collidersToToggle == null || collidersToToggle.Length == 0)
       //     collidersToToggle = GetComponentsInChildren<Collider2D>(true);
    }

    private void OnEnable()
    {
        LocalSetVisible(false); // ������������
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
        StartCoroutine(InitVisibilityForAB());
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


    // ���� B ��ʼ/ֹͣ�ٿأ��� A ��ʱ���� / �ٴ����� ���� //
    [Command]
    public void CmdStartControlByB()
    {
        var connA = FindAConnection();
        if (connA != null) TargetSetVisible(connA, true);
    }

    [Command]
    public void CmdStopControlByB()
    {
        var connA = FindAConnection();
        if (connA != null) TargetSetVisible(connA, false);
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

    NetworkConnectionToClient FindAConnection()
    {
        foreach (var conn in NetworkServer.connections.Values)
        {
            if (conn == null || !conn.isReady || conn.identity == null) continue;
            var pc = conn.identity.GetComponent<PlayerController>();
            if (pc != null && pc.isPlayerA) return conn;
        }
        return null;
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
