using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseModeBehaviour : MonoBehaviour
{
    public Vector3 choosePosition; // ���� �ֻ��� ������

    public int currentDiceCount = 5; // ���� ���� �ֻ��� ����
    public float scaleFactor = 1; // �ֻ��� ����

    public bool selectAble = false; // ������ ������ ��

    public Transform[] selectPosition; // ������ �ֻ����� ��ġ

    int maxDiceCount = 5; // �ִ� �ֻ��� ����
    public int selectPointIdx = 0; // �����ϰ� �ִ� �ֻ���

    public List<DiceController> chooseList = new List<DiceController>(); // ���� ����Ʈ
    public List<DiceController> selectList = new List<DiceController>(); // ���õ� ����Ʈ

    ShakeBasketController shakeBasketController;
    SelectUI selectUI;

    void Start()
    {
        shakeBasketController = GetComponent<ShakeBasketController>();
        selectUI = GetComponent<SelectUI>();
    }

    void Update()
    {
        // ���� ���ø���϶� && ���ð��ɻ��� �϶�
        if (ModeManager.Instance.currentMode == ModeManager.Mode.Choose && selectAble)
        {
            // ���� ������ ���� ����
            if (selectPointIdx > chooseList.Count - 1)
            {
                selectPointIdx = chooseList.Count - 1;
            }
            if (selectPointIdx < 0)
            {
                selectPointIdx = 0;
            }

            // ���� ������ ��ġ
            selectUI.LocateSelectedDice(chooseList[selectPointIdx].gameObject.transform.position.x);

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
                // ������ �ֻ����� ���� �̵�
                StartCoroutine(MoveToPosition(chooseList[selectPointIdx].gameObject, selectPosition[selectList.Count].position, chooseList[selectPointIdx].SetRotationForDicenumber(), 0.25f));

                // ���õ� ����Ʈ�� �̵�
                selectList.Add(chooseList[selectPointIdx]);
                chooseList.Remove(chooseList[selectPointIdx]);

                currentDiceCount--; // ���� �ֻ��� ���� ����

                // �ֻ��� ��ġ ������
                StartCoroutine(ArrangeChooseList());

                // �ֻ����� ��� �������� ��
                if (currentDiceCount <= 0)
                {
                    TurnManager.Instance.SetScoreMode();
                    selectAble = false;
                    selectUI.UnactiveSelectUI();
                }

            }
            // QŰ�� ������ ��
            if (Input.GetKeyDown(KeyCode.Q))
            {
                // ���õ��� ���� �ֻ��� ����
                DestoryNoneSelectedDice();
                selectUI.UnactiveSelectUI(); // UI ��Ȱ��ȭ

                ScoreManager.Instance.PublishClearScore(); // ��� �� ��� ����

                shakeBasketController.SetShakeMode(); // ���� ���� ��ȯ
            }
        }
    }

    // �ֻ��� ��ü ��������
    public void GetDiceList()
    {
        TurnManager.Instance.NextChoose();

        for(int i=0; i<currentDiceCount; i++)
        {
            // shakeBasket���� ������
            chooseList.Add(shakeBasketController.diceControllers[i]); 
            chooseList[i].diceNumber = shakeBasketController.diceControllers[i].diceNumber;

            chooseList[i].rb.isKinematic = true;  // �ֻ��� ���� ����
            Debug.Log($"Added Dice {i}: diceNumber = {chooseList[i].diceNumber}"); // Debug �α�

        }
        chooseList.Sort((a, b) => a.diceNumber.CompareTo(b.diceNumber)); // diceNumber�� ���� ����

        StartCoroutine(SetChooseList());
    }

    // �ٽ� ���� ���ƿ����� �ʱ�ȭ
    public void ReturnShake()
    {
        currentDiceCount = maxDiceCount;
        foreach(var obj in selectList)
        {
            Destroy(obj.gameObject);
        }
        selectList.Clear();
    }

    // ���õ��� ���� �ֻ��� ����
    public void DestoryNoneSelectedDice()
    {
        selectPointIdx = 0; // ������ 0���� ���ġ
        selectAble = false; // ���� �Ұ��� ����
        // �ֻ��� ������Ʈ ��� ����
        foreach(DiceController dice in chooseList)
        {
            Destroy(dice.gameObject);
        }
        // ���� ����Ʈ ��� ����
        chooseList.Clear();
    }

    // ��� �ڵ� �����ϴ� ��ƾ
    public IEnumerator AutoSelectRoutine()
    {
        selectAble = false;
        selectUI.UnactiveSelectUI();

        yield return new WaitForSeconds(0.5f);

        for(int i=0; i<chooseList.Count; i++)
        {
            // ������ �ֻ����� ���� �̵�
            StartCoroutine(MoveToPosition(chooseList[i].gameObject, selectPosition[selectList.Count].position, chooseList[i].SetRotationForDicenumber(), 0.25f));

            // ���õ� ����Ʈ�� �̵�
            selectList.Add(chooseList[selectPointIdx]);

            currentDiceCount--; // ���� �ֻ��� ���� ����

            // �ֻ����� ��� �������� ��
            if (currentDiceCount <= 0)
            {
                TurnManager.Instance.SetScoreMode();
                Debug.Log("����");
            }

            yield return new WaitForSeconds(0.1f);
        }
        chooseList.Clear();
    }

    // ���ð����ϵ��� ��ġ��Ű�� ��ƾ
    public IEnumerator SetChooseList()
    {
        // �ֻ��� ������ ���� ����
        StartCoroutine(ArrangeChooseList());

        // ����ؼ� ����
        ScoreManager.Instance.AddScoreList(selectList, chooseList);
        ScoreManager.Instance.PublishUpdateScore();
        yield return StartCoroutine(ScoreManager.Instance.GenealogyText());

        // UI Ȱ��ȭ �� ���� ���ɻ���
        selectAble = true;
        selectUI.ActiveSelectUI();
        TurnManager.Instance.MoveScoreCanvas();

        // 3��° ������ �� ��
        if (TurnManager.Instance.CheckMaxChooseCount())
        {
            StartCoroutine(AutoSelectRoutine());
        }
    }

    public IEnumerator ArrangeChooseList()
    {
        // �ֻ��� ������ ���� ����
        float offset = (currentDiceCount - 1) * scaleFactor / 2.0f;
        for (int i = 0; i < currentDiceCount; i++)
        {
            chooseList[i].OnColliderClose();

            float x = i * scaleFactor - offset;
            Vector3 targetPosition = new Vector3(x, choosePosition.y, choosePosition.z);

            // ������Ʈ �̵� ��ƾ
            yield return StartCoroutine(MoveToPosition(chooseList[i].gameObject, targetPosition, chooseList[i].SetRotationForDicenumber(), 0.1f));
        }
    }

    // ������Ʈ�� �̵���Ű�� ��ƾ
    public IEnumerator MoveToPosition(GameObject dice, Vector3 targetPosition, Quaternion targetRotation, float duration)
    {
        Vector3 startPosition = dice.transform.position;
        Quaternion startRotation = dice.transform.rotation;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // ���� �������� ��ġ�� ȸ�� �̵�
            dice.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            dice.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

            yield return null;
        }

        // ���� ��ǥ ��ġ,ȸ�� ����
        dice.transform.position = targetPosition;
        dice.transform.rotation = targetRotation;
    }
}
