using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoolArray
{
    [System.Serializable]
    public struct rowData
    {
        public bool[] row;
    }

    public rowData[] column;

    public BoolArray(int x, int y)
    {
        column = new rowData[y];

        for (int i = 0; i < y; i++)
        {
            column[i].row = new bool[x];
        }
    }
}
