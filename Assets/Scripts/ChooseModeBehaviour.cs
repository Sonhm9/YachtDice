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
                    Debug.Log("����");
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

    // �ֻ��� ��ü ��������
    public void GetDiceList()
    {
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

    // ���õ��� ���� �ֻ��� ����
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

    // ���ð����ϵ��� ��ġ��Ű�� ��ƾ
    public IEnumerator SetChooseList()
    {
        float offset = (currentDiceCount - 1) * scaleFactor / 2.0f;
        for (int i = 0; i < currentDiceCount; i++)
        {
            chooseList[i].OnColliderClose();

            float x = i * scaleFactor - offset;
            Vector3 targetPosition = new Vector3(x, choosePosition.y, choosePosition.z);

            yield return StartCoroutine(MoveToPosition(chooseList[i].gameObject, targetPosition, chooseList[i].SetRotationForDicenumber(), 0.1f)); // �̵� �ð� 0.5��

            selectUI.ActiveSelectUI();
            selectAble = true;
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

            yield return null; // ���� ����������
        }

        // ��Ȯ�� ��ǥ ��ġ/ȸ������ ����
        dice.transform.position = targetPosition;
        dice.transform.rotation = targetRotation;
    }
}
