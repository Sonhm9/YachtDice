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
        // 현재 점수모드일때
        if (ModeManager.Instance.currentMode == ModeManager.Mode.Score)
        {
            // 선택 포인터 범위 설정
            if (selectPointIdx > scorePositions.Count - 1)
            {
                selectPointIdx = scorePositions.Count - 1;
            }
            if (selectPointIdx < 0)
            {
                selectPointIdx = 0;
            }

            // 선택 포인터 위치
            pointer.transform.position = new Vector3(pointer.transform.position.x, scorePositions[selectPointIdx].transform.position.y-20, pointer.transform.position.z);

            foreach (var position in scorePositions)
            {
                IScore scoreComponent = position.GetComponentInChildren<IScore>(); // IScore 컴포넌트 가져오기
                if (scoreComponent != null)
                {
                    scoreComponent.ColorTint(0.5f); // 색상 강도 낮추기
                }
            }
            IScore selectedScoreComponent = scorePositions[selectPointIdx].GetComponentInChildren<IScore>();
            if (selectedScoreComponent != null)
            {
                selectedScoreComponent.ColorTint(1); // 색상 강도 최대로
            }

            // D키 눌렀을 때
            if (Input.GetKeyDown(KeyCode.D))
            {
                selectPointIdx++;
            }
            // A키 눌렀을 때
            if (Input.GetKeyDown(KeyCode.A))
            {
                selectPointIdx--;
            }
            // E키 눌렀을 때
            if (Input.GetKeyDown(KeyCode.E))
            {
                // 점수 선택
                IScore score = scorePositions[selectPointIdx].GetComponentInChildren<IScore>();
                score.SelectValue();

                // 선택할 수 있는 포인터에서 제거
                scorePositions.Remove(scorePositions[selectPointIdx]);
                selectPointIdx = 0;

                // 스코어 초기화
                ScoreManager.Instance.PublishClearScore();

                audioSource.Play();

                // 턴 넘기기
                pointer.SetActive(false);
                TurnManager.Instance.NextTurn();

                // 쉐이크 모드로 전환
                GameObject shake = GameObject.FindWithTag("ShakeBasket");
                StartCoroutine(shake.GetComponent<ChooseModeBehaviour>().ReturnShake());
                shake.GetComponent<ShakeBasketController>().SetShakeMode();
            }
        }
    }
}
