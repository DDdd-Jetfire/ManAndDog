using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    protected AniBase ani;
    [Header("移动设置")]
    public float moveSpeed = 5f;       // 移动速度
    public float jumpForce = 10f;      // 跳跃力度

    public bool isPlayerA = false;

    [Header("检测")]

    public int preface = 0;
    public bool isGrounded = false;
    public bool isTouchingWall = false;
    public bool is1LevelBlockedOnWall = false;

    public LayerMask groundLayer;      // 地面层
    public float groundCheckDistance = 0.1f;  // 射线长度


    public GameObject groundChecker;
    public float groundCheckWidth = 0.6f;
    public GameObject wallChecker;
    public float wallCheckDistance = 0.3f;
    public float verticalOffset = 0.8f;

    protected int move = 0;
    protected KeyCode lastMove = KeyCode.None;


    protected Rigidbody2D rb;

     protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ani = gameObject.GetComponent<AniBase>();
        preface = gameObject.transform.eulerAngles.y == 0 ? 1 : -1;
    }


    protected void MoveController()
    {
        move = 0;
        if (lastMove == KeyCode.A)
        {
            move = -1;
        }
        if (lastMove == KeyCode.D)
        {
            move = 1;
        }
    }

    protected void MoveChecker()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            lastMove = KeyCode.A;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            if (Input.GetKey(KeyCode.D))
            {
                lastMove = KeyCode.D;
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            lastMove = KeyCode.D;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.A))
            {
                lastMove = KeyCode.A;
            }
        }
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            lastMove = KeyCode.None;
        }

    }

    protected void GroundChecker()
    {
        // 检查角色是否在地面上
        //isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, 0.1f, groundLayer);

        Vector2 origin = groundChecker.transform.position;

        // 三个点：左、中、右
        Vector2 left = origin + Vector2.left * groundCheckWidth * 0.5f * preface;
        Vector2 right = origin + Vector2.right * groundCheckWidth * 0.5f * preface;
        Vector2 center = origin;

        // 发出三条向下的射线
        RaycastHit2D hitLeft = Physics2D.Raycast(left, Vector2.up, groundCheckDistance, groundLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(right, Vector2.up, groundCheckDistance, groundLayer);
        RaycastHit2D hitCenter = Physics2D.Raycast(center, Vector2.up, groundCheckDistance, groundLayer);

        isGrounded = hitLeft.collider != null || hitRight.collider != null || hitCenter.collider != null;
        //落地重置位移技能
        if (isGrounded)
        {
            ResetAllMoveSkill();
        }
    }

    protected void WallChecker()
    {

        Vector2 center = wallChecker.transform.position;


        // 上中下三个起点
        Vector2 top = center + Vector2.up * verticalOffset;
        Vector2 middle = center + Vector2.down * verticalOffset * 1 / 3;
        Vector2 bottom = center + Vector2.down * verticalOffset;

        Vector2 dir = preface >= 0 ? Vector2.left : Vector2.right;

        // 三条射线均向右
        RaycastHit2D hitTop = Physics2D.Raycast(top, dir, wallCheckDistance, groundLayer);
        RaycastHit2D hitMiddle = Physics2D.Raycast(middle, dir, wallCheckDistance, groundLayer);
        RaycastHit2D hitBottom = Physics2D.Raycast(bottom, dir, wallCheckDistance, groundLayer);

        isTouchingWall = hitTop.collider != null || hitBottom.collider != null; 
        //is1LevelBlockedOnWall = hitTop.collider == null && hitMiddle.collider == null && hitBottom.collider != null;
        //is2LevelBlockedOnWall = hitTop.collider == null && hitMiddle.collider != null && hitBottom.collider != null;

    }

    private void OnDrawGizmos()
    {
        {

            if (groundChecker == null) return;

            Vector2 origin = groundChecker.transform.position;

            Vector2 left = origin + Vector2.left * groundCheckWidth * 0.5f;
            Vector2 right = origin + Vector2.right * groundCheckWidth * 0.5f;
            Vector2 center = origin;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(left, left + Vector2.up * groundCheckDistance);
            Gizmos.DrawLine(center, center + Vector2.up * groundCheckDistance);
            Gizmos.DrawLine(right, right + Vector2.up * groundCheckDistance);
        }


        if (wallChecker == null) return;

        Vector2 center2 = wallChecker.transform.position;

        Vector2 top = center2 + Vector2.up * verticalOffset;
        Vector2 middle = center2 + Vector2.down * verticalOffset * 1 / 3;
        Vector2 bottom = center2 + Vector2.down * verticalOffset;

        Vector2 dir = preface > 0 ? Vector2.left : Vector2.right;

        Gizmos.color = Color.cyan;

        Gizmos.DrawLine(top, top + dir * wallCheckDistance);
        Gizmos.DrawLine(middle, middle + dir * wallCheckDistance);
        Gizmos.DrawLine(bottom, bottom + dir * wallCheckDistance);

    }

    protected void ResetAllMoveSkill()
    {
        
    }
}
