using System.Collections;
using UnityEngine;

public class ShakeBasketController : MonoBehaviour
{
    public GameObject dicePrefab; // �ֻ��� ������
    public Transform throwingPos; // �ֻ��� ���� �� ������
    public Transform[] spawnPos; // �ֻ��� ���� ������
    public Transform groundPos; // ���� �׶��� ������
    public Transform readyPos; // �غ���� �ٽ��� ������
    public DiceController[] diceControllers; // �ֻ��� ��Ʈ�ѷ�

    public float sensitivity = 0.1f; // ���콺 ����
    public float maxShakeRange = 1f; // ���� �ִ� �̵� ����

    public float rotationDuration = 0.25f; // ȸ���� �ɸ��� �ð�

    private Vector3 initialPosition; // ������Ʈ�� �ʱ� ��ġ

    void Start()
    {
        InitializeDice(5);
        initialPosition = transform.localPosition;
    }

    private void Update()
    {
        // ���� ����ũ ��� �� ��
        if (ModeManager.Instance.currentMode == ModeManager.Mode.Shake)
        {
            // ���콺�� ������ ������
            if (Input.GetMouseButton(0))
            {
                float mouseX = Input.GetAxis("Mouse X"); // ���콺�� X�� ������
                float mouseY = Input.GetAxis("Mouse Y"); // ���콺�� Y�� ������

                Vector3 shakeOffset = new Vector3(mouseX, 0, mouseY) * sensitivity; // y�� ������ ����(�� �Ʒ�)
                shakeOffset = Vector3.ClampMagnitude(shakeOffset, maxShakeRange); // �� �̵� ���� ����

                RollingDice(shakeOffset.x, shakeOffset.z); // �ֻ��� ������

                transform.position = initialPosition + shakeOffset; // ���� �� ��ġ
            }
            // ���콺�� ����
            if (Input.GetMouseButtonUp(0))
            {
                StartCoroutine(ThrowingDiceRoutine(throwingPos.position));
            }
        }
    }

    // �ֻ����� �ſ� �ִ� �޼���
    void InitializeDice(int count)
    {
        diceControllers = new DiceController[count];

        for (int i=0; i< count; i++)
        {
            Vector3 position = new Vector3(spawnPos[i].position.x, spawnPos[i].position.y, spawnPos[i].position.z);

            GameObject dice = Instantiate(dicePrefab, position, Random.rotation);

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

    // �ֻ����� �׶���� ������ �ڷ�ƾ
    IEnumerator ThrowingDice(Transform pos)
    {
        yield return new WaitForSeconds(0.1f);

        Vector3 groundPos = pos.position;
        foreach (DiceController dice in diceControllers)
        {
            Vector3 shootDirection = (groundPos - dice.transform.position).normalized;
            dice.GetComponent<Rigidbody>().AddForce(shootDirection * 2500, ForceMode.Impulse);
        }
    }

    // �ֻ����� ������ �ڷ�ƾ
    private IEnumerator ThrowingDiceRoutine(Vector3 targetPosition)
    {
        ModeManager.Instance.currentMode = ModeManager.Mode.Choose;

        float elapsedTime = 0f;
        float startXpos = transform.position.x;
        float endXpos = targetPosition.x;

        while (elapsedTime < 0.25f)
        {
            float xPos = Mathf.Lerp(startXpos, endXpos, elapsedTime / rotationDuration);
            transform.position = new Vector3(xPos, initialPosition.y, initialPosition.z);
            foreach (DiceController dice in diceControllers)
            {
                RollingDice(xPos, 0);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;

        StartCoroutine(RotateBasket(120f));
    }

    // �ֻ����� ������ �ڷ�ƾ
    private IEnumerator SetReady(Transform targetPosition)
    {
        yield return new WaitForSeconds(2);
        transform.position = targetPosition.position;
        transform.rotation = Quaternion.identity;
    }

    // �ٽ����� ȸ���ϴ� �ڷ�ƾ
    private IEnumerator RotateBasket(float targetAngle)
    {
        float elapsedTime = 0f;
        float startAngle = transform.localEulerAngles.z; // ���� Z�� ����
        float endAngle = targetAngle; // ��ǥ ����

        while (elapsedTime < rotationDuration)
        {
            float newAngle = Mathf.Lerp(startAngle, endAngle, elapsedTime / rotationDuration);
            transform.localEulerAngles = new Vector3(0f, 0f, newAngle);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localEulerAngles = new Vector3(0f, 0f, endAngle); // ��Ȯ�� ��ǥ ������ ����
        StartCoroutine(ThrowingDice(groundPos));
        StartCoroutine(SetReady(readyPos));

    }
}
