using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class DiceController : MonoBehaviour
{
    public Collider[] colliders;
    // 주사위 상태
    public enum State
    {
        Move,
        Stop,
        Select
    }
    public State diceState;

    public float forceMagnitude = 5;
    public int diceNumber;
    public Rigidbody rb;

    void Start()
    {

    }

    void Update()
    {
       
    }

    // 주사위에 힘을 가하는 메서드
    public void AddForceToDice(float x, float z)
    {
        Vector3 direction = new Vector3(x, 0, z);
        rb.AddForce(direction.normalized * forceMagnitude, ForceMode.Impulse);
    }

    // 주사위의 눈을 검출하는 메서드
    public void GetDiceNumber(int number)
    {
        diceNumber = number;
        Debug.Log(diceNumber);
    }

    // 콜라이더를 키는 메서드
    public void OnColliderOpen()
    {
        foreach(Collider col in colliders)
        {
            col.enabled = true;
        }
    }

    // 콜라이더를 끄는 메서드
    public void OnColliderClose()
    {
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }
    }

    // 주사위 눈에 따라 회전값을 적용
    public void SetRotationForDicenumber()
    {
        switch (diceNumber)
        {
            case 1:
                transform.Rotate(-90, 0, 0);
                break;

            case 2:
                transform.Rotate(-0, 0, 0);
                break;

            case 3:
                transform.Rotate(0, 0, -90);
                break;

            case 4:
                transform.Rotate(0, 0, 90);
                break;

            case 5:
                transform.Rotate(-180, 0, 0);
                break;

            case 6:
                transform.Rotate(90, 0, 0);
                break;
        }
    }

}
