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

    [ContextMenu("Test")]
    public void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TriggerEnter");
        Player player = other.GetComponentInParent<Player>();
        if (other.gameObject.layer == 6)
        {
            Debug.Log("PlayerArrived");
            player.IsArrived = true;
        }
    }

	public void OnTriggerExit2D(Collider2D other)
    {
        Player player = other.GetComponentInParent<Player>();
		if (other.gameObject.layer == 6)
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
            //TextToShow.enabled = true;
            //TextToShow.text = "Team 1 win !";
			UIManager.Instance.OpenWinScreen();

		}

		if (trackedTeam2.GetPlayers()[0].IsArrived && trackedTeam2.GetPlayers()[1].IsArrived)
        {
            trackedTeam1.GetPlayers()[0].StartFalling();
            trackedTeam1.GetPlayers()[1].StartFalling();
            gameManager.EndGame();
            //TextToShow.enabled = true;
            //TextToShow.text = "Team 2 win !";
			UIManager.Instance.OpenWinScreen();

		}
	}
}
