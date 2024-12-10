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

        // ���ڸ� �׷�ȭ�Ͽ� �� ������ ���� Ȯ��
        var groups = scores.GroupBy(n => n);

        // Three of a Kind�� Pair�� �ִ��� Ȯ��
        var threeOfAKind = groups.FirstOrDefault(group => group.Count() == 3);
        var pair = groups.FirstOrDefault(group => group.Count() == 2);

        // ������ �����ϸ� �հ� ��ȯ, �׷��� ������ 0 ��ȯ
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
