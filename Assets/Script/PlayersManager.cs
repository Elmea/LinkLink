using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayersManager : MonoBehaviour
{
    [SerializeField] private Player[] m_playersBody;
    [SerializeField] private GameObject[] m_playersUI;

    public void OnPlayerSpawn(PlayerInput pPlayerInput)
    {
        PlayerConfig playerConfig = pPlayerInput.GetComponent<PlayerConfig>();

        if (playerConfig == null)
        {
            Debug.LogError("PlayerConfig not found on player");
            return;
        }

        int playerIndex = pPlayerInput.playerIndex;

        if (playerIndex >= m_playersBody.Length)
        {
            Debug.LogError("Player index out of range");
            return;
        }

        playerConfig.init(m_playersBody[playerIndex], pPlayerInput);

        m_playersUI[playerIndex].SetActive(true);
        m_playersUI[playerIndex].GetComponent<UIFollowPlayer>().SetPlayerTransform(m_playersBody[playerIndex].uiTarget);
    }
}
