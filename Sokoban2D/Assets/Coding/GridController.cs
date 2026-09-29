using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

public class GridController : MonoBehaviour
{
    public int numGoalsInScene;
    public TileBase blockTile;

    // Button + Spikes
    public TileBase buttonOnTile;
    public TileBase buttonOffTile;
    public TileBase spikesOnTile;
    public TileBase spikesOffTile;
    public GameObject winPanel;
    // Level 1 = 1
    // Level 2 = 3
    // Level 3 = حسب عدد الأزرار
    public int buttonsNeeded = 1;

    // Chest
    public TileBase chestOpenTile;

    // Chest Sound
    public AudioSource audioSource;
    public AudioClip chestSound;

    public static GridController instance;

    private Grid grid;
    private Tilemap tilemap;
    private Tilemap specialTilemap;

    // Prevent opening chest more than once
    private bool levelComplete = false;


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


    // =========================
    // RESTART LEVEL
    // =========================

    private void Update()
    {
        // Press R to restart the current level
        if(
            Keyboard.current != null &&
            Keyboard.current.rKey.wasPressedThisFrame
        )
        {
            RestartLevel();
        }
    }


    private void RestartLevel()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }


    // =========================
    // GRID
    // =========================

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


    // =========================
    // GOALS
    // =========================

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


    // =========================
    // BUTTON
    // =========================

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

        // Empty space
        if(target == null)
        {
            return true;
        }

        // Cannot push another box
        if(target == "Box")
        {
            return false;
        }

        // Everything else blocks the box
        return false;
    }


    public void PushBlock(
        Vector3Int start,
        Vector3Int end,
        int xmove,
        int ymove
    )
    {
        // Remove box from old position
        tilemap.SetTile(start, null);

        // Put box in new position
        tilemap.SetTile(end, blockTile);

        // Check all buttons
        UpdateButtons();
    }


    // =========================
    // BUTTON SYSTEM
    // =========================

    public void ActivateButton(int x, int y)
    {
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


        // Enough buttons pressed
        if(buttonsOn >= buttonsNeeded)
        {
            OpenSpikes();
        }
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
        // Don't activate twice
        if(levelComplete)
        {
            return;
        }

        levelComplete = true;

        // Open chest
        tilemap.SetTile(
            new Vector3Int(x, y, 0),
            chestOpenTile
        );

        // Play chest sound
        if(audioSource != null && chestSound != null)
        {
            audioSource.PlayOneShot(chestSound);
        }

        Debug.Log("LEVEL COMPLETE!");

        // Go to next level
        StartCoroutine(LoadNextLevel());
    }


    // =========================
    // NEXT LEVEL
    // =========================

    private IEnumerator LoadNextLevel()
    {
        // Wait so player can see/hear chest
        yield return new WaitForSeconds(1f);

        int currentScene =
            SceneManager.GetActiveScene().buildIndex;

        int nextScene =
            currentScene + 1;

        // Load next scene if it exists
        if(nextScene < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextScene);
        }
        else
        {
           if(winPanel != null)
    {
        winPanel.SetActive(true);
    }
    }
}
}