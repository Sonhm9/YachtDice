using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class Fives : MonoBehaviour, IScore
{
    public TextMeshProUGUI text;
    public int value;

    private void Start()
    {
        ScoreManager.Instance.updateScore += CalculateValue;
        ScoreManager.Instance.clearValue += ClearValue;

        text.enabled = false;
    }
    public void CalculateValue(List<DiceController> scoreList)
    {
        foreach (var score in scoreList)
        {
            if (score.diceNumber == 5)
            {
                value += score.diceNumber;
            }
        }
        text.enabled = true;

        text.text = value.ToString();
    }

    public void ClearValue()
    {
        text.enabled = false;
        value = 0;
    }

    public void ColorTint(float alpha)
    {
        text.color = new Color(0, 0, 0, alpha);
    }

    public void SelectValue()
    {
        ScoreManager.Instance.updateScore -= CalculateValue;
        ScoreManager.Instance.clearValue -= ClearValue;

        ScoreManager.Instance.AddTotal(value);
        ScoreManager.Instance.AddSubTotal(value);
    }
}
