using UnityEngine;
using UnityEngine.Tilemaps;

public class GridController : MonoBehaviour
{
    public int numGoalsInScene;
    public TileBase blockTile;

    // Button + Spikes
    public TileBase buttonOnTile;
    public TileBase buttonOffTile;
    public TileBase spikesOnTile;
    public TileBase spikesOffTile;

    // Level 1 = 1
    // Level 2 = 3
    public int buttonsNeeded = 1;

    // Chest
    public TileBase chestOpenTile;

    public static GridController instance;

    private Grid grid;
    private Tilemap tilemap;
    private Tilemap specialTilemap;


    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;

        grid = GetComponent<Grid>();

        if(grid == null)
        {
            Debug.LogError(
                "there's no grid...did you add this to the wrong place???"
            );
        }

        tilemap =
            transform.Find("interaction").GetComponent<Tilemap>();

        specialTilemap =
            transform.Find("goals").GetComponent<Tilemap>();
    }


    public Vector3 GridToWorldPos(int x, int y)
    {
        return grid.CellToWorld(
            new Vector3Int(x, y, 0)
        );
    }


    public Vector3Int WorldToGridPos(Vector3 worldPosition)
    {
        return grid.WorldToCell(worldPosition);
    }


    public string GetTile(int x, int y)
    {
        TileBase tile =
            tilemap.GetTile(
                new Vector3Int(x, y, 0)
            );

        if(tile == null)
        {
            return null;
        }

        return tile.name;
    }


    public bool IsGoal(int x, int y)
    {
        TileBase tile =
            specialTilemap.GetTile(
                new Vector3Int(x, y, 0)
            );

        if(tile == null)
        {
            return false;
        }

        return tile.name == "Goal";
    }


    public bool IsButton(int x, int y)
    {
        TileBase tile =
            specialTilemap.GetTile(
                new Vector3Int(x, y, 0)
            );

        if(tile == null)
        {
            return false;
        }

        return tile.name == "ButtonOff" ||
               tile.name == "ButtonOn";
    }


    // =========================
    // BOX
    // =========================

    public bool CanPushBlock(
        Vector3Int blockStart,
        Vector3Int blockEnd
    )
    {
        string target =
            GetTile(blockEnd.x, blockEnd.y);

        // Box can move into empty interaction space
        if(target == null)
        {
            return true;
        }

        // Cannot push another Box
        if(target == "Box")
        {
            return false;
        }

        // Everything else blocks the Box
        return false;
    }


    public void PushBlock(
        Vector3Int start,
        Vector3Int end,
        int xmove,
        int ymove
    )
    {
        // Remove Box from old cell
        tilemap.SetTile(start, null);

        // Put Box in new cell
        tilemap.SetTile(end, blockTile);

        // Re-check all buttons
        UpdateButtons();
    }


    // =========================
    // BUTTONS
    // =========================

    public void ActivateButton(int x, int y)
    {
        // Player script already calls this.
        // We simply check every button.
        UpdateButtons();
    }


    private void UpdateButtons()
    {
        int buttonsOn = 0;

        foreach(
            Vector3Int position
            in specialTilemap.cellBounds.allPositionsWithin
        )
        {
            TileBase buttonTile =
                specialTilemap.GetTile(position);

            if(buttonTile == null)
            {
                continue;
            }


            // Ignore anything that isn't a button
            if(
                buttonTile.name != "ButtonOff" &&
                buttonTile.name != "ButtonOn"
            )
            {
                continue;
            }


            bool hasBox =
                GetTile(position.x, position.y) == "Box";


            // BOX ON BUTTON
            if(hasBox)
            {
                if(buttonTile.name == "ButtonOff")
                {
                    specialTilemap.SetTile(
                        position,
                        buttonOnTile
                    );
                }

                buttonsOn++;
            }


            // NO BOX ON BUTTON
            else
            {
                if(buttonTile.name == "ButtonOn")
                {
                    specialTilemap.SetTile(
                        position,
                        buttonOffTile
                    );
                }
            }
        }


        Debug.Log(
            "Buttons: "
            + buttonsOn
            + "/"
            + buttonsNeeded
        );


        // Enough buttons are pressed
        if(buttonsOn >= buttonsNeeded)
        {
            OpenSpikes();
        }

        // Not enough buttons
        else
        {
            CloseSpikes();
        }
    }


    // =========================
    // SPIKES
    // =========================

    private void OpenSpikes()
    {
        foreach(
            Vector3Int position
            in tilemap.cellBounds.allPositionsWithin
        )
        {
            TileBase tile =
                tilemap.GetTile(position);

            if(
                tile != null &&
                tile.name == "SpikesOn"
            )
            {
                tilemap.SetTile(
                    position,
                    spikesOffTile
                );
            }
        }
    }


    private void CloseSpikes()
    {
        foreach(
            Vector3Int position
            in tilemap.cellBounds.allPositionsWithin
        )
        {
            TileBase tile =
                tilemap.GetTile(position);

            if(
                tile != null &&
                tile.name == "SpikesOff"
            )
            {
                tilemap.SetTile(
                    position,
                    spikesOnTile
                );
            }
        }
    }


    // =========================
    // CHEST
    // =========================

    public bool IsChest(int x, int y)
    {
        TileBase tile =
            tilemap.GetTile(
                new Vector3Int(x, y, 0)
            );

        if(tile == null)
        {
            return false;
        }

        return tile.name == "ChestClosed";
    }


    public void OpenChest(int x, int y)
    {
        tilemap.SetTile(
            new Vector3Int(x, y, 0),
            chestOpenTile
        );

        Debug.Log("LEVEL COMPLETE!");
    }
}