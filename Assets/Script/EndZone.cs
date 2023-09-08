using System;
using System.Net.Mime;
using TMPro;
using UnityEngine;

public class EndZone : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Team trackedTeam1;
    [SerializeField] private Team trackedTeam2;
    [SerializeField] private TextMeshPro TextToShow;

    private void Start()
    {
        //TextToShow.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.IsArrived = true;
            UIManager.Instance.OpenWinScreen();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.IsArrived = false;
        }
    }

    private void Update()
    {
        if (trackedTeam1.GetPlayers()[0].IsArrived && trackedTeam1.GetPlayers()[1].IsArrived)
        {
            trackedTeam2.GetPlayers()[0].StartFalling();
            trackedTeam2.GetPlayers()[1].StartFalling();
            gameManager.EndGame();
            TextToShow.enabled = true;
            TextToShow.text = "Team 1 win !";
        }
        
        if (trackedTeam2.GetPlayers()[0].IsArrived && trackedTeam2.GetPlayers()[1].IsArrived)
        {
            trackedTeam1.GetPlayers()[0].StartFalling();
            trackedTeam1.GetPlayers()[1].StartFalling();
            gameManager.EndGame();
            TextToShow.enabled = true;
            TextToShow.text = "Team 2 win !";
        }
    }
}
