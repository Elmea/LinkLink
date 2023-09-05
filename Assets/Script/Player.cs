using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using Vector2 = UnityEngine.Vector2;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
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
        }

        transform.position += velocity * Time.fixedDeltaTime;
    }

    // Unity event called by new input system
    public void OnMove(CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        velocity = new Vector3(input.x, input.y, 0) * speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
