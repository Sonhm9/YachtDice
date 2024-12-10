using System.Collections.Generic;
using UnityEngine;

public class ScoreBehaviour : MonoBehaviour
{
    public List<Transform> scorePositions = new List<Transform>();
    public GameObject pointer;

    int selectPointIdx = 0;

    AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // ���� ��������϶�
        if (ModeManager.Instance.currentMode == ModeManager.Mode.Score)
        {
            // ���� ������ ���� ����
            if (selectPointIdx > scorePositions.Count - 1)
            {
                selectPointIdx = scorePositions.Count - 1;
            }
            if (selectPointIdx < 0)
            {
                selectPointIdx = 0;
            }

            // ���� ������ ��ġ
            pointer.transform.position = new Vector3(pointer.transform.position.x, scorePositions[selectPointIdx].transform.position.y-20, pointer.transform.position.z);

            foreach (var position in scorePositions)
            {
                IScore scoreComponent = position.GetComponentInChildren<IScore>(); // IScore ������Ʈ ��������
                if (scoreComponent != null)
                {
                    scoreComponent.ColorTint(0.5f); // ���� ���� ���߱�
                }
            }
            IScore selectedScoreComponent = scorePositions[selectPointIdx].GetComponentInChildren<IScore>();
            if (selectedScoreComponent != null)
            {
                selectedScoreComponent.ColorTint(1); // ���� ���� �ִ��
            }

            // DŰ ������ ��
            if (Input.GetKeyDown(KeyCode.D))
            {
                selectPointIdx++;
            }
            // AŰ ������ ��
            if (Input.GetKeyDown(KeyCode.A))
            {
                selectPointIdx--;
            }
            // EŰ ������ ��
            if (Input.GetKeyDown(KeyCode.E))
            {
                // ���� ����
                IScore score = scorePositions[selectPointIdx].GetComponentInChildren<IScore>();
                score.SelectValue();

                // ������ �� �ִ� �����Ϳ��� ����
                scorePositions.Remove(scorePositions[selectPointIdx]);
                selectPointIdx = 0;

                // ���ھ� �ʱ�ȭ
                ScoreManager.Instance.PublishClearScore();

                audioSource.Play();

                // �� �ѱ��
                pointer.SetActive(false);
                TurnManager.Instance.NextTurn();

                // ����ũ ���� ��ȯ
                GameObject shake = GameObject.FindWithTag("ShakeBasket");
                StartCoroutine(shake.GetComponent<ChooseModeBehaviour>().ReturnShake());
                shake.GetComponent<ShakeBasketController>().SetShakeMode();
            }
        }
    }
}
