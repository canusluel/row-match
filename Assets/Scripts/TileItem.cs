using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileItem : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] List<Sprite> spriteList;

    private bool isCompleted;

    void Start()
    {
        isCompleted = false;
    }
    public void changeSprite(char color)
    {
        switch (color)
        {
            case 'r':
                spriteRenderer.sprite = spriteList[3];
                this.name = "Red";
                break;
            case 'g':
                spriteRenderer.sprite = spriteList[2];
                this.name = "Green";
                break;
            case 'b':
                spriteRenderer.sprite = spriteList[1];
                this.name = "Blue";
                break;
            case 'y':
                spriteRenderer.sprite = spriteList[0];
                this.name = "Yellow";
                break;
            case 'd':
                spriteRenderer.sprite = spriteList[4];
                break;
        }
    }

    public void SetIsCompleted(bool val)
    {
        isCompleted = val;
    }

    public bool GetIsCompleted()
    {
        return isCompleted;
    }
}
