using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField] private GameObject firstAncor;
    [SerializeField] private GameObject secondAncor;
    [SerializeField] private GameObject segment;
    [SerializeField] private int segmentCount = 20;
    [SerializeField] private float minimumTenseDistance = 8;
    [SerializeField] private float maximumTenseDistance = 12;
    [SerializeField] private float maximumTenseForce = 20;
    [SerializeField] private float minDistanceToPull = 5;
    [SerializeField] private float maximumDistanceBetweenAncor = 15;

    private List<GameObject> ropeSegments = new List<GameObject>();
    private bool firstFrame = true;
    private Vector3 positionP2Holder;
    private float tenseScale;
    private Rigidbody2D firstSegmentBody;
    private Rigidbody2D lastSegmentBody;
    private bool shouldPull;

    private float tenseTimer = 0.0f;
    
    private float tensionForce;
    
    public Vector2 ancorsDistance;

    public bool IsInTension() { return tensionForce > 0; }
    public bool ShouldPull() { return shouldPull; }

    public float GetTensionForce() { return tensionForce; }
    private Vector2 GetTensionDirAncor1()
    {
        return new Vector2(secondAncor.transform.position.x - firstAncor.transform.position.x,
            secondAncor.transform.position.y - firstAncor.transform.position.y);
    }

    private Vector2 GetTensionDirAncor2()
    {
        return new Vector2(firstAncor.transform.position.x - secondAncor.transform.position.x,
            firstAncor.transform.position.y - secondAncor.transform.position.y);
    }

    public void ReleaseTenseOnThisAncor(GameObject ancor)
    {
        if (tenseTimer > 0)
            return;

        tenseTimer = 2.0f;
        Vector2 resultForce;
        if (ancor == firstAncor)
        {
            resultForce = tensionForce * GetTensionDirAncor1();
        }
        else if (ancor == secondAncor)
        {
            resultForce = tensionForce * GetTensionDirAncor2();
        }
        else
        {
            Debug.Log("fail");
            return;
        }

        ancor.GetComponent<Rigidbody2D>().AddForce(resultForce, ForceMode2D.Impulse);
        Debug.Log(resultForce);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        positionP2Holder = secondAncor.transform.position;
        
        Vector3 startPos = firstAncor.transform.position;

        firstAncor.GetComponent<DistanceJoint2D>().connectedBody = secondAncor.GetComponent<Rigidbody2D>();
        secondAncor.GetComponent<DistanceJoint2D>().connectedBody = firstAncor.GetComponent<Rigidbody2D>();

        for (int i = 0; i < segmentCount; i++)
        {
            GameObject instantiated = Instantiate(segment);
            instantiated.GetComponent<RopeSegment>().AssignRope(this);
            instantiated.transform.position = new Vector3(
                startPos.x - firstAncor.transform.localScale.x * 0.5f - (1 + i) * instantiated.transform.localScale.x * 1.25f,
                startPos.y,
                startPos.z);
            ropeSegments.Add(instantiated);

            HingeJoint2D[] joints = ropeSegments[i].GetComponents<HingeJoint2D>();
            if (i == 0)
            {
                joints[0].connectedBody = firstAncor.GetComponent<Rigidbody2D>();
                firstAncor.GetComponent<HingeJoint2D>().connectedBody = instantiated.GetComponent<Rigidbody2D>();
                continue;
            }

            joints[0].connectedBody = ropeSegments[i - 1].GetComponent<Rigidbody2D>();
        }

        for (int i = 0; i < segmentCount; i++)
        {
            HingeJoint2D[] joints = ropeSegments[i].GetComponents<HingeJoint2D>();
            if (i == segmentCount - 1)
            {
                joints[1].connectedBody = secondAncor.GetComponent<Rigidbody2D>();
                secondAncor.transform.position = new Vector3(
                        ropeSegments[i].transform.position.x - ropeSegments[i].transform.localScale.x * 2,
                        ropeSegments[i].transform.position.y,
                        ropeSegments[i].transform.position.z);
                secondAncor.GetComponent<HingeJoint2D>().connectedBody = ropeSegments[i].GetComponent<Rigidbody2D>();
                ropeSegments[i].GetComponent<RopeSegment>().AssignNeighbor( ropeSegments[i - 1], secondAncor);

                continue;
            }

            joints[1].connectedBody = ropeSegments[i + 1].GetComponent<Rigidbody2D>();

            if (i == 0)
            {
                ropeSegments[i].GetComponent<RopeSegment>().AssignNeighbor( firstAncor, ropeSegments[i + 1]);
                continue;
            }

            ropeSegments[i].GetComponent<RopeSegment>().AssignNeighbor( ropeSegments[i - 1], ropeSegments[i + 1]);
        }

        tenseScale = maximumTenseDistance - minimumTenseDistance;

        Player firstPlayer = firstAncor.GetComponent<Player>();
        Player secondPlayer = secondAncor.GetComponent<Player>();
        if (firstPlayer != null)
        {
            firstPlayer.LinkRope(this);
            firstPlayer.GetComponent<DistanceJoint2D>().distance = maximumDistanceBetweenAncor;
        }

        if (secondPlayer != null)
        {
            secondPlayer.LinkRope(this);
            firstPlayer.GetComponent<DistanceJoint2D>().distance = maximumDistanceBetweenAncor;
        }

        if (firstPlayer != null && secondPlayer != null)
        {
            firstPlayer.SetTeamMateCollider(secondPlayer.GetComponentInChildren<CapsuleCollider2D>());
            secondPlayer.SetTeamMateCollider(firstPlayer.GetComponentInChildren<CapsuleCollider2D>());
        }
    }

    private void Update()
    {
        if (firstFrame)
        {
            firstAncor.GetComponent<HingeJoint2D>().autoConfigureConnectedAnchor = true;
            secondAncor.GetComponent<HingeJoint2D>().autoConfigureConnectedAnchor = true;
            secondAncor.transform.position = positionP2Holder;
            firstFrame = false;
        }

        ancorsDistance = new Vector2(secondAncor.transform.position.x - firstAncor.transform.position.x,
            secondAncor.transform.position.y - firstAncor.transform.position.y);
        float tensionMag = ancorsDistance.magnitude;

        if (tensionMag > minimumTenseDistance)  
        {
            float tensionPercent = (tensionMag - minimumTenseDistance) / tenseScale;
            tensionForce = tensionPercent * maximumTenseForce; 
        }
        else
        {
            tensionForce = 0;
        }

        if (tensionMag > minDistanceToPull)
            shouldPull = true;
        else
            shouldPull = false;
        
        if (tenseTimer > 0)
            tenseTimer -= Time.deltaTime;
    }
}
