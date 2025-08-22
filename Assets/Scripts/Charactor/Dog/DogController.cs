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
    
    // 这里是用来在本地玩家对象初始化完成时调用
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        LocalGameManager.instance.InitSystem(false);
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

        // 水平移动
        rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);

        // 跳跃
        if (jumpRequest)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpRequest = false;
        }
    }


}
