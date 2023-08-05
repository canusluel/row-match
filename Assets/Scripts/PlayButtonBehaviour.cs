using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayButtonBehaviour : MonoBehaviour
{
    public Button playButton;
    public Image buttonImage;
    public GameObject playText;
    public Sprite unlockSprite;
    public Image lockImage;
    public void OnButtonClicked()
    {
        StartCoroutine(AudioManager.instance.PlayClip(AudioManager.instance.buttonClick));
        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelScene");
    }

    public void UnlockPlayButton()
    {
        buttonImage.sprite = unlockSprite;
        playText.SetActive(true);
        lockImage.color = new Color32(255,255,255,0);
        playButton.interactable = true;
    }
}
