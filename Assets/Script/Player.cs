using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.InputSystem.InputAction;
using Vector2 = UnityEngine.Vector2;

public class Player : MonoBehaviour
{
    [SerializeField] private float m_speed = 125f;
    [SerializeField] private float m_acceleration = 20f;
    [SerializeField] private float m_grabForce = 40f;
    private Vector2 m_velocity;
    private Vector2 m_moveInputValue;
    private Vector2 m_position2D;
    private Rigidbody2D body;
    private PlayerInput m_playerInput;
    [SerializeField] private bool offWall = false;
    private bool grabbing = false;

    [Header("Triggers Effect")]
    [SerializeField] [Range(0, 1)] private float m_honeySlowDownFactor = 0.5f;
    [SerializeField] [Tooltip("La vitesse de glisse")] private float m_slippingSpeed = 200f;
    [SerializeField] [Tooltip("l'acceleration de la glisse")] private float m_slippingAcceleration = 20f;
    [SerializeField] [Range(0, 1), Tooltip("Reduit la force de grab")] private float m_grabForceReductionOnSlip = 0.3f;
    [SerializeField] [Tooltip("Le player glisse toutes les x sec")] private float m_slippingDelay = 1f;
    [SerializeField] [Tooltip("Le player glisse pendant x sec")] private float m_slippingDuration = 0.5f;

    [Header("Animator")]
    [SerializeField] private Animator m_animator;

    private Action m_doAction;
    private Action m_doTriggerAction;
    private Rope linkedRope; 
    private Vector3 grabPos;
    private CapsuleCollider2D myCollider;
    private CapsuleCollider2D teamMateCollider;

    private float m_defaultSpeed;
    private bool m_onGap;
    private float m_defaultGrabForce;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        m_doAction = DoMoveOnWall;
        m_doTriggerAction = DoVoid;
        myCollider = GetComponentInChildren<CapsuleCollider2D>();
        m_defaultSpeed = m_speed;
        m_defaultGrabForce = m_grabForce;
    }

    public void Init(PlayerInput pPlayerInput)
    {
        ResetPlayerInput();

        m_playerInput = pPlayerInput;

        pPlayerInput.actions.FindAction("Move").performed += OnMoveInput;
        pPlayerInput.actions.FindAction("Jump").performed += OnJumpInput;
        pPlayerInput.actions.FindAction("Grab").performed += OnGrabInput;
    }

    public void LinkRope(Rope ropeTOLink)
    {
        linkedRope = ropeTOLink;
    }
    
    public void SetTeamMateCollider(CapsuleCollider2D _teamMateCollider)
    {
        teamMateCollider = _teamMateCollider;
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
        m_doTriggerAction();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        string tag = other.gameObject.tag;

        switch (tag)
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
                break;
            default:
                break;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        string tag = other.gameObject.tag;

        switch (tag)
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

        if (m_isSlipping)
        {
            m_velocity = Vector2.MoveTowards(m_velocity, Vector2.down * m_slippingSpeed * Time.fixedDeltaTime, m_slippingAcceleration * Time.fixedDeltaTime);

            body.MovePosition(m_position2D + m_velocity * Time.fixedDeltaTime);

            if (grabbing)
            {
                m_slipTimer = 0f;
                m_isSlipping = false;
            }
        }
        else if (m_slipTimer >= m_slippingDelay)
        {
            m_isSlipping = true;
            m_slipTimer = 0f;
        }
    }

    private void DoMoveOnWall()
    {
        if (offWall)
        {
            m_doAction = DoFall;
            return;
        }
        
        m_velocity = Vector2.MoveTowards(m_velocity, m_moveInputValue * m_speed * Time.fixedDeltaTime, m_acceleration * Time.fixedDeltaTime);

        body.MovePosition(m_position2D + m_velocity * Time.fixedDeltaTime);

    }

    private void DoGrab()
    {
        m_velocity = Vector2.MoveTowards(body.velocity, Vector2.zero, m_grabForce * Time.fixedDeltaTime);
        
        if (m_velocity.magnitude < 0.25f || !offWall)
        {
            body.velocity = new Vector2(0, 0);
            m_velocity = new Vector2(0, 0);
            transform.position = grabPos;
            offWall = false;
        }
        else
        {
            grabPos = transform.position;
        }
    }

    private void DoFall()
    {
        body.gravityScale = 1;
    }

    // Unity event called by new input system
    public void OnMoveInput(CallbackContext ctx)
    {
        m_moveInputValue = ctx.ReadValue<Vector2>();
    }

    public void OnJumpInput(CallbackContext ctx)
    {
        if (ctx.performed && !offWall)
        {
            offWall = true;
            grabbing = false;
            body.gravityScale = 1;
            Physics2D.IgnoreCollision(myCollider, teamMateCollider, true);

            if (linkedRope.IsInTension())
            {
                linkedRope.ReleaseTenseOnThisAncor(this.gameObject);
            }
        }
    }

    public void OnGrabInput(CallbackContext ctx)
    {
        if(ctx.performed)
        {
            if (offWall)
                Physics2D.IgnoreCollision(myCollider, teamMateCollider, false);
            
            body.gravityScale = 0;
            m_doAction = DoGrab;
            grabbing = true;
            grabPos = transform.position;
        }
        else if(ctx.canceled)
        {
            grabbing = false;
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

    private void Update()
    {

    }
}
