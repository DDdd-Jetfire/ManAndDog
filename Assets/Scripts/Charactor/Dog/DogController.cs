using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogController : PlayerController
{

    public EnemyVisibilityController enemyInControl;

    public EnemyManager enemyManager; // 物体管理器

    private bool jumpRequest;

    public float detectionRadius = 5f;  // 检测半径
    public LayerMask sightLayer;        // 敌人所在的Layer

    public bool biteFlag = false;
                                        //void Awake()
                                        //{

    //}

    // 这里是用来在本地玩家对象初始化完成时调用
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        enemyManager = EnemyManager.Instance;
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
        if (!isLocalPlayer) return;

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

        // 调用检测功能
        enemyInControl = DetectEnemiesInRange();



        // 当 B 玩家按下 E 键时，开始控制敌人

        if (Input.GetKey(KeyCode.E) && enemyInControl != null)
        {
            
            {
                gameObject.transform.position = enemyInControl.gameObject.transform.position;
                Debug.Log("mark");
                biteFlag = true;
                enemyInControl.CmdUpdateBit(true);
            }
        }
        else
        {
            biteFlag = false;
        }


        // 当 B 玩家松开 E 键时，停止控制敌人
        if (Input.GetKeyUp(KeyCode.E))
        {
            if (enemyInControl != null)
            {
                enemyInControl.StopBit();
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

        if (biteFlag)
        {
            ani.TransAction("bite");
        }
        else
        {

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
    }

    void FixedUpdate()
    {
        if (biteFlag)
        {
            rb.velocity = new Vector2(0, 0);
        }
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





    // RPC 方法：更新敌人显示给 A 端
    [ClientRpc]
    void RpcUpdateEnemyVisibility(bool visible)
    {
        if (isServer) return;  // 只有客户端执行

        // 通过管理器设置 A 端显示该敌人
        if (enemyManager != null)
        {
            GameObject enemy = enemyManager.GetMarkedEnemy();
            if (enemy != null)
            {
                enemyManager.SetEnemyVisibility(connectionToClient, visible);
            }
        }
    }

    // 检测范围内的敌人
    EnemyVisibilityController DetectEnemiesInRange()
    {
        // 使用OverlapCircle方法检测范围内的敌人
        Collider2D[] c = Physics2D.OverlapCircleAll(transform.position, detectionRadius, sightLayer);
        foreach(var c1 in c)
        {
            var x = c1.GetComponent<EnemyVisibilityController>();
            if (x != null) return x;
        }
        return null;
    }

    // 画出检测的范围（调试用）
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
