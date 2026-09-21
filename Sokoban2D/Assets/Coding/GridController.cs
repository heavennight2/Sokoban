using UnityEngine;
using UnityEngine.Tilemaps;

public class GridController : MonoBehaviour
{
    public TileBase blockTile;

    public static GridController instance;

    private Grid grid;
    private Tilemap tilemap;
    private Tilemap goalsTilemap;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;

        grid = GetComponent<Grid>();

        if (grid == null)
        {
            Debug.LogError("There's no Grid component!");
            return;
        }

        Transform interactionObject = transform.Find("interaction");
        Transform goalsObject = transform.Find("goals");

        if (interactionObject == null)
        {
            Debug.LogError("Couldn't find the interaction Tilemap!");
            return;
        }

        if (goalsObject == null)
        {
            Debug.LogError("Couldn't find the goals Tilemap!");
            return;
        }

        tilemap = interactionObject.GetComponent<Tilemap>();
        goalsTilemap = goalsObject.GetComponent<Tilemap>();

        if (tilemap == null)
        {
            Debug.LogError("interaction doesn't have a Tilemap component!");
        }

        if (goalsTilemap == null)
        {
            Debug.LogError("goals doesn't have a Tilemap component!");
        }
    }

    public Vector3 GridToWorldPos(int x, int y)
    {
        return grid.CellToWorld(new Vector3Int(x, y, 0));
    }

    public string GetTile(int x, int y)
    {
        TileBase tile =
            tilemap.GetTile(new Vector3Int(x, y, 0));

        if (tile == null)
        {
            return null;
        }

        return tile.name;
    }

    private bool IsBox(Vector3Int position)
    {
        TileBase tile = tilemap.GetTile(position);

        return tile == blockTile;
    }

    public bool CanPushBlock(
        Vector3Int blockStart,
        Vector3Int blockEnd
    )
    {
        Vector3Int direction = blockEnd - blockStart;

        while (IsBox(blockEnd))
        {
            blockEnd += direction;
        }

        TileBase targetTile = tilemap.GetTile(blockEnd);

        if (targetTile == null)
        {
            return true;
        }

        return false;
    }

    public void PushBlock(
        Vector3Int blockStart,
        Vector3Int blockEnd
    )
    {
        Vector3Int direction = blockEnd - blockStart;
        Vector3Int currentPosition = blockEnd;

        
        while (IsBox(currentPosition))
        {
            currentPosition += direction;
        }

        while (currentPosition != blockStart)
        {
            tilemap.SetTile(currentPosition, blockTile);
            currentPosition -= direction;
        }

        tilemap.SetTile(blockStart, null);

        CheckWin();
    }

    private void CheckWin()
    {
        int numberOfGoals = 0;

        foreach (
            Vector3Int position
            in goalsTilemap.cellBounds.allPositionsWithin
        )
        {
            if (goalsTilemap.HasTile(position))
            {
                numberOfGoals++;

                if (!IsBox(position))
                {
                    return;
                }
            }
        }

        if (numberOfGoals > 0)
        {
            Debug.Log("YOU WIN!");

            Time.timeScale = 0f;
        }
    }
}