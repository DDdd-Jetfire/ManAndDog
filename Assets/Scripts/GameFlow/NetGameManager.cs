//using Mirror;
//using UnityEngine;

//public class NetGameManager : NetworkBehaviour
//{
//    public static NetGameManager Instance;

//    [Header("Leash Settings")]
//    public float leashRadius = 6f;            // ˩���뾶����λ���ף�
//    [SyncVar(hook = nameof(OnLeashStateChanged))]
//    private bool leashActive = true;          // true=˩����false=����

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

//    // ע�� A/B ��ɫ
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
//        // �� B ��ɫ��� HideFromAVisibility ���
//        var vis = player.GetComponent<HideFromAVisibility>();
//        if (vis == null) vis = player.gameObject.AddComponent<HideFromAVisibility>();
//        vis.Initialize(this);
//    }

//    // ��ȡ˩��״̬
//    public bool IsLeashActive() => leashActive;

//    // ����˩��״̬���� A ���ã�
//    public void SetLeashActive(bool active)
//    {
//        leashActive = active;
//        ApplyLeashServerSide();
//    }

//    // ע�� A/B ��ɫ��������˩��״̬��ʾ����
//    [Server]
//    private void TryWireChainServer()
//    {
//        if (aIdentity == null || bIdentity == null) return;

//        // �� ChainManager ��ס A/B
//        if (ChainManager.instance != null)
//        {
//            ChainManager.instance.SetA(aIdentity.gameObject);
//            ChainManager.instance.SetB(bIdentity.gameObject);
//        }

//        rbB = bIdentity.GetComponent<Rigidbody2D>();
//        ApplyLeashServerSide();
//    }

//    // ˩��״̬�仯ʱ��SyncVar hook��
//    private void OnLeashStateChanged(bool oldVal, bool newVal)
//    {
//        if (bIdentity != null)
//        {
//            var hideScript = bIdentity.GetComponent<HideFromAVisibility>();
//            hideScript.UpdateVisibility();
//        }
//    }

//    // �������ˣ�����˩��״̬����������ʾ�� B �Ŀɼ���
//    [Server]
//    private void ApplyLeashServerSide()
//    {
//        // ������ʾ/����
//        if (ChainManager.instance != null)
//        {
//            if (leashActive)
//            {
//                // ������ʾ����
//                ChainManager.instance.SetA(aIdentity ? aIdentity.gameObject : null);
//                ChainManager.instance.SetB(bIdentity ? bIdentity.gameObject : null);
//            }
//            else
//            {
//                // ��������
//                ChainManager.instance.ClearChain();
//            }
//        }

//        // �۲��߸��£�����ʱ��A ���ܿ��� B
//        if (bIdentity != null)
//        {
//            NetworkServer.RebuildObservers(bIdentity, initialize: false);
//        }
//    }

//    // ÿ֡��� B ��ɫ�Ƿ񳬳�˩���뾶��������������
//    void Update()
//    {
//        if (!isServer) return;
//        if (!leashActive) return;
//        if (aIdentity == null || bIdentity == null) return;

//        Vector2 aPos = aIdentity.transform.position;
//        Vector2 bPos = bIdentity.transform.position;

//        Vector2 delta = bPos - aPos;
//        float dist = delta.magnitude;

//        // ���� B ��ɫ��˩���뾶��
//        if (dist > leashRadius)
//        {
//            Vector2 clamped = aPos + delta.normalized * leashRadius;
//            bIdentity.transform.position = clamped;

//            // ��������ٶȣ���ֹ�ص�
//            if (rbB == null) rbB = bIdentity.GetComponent<Rigidbody2D>();
//            if (rbB != null) rbB.velocity = Vector2.zero;
//        }
//    }

//    // �ṩ A ���ӵ��������ӣ����� HideFromAVisibility��
//    public NetworkConnectionToClient GetAConnection() => aIdentity ? aIdentity.connectionToClient : null;
//}
