

using UnityEngine;
[System.Serializable]
public class GridProbability
{
    public Grid grid;          // riferimento alla griglia
    [Range(0f, 100)]
    public int probability;  // probabilità del teletrasporto
}