using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseModeBehaviour : MonoBehaviour
{
    public Vector3 choosePosition; // 선택 주사위 포지션

    public int currentDiceCount = 5; // 현재 던질 주사위 갯수
    public float scaleFactor = 1; // 주사위 간격

    public bool selectAble = false; // 선택이 가능한 지

    public Transform[] selectPosition; // 선택한 주사위의 위치

    int maxDiceCount = 5; // 최대 주사위 갯수
    public int selectPointIdx = 0; // 선택하고 있는 주사위

    public List<DiceController> chooseList = new List<DiceController>(); // 선택 리스트
    public List<DiceController> selectList = new List<DiceController>(); // 선택된 리스트

    ShakeBasketController shakeBasketController;
    SelectUI selectUI;

    void Start()
    {
        shakeBasketController = GetComponent<ShakeBasketController>();
        selectUI = GetComponent<SelectUI>();
    }

    void Update()
    {
        // 현재 선택모드일때 && 선택가능상태 일때
        if (ModeManager.Instance.currentMode == ModeManager.Mode.Choose && selectAble)
        {
            // 선택 포인터 범위 설정
            if (selectPointIdx > chooseList.Count - 1)
            {
                selectPointIdx = chooseList.Count - 1;
            }
            if (selectPointIdx < 0)
            {
                selectPointIdx = 0;
            }

            // 선택 포인터 위치
            selectUI.LocateSelectedDice(chooseList[selectPointIdx].gameObject.transform.position.x);

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
                // 선택한 주사위를 위로 이동
                StartCoroutine(MoveToPosition(chooseList[selectPointIdx].gameObject, selectPosition[selectList.Count].position, chooseList[selectPointIdx].SetRotationForDicenumber(), 0.25f));

                // 선택된 리스트로 이동
                selectList.Add(chooseList[selectPointIdx]);
                chooseList.Remove(chooseList[selectPointIdx]);

                currentDiceCount--; // 현재 주사위 갯수 감소

                // 주사위 위치 재정렬
                StartCoroutine(ArrangeChooseList());

                // 주사위를 모두 선택했을 때
                if (currentDiceCount <= 0)
                {
                    TurnManager.Instance.SetScoreMode();
                    selectAble = false;
                    selectUI.UnactiveSelectUI();
                }

            }
            // Q키를 눌렀을 때
            if (Input.GetKeyDown(KeyCode.Q))
            {
                // 선택되지 않은 주사위 삭제
                DestoryNoneSelectedDice();
                selectUI.UnactiveSelectUI(); // UI 비활성화

                ScoreManager.Instance.PublishClearScore(); // 모든 값 요소 삭제

                shakeBasketController.SetShakeMode(); // 섞기 모드로 전환
            }
        }
    }

    // 주사위 객체 가져오기
    public void GetDiceList()
    {
        TurnManager.Instance.NextChoose();

        for(int i=0; i<currentDiceCount; i++)
        {
            // shakeBasket에서 가져옴
            chooseList.Add(shakeBasketController.diceControllers[i]); 
            chooseList[i].diceNumber = shakeBasketController.diceControllers[i].diceNumber;

            chooseList[i].rb.isKinematic = true;  // 주사위 물리 해제
            Debug.Log($"Added Dice {i}: diceNumber = {chooseList[i].diceNumber}"); // Debug 로그

        }
        chooseList.Sort((a, b) => a.diceNumber.CompareTo(b.diceNumber)); // diceNumber에 따라 정렬

        StartCoroutine(SetChooseList());
    }

    // 다시 턴이 돌아왔을때 초기화
    public void ReturnShake()
    {
        currentDiceCount = maxDiceCount;
        foreach(var obj in selectList)
        {
            Destroy(obj.gameObject);
        }
        selectList.Clear();
    }

    // 선택되지 못한 주사위 삭제
    public void DestoryNoneSelectedDice()
    {
        selectPointIdx = 0; // 포인터 0으로 재배치
        selectAble = false; // 선택 불가능 상태
        // 주사위 오브젝트 모두 삭제
        foreach(DiceController dice in chooseList)
        {
            Destroy(dice.gameObject);
        }
        // 선택 리스트 모두 삭제
        chooseList.Clear();
    }

    // 모두 자동 선택하는 루틴
    public IEnumerator AutoSelectRoutine()
    {
        selectAble = false;
        selectUI.UnactiveSelectUI();

        yield return new WaitForSeconds(0.5f);

        for(int i=0; i<chooseList.Count; i++)
        {
            // 선택한 주사위를 위로 이동
            StartCoroutine(MoveToPosition(chooseList[i].gameObject, selectPosition[selectList.Count].position, chooseList[i].SetRotationForDicenumber(), 0.25f));

            // 선택된 리스트로 이동
            selectList.Add(chooseList[selectPointIdx]);

            currentDiceCount--; // 현재 주사위 갯수 감소

            // 주사위를 모두 선택했을 때
            if (currentDiceCount <= 0)
            {
                TurnManager.Instance.SetScoreMode();
                Debug.Log("없음");
            }

            yield return new WaitForSeconds(0.1f);
        }
        chooseList.Clear();
    }

    // 선택가능하도록 위치시키는 루틴
    public IEnumerator SetChooseList()
    {
        // 주사위 갯수에 따라 정렬
        StartCoroutine(ArrangeChooseList());

        // 계산해서 띄우기
        ScoreManager.Instance.AddScoreList(selectList, chooseList);
        ScoreManager.Instance.PublishUpdateScore();
        yield return StartCoroutine(ScoreManager.Instance.GenealogyText());

        // UI 활성화 및 선택 가능상태
        selectAble = true;
        selectUI.ActiveSelectUI();
        TurnManager.Instance.MoveScoreCanvas();

        // 3번째 던지기 일 때
        if (TurnManager.Instance.CheckMaxChooseCount())
        {
            StartCoroutine(AutoSelectRoutine());
        }
    }

    public IEnumerator ArrangeChooseList()
    {
        // 주사위 갯수에 따라 정렬
        float offset = (currentDiceCount - 1) * scaleFactor / 2.0f;
        for (int i = 0; i < currentDiceCount; i++)
        {
            chooseList[i].OnColliderClose();

            float x = i * scaleFactor - offset;
            Vector3 targetPosition = new Vector3(x, choosePosition.y, choosePosition.z);

            // 오브젝트 이동 루틴
            yield return StartCoroutine(MoveToPosition(chooseList[i].gameObject, targetPosition, chooseList[i].SetRotationForDicenumber(), 0.1f));
        }
    }

    // 오브젝트를 이동시키는 루틴
    public IEnumerator MoveToPosition(GameObject dice, Vector3 targetPosition, Quaternion targetRotation, float duration)
    {
        Vector3 startPosition = dice.transform.position;
        Quaternion startRotation = dice.transform.rotation;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // 선형 보간으로 위치와 회전 이동
            dice.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            dice.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

            yield return null;
        }

        // 최종 목표 위치,회전 설정
        dice.transform.position = targetPosition;
        dice.transform.rotation = targetRotation;
    }
}
