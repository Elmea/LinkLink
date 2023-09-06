using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveArround : MonoBehaviour
{
    private float timer = 5.0f;

    private Vector3 dir = new Vector3(1.0f, 0.0f, 0.0f);
    private Rigidbody2D body;
    [SerializeField] private float force;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        body.MovePosition(transform.position + dir * force);

        timer -= Time.deltaTime;
        if (timer < 0)
        {
            timer = 5.0f;
            dir = -dir;
        }
    }
}
