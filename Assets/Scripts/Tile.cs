using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    [SerializeField] Color normalColor, offsetColor;
    [SerializeField] SpriteRenderer spriteRenderer;
    private Dictionary<string, Tile> adjacentTiles;
    private TileItem tileItem;

    private Vector2 pos;

    public bool IsAdjacentTo(Tile otherTile)
    {
        if (adjacentTiles.ContainsValue(otherTile))
        {
            return true;
        }

        return false;
    }

    public void SetPosition(Vector2 nextPos){
        pos = nextPos;
    }

    public Vector2 GetPosition(){
        return pos;
    }
    public TileItem GetTileItem()
    {
        return tileItem;
    }

     public void SetTileItem(TileItem newItem)
    {

        // Set the new TileItem
        tileItem = newItem;

        // Set the new TileItem's position relative to this Tile
        if (tileItem != null)
        {
            tileItem.transform.SetParent(transform, false);
            tileItem.transform.position = transform.position;
        }

        // Update the visual representation of the tile based on the new item.
        // You may want to change the sprite or color of the tile based on the item.
        // Alternatively, you can update the sprite or color directly in the 'TileItem' class.
    }

    public Dictionary<string, Tile> GetAdjacentTiles()
    {
        return adjacentTiles;
    }

    public void SetAdjacentTiles(Dictionary<string, Tile> tiles)
    {
        adjacentTiles = tiles;
    }

    public void AddAdjacentTile(string direction, Tile adjacentTile){
        adjacentTiles.Add(direction, adjacentTile);
    }

    public Tile GetAdjacentTile(string direction){
        if(adjacentTiles.ContainsKey(direction)){
            return adjacentTiles[direction];
        }
        return null;
    }
    public void changeOffsetColor(bool isOffset)
    {
        if (isOffset)
        {
            spriteRenderer.color = offsetColor;
        }
        else
        {
            spriteRenderer.color = normalColor;
        }
    }
}
