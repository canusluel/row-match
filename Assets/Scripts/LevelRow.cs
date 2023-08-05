using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelRow : MonoBehaviour
{
    public Button playButton;

    private int levelNumber;
    public void Initialize(int levelNumber)
    {
        this.levelNumber = levelNumber;
        playButton.onClick.AddListener(OnPlayButtonClicked);
    }

    private void OnPlayButtonClicked()
    {
        LevelDataManager.instance.OnLevelSelected(levelNumber);
    }
}
