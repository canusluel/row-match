using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LosePanel : MonoBehaviour
{
    public TextMeshProUGUI losePanelText;
    public Animator animator;

    public void TriggerLosePanel()
    {
        animator.SetTrigger("TriggerLosePanel");
    }

    public void LoadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

}
