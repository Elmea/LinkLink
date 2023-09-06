using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using Vector2 = UnityEngine.Vector2;

public class Player : MonoBehaviour
{
    [SerializeField] private float m_speed = 125f;
    [SerializeField] private float m_acceleration = 20f;
    [SerializeField] private float m_airAcceleration = 5f;
    [SerializeField] private float m_grabForce = 40f;
    private Vector2 m_velocity;
    private Vector2 m_moveInputValue;
    private Vector2 m_position2D;
    private Rigidbody2D body;
    private PlayerInput m_playerInput;
    [SerializeField] private bool offWall = false;

    private Action m_doAction;
    
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        m_doAction = DoMoveOnWall;
    }

    public void Init(PlayerInput pPlayerInput)
    {
        ResetPlayerInput();

        m_playerInput = pPlayerInput;

        pPlayerInput.actions.FindAction("Move").performed += OnMoveInput;
        pPlayerInput.actions.FindAction("Jump").performed += OnJumpInput;
        pPlayerInput.actions.FindAction("Grab").performed += OnGrabInput;
    }

    private void ResetPlayerInput()
    {
        if(m_playerInput == null)
            return;
        
        m_playerInput.actions.FindAction("Move").performed -= OnMoveInput;
        m_playerInput.actions.FindAction("Jump").performed -= OnJumpInput;
        m_playerInput.actions.FindAction("Grab").performed -= OnGrabInput;
    }

    private void DoVoid()
    {
    }

    private void FixedUpdate()
    {
        m_position2D = transform.position;
        m_doAction();
    }

    private void DoMoveOnWall()
    {
        m_velocity = Vector2.MoveTowards(m_velocity, m_moveInputValue * m_speed * Time.fixedDeltaTime, m_acceleration * Time.fixedDeltaTime);

        body.MovePosition(m_position2D + m_velocity * Time.fixedDeltaTime);

        if(offWall)
            m_doAction = DoFall;
    }

    private void DoGrab()
    {
        m_velocity = Vector2.MoveTowards(m_velocity, Vector2.zero, m_grabForce * Time.fixedDeltaTime);

        body.MovePosition(m_position2D + m_velocity * Time.fixedDeltaTime);
    }

    private void DoFall()
    {
        m_velocity.y += WorldSettings.gravity.y * Time.fixedDeltaTime;
        m_velocity.x = Mathf.MoveTowards(m_velocity.x, m_moveInputValue.x * m_speed * Time.fixedDeltaTime, m_airAcceleration * Time.fixedDeltaTime);

        body.MovePosition(m_position2D + m_velocity * Time.fixedDeltaTime);
    }

    // Unity event called by new input system
    public void OnMoveInput(CallbackContext ctx)
    {
        m_moveInputValue = ctx.ReadValue<Vector2>();
    }

    public void OnJumpInput(CallbackContext ctx)
    {
        if(ctx.performed && !offWall)
            offWall = true;
    }

    public void OnGrabInput(CallbackContext ctx)
    {
        if(ctx.performed)
        {
            offWall = false;
            m_doAction = DoGrab;
        }
        else if(ctx.canceled)
        {
            if(offWall)
                m_doAction = DoFall;
            else
                m_doAction = DoMoveOnWall;
        }
    }

    private void OnDisable()
    {
        if(m_playerInput == null)
            return;
        
        m_playerInput.actions.FindAction("Move").performed -= OnMoveInput;
        m_playerInput.actions.FindAction("Jump").performed -= OnJumpInput;
        m_playerInput.actions.FindAction("Grab").performed -= OnGrabInput;

        Debug.Log("Player " + m_playerInput.playerIndex + " disabled");

        m_playerInput = null;
    }
}
