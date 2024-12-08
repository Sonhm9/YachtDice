using System.Collections;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public RectTransform scoreCanvas;
    public float targetX; // ��ǥ X ��ġ
    public float moveSpeed = 500f; // �̵� �ӵ� (�ȼ�/��)

    int maxTurn = 12;
    int currentTurn = 1;

    int maxChoose = 3;
    int currentChoose = 0;

    float initialPosition;



    static TurnManager instance;
    static public TurnManager Instance
    {
        get
        {
            return instance;
        }
    }
    private void Awake()
    {
        // �ν��Ͻ��� �����Ѵٸ�
        if (instance != null)
        {
            Destroy(gameObject);
        }

        // �ν��Ͻ��� ó�� �����ȴٸ�
        else
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
        initialPosition = scoreCanvas.anchoredPosition.x;
    }

    // �� �ѱ��
    public void NextChoose()
    {
        currentChoose++;
    }

    // ���� ��� ���� ��ȯ
    public void SetScoreMode()
    {
        Debug.Log("Score");
        currentChoose = 0;

        ModeManager.Instance.currentMode = ModeManager.Mode.Score;
    }

    // 3��° �� ���� �˻�
    public bool CheckMaxChooseCount()
    {
        if (currentChoose >= maxChoose)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void MoveScoreCanvas()
    {
        StartCoroutine(ScoreCanvasMoving(scoreCanvas,targetX,moveSpeed));
    }

    public void ReturnScoreCanvas()
    {
        StartCoroutine(ScoreCanvasMoving(scoreCanvas, initialPosition, moveSpeed));
    }

    // ScoreCanvas�� �̵���Ű�� ��ƾ
    private IEnumerator ScoreCanvasMoving(RectTransform ui, float targetX, float speed)
    {
        Vector2 startPos = ui.anchoredPosition; // �ʱ� ��ġ
        Vector2 targetPos = new Vector2(targetX, startPos.y); // ��ǥ ��ġ

        while (Vector2.Distance(ui.anchoredPosition, targetPos) > 0.1f)
        {
            ui.anchoredPosition = Vector2.MoveTowards(ui.anchoredPosition, targetPos, speed * Time.deltaTime);
            yield return null;
        }

        ui.anchoredPosition = targetPos; // ��Ȯ�� ��ǥ ��ġ�� ����
    }
}
