using UnityEngine;
using UnityEngine.InputSystem;

public class NewMonoBehaviourScript : MonoBehaviour

{
    private int x;
    private int y;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        x = 0;
        y = 0;
        Move(0, 0); // this just snaps us to (0,0) on the grid
    }

    // Update is called once per frame
    void Update()
    {
        // your job: move based on player input
    }


    private void Move(int xmove, int ymove)
    {
        // calculate where we want to move to...
        int targetx = x + xmove;
        int targety = y + ymove;

        
        string target = GridController.instance.GetTile(targetx, targety);

        // if null, the spot is empty
        if(target == null) {
            x = targetx;
            y = targety;
        }
        else if (target =="Box"){
            
            Vector3Int blockStart = new Vector3Int(targetx, targety, 0);
            Vector3Int blockEnd = new Vector3Int(targetx + xmove, targety + ymove, 0);
            GridController.instance.PushBlock(blockStart, blockEnd);

            x = targetx;
            y = targety;



                
                  }

        // convert grid coordinates to world space, and move to the world space position
        transform.position = GridController.instance.GridToWorldPos(x, y);
    }


    public void OnPlayerMovement(InputAction.CallbackContext context)
    {
        // same as Input.GetKeyDown
        if(context.started) {
            Vector2 movementInput = context.ReadValue<Vector2>();
            Move((int)movementInput.x, (int)movementInput.y);
        }
    }
}