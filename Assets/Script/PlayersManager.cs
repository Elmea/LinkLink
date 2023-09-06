using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class PlayersManager : MonoBehaviour
{
    private ReadOnlyArray<Gamepad> m_gamepads;
    private Player[] m_players; 
    private PlayerInput[] m_playerInputs; 

    // Start is called before the first frame update
    void Start()
    {
        m_players = FindObjectsByType<Player>(FindObjectsSortMode.InstanceID);
        m_playerInputs = FindObjectsByType<PlayerInput>(FindObjectsSortMode.InstanceID);
        m_gamepads = Gamepad.all;
    }

    public void AssignPlayerToController(int playerId, int gamepadId)
    {
        m_playerInputs[gamepadId].SwitchCurrentControlScheme(m_gamepads[gamepadId]);
        m_players[playerId].Init(m_playerInputs[playerId]);
    }
}
