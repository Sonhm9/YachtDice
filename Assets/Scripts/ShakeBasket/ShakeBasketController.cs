using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeBasketController : MonoBehaviour
{
    public GameObject dicePrefab; // 주사위 프리팹
    public Transform throwingPos; // 주사위 던질 때 포지션
    public Transform[] spawnPos; // 주사위 스폰 포지션
    public Transform groundPos; // 던질 그라운드 포지션
    public Transform readyPos; // 준비상태 바스켓 포지션
    public List<DiceController> diceControllers = new List<DiceController>(); // 주사위 컨트롤러

    public float sensitivity = 0.1f; // 마우스 감도
    public float maxShakeRange = 1f; // 컵의 최대 이동 범위

    public float rotationDuration = 0.25f; // 회전에 걸리는 시간

    private Vector3 initialPosition; // 오브젝트의 초기 위치
    ChooseModeBehaviour chooseMode;

    void Start()
    {
        chooseMode = GetComponent<ChooseModeBehaviour>();
        initialPosition = transform.localPosition;
        SetShakeMode();

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
                StartCoroutine(ThrowingDiceRoutine(throwingPos.position)); // 던지는 루틴 실행
            }
        }
    }
    public void SetShakeMode()
    {
        ModeManager.Instance.currentMode = ModeManager.Mode.Shake;
        gameObject.transform.position = initialPosition;
        InitializeDice(chooseMode.currentDiceCount);
    }

    // 주사위를 컵에 넣는 메서드
    void InitializeDice(int count)
    {
        for (int i=0; i< count; i++)
        {
            Vector3 position = new Vector3(spawnPos[i].position.x, spawnPos[i].position.y, spawnPos[i].position.z);

            DiceController dice = Instantiate(dicePrefab, position, Random.rotation).GetComponent<DiceController>();

            diceControllers.Add(dice);
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

    // 주사위를 던지는 루틴
    private IEnumerator ThrowingDiceRoutine(Vector3 targetPosition)
    {
        ModeManager.Instance.currentMode = ModeManager.Mode.Choose; // 선택 모드로 전환

        // 시간과 목표 위치 설정
        float elapsedTime = 0f;
        float startXpos = transform.position.x;
        float endXpos = targetPosition.x;

        // 목표 위치로 이동
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

        // 바스켓을 기울이는 루틴
        StartCoroutine(RotateBasket(120f));
    }

    // 바스켓을 회전하는 코루틴
    private IEnumerator RotateBasket(float targetAngle)
    {
        float elapsedTime = 0f;
        float startAngle = transform.localEulerAngles.z; // 현재 Z축 각도
        float endAngle = targetAngle; // 목표 각도

        // 목표 회전까지 회전
        while (elapsedTime < rotationDuration)
        {
            float newAngle = Mathf.Lerp(startAngle, endAngle, elapsedTime / rotationDuration);
            transform.localEulerAngles = new Vector3(0f, 0f, newAngle);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localEulerAngles = new Vector3(0f, 0f, endAngle); // 정확히 목표 각도로 설정

        StartCoroutine(ThrowingDice(groundPos)); // 땅으로 주사위 던지는 루틴
        StartCoroutine(SetReady(readyPos)); // 바스켓을 옆으로 치우는 루틴
    }

    // 주사위를 그라운드로 던지는 코루틴
    IEnumerator ThrowingDice(Transform pos)
    {
        yield return new WaitForSeconds(0.1f);

        // 주사위를 땅에 던짐
        Vector3 groundPos = pos.position;
        foreach (DiceController dice in diceControllers)
        {
            Vector3 shootDirection = (groundPos - dice.transform.position).normalized;
            dice.GetComponent<Rigidbody>().AddForce(shootDirection * 2500, ForceMode.Impulse);
        }

        // 일정 시간 후 콜라이더를 켜서 주사위 눈 검출
        yield return new WaitForSeconds(2.5f);
        foreach (DiceController dice in diceControllers)
        {
            dice.OnColliderOpen();
        }

        yield return new WaitForSeconds(0.5f);

        chooseMode.GetDiceList(); // 주사위 객체 넘겨주기
        diceControllers.Clear(); // 주사위 리스트 초기화
    }

    // 바스켓을 치우는 루틴
    private IEnumerator SetReady(Transform targetPosition)
    {
        yield return new WaitForSeconds(2);
        transform.position = targetPosition.position;
        transform.rotation = Quaternion.identity;
    }


}
