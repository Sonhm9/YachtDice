using UnityEngine;

public class GetDiceNumber : MonoBehaviour
{
    public int number;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ground")
        {
            DiceController dice = GetComponentInParent<DiceController>();
            dice.GetDiceNumber(number);
        }
    }
}
