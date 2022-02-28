using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGridChunk : MonoBehaviour
{
    HexCell[] cells;

    public HexMesh terrain;

    private void Awake()
    {
        cells = new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
    }

    public void AddCell(int index, HexCell cell)
    {
        cells[index] = cell;
        cell.chunk = this;
        cell.transform.SetParent(transform, false);
    }

    public void Refresh()
    {
        enabled = true;
    }

    private void LateUpdate()
    {
        Triangulate();
        enabled = false;
    }

    public void Triangulate()
    {
        terrain.Clear();

        for (int i = 0; i < cells.Length; i++)
        {
            //Triangulate(cells[i]);
        }

        terrain.Apply();
    }
}
