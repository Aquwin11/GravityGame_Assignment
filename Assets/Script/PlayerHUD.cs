using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    public TMP_Text timerText;
    public TMP_Text objectiveText;
    public TMP_Text endGameText;

    public void SetTimerValue(float minutes, float seconds)
    {
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void SetObjectiveText(int point)
    {
        objectiveText.text = "Collect all the spheres : " + point + "/ 5";
    }
    public void SetEndGameText(string text)
    {
        endGameText.text = text;
    }
}
