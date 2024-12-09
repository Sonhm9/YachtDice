using UnityEngine;

public class SelectUI : MonoBehaviour
{
    public GameObject selectPointer;
    public GameObject selectUI;

    ChooseModeBehaviour chooseMode;

    private void Start()
    {
        chooseMode = GetComponent<ChooseModeBehaviour>();
    }

    // ������ ��ġ �޼���
    public void LocateSelectedDice(float x)
    {
        float adjustment = -x * 0.08f;
        float correctedX = x + adjustment;
        selectPointer.transform.position = new Vector3(correctedX, selectPointer.transform.position.y, selectPointer.transform.position.z);
    }

    // UI Ȱ��ȭ �޼���
    public void ActiveSelectUI()
    {
        if (chooseMode.selectAble)
        {
            selectPointer.SetActive(true);
            selectUI.SetActive(true);
        }
    }

    // UI ��Ȱ��ȭ �޼���
    public void UnactiveSelectUI()
    {
        selectPointer.SetActive(false);
        selectUI.SetActive(false);
    }
}
