using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinPanel : MonoBehaviour
{
    public TextMeshProUGUI winText;
    public TextMeshProUGUI newHighestScoreText;
    public TextMeshProUGUI scoreText;
    public Animator animator;
    public ParticleSystem winParticles;

    void Awake()
    {
        winText.outlineWidth = 0.2f;
        winText.outlineColor = new Color32(255, 215, 0, 255);
        newHighestScoreText.outlineWidth = 0.2f;
        newHighestScoreText.outlineColor = new Color32(255, 215, 0, 255);
        scoreText.outlineWidth = 0.2f;
        scoreText.outlineColor = new Color32(255, 215, 0, 255);
    }

    public void LoadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

    public void TriggerWinPanel()
    {
        LevelDataManager.instance.hasAchievedHighestScore = true;
        StartCoroutine(AudioManager.instance.PlayClip(AudioManager.instance.winClip));
        animator.SetTrigger("TriggerWinPanel");
        winParticles.Play();
    }

    public void ChangeScoreText(string text)
    {
        scoreText.SetText(text);
    }

}
