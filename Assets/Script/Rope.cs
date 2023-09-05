using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField] private GameObject firstAncor;
    [SerializeField] private GameObject secondAncor;
    
    [SerializeField] private GameObject segment;
    [SerializeField] private int segmentCount = 20;
    private List<GameObject> ropeSegments = new List<GameObject>();
    private bool firstFrame = true;
    
    // Start is called before the first frame update
    void Start()
    {
        Vector3 holder = secondAncor.transform.position;

        Vector3 startPos = firstAncor.transform.position;

        for (int i = 0; i < segmentCount; i++)
        {
            GameObject instantiated = Instantiate(segment);
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

                continue;
            }

            joints[1].connectedBody = ropeSegments[i + 1].GetComponent<Rigidbody2D>();
        }
    }
}
