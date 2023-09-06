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
    [SerializeField] private float m_airAcceleration = 5f;
    [SerializeField] private float m_grabForce = 40f;
    private Vector2 m_velocity;
    private Vector2 m_moveInputValue;
    private Vector2 m_position2D;
    private Rigidbody2D body;
    private PlayerInput m_playerInput;
    [SerializeField] private bool offWall = false;
    private bool grabbing = false;

    private Action m_doAction;
    private Rope linkedRope; 
    private Vector3 grabPos;
    private CapsuleCollider2D myCollider;
    private CapsuleCollider2D teamMateCollider;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        m_doAction = DoMoveOnWall;
        myCollider = GetComponentInChildren<CapsuleCollider2D>();
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
        body.MovePosition(m_position2D + m_velocity * Time.fixedDeltaTime);
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
            offWall = false;
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
        if (grabbing)
        {
            if (m_velocity.magnitude < 0.1f)
            {
                body.velocity = new Vector2(0, 0);
                m_velocity = new Vector2(0, 0);
                transform.position = grabPos;
            }
        }
    }
}
