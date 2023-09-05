using UnityEngine;

public class WorldSettings : MonoBehaviour
{
    [SerializeField] private Vector3 m_gravityParam = new Vector3(0.0f, -9.81f, 0.0f);
    
    public static Vector3 gravity;

    private void Start()
    {
        gravity = m_gravityParam;
    }
}
