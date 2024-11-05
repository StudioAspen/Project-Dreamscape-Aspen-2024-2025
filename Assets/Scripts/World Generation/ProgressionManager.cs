using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    public int EmpowerTokens { get; private set; }
    public int WeakenTokens { get; private set; }

    public void RestockTokens(int landCount)
    {
        EmpowerTokens = landCount / 2;
        WeakenTokens = landCount / 2;
    }

    public void ConsumeWeakenToken()
    {
        WeakenTokens--;
    }

    public void ConsumeEmpowerToken()
    {
        EmpowerTokens--;
    }
}
