using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class RopeSegment : MonoBehaviour
{
    [SerializeField] private float pullForce = 1.0f;
    private Rope myRope;
    
    private GameObject before;
    private Rigidbody2D beforeBody;

    private GameObject after;
    private Rigidbody2D afterBody;

    private bool isSettedUp = false;

    public void AssignRope(Rope _rope)
    {
        myRope = _rope;
    }
    
    public void AssignNeighbor(GameObject _before, GameObject _after)
    {
        before = _before;
        beforeBody = before.GetComponent<Rigidbody2D>();
        after = _after;
        afterBody = after.GetComponent<Rigidbody2D>();

        if (beforeBody != null && afterBody != null)
        {
            isSettedUp = true;
        }
    }

    private void PullNeighbors()
    {
        if (isSettedUp)
        {
            Vector2 forceDirection = new Vector2(transform.position.x - before.transform.position.x ,
                                    transform.position.y - before.transform.position.y);
            beforeBody.AddForce(forceDirection * pullForce);
            
            forceDirection = new Vector2(transform.position.x - after.transform.position.x ,
                transform.position.y - after.transform.position.y);
            afterBody.AddForce(forceDirection * pullForce);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (myRope.ShouldPull())
            PullNeighbors();
    }
}
