using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogController : PlayerController
{
    private bool jumpRequest;

    //void Awake()
    //{

    //}
    
    // �����������ڱ�����Ҷ����ʼ�����ʱ����
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        LocalGameManager.instance.InitSystem(false);
        CmdRegisterAsB();
    }

    [Command]
    private void CmdRegisterAsB()
    {
        // �����ִ�У����ҵǼ�ΪB
        if (ChainManager.instance != null)
            ChainManager.instance.SetB(gameObject);
    }
    void Update()
    {


        MoveChecker();

        MoveChecker();
        GroundChecker();
        WallChecker();

        MoveController();


        // ��Ծ����
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            jumpRequest = true;
        }


        // prefaceԤ������
        if (move != 0)
        {
            //if (!isLockAttacking)
            //{
               preface = move > 0 ? 1 : -1;
            //    //Debug.Log("unlock");
            //}
            //else
            //{
            //    //Debug.Log("lock");
            //}
            gameObject.transform.localEulerAngles = new Vector3(0, preface < 0 ? 180 : 0, 0);//���ĳ���
            ani.TransAction("walk");
        }
        else
        {
            ani.TransAction("idle");
        }
    }

    void FixedUpdate()
    {
        if (isTouchingWall)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        // ˮƽ�ƶ�
        rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);

        // ��Ծ
        if (jumpRequest)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpRequest = false;
        }
    }


}
