using Sirenix.OdinInspector;
using UnityEngine;

public class DiceController : MonoBehaviour
{
    // 주사위의 상태
    public enum State
    {
        Move,
        Stop,
        Select
    }
    public State diceState;

    public float forceMagnitude = 5; // 가하는 힘의 크기
    public Rigidbody rb;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    // 주사위에 x,z축으로 힘을 가하는 메서드
    public void AddForceToDice(float x, float z)
    {
        Vector3 direction = new Vector3(x, 0, z);
        rb.AddForce(direction.normalized * forceMagnitude, ForceMode.Impulse);
    }
}
