using Sirenix.OdinInspector;
using UnityEngine;

public class DiceController : MonoBehaviour
{
    // �ֻ����� ����
    public enum State
    {
        Move,
        Stop,
        Select
    }
    public State diceState;

    public float forceMagnitude = 5; // ���ϴ� ���� ũ��
    public Rigidbody rb;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    // �ֻ����� x,z������ ���� ���ϴ� �޼���
    public void AddForceToDice(float x, float z)
    {
        Vector3 direction = new Vector3(x, 0, z);
        rb.AddForce(direction.normalized * forceMagnitude, ForceMode.Impulse);
    }
}
