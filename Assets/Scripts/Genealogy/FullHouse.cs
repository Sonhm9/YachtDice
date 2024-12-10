using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class FullHouse : MonoBehaviour, IScore
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

        // 숫자를 그룹화하여 각 숫자의 개수 확인
        var groups = scores.GroupBy(n => n);

        // Three of a Kind와 Pair가 있는지 확인
        var threeOfAKind = groups.FirstOrDefault(group => group.Count() == 3);
        var pair = groups.FirstOrDefault(group => group.Count() == 2);

        // 조건을 만족하면 합계 반환, 그렇지 않으면 0 반환
        if (threeOfAKind != null && pair != null)
        {
            value = (threeOfAKind.Key * 3) + (pair.Key * 2);
        }

        text.enabled = true;

        text.text = value.ToString();

        if (value > ScoreManager.Instance.maxValue)
        {
            ScoreManager.Instance.maxValue = value;
            ScoreManager.Instance.genealogy = ScoreManager.Genealogy.FullHouse;
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

        ScoreManager.Instance.AddTotal(value);
    }
}
