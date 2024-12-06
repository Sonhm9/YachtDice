using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseModeBehaviour : MonoBehaviour
{
    public Vector3 choosePosition; // ���� �ֻ��� ������

    public int currentDiceCount = 5; // ���� ���� �ֻ��� ����
    public float scaleFactor = 1; // �ֻ��� ����

    public List<DiceController> chooseList = new List<DiceController>();
    List<DiceController> selectList = new List<DiceController>();

    ShakeBasketController shakeBasketController;
    int maxDiceCount = 5;

    void Start()
    {
        shakeBasketController = GetComponent<ShakeBasketController>();
    }

    void Update()
    {
        
    }

    // �ֻ��� ��ü ��������
    public void GetDiceList()
    {
        for(int i=0; i<currentDiceCount; i++)
        {
            chooseList.Add(shakeBasketController.diceControllers[i]);
            chooseList[i].rb.isKinematic = true; 
        }
        //selectList.Sort();

        StartCoroutine(SetChooseList());
    }

    // ���ð����ϵ��� ��ġ��Ű��
    IEnumerator SetChooseList()
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < currentDiceCount; i++)
        {
            chooseList[i].OnColliderClose();
            float x = (i - ((currentDiceCount - 1) / 2)) * scaleFactor;
            chooseList[i].gameObject.transform.position = new Vector3(x, choosePosition.y, choosePosition.z);
            chooseList[i].gameObject.transform.rotation = Quaternion.identity;
            chooseList[i].SetRotationForDicenumber();
        }
    }
}
