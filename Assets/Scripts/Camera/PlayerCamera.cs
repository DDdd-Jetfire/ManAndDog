using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    public Camera playerCamera;
    public LayerMask layerA;  // A��ҿɼ�������
    public LayerMask layerB;  // B��ҿɼ�������

    void Start()
    {
        if (isLocalPlayer)
        {
            // ���ݵ�ǰ�������������Ŀɼ���
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
