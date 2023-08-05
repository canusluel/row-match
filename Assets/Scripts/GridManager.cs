using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private TileItem tileItemPrefab;

    [SerializeField] private Transform cam;

    public static Dictionary<Vector2, Tile> tiles;
    private List<TileItem> tileItems;
    private static int width, height;
    public LevelData currentLevel;

    void Start()
    {
        currentLevel = LevelDataManager.instance.GetCurrentLevel();
        GenerateGrid();
        DetermineAdjacentTiles();
    }

    private void GenerateGrid()
    {
        tiles = new Dictionary<Vector2, Tile>();
        tileItems = new List<TileItem>();

        width = currentLevel.gridWidth;
        height = currentLevel.gridHeight;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 currentPos = new Vector3(x, y);
                var tile = Instantiate(tilePrefab, currentPos, Quaternion.identity);
                tile.name = "Tile " + x + " " + y;

                bool isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                tile.changeOffsetColor(isOffset);

                var tileItem = Instantiate(tileItemPrefab, tile.transform, false);
                tileItem.transform.position = currentPos;

                tiles[new Vector2(x, y)] = tile;
                tileItems.Add(tileItem);
                tile.SetTileItem(tileItem);
                tile.SetAdjacentTiles(new Dictionary<string, Tile>());
                tile.SetPosition(new Vector2(x, y));
            }

        }
        if (width % 2 == 0)
        {
            cam.transform.position = new Vector3(width / 2 - 0.5f, height / 2, -10);
        }
        else
        {
            cam.transform.position = new Vector3(width / 2, height / 2, -10);
        }

        for (int i = 0; i < tileItems.Count; i++)
        {
            tileItems[i].changeSprite(currentLevel.gridData[i]);
        }

    }

    private void DetermineAdjacentTiles()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 currentPos = new Vector2(x, y);
                Tile currentTile = tiles[currentPos];

                Vector2 left = new Vector2(x - 1, y);
                Vector2 right = new Vector2(x + 1, y);
                Vector2 up = new Vector2(x, y + 1);
                Vector2 down = new Vector2(x, y - 1);

                if (tiles.ContainsKey(left))
                {
                    currentTile.AddAdjacentTile("left", tiles[left]);
                }

                if (tiles.ContainsKey(right))
                {
                    currentTile.AddAdjacentTile("right",tiles[right]);
                }

                if (tiles.ContainsKey(up))
                {
                    currentTile.AddAdjacentTile("up",tiles[up]);
                }

                if (tiles.ContainsKey(down))
                {
                    currentTile.AddAdjacentTile("down",tiles[down]);
                }
            }
        }

    }

}
