using System;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using Vector2 = UnityEngine.Vector2;

public class Player : MonoBehaviour
{
    [SerializeField] private float m_speed = 125f;
    [SerializeField] private float m_acceleration = 20f;
    [SerializeField] private float m_airAcceleration = 5f;
    [SerializeField] private float m_grabForce = 40f;
    private Vector3 m_velocity;
    private Vector2 m_moveInputValue;
    private Rigidbody2D body;
    [SerializeField] private bool offWall = false;

    private Action m_doAction;
    
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        m_doAction = DoMoveOnWall;
    }

    private void DoVoid()
    {
    }

    private void FixedUpdate()
    {
        m_doAction();
    }

    private void DoMoveOnWall()
    {
        m_velocity = Vector2.MoveTowards(m_velocity, m_moveInputValue * m_speed * Time.fixedDeltaTime, m_acceleration * Time.fixedDeltaTime);

        transform.position += m_velocity * Time.fixedDeltaTime;

        if(offWall)
            m_doAction = DoFall;
    }

    private void DoGrab()
    {
        m_velocity = Vector2.MoveTowards(m_velocity, Vector2.zero, m_grabForce * Time.fixedDeltaTime);

        transform.position += m_velocity * Time.fixedDeltaTime;
    }

    private void DoFall()
    {
        m_velocity.y += WorldSettings.gravity.y * Time.fixedDeltaTime;
        m_velocity.x = Mathf.MoveTowards(m_velocity.x, m_moveInputValue.x * m_speed * Time.fixedDeltaTime, m_airAcceleration * Time.fixedDeltaTime);

        transform.position += m_velocity * Time.fixedDeltaTime;
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
}
