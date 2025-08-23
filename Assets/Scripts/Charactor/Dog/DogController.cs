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
    
    // 这里是用来在本地玩家对象初始化完成时调用
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
        // 服务端执行：把我登记为B
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


        // 跳跃输入
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            jumpRequest = true;
        }


        // 当 B 玩家按下 E 键时，开始控制敌人
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (enemyInControl != null)
            {
                enemyInControl.CmdStartControlByB();  // B 开始控制
            }
        }

        // 当 B 玩家松开 E 键时，停止控制敌人
        if (Input.GetKeyUp(KeyCode.E))
        {
            if (enemyInControl != null)
            {
                enemyInControl.CmdStopControlByB();  // B 停止控制
            }
        }


        // preface预估朝向
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
            gameObject.transform.localEulerAngles = new Vector3(0, preface < 0 ? 180 : 0, 0);//更改朝向
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
            // 水平移动
            rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);
        }

        // 跳跃
        if (jumpRequest)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpRequest = false;
        }
    }


}
