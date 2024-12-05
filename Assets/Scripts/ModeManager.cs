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
    public enum Mode
    {
        Shake,
        Choose,
        Score
    }
    public Mode currentMode;


}
