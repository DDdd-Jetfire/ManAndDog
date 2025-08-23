using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanController : PlayerController
{

    private bool jumpRequest;





    private void Start()
    {

    }

    // �����������ڱ�����Ҷ����ʼ�����ʱ����
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        isPlayerA = true;
        LocalGameManager.instance.InitSystem(true);
        if (isLocalPlayer)
            NetGameManager.Instance.CmdRegisterA(GetComponent<NetworkIdentity>());
        CmdRegisterAsA();
    }

    [Command]
    private void CmdRegisterAsA()
    {
        // �����ִ�У����ҵǼ�ΪA
        if (ChainManager.instance != null)
            ChainManager.instance.SetA(gameObject);
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        MoveChecker();
        GroundChecker();
        WallChecker();

        MoveController();


        // ��Ծ����
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            jumpRequest = true;
        }


        if (Input.GetKeyDown(KeyCode.E))  // ����E ���л�
        {
            bool wantActive = !NetGameManager.Instance.IsLeashActive(); // ����Ի���/��ѯ��ǰ״̬
            NetGameManager.Instance.CmdSetLeashActive(wantActive);
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
        }

        if (isGrounded)
        {
            if (move != 0)
            {
                ani.TransAction("walk");
            }
            else
            {
                ani.TransAction("idle");
            }
        }
        else
        {
            ani.TransAction("jump");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("fire");
            ani.PlayOneShoot("fire");
        }
    }

    void FixedUpdate()
    {
        if (isTouchingWall)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
            // ˮƽ�ƶ�
            rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);
        }

        // ��Ծ
        if (jumpRequest)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpRequest = false;
        }
    }




}
