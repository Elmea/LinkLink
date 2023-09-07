using System;
using System.Collections;
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

    [Header("Triggers Effect")]
    [SerializeField][Range(0, 1)] private float m_honeySlowDownFactor = 0.5f;
    [SerializeField][Tooltip("La vitesse de glisse")] private float m_slippingSpeed = 200f;
    [SerializeField][Tooltip("l'acceleration de la glisse")] private float m_slippingAcceleration = 20f;
    [SerializeField][Range(0,1), Tooltip("Reduit la force de grab")] private float m_grabForceReductionOnSlip = 0.3f;
    [SerializeField][Tooltip("Le player glisse toutes les x sec")] private float m_slippingDelay = 1f;
    [SerializeField][Tooltip("Le player glisse pendant x sec")] private float m_slippingDuration = 0.5f;
    [SerializeField] private float m_stunDuration = 2f;

    [Header("Animator")]
    [SerializeField] private Animator m_animator;

    [Header("Camera")]
    [SerializeField] private Camera m_camera;


    private float m_defaultSpeed;
    private float m_defaultGrabForce;
    private Vector2 m_velocity;
    private Vector2 m_moveInputValue;
    private Vector2 m_position2D;
    private Rigidbody2D body;
    private PlayerInput m_playerInput;
    private bool m_onGap = false;
    [SerializeField] private bool offWall = false;

    private Action m_doAction;
    private Action m_doTriggerAction;
    
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        m_doAction = DoMoveOnWall;
        m_doTriggerAction = DoVoid;
        m_defaultSpeed = m_speed;
        m_defaultGrabForce = m_grabForce;
    }

 #region Triggers
    void OnTriggerEnter2D(Collider2D other)
    {
        string tag = other.gameObject.tag;

        switch(tag)
        {
            case "SlowDown":
                m_speed = m_defaultSpeed - m_defaultSpeed * m_honeySlowDownFactor;
                break;
            case "Slippery":
                SetModeSlipOnWall();
                break;
            case "Gap":
                offWall = true;
                m_onGap = true;
                m_animator.SetTrigger("Fall");
                break;
            case "Projectile":
                Destroy(other.gameObject);
                SetModeStun();
                break;
            default:
                break;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        string tag = other.gameObject.tag;

        switch(tag)
        {
            case "SlowDown":
                m_speed = m_defaultSpeed;
                break;
            case "Slippery":
                m_grabForce = m_defaultGrabForce;
                m_doTriggerAction = DoVoid;
                break;
            case "Gap":
                m_onGap = false;
                break;
            default:
                break;
        }
    }

    private float m_stunTimer = 0f;
    private bool m_isStun = false;
    private void SetModeStun()
    {
        m_animator.SetTrigger("Stun");
        m_stunTimer = 0f;
        body.gravityScale = 1;
        offWall = true;
        m_isStun = true;
        m_doAction = DoStun;
    }

    private void DoStun()
    {
        m_stunTimer += Time.fixedDeltaTime;

        if(m_stunTimer >= m_stunDuration)
        {
            m_animator.SetTrigger("EndStun");
            m_isStun = false;
            m_doAction = DoMoveOnWall;
            m_stunTimer = 0f;
        }
    }

    private float m_slipTimer = 0f;
    private bool m_isSlipping = false;
    private void SetModeSlipOnWall()
    {
        m_slipTimer = 0f;
        m_grabForce = m_defaultGrabForce - m_defaultGrabForce * m_grabForceReductionOnSlip;
        m_isSlipping = false;
        m_doTriggerAction = SlipOnWall;
    }

    private void SlipOnWall()
    {
        m_slipTimer += Time.fixedDeltaTime;

        if(m_isSlipping)
        {
            m_velocity = Vector2.MoveTowards(m_velocity, Vector2.down * m_slippingSpeed * Time.fixedDeltaTime, m_slippingAcceleration * Time.fixedDeltaTime);

            body.MovePosition(m_position2D + m_velocity * Time.fixedDeltaTime);

            if(m_isGrabbing)
            {
                m_slipTimer = 0f;
                m_isSlipping = false;
                m_animator.SetBool("Slipping", false);
            }
        }
        else if(m_slipTimer >= m_slippingDelay)
        {
            m_isSlipping = true;
            m_animator.SetBool("Slipping", true);
            m_slipTimer = 0f;
        }
    }
#endregion

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

    public void SetOffWall()
    {
        offWall = true;
    }

    private void FixedUpdate()
    {
        m_position2D = transform.position;
        m_doAction();
        m_doTriggerAction();
    }

    private void DoMoveOnWall()
    {
        m_velocity = Vector2.MoveTowards(m_velocity, m_moveInputValue * m_speed * Time.fixedDeltaTime, m_acceleration * Time.fixedDeltaTime);

        body.MovePosition(m_position2D + m_velocity * Time.fixedDeltaTime);

        float lRatio = m_defaultSpeed * Time.fixedDeltaTime;

        m_animator.SetFloat("InputX", m_velocity.x / lRatio);
        m_animator.SetFloat("InputY", m_velocity.y / lRatio);
        m_animator.SetFloat("Velocity", (m_velocity.magnitude / lRatio) /* Mathf.Sign(m_moveInputValue.x) * Mathf.Sign(m_moveInputValue.y)*/);
        // m_animator.SetFloat("Velocity", (m_velocity.magnitude / lRatio) * Mathf.Sign(m_velocity.x == 0 ? 1 : m_velocity.x) * Mathf.Sign(m_velocity.y == 0 ? 1 : m_velocity.y));

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

    private bool m_isGrabbing = false;
    public void OnGrabInput(CallbackContext ctx)
    {
        if(ctx.performed && !m_onGap && !m_isStun)
        {
            m_isGrabbing = true;
            offWall = false;
            m_animator.SetBool("Grab", true);
            m_doAction = DoGrab;
        }
        else if(ctx.canceled)
        {
            m_isGrabbing = false;
            m_animator.SetBool("Grab", false);

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
