using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanController : PlayerController
{

    private bool jumpRequest;
    public bool canWalk = true;

    [SyncVar]public bool trainHit = false;


    [Command(requiresAuthority = false)]
    public void CmdUpdateTrainHit(bool flag)
    {
        trainHit = flag;
    }


    public EnemyVisibilityController evc;

    private void Start()
    {

    }

    // 这里是用来在本地玩家对象初始化完成时调用
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        isPlayerA = true;
        LocalGameManager.instance.InitSystem(true);
        //if (isLocalPlayer)
            //NetGameManager.Instance.CmdRegisterA(GetComponent<NetworkIdentity>());
        CmdRegisterAsA();
    }

    [Command]
    private void CmdRegisterAsA()
    {
        // 服务端执行：把我登记为A
        if (ChainManager.instance != null)
            ChainManager.instance.SetA(gameObject);
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (ChainManager.instance.isSmokeing)
        {
            ani.TransAction("smoke");
            return;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            ChainManager.instance.ResetCurrentScene();
        }
        if (trainHit)
        {

            ani.TransAction("meat");
            return;
        }
        MoveChecker();
        GroundChecker();
        WallChecker();

        MoveController();


        

        // 跳跃输入
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            jumpRequest = true;
        }


        if (Input.GetKeyDown(KeyCode.E))  // 例：E 键切换
        {
            if (!ChainManager.instance.isChain && ChainManager.instance.isNear)
            {
                ResetChain();
            }
            //bool wantActive = !NetGameManager.Instance.IsLeashActive(); // 你可以缓存/查询当前状态

            //CmdSetLeashActive(wantActive);  // 只允许 A 角色调用
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

        if (Input.GetKeyDown(KeyCode.Space) && !ani.inPlayOneShoot)
        {
            if (evc != null)
            {
                evc.KillEnemy(gameObject.transform.position);
            }
            ani.PlayOneShoot("fire");
            AudioManager.instance.CmdPlaySound(2, transform.position);
        }

    }

    public void ShootFrame()
    {
        ChainManager.instance.CmdUpdateShoot(transform.position);
    }

    public void TrianHit()
    {
    }

    [Command]
    private void ResetChain()
    {

        //gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
        ChainManager.instance.SetA(gameObject);
    }


    // Cmd：A 角色可以通过此方法修改拴链状态
    [Command]
    public void CmdSetLeashActive(bool active)
    {
        // 确保是 A 角色调用
        //if (connectionToClient != NetGameManager.Instance.GetAConnection())
        //{
        //    Debug.LogWarning("Only A player can change the leash state.");
        //    return;  // 只有 A 角色才能调用
        //}

        //NetGameManager.Instance.SetLeashActive(active);  // 更新拴链状态
    }

    void FixedUpdate()
    {
        if (trainHit || ChainManager.instance.isSmokeing)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
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

}
