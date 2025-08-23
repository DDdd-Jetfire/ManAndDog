using Mirror;
using UnityEngine;

public class HideFromAVisibility : NetworkBehaviour
{
    private NetGameManager gm;

    public void Initialize(NetGameManager manager)
    {
        gm = manager;
    }

    // �ֶ����¹۲����б�������ʱ����A���ָ�ʱ��ʾA��
    public void UpdateVisibility()
    {
        if (gm == null) return;

        // ���¹��� B ��ɫ�Ĺ۲����б����� A ������ B
        if (gm.aIdentity != null)
        {
            var aConnection = gm.aIdentity.connectionToClient;
            if (aConnection != null)
            {
                // ��ȡ�� A �����ӣ����¹��� B �Ĺ۲����б�
                NetworkServer.RebuildObservers(gm.bIdentity, initialize: false);
            }
        }
    }
}
