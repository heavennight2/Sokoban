using UnityEngine;
using UnityEngine.Tilemaps;

public class GridController : MonoBehaviour
{
    public int numGoalsInScene;
    public TileBase blockTile;

    public static GridController instance;

    private Grid grid;
    private Tilemap tilemap;
    private Tilemap specialTilemap;

    private void Awake()
    {
        if(instance != null) {
            Destroy(this);
            return;
        }

        instance = this;

        grid = GetComponent<Grid>();
        if(grid == null) {
            Debug.LogError("there's no grid...did you add this to the wrong place???");
        }

        tilemap = transform.Find("interaction").GetComponent<Tilemap>();
        specialTilemap = transform.Find("goals").GetComponent<Tilemap>();
    }


    public Vector3 GridToWorldPos(int x, int y)
    {
        return grid.CellToWorld(new Vector3Int(x, y, 0));
    }


    public string GetTile(int x, int y)
    {
        TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));

        if(tile == null) {
            return null;
        }

        return tile.name;
    }


    public bool IsGoal(int x, int y)
    {
        TileBase tile = specialTilemap.GetTile(new Vector3Int(x, y, 0));

        if(tile == null) {
            return false;
        }

        return tile.name == "Goal";
    }


    public void PushBlock(Vector3Int start, Vector3Int end, int xmove, int ymove)
    {
        if(GetTile(end.x, end.y) == "Box") {

            Vector3Int blockStart = end;

            Vector3Int blockEnd =
                blockStart + new Vector3Int(xmove, ymove, 0);

            PushBlock(blockStart, blockEnd, xmove, ymove);
        }

        tilemap.SetTile(start, null);

        // draw a box where it ends up
        tilemap.SetTile(end, blockTile);
    }


    public bool CanPushBlock(Vector3Int blockStart, Vector3Int blockEnd)
    {
        Vector3Int direction = blockEnd - blockStart;

        while(GetTile(blockEnd.x, blockEnd.y) == "Box") {
            blockEnd += direction;
        }

        string target = GetTile(blockEnd.x, blockEnd.y);

        if(target == null || target == "goal")
        {
            return true;
        }

        return false;
    }
}