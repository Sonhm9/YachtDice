using System.Collections;
using UnityEngine;

public class ShakeBasketController : MonoBehaviour
{
    public GameObject dicePrefab;
    public Transform[] spawnPosition;

    public float shakeDuration = 1f; // ���� �ð�
    public float shakeMagnitude = 0.5f; // ��鸲 ũ��
    public float shakeSpeed = 5f; // ��鸲 �ӵ�
    public float rotationDuration = 1f; // ȸ���� �ɸ��� �ð�

    private Vector3 initialPosition; // ������Ʈ�� �ʱ� ��ġ
    private float currentShakeTime; // ���� ���� ���� �ð�

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

        transform.localPosition = initialPosition; // ��鸲�� ���� �� ���� ��ġ�� ����
        StartCoroutine(SlowRotateAroundPivot(125f)); // Z�� �������� õõ�� ȸ��
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
