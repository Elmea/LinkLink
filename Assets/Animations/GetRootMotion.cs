using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetRootMotion : MonoBehaviour
{
    [SerializeField] private Animator m_otherAnimator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += m_otherAnimator.velocity * Time.deltaTime;
    }
}
