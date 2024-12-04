using System.Collections;
using UnityEngine;

public class ShakeBasketController : MonoBehaviour
{
    public GameObject dicePrefab; // �ֻ��� ������
    public Transform[] spawnPosition; // �ֻ��� ���� ������
    public DiceController[] diceControllers; // �ֻ��� ��Ʈ�ѷ�

    public float sensitivity = 0.1f; // ���콺 ����
    public float maxShakeRange = 1f; // ���� �ִ� �̵� ����

    public float rotationDuration = 1f; // ȸ���� �ɸ��� �ð�

    private Vector3 initialPosition; // ������Ʈ�� �ʱ� ��ġ
    private float currentShakeTime; // ���� ���� ���� �ð�

    void Start()
    {
        InitializeDice();
        initialPosition = transform.localPosition;
    }

    private void Update()
    {
        // ���콺 ������ ����
        float mouseX = Input.GetAxis("Mouse X"); // ���콺�� X�� ������
        float mouseY = Input.GetAxis("Mouse Y"); // ���콺�� Y�� ������

        Vector3 shakeOffset = new Vector3(mouseX, 0, mouseY) * sensitivity; // y�� ������ ����(�� �Ʒ�)
        shakeOffset = Vector3.ClampMagnitude(shakeOffset, maxShakeRange); // �� �̵� ���� ����

        RollingDice(shakeOffset.x, shakeOffset.z); // �ֻ��� ������

        transform.position = initialPosition + shakeOffset; // ���� �� ��ġ
    }

    // �ֻ����� �ſ� �ִ� �޼���
    void InitializeDice()
    {
        diceControllers = new DiceController[5];

        for (int i=0; i<5; i++)
        {
            Vector3 position = new Vector3(spawnPosition[i].position.x, spawnPosition[i].position.y, spawnPosition[i].position.z);

            GameObject dice = Instantiate(dicePrefab, position, Quaternion.identity); ;

            diceControllers[i] = dice.GetComponent<DiceController>();
        }
    }

    // �ֻ����� ���� �޼���
    void RollingDice(float x, float z)
    {
        foreach(DiceController dice in diceControllers)
        {
            if (!dice)
            {
                Debug.LogWarning("No Dice");
            }
            dice.AddForceToDice(x,z);
        }
    }



    private IEnumerator SlowRotateAroundPivot(float targetAngle)
    {
        float elapsedTime = 0f;
        float startAngle = transform.localEulerAngles.z; // ���� Z�� ����
        float endAngle = startAngle + targetAngle; // ��ǥ ����

        while (elapsedTime < rotationDuration)
        {
            float newAngle = Mathf.Lerp(startAngle, endAngle, elapsedTime / rotationDuration);
            transform.localEulerAngles = new Vector3(0f, 0f, newAngle);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ��Ȯ�� ��ǥ ������ ����
        transform.localEulerAngles = new Vector3(0f, 0f, endAngle);
    }
}
