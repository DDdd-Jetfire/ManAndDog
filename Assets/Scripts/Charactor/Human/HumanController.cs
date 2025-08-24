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

    // �����������ڱ�����Ҷ����ʼ�����ʱ����
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
        // �����ִ�У����ҵǼ�ΪA
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


        

        // ��Ծ����
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            jumpRequest = true;
        }


        if (Input.GetKeyDown(KeyCode.E))  // ����E ���л�
        {
            if (!ChainManager.instance.isChain && ChainManager.instance.isNear)
            {
                ResetChain();
            }
            //bool wantActive = !NetGameManager.Instance.IsLeashActive(); // ����Ի���/��ѯ��ǰ״̬

            //CmdSetLeashActive(wantActive);  // ֻ���� A ��ɫ����
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


    // Cmd��A ��ɫ����ͨ���˷����޸�˩��״̬
    [Command]
    public void CmdSetLeashActive(bool active)
    {
        // ȷ���� A ��ɫ����
        //if (connectionToClient != NetGameManager.Instance.GetAConnection())
        //{
        //    Debug.LogWarning("Only A player can change the leash state.");
        //    return;  // ֻ�� A ��ɫ���ܵ���
        //}

        //NetGameManager.Instance.SetLeashActive(active);  // ����˩��״̬
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
