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
        if (ModeManager.Instance.currentMode == ModeManager.Mode.Choose && selectAble)
        {
            if (selectPointIdx > chooseList.Count - 1)
            {
                selectPointIdx = chooseList.Count - 1;
            }
            if (selectPointIdx < 0)
            {
                selectPointIdx = 0;
            }

            selectUI.LocateSelectedDice(chooseList[selectPointIdx].gameObject.transform.position.x);

            if (Input.GetKeyDown(KeyCode.D))
            {
                selectPointIdx++;
                
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                selectPointIdx--;
                
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(MoveToPosition(chooseList[selectPointIdx].gameObject, selectPosition[selectList.Count].position, chooseList[selectPointIdx].SetRotationForDicenumber(), 0.25f));

                selectList.Add(chooseList[selectPointIdx]);
                chooseList.Remove(chooseList[selectPointIdx]);

                currentDiceCount--;
                if (currentDiceCount <= 0)
                {
                    shakeBasketController.SetShakeMode();
                    Debug.Log("없음");
                }

                StartCoroutine(SetChooseList());

            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                DestoryNoneSelectedDice();
                selectUI.UnactiveSelectUI();
                shakeBasketController.SetShakeMode();
            }
        }
    }

    // 주사위 객체 가져오기
    public void GetDiceList()
    {
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

    // 선택되지 못한 주사위 삭제
    public void DestoryNoneSelectedDice()
    {
        selectPointIdx = 0;
        selectAble = false;
        foreach(DiceController dice in chooseList)
        {
            Destroy(dice.gameObject);
        }
        chooseList.Clear();
    }

    // 선택가능하도록 위치시키는 루틴
    public IEnumerator SetChooseList()
    {
        float offset = (currentDiceCount - 1) * scaleFactor / 2.0f;
        for (int i = 0; i < currentDiceCount; i++)
        {
            chooseList[i].OnColliderClose();

            float x = i * scaleFactor - offset;
            Vector3 targetPosition = new Vector3(x, choosePosition.y, choosePosition.z);

            yield return StartCoroutine(MoveToPosition(chooseList[i].gameObject, targetPosition, chooseList[i].SetRotationForDicenumber(), 0.1f)); // 이동 시간 0.5초

            selectUI.ActiveSelectUI();
            selectAble = true;
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

            yield return null; // 다음 프레임으로
        }

        // 정확히 목표 위치/회전으로 설정
        dice.transform.position = targetPosition;
        dice.transform.rotation = targetRotation;
    }
}
