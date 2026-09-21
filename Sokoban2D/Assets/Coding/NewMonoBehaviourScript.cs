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

    private void Move(int xmove, int ymove)
    {
        int targetx = x + xmove;
        int targety = y + ymove;

        string target =
            GridController.instance.GetTile(targetx, targety);

        if (target == null || target == "tree" || target == "goal")
        {
            x = targetx;
            y = targety;
        }
        else if (target == "Box")
        {
            Vector3Int blockStart =
                new Vector3Int(targetx, targety, 0);

            Vector3Int blockEnd =
                new Vector3Int(
                    targetx + xmove,
                    targety + ymove,
                    0
                );

            if (GridController.instance.CanPushBlock(blockStart, blockEnd))
            {
                GridController.instance.PushBlock(blockStart, blockEnd);

                x = targetx;
                y = targety;
            }
        }

        transform.position =
            GridController.instance.GridToWorldPos(x, y);
    }

    public void OnPlayerMovement(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Vector2 movementInput = context.ReadValue<Vector2>();

            int horizontal = Mathf.RoundToInt(movementInput.x);
            int vertical = Mathf.RoundToInt(movementInput.y);

            Move(horizontal, vertical);
        }
    }
}