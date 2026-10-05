using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Player
{
    playerOne,
    playerTwo
}

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float movingSpeed = 5f;
    [SerializeField]
    float jumpSpeed = 5f;
    [SerializeField]
    float softHitForce = 2.5f;
    [SerializeField]
    float hardHitForce = 5f;
    [SerializeField]
    private Player player;
    public Player PlayerID
    {
        get { return player; }
    }
    public int horizontalMoveDir = 0; // 1 for right, 0 not moving, -1 for left
    public int ifHit = 0; // 1 for soft, 2 for hard, 0 for no
    public bool ifJump = false;
    [SerializeField]
    private Rigidbody2D ball = null;
    public bool BallInHitRange
    {
        get { return ball != null; }
    }
    [Header("Hit Buffer")]
    [SerializeField]
    private float hitBufferTime = 0.15f;
    private int bufferedHit = 0;
    private float hitBufferTimer = 0f;
    private Rigidbody2D rb;
    private readonly ContactPoint2D[] groundContacts = new ContactPoint2D[8];
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        HandleServe();
        if (RoundSystem.Instance != null && RoundSystem.Instance.WaitingForServe)
            return;
        UpdateHitBuffer();
    }
    private void FixedUpdate()
    {
        Move();
        HitBall();
    }
    private void HandleServe()
    {
        if (RoundSystem.Instance == null)
            return;
        if (!RoundSystem.Instance.WaitingForServe)
            return;
        if (ifHit == 0)
            return;
        if (ifHit != 1)
        {
            ifHit = 0;
            return;
        }
        if (!RoundSystem.Instance.TryResumeRound(player))
        {
            ifHit = 0;
            return;
        }
        ifHit = 0;
        bufferedHit = 0;
        hitBufferTimer = 0f;
    }

    private void UpdateHitBuffer()
    {
        if (ifHit != 0)
        {
            bufferedHit = ifHit;
            hitBufferTimer = hitBufferTime;
            ifHit = 0;
        }
        if (bufferedHit != 0)
        {
            hitBufferTimer -= Time.unscaledDeltaTime;
            if (hitBufferTimer <= 0f)
            {
                ClearHitBuffer();
            }
        }
    }

    private void Move()
    {
        float horizontalSpeed = movingSpeed * horizontalMoveDir;
        Vector2 newSpeed = rb.linearVelocity;
        newSpeed.x = horizontalSpeed;
        if (ifJump && IsGrounded())
        {
            newSpeed.y = jumpSpeed;
        }
        ifJump = false;
        rb.linearVelocity = newSpeed;
    }
    private bool IsGrounded()
    {
        int contactCount = rb.GetContacts(groundContacts);
        for (int i = 0; i < contactCount; i++)
        {
            ContactPoint2D contact = groundContacts[i];

            bool touchesField =
                IsFieldCollider(contact.collider) ||
                IsFieldCollider(contact.otherCollider);
            if (touchesField && Mathf.Abs(contact.normal.y) > 0.5f)
            {
                return true;
            }
        }
        return false;
    }
    private bool IsFieldCollider(Collider2D collider)
    {
        return collider != null &&
               collider.GetComponentInParent<FieldRotation>() != null;
    }
    private void HitBall()
    {
        if (bufferedHit == 0)
            return;
        if (ball == null)
            return;
        BallController ballController = ball.GetComponent<BallController>();
        if (ballController != null)
        {
            if (!ballController.TryHit(player))
            {
                ClearHitBuffer();
                return;
            }
        }
        Vector2 hitDir =
            (ball.transform.position - transform.position).normalized;

        if (bufferedHit == 1)
        {
            ball.linearVelocity = softHitForce * hitDir;
        }
        else if (bufferedHit == 2)
        {
            ball.linearVelocity = hardHitForce * hitDir;
        }
        ClearHitBuffer();
    }
    private void ClearHitBuffer()
    {
        bufferedHit = 0;
        hitBufferTimer = 0f;
    }
    public void BallEnterHitRange(Rigidbody2D ball)
    {
        this.ball = ball;
    }
    public void BallExitHitRange(Rigidbody2D ball)
    {
        if (this.ball == ball)
        {
            this.ball = null;
        }
    }
    public void ResetPlayer(Vector2 position)
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        transform.position = position;
        rb.position = position;

        horizontalMoveDir = 0;
        ifJump = false;
        ifHit = 0;

        ball = null;

        ClearHitBuffer();
    }
}
