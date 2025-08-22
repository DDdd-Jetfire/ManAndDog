using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    public Camera playerCamera;
    public LayerMask layerA;  // A玩家可见的物体
    public LayerMask layerB;  // B玩家可见的物体

    void Start()
    {
        if (isLocalPlayer)
        {
            // 根据当前玩家设置摄像机的可见层
           // if (isAPlayer)
           // {
           //     playerCamera.cullingMask = layerA;
           // }
           // else
           // {
           //     playerCamera.cullingMask = layerB;
           // }
        }
    }
}
