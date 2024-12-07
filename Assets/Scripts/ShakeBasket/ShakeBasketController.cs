using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeBasketController : MonoBehaviour
{
    public GameObject dicePrefab; // �ֻ��� ������
    public Transform throwingPos; // �ֻ��� ���� �� ������
    public Transform[] spawnPos; // �ֻ��� ���� ������
    public Transform groundPos; // ���� �׶��� ������
    public Transform readyPos; // �غ���� �ٽ��� ������
    public List<DiceController> diceControllers = new List<DiceController>(); // �ֻ��� ��Ʈ�ѷ�

    public float sensitivity = 0.1f; // ���콺 ����
    public float maxShakeRange = 1f; // ���� �ִ� �̵� ����

    public float rotationDuration = 0.25f; // ȸ���� �ɸ��� �ð�

    private Vector3 initialPosition; // ������Ʈ�� �ʱ� ��ġ
    ChooseModeBehaviour chooseMode;

    void Start()
    {
        chooseMode = GetComponent<ChooseModeBehaviour>();
        initialPosition = transform.localPosition;
        SetShakeMode();

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
                StartCoroutine(ThrowingDiceRoutine(throwingPos.position)); // ������ ��ƾ ����
            }
        }
    }
    public void SetShakeMode()
    {
        ModeManager.Instance.currentMode = ModeManager.Mode.Shake;
        gameObject.transform.position = initialPosition;
        InitializeDice(chooseMode.currentDiceCount);
    }

    // �ֻ����� �ſ� �ִ� �޼���
    void InitializeDice(int count)
    {
        for (int i=0; i< count; i++)
        {
            Vector3 position = new Vector3(spawnPos[i].position.x, spawnPos[i].position.y, spawnPos[i].position.z);

            DiceController dice = Instantiate(dicePrefab, position, Random.rotation).GetComponent<DiceController>();

            diceControllers.Add(dice);
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

    // �ֻ����� ������ ��ƾ
    private IEnumerator ThrowingDiceRoutine(Vector3 targetPosition)
    {
        ModeManager.Instance.currentMode = ModeManager.Mode.Choose; // ���� ���� ��ȯ

        // �ð��� ��ǥ ��ġ ����
        float elapsedTime = 0f;
        float startXpos = transform.position.x;
        float endXpos = targetPosition.x;

        // ��ǥ ��ġ�� �̵�
        while (elapsedTime < 0.25f)
        {
            float xPos = Mathf.Lerp(startXpos, endXpos, elapsedTime / rotationDuration);
            transform.position = new Vector3(xPos * 1.1f, initialPosition.y, initialPosition.z);
            foreach (DiceController dice in diceControllers)
            {
                RollingDice(xPos, 0);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;

        // �ٽ����� ����̴� ��ƾ
        StartCoroutine(RotateBasket(120f));
    }

    // �ٽ����� ȸ���ϴ� �ڷ�ƾ
    private IEnumerator RotateBasket(float targetAngle)
    {
        float elapsedTime = 0f;
        float startAngle = transform.localEulerAngles.z; // ���� Z�� ����
        float endAngle = targetAngle; // ��ǥ ����

        // ��ǥ ȸ������ ȸ��
        while (elapsedTime < rotationDuration)
        {
            float newAngle = Mathf.Lerp(startAngle, endAngle, elapsedTime / rotationDuration);
            transform.localEulerAngles = new Vector3(0f, 0f, newAngle);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localEulerAngles = new Vector3(0f, 0f, endAngle); // ��Ȯ�� ��ǥ ������ ����

        StartCoroutine(ThrowingDice(groundPos)); // ������ �ֻ��� ������ ��ƾ
        StartCoroutine(SetReady(readyPos)); // �ٽ����� ������ ġ��� ��ƾ
    }

    // �ֻ����� �׶���� ������ �ڷ�ƾ
    IEnumerator ThrowingDice(Transform pos)
    {
        yield return new WaitForSeconds(0.1f);

        // �ֻ����� ���� ����
        Vector3 groundPos = pos.position;
        foreach (DiceController dice in diceControllers)
        {
            Vector3 shootDirection = (groundPos - dice.transform.position).normalized;
            dice.GetComponent<Rigidbody>().AddForce(shootDirection * 2500, ForceMode.Impulse);
        }

        // ���� �ð� �� �ݶ��̴��� �Ѽ� �ֻ��� �� ����
        yield return new WaitForSeconds(2.5f);
        foreach (DiceController dice in diceControllers)
        {
            dice.OnColliderOpen();
        }

        yield return new WaitForSeconds(0.5f);

        chooseMode.GetDiceList(); // �ֻ��� ��ü �Ѱ��ֱ�
        diceControllers.Clear(); // �ֻ��� ����Ʈ �ʱ�ȭ
    }

    // �ٽ����� ġ��� ��ƾ
    private IEnumerator SetReady(Transform targetPosition)
    {
        yield return new WaitForSeconds(2);
        transform.position = targetPosition.position;
        transform.rotation = Quaternion.identity;
    }


}
