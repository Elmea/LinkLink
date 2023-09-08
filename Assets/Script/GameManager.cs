using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public event Action OnGameStart;
    public event Action OnGameEnd;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple GameManager in scene");
            Destroy(this);
        }
    }

    [ContextMenu("StartGame")]
    public void StartGame()
    {
        OnGameStart?.Invoke();
    }

    [ContextMenu("EndGame")]
    public void EndGame()
    {
        OnGameEnd?.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
