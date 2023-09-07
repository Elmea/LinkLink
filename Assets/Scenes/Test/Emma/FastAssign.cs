using UnityEngine;

public class FastAssign : MonoBehaviour
{
    private bool once = true;
    
    // Update is called once per frame
    void Update()
    {
        if (once)
        {
            PlayersManager manager = GetComponent<PlayersManager>();

            manager.AssignPlayerToController(1, 0);
            
            once = false;
        }
    }
}
