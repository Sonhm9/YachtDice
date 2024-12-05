using System.Collections;
using UnityEngine;

public class ShakeBasketController : MonoBehaviour
{
    public GameObject dicePrefab; // 주사위 프리팹
    public Transform throwingPos; // 주사위 던질 때 포지션
    public Transform[] spawnPos; // 주사위 스폰 포지션
    public Transform groundPos; // 던질 그라운드 포지션
    public Transform readyPos; // 준비상태 바스켓 포지션
    public DiceController[] diceControllers; // 주사위 컨트롤러

    public float sensitivity = 0.1f; // 마우스 감도
    public float maxShakeRange = 1f; // 컵의 최대 이동 범위

    public float rotationDuration = 0.25f; // 회전에 걸리는 시간

    private Vector3 initialPosition; // 오브젝트의 초기 위치

    void Start()
    {
        InitializeDice(5);
        initialPosition = transform.localPosition;
    }

    private void Update()
    {
        // 현재 쉐이크 모드 일 때
        if (ModeManager.Instance.currentMode == ModeManager.Mode.Shake)
        {
            // 마우스를 누르고 있을때
            if (Input.GetMouseButton(0))
            {
                float mouseX = Input.GetAxis("Mouse X"); // 마우스의 X축 움직임
                float mouseY = Input.GetAxis("Mouse Y"); // 마우스의 Y축 움직임

                Vector3 shakeOffset = new Vector3(mouseX, 0, mouseY) * sensitivity; // y축 움직임 제한(위 아래)
                shakeOffset = Vector3.ClampMagnitude(shakeOffset, maxShakeRange); // 컵 이동 범위 제한

                RollingDice(shakeOffset.x, shakeOffset.z); // 주사위 움직임

                transform.position = initialPosition + shakeOffset; // 컵의 새 위치
            }
            // 마우스를 떼면
            if (Input.GetMouseButtonUp(0))
            {
                StartCoroutine(ThrowingDiceRoutine(throwingPos.position));
            }
        }
    }

    // 주사위를 컵에 넣는 메서드
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

    // 주사위를 섞는 메서드
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

    // 주사위를 그라운드로 던지는 코루틴
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

    // 주사위를 던지는 코루틴
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

    // 주사위를 던지는 코루틴
    private IEnumerator SetReady(Transform targetPosition)
    {
        yield return new WaitForSeconds(2);
        transform.position = targetPosition.position;
        transform.rotation = Quaternion.identity;
    }

    // 바스켓을 회전하는 코루틴
    private IEnumerator RotateBasket(float targetAngle)
    {
        float elapsedTime = 0f;
        float startAngle = transform.localEulerAngles.z; // 현재 Z축 각도
        float endAngle = targetAngle; // 목표 각도

        while (elapsedTime < rotationDuration)
        {
            float newAngle = Mathf.Lerp(startAngle, endAngle, elapsedTime / rotationDuration);
            transform.localEulerAngles = new Vector3(0f, 0f, newAngle);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localEulerAngles = new Vector3(0f, 0f, endAngle); // 정확히 목표 각도로 설정
        StartCoroutine(ThrowingDice(groundPos));
        StartCoroutine(SetReady(readyPos));

    }
}
