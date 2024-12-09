using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public event Action<List<DiceController>> updateScore;
    public event Action clearValue;

    public List<DiceController> scoreList = new List<DiceController>(); // ���� ����Ʈ    
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI totalText;
    public TextMeshProUGUI subTotalText;
    public TextMeshProUGUI bonusText;

    public int maxValue = 0;
    public int totalValue = 0;
    public int subTotalValue = 0;

    public List<IScore> scores = new List<IScore>();

    public enum Genealogy
    {
        None,
        FourofKind,
        FullHouse,
        SmallStraight,
        LargeStraight,
        Yacht
    }
    public Genealogy genealogy;

    static ScoreManager instance;
    static public ScoreManager Instance
    {
        get
        {
            return instance;
        }
    }
    private void Awake()
    {
        // �ν��Ͻ��� �����Ѵٸ�
        if (instance != null)
        {
            Destroy(gameObject);
        }

        // �ν��Ͻ��� ó�� �����ȴٸ�
        else
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        scores.Add(new Ones());
    }

    // ���ھ� ������Ʈ ����
    public void PublishUpdateScore()
    {
        updateScore?.Invoke(scoreList);
    }

    // ���ھ� �ʱ�ȭ ����
    public void PublishClearScore()
    {
        scoreList.Clear();
        //totalValue = 0;
        clearValue?.Invoke();
        genealogy = Genealogy.None;
    }

    // ���� �ؽ�Ʈ�� �����ִ� ��ƾ
    public IEnumerator GenealogyText()
    {
        if (genealogy != Genealogy.None)
        {
            switch (genealogy)
            {
                case Genealogy.FourofKind:
                    Debug.Log("FourofKind");
                    scoreText.gameObject.SetActive(true);
                    scoreText.text = "Four Of Kind";
                    break;

                case Genealogy.FullHouse:
                    Debug.Log("FullHouse");
                    scoreText.gameObject.SetActive(true);
                    scoreText.text = "Full House";
                    break;

                case Genealogy.SmallStraight:
                    Debug.Log("SmallStraight");
                    scoreText.gameObject.SetActive(true);
                    scoreText.text = "Small Straight";
                    break;

                case Genealogy.LargeStraight:
                    Debug.Log("LargeStraight");
                    scoreText.gameObject.SetActive(true);
                    scoreText.text = "Large Straight";
                    break;

                case Genealogy.Yacht:
                    Debug.Log("Yacht");
                    scoreText.gameObject.SetActive(true);
                    scoreText.text = "Yacht";
                    break;

                default:
                    Debug.Log("None");
                    scoreText.gameObject.SetActive(true);
                    break;
            }
            yield return new WaitForSeconds(2);
            scoreText.gameObject.SetActive(false);
        }
    }

    // ���ھ� ����Ʈ�� ��� �߰�
    public void AddScoreList(List<DiceController> selectList, List<DiceController> chooseList)
    {
        foreach(var value in selectList)
        {
            scoreList.Add(value);
        }
        foreach (var value in chooseList)
        {
            scoreList.Add(value);
        }
    }

    // ���ھ� ����Ʈ ��� ����
    public void ClearScoreList()
    {
        scoreList.Clear();
    }


    public void AddTotal(int value)
    {
        totalValue += value;
        totalText.text = totalValue.ToString();
    }

    public void AddSubTotal(int value)
    {
        subTotalValue += value;
        subTotalText.text = subTotalValue.ToString() + "/" + 63;

        if (subTotalValue >= 63)
        {
            AddTotal(35);
            bonusText.text = "+" + 35.ToString();
        }
    }
}
