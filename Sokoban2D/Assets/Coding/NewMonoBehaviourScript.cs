using UnityEngine;
using UnityEngine.InputSystem;

public class NewMonoBehaviourScript : MonoBehaviour
{
    private int x;
    private int y;

    void Start()
    {
        x = 0;
        y = 0;
        Move(0, 0); 
    }

  
    void Update()
    {
        
    }


    private void Move(int xmove, int ymove)
    {
        int targetx = x + xmove;
        int targety = y + ymove;

        string target = GridController.instance.GetTile(targetx, targety);

        if(target == null) {
            x = targetx;
            y = targety;
        }
        else if(target == "tree") {
        }
        else if(target == "Box") {
            Vector3Int blockStart = new Vector3Int(targetx, targety, 0);
            Vector3Int blockEnd = new Vector3Int(targetx + xmove, targety + ymove, 0);
            
            if(GridController.instance.CanPushBlock(blockStart, blockEnd))
            {
                // if so, push!
                GridController.instance.PushBlock(blockStart, blockEnd, xmove, ymove);

                // did we push it onto a goal?
                if(GridController.instance.IsGoal(blockEnd.x, blockEnd.y))
                {
                }

                if(GridController.instance.IsGoal(blockStart.x, blockStart.y))
                {
                }

                x = targetx;
                y = targety;
            }            
        }

        transform.position = GridController.instance.GridToWorldPos(x, y);
    }


    public void OnPlayerMovement(InputAction.CallbackContext context)
    {
        if(context.started) {
            Vector2 movementInput = context.ReadValue<Vector2>();
            Move((int)movementInput.x, (int)movementInput.y);
        }
    }
}