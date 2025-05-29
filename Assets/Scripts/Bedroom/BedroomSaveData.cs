using System.Collections.Generic;

[System.Serializable]
public class BedroomSaveData
{
    public int Currency;
    public List<int> ActivatedItemIDs;

    public BedroomSaveData()
    {
        Currency = 0;
        ActivatedItemIDs = new List<int>();
    }
}
