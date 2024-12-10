using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    static public GameManager Instance
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

    public GameObject turnUI;
    public GameObject shakeUI;
    public GameObject endGameUI;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void EndGame()
    {
        turnUI.SetActive(false);
        shakeUI.SetActive(false);

        endGameUI.SetActive(true);
    }
}
