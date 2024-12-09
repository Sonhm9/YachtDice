using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Linq;
public class SmallStraight : MonoBehaviour, IScore
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
        List<int> scores = new List<int>();
        foreach (var score in scoreList)
        {
            scores.Add(score.diceNumber);
        }

        // 중복 제거 후 정렬
        var uniqueSortedNumbers = scores.Distinct().OrderBy(n => n).ToList();

        // 연속된 숫자 4개가 있는지 확인
        for (int i = 0; i <= uniqueSortedNumbers.Count - 4; i++)
        {
            if (uniqueSortedNumbers[i + 1] == uniqueSortedNumbers[i] + 1 &&
                uniqueSortedNumbers[i + 2] == uniqueSortedNumbers[i] + 2 &&
                uniqueSortedNumbers[i + 3] == uniqueSortedNumbers[i] + 3)
            {
                value = 15;
            }
        }

        text.enabled = true;

        text.text = value.ToString();

        if (value > ScoreManager.Instance.maxValue)
        {
            ScoreManager.Instance.maxValue = value;
            ScoreManager.Instance.genealogy = ScoreManager.Genealogy.SmallStraight;
        }
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

        ScoreManager.Instance.totalValue += value;
    }
}
