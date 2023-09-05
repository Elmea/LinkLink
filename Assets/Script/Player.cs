using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class Player : MonoBehaviour
{
    private Vector3 velocity;
    private Rigidbody2D body;
    [SerializeField] private bool offWall = false;
    
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }
    
    private void FixedUpdate()
    {
        if (offWall)
        {
            velocity += WorldSettings.gravity * Time.fixedDeltaTime;
            transform.position += velocity * Time.fixedDeltaTime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
