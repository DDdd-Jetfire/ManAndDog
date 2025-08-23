using Mirror;
using UnityEngine;

public class HideFromAVisibility : NetworkBehaviour
{
    private NetGameManager gm;

    public void Initialize(NetGameManager manager)
    {
        gm = manager;
    }

    // 手动更新观察者列表（在松链时隐藏A，恢复时显示A）
    public void UpdateVisibility()
    {
        if (gm == null) return;

        // 重新构建 B 角色的观察者列表，触发 A 看不到 B
        if (gm.aIdentity != null)
        {
            var aConnection = gm.aIdentity.connectionToClient;
            if (aConnection != null)
            {
                // 获取到 A 的连接，重新构建 B 的观察者列表
                NetworkServer.RebuildObservers(gm.bIdentity, initialize: false);
            }
        }
    }
}
