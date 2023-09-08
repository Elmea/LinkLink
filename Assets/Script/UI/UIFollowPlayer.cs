using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowPlayer : MonoBehaviour
{
    private Transform m_playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetPlayerTransform(Transform pPlayerTransform)
    {
        m_playerTransform = pPlayerTransform;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_playerTransform != null)
        {
            // follow the screen position of the player
            Vector3 screenPos = Camera.main.WorldToScreenPoint(m_playerTransform.position);
            transform.position = screenPos;
        }
    }
}
