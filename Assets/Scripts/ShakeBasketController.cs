using System.Collections;
using UnityEngine;

public class ShakeBasketController : MonoBehaviour
{
    public GameObject dicePrefab;
    public Transform[] spawnPosition;

    public float shakeDuration = 1f; // 흔드는 시간
    public float shakeMagnitude = 0.5f; // 흔들림 크기
    public float shakeSpeed = 5f; // 흔들림 속도
    public float rotationDuration = 1f; // 회전에 걸리는 시간

    private Vector3 initialPosition; // 오브젝트의 초기 위치
    private float currentShakeTime; // 현재 남은 흔드는 시간

    void Start()
    {
        InitializeDice();
        initialPosition = transform.localPosition;
        StartShake();
    }
    void InitializeDice()
    {
        /*float randx = random.range(spawnposition.position.x - 0.2f, spawnposition.position.x + 0.2f);
        float randy = random.range(spawnposition.position.y - 0.2f, spawnposition.position.y + 0.2f);
        float randz = random.range(spawnposition.position.z - 0.2f, spawnposition.position.z + 0.2f);

        vector3 randposition = new vector3(randx, randy, randz);*/

        for (int i=0; i<5; i++)
        {
            Vector3 position = new Vector3(spawnPosition[i].position.x, spawnPosition[i].position.y, spawnPosition[i].position.z);
            Instantiate(dicePrefab, position, Quaternion.identity);
        }
    }

    public void StartShake()
    {
        currentShakeTime = shakeDuration;
        StartCoroutine(ShakeHorizontally());
    }

    private IEnumerator ShakeHorizontally()
    {
        yield return new WaitForSeconds(1f);
        while (currentShakeTime > 0)
        {
            float offsetX = Mathf.Sin(Time.time * shakeSpeed) * shakeMagnitude;

            transform.localPosition = new Vector3(
                initialPosition.x + offsetX,
                initialPosition.y,
                initialPosition.z
            );

            currentShakeTime -= Time.deltaTime;
            yield return null;
        }

        transform.localPosition = initialPosition; // 흔들림이 끝난 후 원래 위치로 복귀
        StartCoroutine(SlowRotateAroundPivot(125f)); // Z축 기준으로 천천히 회전
    }

    private IEnumerator SlowRotateAroundPivot(float targetAngle)
    {
        float elapsedTime = 0f;
        float startAngle = transform.localEulerAngles.z; // 현재 Z축 각도
        float endAngle = startAngle + targetAngle; // 목표 각도

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
