using System.Collections;
using UnityEngine;

public class ShakeBasketController : MonoBehaviour
{
    public GameObject dicePrefab; // 주사위 프리팹
    public Transform throwingPos; // 주사위 던질 때 포지션
    public Transform[] spawnPos; // 주사위 스폰 포지션
    public DiceController[] diceControllers; // 주사위 컨트롤러

    public float sensitivity = 0.1f; // 마우스 감도
    public float maxShakeRange = 1f; // 컵의 최대 이동 범위

    public float rotationDuration = 0.5f; // 회전에 걸리는 시간

    private Vector3 initialPosition; // 오브젝트의 초기 위치

    void Start()
    {
        InitializeDice();
        initialPosition = transform.localPosition;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            // 마우스 움직임 감지
            float mouseX = Input.GetAxis("Mouse X"); // 마우스의 X축 움직임
            float mouseY = Input.GetAxis("Mouse Y"); // 마우스의 Y축 움직임

            Vector3 shakeOffset = new Vector3(mouseX, 0, mouseY) * sensitivity; // y축 움직임 제한(위 아래)
            shakeOffset = Vector3.ClampMagnitude(shakeOffset, maxShakeRange); // 컵 이동 범위 제한

            RollingDice(shakeOffset.x, shakeOffset.z); // 주사위 움직임

            transform.position = initialPosition + shakeOffset; // 컵의 새 위치
        }
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 pos = new Vector3(throwingPos.position.x, throwingPos.position.y, throwingPos.position.z);
            StartCoroutine(ThrowingDice(pos));
        }
    }

    // 주사위를 컵에 넣는 메서드
    void InitializeDice()
    {
        diceControllers = new DiceController[5];

        for (int i=0; i<5; i++)
        {
            Vector3 position = new Vector3(spawnPos[i].position.x, spawnPos[i].position.y, spawnPos[i].position.z);

            GameObject dice = Instantiate(dicePrefab, position, Quaternion.identity); ;

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

    private IEnumerator ThrowingDice(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        float startXpos = transform.position.x;
        float endXpos = targetPosition.x;

        while (elapsedTime < 0.25f)
        {
            float xPos = Mathf.Lerp(startXpos, endXpos, elapsedTime / rotationDuration);
            transform.position = new Vector3(xPos, initialPosition.y, initialPosition.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 정확히 목표 각도로 설정
        transform.position = targetPosition;

        StartCoroutine(RotateBasket(120f));
    }

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

        // 정확히 목표 각도로 설정
        transform.localEulerAngles = new Vector3(0f, 0f, endAngle);
    }
}
