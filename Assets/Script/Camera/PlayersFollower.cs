using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersFollower : MonoBehaviour
{
    [SerializeField] Transform[] Players;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 ltarget = Vector2.zero;
        int counter = 0;
        foreach (var p in Players)
        {
            counter++;
            ltarget += p.position;
        }

        if(counter != 0)
            ltarget /= counter;

        transform.position = ltarget;
    }
}
