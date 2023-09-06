using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GapTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("GapTrigger OnTriggerEnter");
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Player>().SetOffWall();
            Debug.Log("Player fall of the wall");
        }
    }
}
