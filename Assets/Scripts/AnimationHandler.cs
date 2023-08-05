using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationHandler : MonoBehaviour
{
    public Animator startPanelAnimator;
    public Animator levelsPopupAnimator;

    void Start()
    {
        if (LevelDataManager.instance.isLevelSelected)
        {
            TriggerLevelsPopup();
        }
    }
    public void TriggerLevelsPopup()
    {
        startPanelAnimator.SetTrigger("TriggerLevelsButton");
        levelsPopupAnimator.SetTrigger("TriggerLevelsPopup");
    }

    public static void TriggerUnlockLevelButton(Button playButton)
    {
        Animator animator = playButton.GetComponent<Animator>();
        animator.SetTrigger("UnlockPlayButton");
    }
}
