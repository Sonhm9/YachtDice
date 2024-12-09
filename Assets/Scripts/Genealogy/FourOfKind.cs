using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class FourOfKind : MonoBehaviour, IScore
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
        foreach(var score in scoreList)
        {
            scores.Add(score.diceNumber);
        }
        // 숫자를 그룹화하고, 4개인 그룹의 숫자를 찾음
        var fourKindGroup = scores.GroupBy(n => n).FirstOrDefault(group => group.Count() == 4);

        // 그룹이 없으면 0을 반환, 있으면 해당 숫자의 합을 반환
        value = fourKindGroup != null ? fourKindGroup.Key * 4 : 0;

        text.enabled = true;

        text.text = value.ToString();

        if (value > ScoreManager.Instance.maxValue)
        {
            ScoreManager.Instance.maxValue = value;
            ScoreManager.Instance.genealogy = ScoreManager.Genealogy.FourofKind;
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
