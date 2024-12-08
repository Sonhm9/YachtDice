using System.Collections;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public RectTransform scoreCanvas;
    public float targetX; // 목표 X 위치
    public float moveSpeed = 500f; // 이동 속도 (픽셀/초)

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
        // 인스턴스가 존재한다면
        if (instance != null)
        {
            Destroy(gameObject);
        }

        // 인스턴스가 처음 생성된다면
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

    // 턴 넘기기
    public void NextChoose()
    {
        currentChoose++;
    }

    // 점수 기록 모드로 전환
    public void SetScoreMode()
    {
        Debug.Log("Score");
        currentChoose = 0;

        ModeManager.Instance.currentMode = ModeManager.Mode.Score;
    }

    // 3번째 턴 인지 검사
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

    // ScoreCanvas를 이동시키는 루틴
    private IEnumerator ScoreCanvasMoving(RectTransform ui, float targetX, float speed)
    {
        Vector2 startPos = ui.anchoredPosition; // 초기 위치
        Vector2 targetPos = new Vector2(targetX, startPos.y); // 목표 위치

        while (Vector2.Distance(ui.anchoredPosition, targetPos) > 0.1f)
        {
            ui.anchoredPosition = Vector2.MoveTowards(ui.anchoredPosition, targetPos, speed * Time.deltaTime);
            yield return null;
        }

        ui.anchoredPosition = targetPos; // 정확히 목표 위치로 설정
    }
}
