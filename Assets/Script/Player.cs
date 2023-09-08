using System;
using JetBrains.Annotations;
using UnityEditor.Timeline;
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
    [SerializeField] private GameObject arrow;
    [SerializeField] private bool offWall = false;

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
    [SerializeField] public Transform uiTarget;


    private float m_defaultSpeed;
    private float m_defaultGrabForce;
    private Vector2 m_velocity;
    private Vector2 m_moveInputValue;
    // private Vector2 m_position2D;
    private Rigidbody2D body;
    private PlayerInput m_playerInput;
    private Action m_doAction;
    private Rope linkedRope; 
    private CapsuleCollider2D myCollider;
    private CapsuleCollider2D teamMateCollider;
    private bool m_onGap = false;
    private Action m_doTriggerAction;
    private bool m_isGrabbing = false;

    private bool m_isPlaying = false;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        m_doAction = DoFixOnPlace ;
        myCollider = GetComponentInChildren<CapsuleCollider2D>();
        m_doTriggerAction = DoVoid;
        m_defaultSpeed = m_speed;
        m_defaultGrabForce = m_grabForce;
        arrow.SetActive(false);

        GameManager.Instance.OnGameStart += StartPlaying;
        GameManager.Instance.OnGameEnd += StopPlaying;
    }

    private void StartPlaying()
    {
        m_isPlaying = true;
        m_doAction = DoMoveOnWall;
    }

    private void StopPlaying()
    {
        m_isPlaying = false;
        m_doAction = DoFixOnPlace;
    }

    public void Init(PlayerInput pPlayerInput)
    {
        ResetPlayerInput();

        m_playerInput = pPlayerInput;

        pPlayerInput.actions.FindAction("Move").performed += OnMoveInput;
        pPlayerInput.actions.FindAction("Move").canceled += OnMoveInput;
        pPlayerInput.actions.FindAction("Jump").performed += OnJumpInput;
        pPlayerInput.actions.FindAction("Grab").performed += OnGrabInput;
        pPlayerInput.actions.FindAction("Grab").canceled += OnGrabInput;
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
        m_playerInput.actions.FindAction("Move").canceled -= OnMoveInput;
        m_playerInput.actions.FindAction("Jump").performed -= OnJumpInput;
        m_playerInput.actions.FindAction("Grab").performed -= OnGrabInput;
        m_playerInput.actions.FindAction("Grab").canceled -= OnGrabInput;
    }

    private void DoVoid()
    {
    }

    private void DoFixOnPlace()
    {
        body.velocity = Vector2.zero;
        body.gravityScale = 0;
    }
    private void FixedUpdate()
    {
        // m_position2D = transform.position;
        m_velocity = body.velocity;
        m_doAction();
        m_doTriggerAction();
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
                // m_animator.SetTrigger("Fall");
                break;
            case "Projectile":
                Destroy(other.gameObject);
                offWall = true;
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

            // body.MovePosition(m_position2D + m_velocity * Time.fixedDeltaTime);
            body.velocity = m_velocity;

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

    private void DoMoveOnWall()
    {
        if (offWall)
        {
            // if(m_onGap && m_velocity.y > Physics2D.gravity.y)
            // {
            //     m_velocity.y = Physics2D.gravity.y;
            //     body.velocity = m_velocity;
            // }

            SetModeFall();
            return;
        }
        
        m_velocity = Vector2.MoveTowards(m_velocity, m_moveInputValue * (m_speed * Time.fixedDeltaTime), m_acceleration * Time.fixedDeltaTime);
        
        body.velocity = m_velocity;
        // body.MovePosition(m_position2D + m_velocity * Time.fixedDeltaTime);

        float lRatio = m_defaultSpeed * Time.fixedDeltaTime;

        m_animator.SetFloat("InputX", m_velocity.x / lRatio);
        m_animator.SetFloat("InputY", m_velocity.y / lRatio);
        m_animator.SetFloat("Velocity", (m_velocity.magnitude / lRatio) /* Mathf.Sign(m_moveInputValue.x) * Mathf.Sign(m_moveInputValue.y)*/);
        
    }

    private void DoGrab()
    {
        // if (m_velocity.magnitude < 0.25f || !offWall)
        // {
        //     offWall = false;
        //     m_doAction = DoMoveOnWall;
        // }
        // else
        // {
            m_velocity = Vector2.MoveTowards(body.velocity, Vector2.zero, m_grabForce * Time.fixedDeltaTime);
            
            // body.MovePosition(m_position2D + m_velocity * Time.fixedDeltaTime);
            body.velocity = m_velocity;
        // }
    }

    private void SetModeFall()
    {
        m_animator.SetTrigger("Fall");
        body.gravityScale = 1;
        m_doAction = DoFall;
    }

    private void DoFall()
    {
        m_animator.SetFloat("Velocity", Mathf.Clamp01(Mathf.Abs(m_velocity.y)/(-Physics2D.gravity.y)));
    }

    // Unity event called by new input system
    public void OnMoveInput(CallbackContext ctx)
    {
        if(!m_isPlaying)
            return;
        
        m_moveInputValue = ctx.ReadValue<Vector2>();
    }

    public void OnJumpInput(CallbackContext ctx)
    {
        if(!m_isPlaying)
            return;

        if(ctx.performed && !offWall)
        {
            offWall = true;
            if (linkedRope.IsInTension())
            {
                linkedRope.ReleaseTenseOnThisAncor(gameObject);
            }
        }
    }

    public void OnGrabInput(CallbackContext ctx)
    {
        if(!m_isPlaying)
            return;
        
        if(ctx.performed && !m_onGap && !m_isStun)
        {
            m_isGrabbing = true;
            offWall = false;
            m_animator.SetBool("Grab", true);
            Physics2D.IgnoreCollision(myCollider, teamMateCollider, false);
            body.gravityScale = 0;
            m_doAction = DoGrab;

        }
        else if(ctx.canceled)
        {
            //offWall = true;

            body.gravityScale = 1;
            Physics2D.IgnoreCollision(myCollider, teamMateCollider, true);
            /*m_velocity = new Vector2();
            body.velocity = new Vector2();*/
            
            // if (linkedRope.IsInTension())
            // {
            //     linkedRope.ReleaseTenseOnThisAncor(this.gameObject);
            // }

            m_isGrabbing = false;
            m_animator.SetBool("Grab", false);

            if(!offWall)
                m_doAction = DoMoveOnWall;
            else
                SetModeFall();
        }
    }

    private void OnDisable()
    {
        if(m_playerInput == null)
            return;
        
        m_playerInput.actions.FindAction("Move").performed -= OnMoveInput;
        m_playerInput.actions.FindAction("Move").canceled -= OnMoveInput;
        m_playerInput.actions.FindAction("Jump").performed -= OnJumpInput;
        m_playerInput.actions.FindAction("Grab").performed -= OnGrabInput;
        m_playerInput.actions.FindAction("Grab").canceled -= OnGrabInput;

        Debug.Log("Player " + m_playerInput.playerIndex + " disabled");

        m_playerInput = null;
    }

    private void Update()
    {
        if (linkedRope.IsInTension())
        {
            arrow.SetActive(true);
            Vector2 dir = teamMateCollider.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) + Mathf.PI;
            
            arrow.transform.localScale = new Vector3(linkedRope.GetTensionRatio() * 10, 1, 1);
            arrow.transform.localPosition = new Vector3(Mathf.Cos(angle), MathF.Sin(angle)) * (1.5f + 0.1f * arrow.transform.localScale.x);
            arrow.transform.localRotation = Quaternion.Euler( 0, 0, angle * 180 / Mathf.PI);
        }
        else
        {
            arrow.SetActive(false);
        }
    }
}
