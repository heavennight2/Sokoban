using UnityEngine;
using UnityEngine.Tilemaps;

public class GridController : MonoBehaviour
{
    public TileBase blockTile;

    public static GridController instance;

    private Grid grid;
    private Tilemap tilemap;

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
        }

        tilemap = transform.Find("interaction").GetComponent<Tilemap>();
    }

    public Vector3 GridToWorldPos(int x, int y)
    {
        return grid.CellToWorld(new Vector3Int(x, y, 0));
    }

    public string GetTile(int x, int y)
    {
        TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));

        if (tile == null)
        {
            return null;
        }

        return tile.name;
    }

    public void PushBlock(Vector3Int blockStart, Vector3Int blockEnd)
    {
        // erase the block from its old position
        tilemap.SetTile(blockStart, null);

        // put the block in its new position
        tilemap.SetTile(blockEnd, blockTile);
    }
}