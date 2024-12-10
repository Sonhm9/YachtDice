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
