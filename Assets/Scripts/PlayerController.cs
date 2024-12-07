using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    public float walkSpeed = 5f;

    public float yVelocity;

    Vector2 moveInput;

    TriggerCustomEvent jump;

    TouchingDirections touchingDirections;

    public TextMeshProUGUI loseMessage;
    public TextMeshProUGUI winMessage;
    public float resetDelay = 2f;

    [SerializeField]
    private bool _isMoving = false;
    public bool IsMoving
    {
        get
        {
            return _isMoving;
        }
        private set
        {
            _isMoving = value;
            animator.SetBool("isMoving", value);
        }
    }

    public bool _isFacingRight = true;

    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        private set
        {
            if (_isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }
            _isFacingRight = value;
        }
    }

    Rigidbody2D rb;
    Animator animator;
    public float jumpImpulse = 5;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveInput.x * walkSpeed, rb.velocity.y);

        animator.SetFloat("yVelocity", rb.velocity.y);

        if (!hasFallen && transform.position.y < fallThreshold)
        {
            HandlePlayerFall();
        }

        if (!hasWon && transform.position.x > winThreshold)
        {
            HandlePlayerWin();
        }
    }

    private void HandlePlayerFall()
    {
        hasFallen = true;
        loseMessage.gameObject.SetActive(true);
        Debug.Log("You Lose!");
        Invoke(nameof(ResetLevel), resetDelay);
    }

    private void HandlePlayerWin()
    {
        hasWon = true;
        winMessage.gameObject.SetActive(true);
        Debug.Log("You Win!");
        Invoke(nameof(ResetLevel), resetDelay);
    }

    private void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        IsMoving = moveInput != Vector2.zero;

        SetFacingDirection(moveInput);
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
        {
            // face right
            IsFacingRight = true;
        }
        else if (moveInput.x < 0 && IsFacingRight)
        {
            // face left
            IsFacingRight = false;
        }
    }

    private int jumpsLeft = 1;
    public float fallThreshold = -10f;
    public float winThreshold = 20f;
    private bool hasFallen = false;
    private bool hasWon = false;

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && (touchingDirections.IsGrounded || jumpsLeft == 1))
        {
            animator.SetTrigger("jump");
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
            jumpsLeft--;
        }
        if (context.started && touchingDirections.IsGrounded)
        {
            jumpsLeft = 1;
        }
    }
}
