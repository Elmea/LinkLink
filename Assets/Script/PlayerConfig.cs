using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerConfig : MonoBehaviour
{
    [SerializeField] private Player m_playerBody;
    private PlayerInput m_playerInput;
    public static event Action<int, int> OnPlayerChangeTeam;
    
    // Start is called before the first frame update
    void Start()
    {
        m_playerInput = GetComponent<PlayerInput>();
    }

    private void OnChangeteam(CallbackContext ctx)
    {
        int direction = ctx.ReadValue<Vector2>().x > 0 ? 1 : -1;

        OnPlayerChangeTeam?.Invoke(m_playerInput.playerIndex, direction);
    }
}
