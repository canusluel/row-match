using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsButtonBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnButtonClicked()
    {
        StartCoroutine(AudioManager.instance.PlayClip(AudioManager.instance.buttonClick));
    }
}
