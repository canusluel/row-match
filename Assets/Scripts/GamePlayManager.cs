using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class GamePlayManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moveCountText;
    [SerializeField] private TextMeshProUGUI scoreText;

    private Tile firstSelectedTile;
    private int currentMoveCount;
    private int currentScore;
    private bool movesAvailable;

    public LosePanel losePanel;
    public WinPanel winPanel;
    private Vector3 swipeStartPos;
    private LevelData currentLevel;
    private bool isSwitchingTiles = false;
    private float switchDuration = 0.2f;

    void Start()
    {
        currentLevel = LevelDataManager.instance.GetCurrentLevel();

        currentScore = 0;
        scoreText.SetText(currentScore.ToString());

        currentMoveCount = currentLevel.moveCount;
        moveCountText.SetText(currentMoveCount.ToString());

        movesAvailable = true;
    }
    void Update()
    {
        // Check for swipe input only if there are available moves and the game is not finished
        if (movesAvailable && currentMoveCount != 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                swipeStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(swipeStartPos, Vector2.zero);
                if (hit.collider != null)
                {
                    firstSelectedTile = hit.collider.GetComponent<Tile>();
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                Vector3 swipeEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                HandleSwipe(swipeStartPos, swipeEndPos);
            }
        }
    }

    private void HandleSwipe(Vector2 startPos, Vector2 endPos)
    {
        if (firstSelectedTile != null)
        {
            Vector2 swipeDelta = endPos - startPos;
            float swipeThreshold = 0.5f;
            // Check if the swipe meets the minimum threshold
            if (swipeDelta.magnitude > swipeThreshold)
            {
                // Check the swipe direction
                if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                {
                    // Horizontal swipe
                    if (swipeDelta.x > 0)
                    {
                        // Right swipe

                        TrySwitchTile(firstSelectedTile, firstSelectedTile.GetAdjacentTile("right"));
                    }
                    else
                    {
                        // Left swipe
                        TrySwitchTile(firstSelectedTile, firstSelectedTile.GetAdjacentTile("left"));
                    }
                }
                else
                {
                    // Vertical swipe
                    if (swipeDelta.y > 0)
                    {
                        // Up swipe
                        TrySwitchTile(firstSelectedTile, firstSelectedTile.GetAdjacentTile("up"));
                    }
                    else
                    {
                        // Down swipe
                        TrySwitchTile(firstSelectedTile, firstSelectedTile.GetAdjacentTile("down"));
                    }
                }
            }
        }
    }

    private void TrySwitchTile(Tile tile1, Tile tile2)
    {
        // Check if both tiles are valid and adjacent to each other
        if (tile2 != null)
        {
            // Perform the switch
            SwitchTiles(tile1, tile2);
            firstSelectedTile = null; // Reset selectedTile for the next switch
        }
    }
    private void SwitchTiles(Tile tile1, Tile tile2)
    {
        if (isSwitchingTiles)
            return;

        isSwitchingTiles = true;

        TileItem tileItem1 = tile1.GetTileItem();
        TileItem tileItem2 = tile2.GetTileItem();

        // Swap the TileItems between the tiles
        if (!tileItem1.GetIsCompleted() && !tileItem2.GetIsCompleted())
        {
            // Get the initial positions of the tiles
            Vector3 tile1ItemInitialPos = tileItem1.transform.position;
            Vector3 tile2ItemInitialPos = tileItem2.transform.position;

            // Set the parents of the TileItems to the opposite tiles
            tileItem1.transform.SetParent(tile2.transform, false);
            tileItem2.transform.SetParent(tile1.transform, false);

            tile1.SetTileItem(tileItem2);
            tile2.SetTileItem(tileItem1);

            // Start the coroutine to animate the switch
            StartCoroutine(AnimateTileSwitch(tileItem1, tileItem2, tile1ItemInitialPos, tile2ItemInitialPos));

            // After the switch, check for any completed rows and update scores accordingly
            currentScore += CheckForCompletedRows(tile1, tile2);
            scoreText.SetText(currentScore.ToString());

            currentMoveCount -= 1;
            moveCountText.SetText(currentMoveCount.ToString());

            if (currentMoveCount == 0 || movesAvailable == false)
            {
                FinishGame();
            }
        }
    }

    private IEnumerator AnimateTileSwitch(TileItem tile1Item, TileItem tile2Item, Vector3 tile1InitialPos, Vector3 tile2InitialPos)
    {
        float elapsedTime = 0f;

        while (elapsedTime < switchDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / switchDuration);

            // Interpolate the position of the tiles
            tile1Item.transform.position = Vector3.Lerp(tile1InitialPos, tile2InitialPos, t);
            tile2Item.transform.position = Vector3.Lerp(tile2InitialPos, tile1InitialPos, t);

            yield return null;
        }

        // Ensure the final position is exactly at the target
        tile1Item.transform.position = tile2InitialPos;
        tile2Item.transform.position = tile1InitialPos;

        StartCoroutine(AudioManager.instance.PlayClip(AudioManager.instance.tileMoved));
        isSwitchingTiles = false;
    }

    // This function checks for completed rows on tileItem switch.
    // Returns the score if user completes a row.
    private int CheckForCompletedRows(Tile movedTile1, Tile movedTile2)
    {
        List<Tile> tileRow1 = new List<Tile>();
        List<Tile> tileRow2 = new List<Tile>();

        int x1 = (int)movedTile1.GetPosition().x;
        int y1 = (int)movedTile1.GetPosition().y;

        int x2 = (int)movedTile2.GetPosition().x;
        int y2 = (int)movedTile2.GetPosition().y;

        Dictionary<Vector2, Tile> tiles = GridManager.tiles;

        // Check the first row starting from tile1
        for (int i = x1; i >= 0; i--)
        {
            Tile leftTile = tiles[new Vector2(i, y1)];
            tileRow1.Add(leftTile);
        }

        for (int i = x1 + 1; i < currentLevel.gridWidth; i++)
        {
            Tile rightTile = tiles[new Vector2(i, y1)];
            tileRow1.Add(rightTile);
        }

        // Check the second row starting from tile2
        for (int i = x2; i >= 0; i--)
        {
            Tile leftTile = tiles[new Vector2(i, y2)];
            tileRow2.Add(leftTile);
        }

        for (int i = x2 + 1; i < currentLevel.gridWidth; i++)
        {
            Tile rightTile = tiles[new Vector2(i, y2)];
            tileRow2.Add(rightTile);
        }

        bool isRow1Completed = IsRowCompleted(tileRow1);
        bool isRow2Completed = IsRowCompleted(tileRow2);

        if (isRow1Completed && isRow2Completed)
        {
            CompleteRow(tileRow1);
            CompleteRow(tileRow2);
            return GetScoreFromColor(tileRow1[0].GetTileItem().name) + GetScoreFromColor(tileRow2[0].GetTileItem().name);
        }
        else if (isRow2Completed)
        {
            CompleteRow(tileRow2);
            return GetScoreFromColor(tileRow2[0].GetTileItem().name);
        }
        else if (isRow1Completed)
        {
            CompleteRow(tileRow1);
            return GetScoreFromColor(tileRow1[0].GetTileItem().name);
        }
        else
        {
            return 0;
        }

    }

    private bool IsRowCompleted(List<Tile> tileRow)
    {
        for (int i = 0; i < tileRow.Count - 1; i++)
        {
            TileItem currentItem = tileRow[i].GetTileItem();
            TileItem nextItem = tileRow[i + 1].GetTileItem();

            if (!currentItem.name.Equals(nextItem.name))
            {
                return false; // Return false if any items don't match
            }
        }

        return true;
    }

    private void CompleteRow(List<Tile> tileRow)
    {
        foreach (Tile tile in tileRow)
        {
            TileItem tileItem = tile.GetTileItem();
            tileItem.SetIsCompleted(true);
            tileItem.changeSprite('d');
        }
        movesAvailable = CheckForAvailableMoves();
        StartCoroutine(AudioManager.instance.PlayClip(AudioManager.instance.rowComplete));
    }

    private int GetScoreFromColor(string color)
    {
        switch (color)
        {
            case "Yellow":
                return 250 * currentLevel.gridWidth;

            case "Blue":
                return 200 * currentLevel.gridWidth;

            case "Green":
                return 150 * currentLevel.gridWidth;

            case "Red":
                return 100 * currentLevel.gridWidth;

            default:
                return 0;
        }
    }

    private bool CheckForAvailableMoves()
    {
        Dictionary<Vector2, Tile> tiles = GridManager.tiles;
        Vector2 currentPos = new Vector2(0, 0);
        Tile tile = tiles[currentPos];
        bool check = false;

        while (currentPos.y < currentLevel.gridHeight)
        {
            List<Tile> uncompleteTiles = new List<Tile>();
            // Skip the completed rows
            while (tile.GetTileItem().GetIsCompleted())
            {
                currentPos.y += 1;
                if (currentPos.y < currentLevel.gridHeight)
                {
                    tile = tiles[currentPos];
                }
                else
                {
                    break;
                }
            }

            // Get the rows that are not complete until next completed row
            while (!tile.GetTileItem().GetIsCompleted())
            {
                for (currentPos.x = 0; currentPos.x < currentLevel.gridWidth; currentPos.x++)
                {
                    tile = tiles[currentPos];
                    uncompleteTiles.Add(tile);
                }
                currentPos.y += 1;
                currentPos.x = 0;
                if (currentPos.y < currentLevel.gridHeight)
                {
                    tile = tiles[currentPos];
                }
                else
                {
                    break;
                }
            }

            if (CountItems(uncompleteTiles))
            {
                check = true;
            }
        }

        return check;
    }

    // This function returns true if there is a possible row match in given list of tiles
    private bool CountItems(List<Tile> uncompleteTiles)
    {
        int[] countArr = new int[] { 0, 0, 0, 0 };

        foreach (Tile tile in uncompleteTiles)
        {
            string tileItemColor = "color";

            if (!tile.GetTileItem().GetIsCompleted())
            {
                tileItemColor = tile.GetTileItem().name;
            }
            switch (tileItemColor)
            {
                case "Red":
                    countArr[0] += 1;
                    break;
                case "Blue":
                    countArr[1] += 1;
                    break;
                case "Green":
                    countArr[2] += 1;
                    break;
                case "Yellow":
                    countArr[3] += 1;
                    break;
            }
        }
        foreach (int count in countArr)
        {
            if (count >= currentLevel.gridWidth)
            {
                return true;
            }
        }
        return false;
    }

    private void FinishGame()
    {
        if (PlayerPrefs.GetInt(currentLevel.levelNumber.ToString()) < currentScore)
        {
            PlayerPrefs.SetInt(currentLevel.levelNumber.ToString(), currentScore);
            winPanel.ChangeScoreText(currentScore.ToString());
            winPanel.TriggerWinPanel();
            return;
        }

        losePanel.TriggerLosePanel();

        if (currentMoveCount == 0)
        {
            losePanel.losePanelText.SetText("You do not have any moves left, returning to level menu.");
        }
        else
        {
            losePanel.losePanelText.SetText("There are no possible moves left, returning to level menu.");
        }
    }

}
