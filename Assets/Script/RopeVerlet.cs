using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RopeSegment
{
    public Vector3 posNow;
    public Vector3 posOld;

    public RopeSegment(Vector3 pos)
    {
        this.posNow = pos;
        this.posOld = pos;
    }
}

public class RopeVerlet : MonoBehaviour
{
    [SerializeField] private GameObject firstAncor;
    [SerializeField] private GameObject secondAncor;
    
    private Vector3 startPoint;
    private Vector3 endPoint;

    [SerializeField] private int segmentCount = 35;
    [SerializeField] private float lineWidth = 0.1f;
    [SerializeField] private float segmentLenght = 0.25f;

    private List<RopeSegment> ropeSegments = new List<RopeSegment>();
    private LineRenderer lineRenderer;
    
    private void RopeSimulation()
    {
        for (int i = 0; i < segmentCount; i++)
        {
            RopeSegment segmentHolder = ropeSegments[i];
            Vector3 velocity = segmentHolder.posNow - segmentHolder.posOld;
            segmentHolder.posNow += velocity;
            segmentHolder.posNow += WorldSettings.gravity * Time.fixedDeltaTime;
            ropeSegments[i] = segmentHolder;
        }
        
        SimulateConstraint();
    }

    private void SimulateConstraint()
    {
        RopeSegment firstSegment = ropeSegments[0];
        firstSegment.posNow = firstAncor.transform.position;
        this.ropeSegments[0] = firstSegment;    

        RopeSegment endSegment = ropeSegments[ropeSegments.Count - 1];
        endSegment.posNow = secondAncor.transform.position;
        this.ropeSegments[ropeSegments.Count - 1] = endSegment;
    }
    
    private void DrawRope()
    {
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        Vector3[] ropePositions = new Vector3[segmentCount];
        for (int i = 0; i < segmentCount; i++)
        {
            ropePositions[i] = this.ropeSegments[i].posNow;
        }

        lineRenderer.positionCount = ropePositions.Length;
        lineRenderer.SetPositions(ropePositions);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        startPoint = firstAncor.transform.position;
        // endPoint = secondAncor.transform.position;
        
        Vector3 ropeSegmentStart = startPoint;
            
        for (int i = 0; i < segmentCount; i++)
        {
            ropeSegments.Add(new RopeSegment(ropeSegmentStart));
            ropeSegmentStart.y -= segmentLenght;
        }
    }

    private void FixedUpdate()
    {
        startPoint = firstAncor.transform.position;
        // endPoint = secondAncor.transform.position;
        RopeSimulation();
    }
    
    // Update is called once per frame
    void Update()
    {
        DrawRope();
    }
}
