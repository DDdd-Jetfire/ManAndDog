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
    [SyncVar] public bool showSelf = false;

    public float maxShowTime = 3f;
    public float currentShowTime = 0;
    //void Awake()
    //{

    // 这里是用来在本地玩家对象初始化完成时调用
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        enemyManager = EnemyManager.Instance;
        LocalGameManager.instance.InitSystem(false);
        //if (isLocalPlayer)
        //    NetGameManager.Instance.CmdRegisterB(GetComponent<NetworkIdentity>());
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


        if (showSelf)
        {
            //gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
        }
        else
        {
            //gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "UnShowEnemy";
        }

        if (!isLocalPlayer) return;

        if (ChainManager.instance.isChain || ChainManager.instance.isNear || currentShowTime > 0)
        {
            CmdUpdateShow(true);
        }
        else
        {
            CmdUpdateShow(false);
        }

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
                biteFlag = true;
            }
        }
        else
        {
            //biteFlag = false;
        }
        if(enemyInControl != null)
        {

            if (biteFlag)
            {

                gameObject.transform.position = enemyInControl.gameObject.transform.position;
                enemyInControl.CmdUpdateBit(true);
                if (showSelf)
                {
                    enemyInControl.CmdUpdateShow(true);
                }
                else
                {
                    enemyInControl.CmdUpdateShow(false);
                }
            }
        }
        else
        {
            biteFlag = false;
        }

        if (enemyInControl == null || enemyInControl.hitPos != null)
        {
            biteFlag = false;
        }


        // 当 B 玩家松开 E 键时，停止控制敌人
        //if (Input.GetKeyUp(KeyCode.E))
        //{
        //    if (enemyInControl != null)
        //    {
        //        enemyInControl.StopBit();
        //    }
        //}


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


        if (Input.GetKeyDown(KeyCode.Space))
        {
            int tempInt = Random.Range(0, 2);
            currentShowTime = maxShowTime;
            //ani.PlayOneShoot("fire");
            AudioManager.instance.CmdPlaySound(tempInt, transform.position);
        }
        if (currentShowTime > 0)
        {
            currentShowTime -= Time.deltaTime;
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

    // 检测范围内的敌人
    EnemyVisibilityController DetectEnemiesInRange()
    {
        // 使用OverlapCircle方法检测范围内的敌人
        Collider2D[] c = Physics2D.OverlapCircleAll(transform.position, detectionRadius, sightLayer);
        foreach (var c1 in c)
        {
            var x = c1.GetComponent<EnemyVisibilityController>();
            if (x != null) return x;
        }
        return null;
    }

    // Command: 客户端调用，服务器执行
    [Command(requiresAuthority = false)]
    public void CmdUpdateShow(bool flag)
    {
        showSelf = flag;

        // 使用 Rpc 将值同步到所有客户端
        //RpcUpdatebit(flag);
    }


    // 画出检测的范围（调试用）
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
