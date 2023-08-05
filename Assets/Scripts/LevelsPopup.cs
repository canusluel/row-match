using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelsPopup : MonoBehaviour
{
    public GameObject levelRowPrefab;
    public Transform content;
    public List<Sprite> buttonSprites;
    public ScrollRect scrollRect;

    private void Start()
    {
        //PlayerPrefs.DeleteAll();
        List<LevelData> levels = LevelDataManager.instance.levels;
        foreach (LevelData level in levels)
        {
            GameObject levelRow = Instantiate(levelRowPrefab, content);
            levelRow.GetComponent<LevelRow>().Initialize(level.levelNumber);
            levelRow.name = "Level" + level.levelNumber + "Row";
            UpdateLevelButton(levelRow, level);
            levelRow.SetActive(true);
        }
    }

    private void UpdateLevelButton(GameObject levelRow, LevelData level)
    {
        TextMeshProUGUI[] ts = levelRow.GetComponentsInChildren<TextMeshProUGUI>();
        ts[0].text = "Level " + level.levelNumber.ToString();
        ts[1].text = level.moveCount.ToString() + " Moves";
        int highestScore = PlayerPrefs.GetInt(level.levelNumber.ToString(), 0);

        if (highestScore == 0)
        {
            ts[2].text = "No Highest Score";
        }
        else
        {
            ts[2].text = "Highest Score: " + highestScore;
        }

        Button button = levelRow.GetComponentInChildren<Button>();
        if (button != null)
        {
            if (level.levelNumber > 3 && (!LevelDataManager.instance.IsPrevLevelUnlocked(level.levelNumber) || LevelDataManager.instance.GetCompletedLevel() + 1 == level.levelNumber))
            {
                LockLevelButton(button);
            }

        }
    }

    private void LockLevelButton(Button button)
    {
        TextMeshProUGUI playText = button.GetComponentInChildren<TextMeshProUGUI>();
        Image[] lockImage = button.GetComponentsInChildren<Image>();

        playText.gameObject.SetActive(false);
        button.image.sprite = buttonSprites[1];
        lockImage[1].color = new Color32(255, 255, 255, 255);
        button.interactable = false;
    }

    public void UnlockCompletedLevelButton()
    {
        int completedLevel = LevelDataManager.instance.GetCompletedLevel();
        if (completedLevel + 1 > 3 && completedLevel + 1 < LevelDataManager.instance.levels.Count && PlayerPrefs.GetInt("maxAchievedLevel") == completedLevel)
        {
            string itemName = "Level" + (completedLevel + 1) + "Row";
            GameObject completedLevelRow = content.transform.Find(itemName).gameObject;
            Button completedPlayButton = completedLevelRow.GetComponentInChildren<Button>();

            float height = completedLevelRow.GetComponent<RectTransform>().rect.height;

            Vector3 targetPosition = completedLevelRow.transform.localPosition;

            StartCoroutine(ScrollToPosition(targetPosition, height, completedPlayButton));
        }
    }

    private IEnumerator ScrollToPosition(Vector3 targetPosition, float height, Button completedPlayButton)
    {
        Vector2 startPosition = scrollRect.content.anchoredPosition;
        float duration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            scrollRect.content.anchoredPosition = Vector2.Lerp(startPosition, new Vector2(0, -targetPosition.y - height * 0.5f), t);
            yield return null;
        }

        scrollRect.content.anchoredPosition = new Vector2(0, -targetPosition.y - height * 0.5f);
        AnimationHandler.TriggerUnlockLevelButton(completedPlayButton);
    }
}
