using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float m_speed = 10f;
    private Rigidbody2D m_rb;


    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
    }

    public void SetSpeed(float speed)
    {
        m_speed = speed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //limit the fall speed
        if (m_rb.velocity.y < -m_speed)
        {
            m_rb.velocity = new Vector2(m_rb.velocity.x, -m_speed);
        }

        Debug.Log(m_rb.velocity);
    }
}
