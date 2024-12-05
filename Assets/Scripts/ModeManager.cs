using UnityEngine;

public class ModeManager : MonoBehaviour
{
    static ModeManager instance;
    static public ModeManager Instance
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
    public enum Mode
    {
        Shake,
        Choose,
        Score
    }
    public Mode currentMode;


}
