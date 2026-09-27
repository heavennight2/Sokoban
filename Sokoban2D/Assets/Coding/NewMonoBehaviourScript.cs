using UnityEngine;
using UnityEngine.InputSystem;

public class NewMonoBehaviourScript : MonoBehaviour
{
    private int x;
    private int y;

    void Start()
    {
        Vector3Int startCell =
            GridController.instance.WorldToGridPos(transform.position);

        x = startCell.x;
        y = startCell.y;

        transform.position =
            GridController.instance.GridToWorldPos(x, y);
    }


    void Update()
    {

    }


    private void Move(int xmove, int ymove)
    {
        int targetx = x + xmove;
        int targety = y + ymove;

        string target =
            GridController.instance.GetTile(targetx, targety);


        // Empty space OR spikes are turned off
        if(target == null || target == "SpikesOff")
        {
            x = targetx;
            y = targety;
        }


        // Wall
        else if(target == "tree")
        {
        }


        // Closed Chest
        else if(target == "ChestClosed")
        {
            GridController.instance.OpenChest(
                targetx,
                targety
            );

            Debug.Log("YOU WIN!");
        }


        // Box
        else if(target == "Box")
        {
            Vector3Int blockStart =
                new Vector3Int(targetx, targety, 0);

            Vector3Int blockEnd =
                new Vector3Int(
                    targetx + xmove,
                    targety + ymove,
                    0
                );


            if(GridController.instance.CanPushBlock(
                blockStart,
                blockEnd))
            {
                // Push the box
                GridController.instance.PushBlock(
                    blockStart,
                    blockEnd,
                    xmove,
                    ymove
                );


                // Did we push it onto a goal?
                if(GridController.instance.IsGoal(
                    blockEnd.x,
                    blockEnd.y))
                {
                }


                // Did we push it onto the button?
                if(GridController.instance.IsButton(
                    blockEnd.x,
                    blockEnd.y))
                {
                    GridController.instance.ActivateButton(
                        blockEnd.x,
                        blockEnd.y
                    );
                }


                if(GridController.instance.IsGoal(
                    blockStart.x,
                    blockStart.y))
                {
                }


                x = targetx;
                y = targety;
            }
        }


        transform.position =
            GridController.instance.GridToWorldPos(x, y);
    }


    public void OnPlayerMovement(
        InputAction.CallbackContext context)
    {
        if(context.started)
        {
            Vector2 movementInput =
                context.ReadValue<Vector2>();

            Move(
                (int)movementInput.x,
                (int)movementInput.y
            );
        }
    }
}