using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;

    [SerializeField] HexCell[] neighbours;

    public HexGridChunk chunk;

    public Vector3 Position
    {
        get { return transform.localPosition; }
    }

    public int Elevation
    {
        get { return elevation; }
        set 
        {
            if (elevation == value) return;

            elevation = value;

            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.elevationStep;
            position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;
            transform.localPosition = position;

            Refresh();
        }
    }

    public Color Color
    {
        get { return color; }
        set
        {
            if (color == value) return;

            color = value;
            Refresh();
        }
    }

    int elevation = int.MinValue;
    Color color;

    public HexCell GetNeighbour (HexDirection direction)
    {
        return neighbours[(int)direction];
    }

    public void SetNeighbour(HexDirection direction, HexCell cell)
    {
        neighbours[(int)direction] = cell;
        cell.neighbours[(int)direction.Opposite()] = this;
    }

    public HexEdgeType GetEdgeType(HexDirection direction)
    {
        return HexMetrics.GetEdgeType(elevation, neighbours[(int)direction].elevation);
    }

    public HexEdgeType GetEdgeType(HexCell otherCell)
    {
        return HexMetrics.GetEdgeType(elevation, otherCell.elevation);
    }

    void Refresh()
    {
        if (chunk) { chunk.Refresh(); }

        for (int i = 0; i < neighbours.Length; i++)
        {
            HexCell neighbour = neighbours[i];

            if(neighbour != null && neighbour.chunk != chunk)
            {
                neighbour.chunk.Refresh();
            }
        }
    }
}
