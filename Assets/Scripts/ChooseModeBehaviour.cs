using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseModeBehaviour : MonoBehaviour
{
    public Vector3 choosePosition; // 선택 주사위 포지션

    public int currentDiceCount = 5; // 현재 던질 주사위 갯수
    public float scaleFactor = 1; // 주사위 간격

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

    // 주사위 객체 가져오기
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

    // 선택가능하도록 위치시키기
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
