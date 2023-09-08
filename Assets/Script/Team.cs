using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    private Player[] players;

    public Player[] GetPlayers()
    {
        return players;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        players = GetComponentsInChildren<Player>();
    }
}
