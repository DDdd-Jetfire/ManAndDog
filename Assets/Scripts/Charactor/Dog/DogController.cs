using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogController : PlayerController
{

    public EnemyVisibilityController enemyInControl;

    public EnemyManager enemyManager; // ���������

    private bool jumpRequest;

    public float detectionRadius = 5f;  // ���뾶
    public LayerMask sightLayer;        // �������ڵ�Layer

    public bool biteFlag = false;
    [SyncVar] public bool showSelf = false;

    public float maxShowTime = 3f;
    public float currentShowTime = 0;
    //void Awake()
    //{

    // �����������ڱ�����Ҷ����ʼ�����ʱ����
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
        // �����ִ�У����ҵǼ�ΪB
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


        // ��Ծ����
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            jumpRequest = true;
        }

        // ���ü�⹦��
        enemyInControl = DetectEnemiesInRange();



        // �� B ��Ұ��� E ��ʱ����ʼ���Ƶ���

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


        // �� B ����ɿ� E ��ʱ��ֹͣ���Ƶ���
        //if (Input.GetKeyUp(KeyCode.E))
        //{
        //    if (enemyInControl != null)
        //    {
        //        enemyInControl.StopBit();
        //    }
        //}


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

    // ��ⷶΧ�ڵĵ���
    EnemyVisibilityController DetectEnemiesInRange()
    {
        // ʹ��OverlapCircle������ⷶΧ�ڵĵ���
        Collider2D[] c = Physics2D.OverlapCircleAll(transform.position, detectionRadius, sightLayer);
        foreach (var c1 in c)
        {
            var x = c1.GetComponent<EnemyVisibilityController>();
            if (x != null) return x;
        }
        return null;
    }

    // Command: �ͻ��˵��ã�������ִ��
    [Command(requiresAuthority = false)]
    public void CmdUpdateShow(bool flag)
    {
        showSelf = flag;

        // ʹ�� Rpc ��ֵͬ�������пͻ���
        //RpcUpdatebit(flag);
    }


    // �������ķ�Χ�������ã�
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
