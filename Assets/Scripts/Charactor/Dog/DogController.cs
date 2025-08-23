using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogController : PlayerController
{

    public EnemyVisibilityController enemyInControl;


    private bool jumpRequest;

    //void Awake()
    //{

    //}
    
    // �����������ڱ�����Ҷ����ʼ�����ʱ����
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        LocalGameManager.instance.InitSystem(false);
        if (isLocalPlayer)
            NetGameManager.Instance.CmdRegisterB(GetComponent<NetworkIdentity>());
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


        // �� B ��Ұ��� E ��ʱ����ʼ���Ƶ���
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (enemyInControl != null)
            {
                enemyInControl.CmdStartControlByB();  // B ��ʼ����
            }
        }

        // �� B ����ɿ� E ��ʱ��ֹͣ���Ƶ���
        if (Input.GetKeyUp(KeyCode.E))
        {
            if (enemyInControl != null)
            {
                enemyInControl.CmdStopControlByB();  // B ֹͣ����
            }
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
