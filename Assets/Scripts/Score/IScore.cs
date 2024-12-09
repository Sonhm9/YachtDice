using System.Collections.Generic;

public interface IScore
{
    void CalculateValue(List<DiceController> scoreList);
    void SelectValue();
    public void ColorTint(float alpha);
}
