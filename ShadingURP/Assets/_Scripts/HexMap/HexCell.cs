using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    //General cells
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

            if (hasOutgoingRiver && elevation < GetNeighbour(outgoingRiver).elevation)
            {
                RemoveOutgoingRiver();
            }

            if (hasIncomingRiver && elevation > GetNeighbour(incomingRiver).elevation)
            {
                RemoveIncomingRiver();
            }

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

    //Rivers
    bool hasIncomingRiver, hasOutgoingRiver;
    HexDirection incomingRiver, outgoingRiver;

    public bool HasRiver
    {
        get
        {
            return hasIncomingRiver || hasOutgoingRiver;
        }
    }

    public bool HasRiverBeginOrEnd
    {
        get
        {
            return hasIncomingRiver != hasOutgoingRiver;
        }
    }

    public bool HasIncomingRiver
    {
        get
        {
            return hasIncomingRiver;
        }
    }

    public bool HasOutgoingRiver
    {
        get
        {
            return hasOutgoingRiver;
        }
    }

    public HexDirection IncomingRiver
    {
        get
        {
            return incomingRiver;
        }
    }

    public HexDirection OutgoingRiver
    {
        get
        {
            return outgoingRiver;
        }
    }

    public bool HasRiverThroughEdge(HexDirection direction)
    {
        return
            hasIncomingRiver && incomingRiver == direction ||
            hasOutgoingRiver && outgoingRiver == direction;
    }

    public float StreamBedY
    {
        get
        {
            return
                (elevation + HexMetrics.streamBedElevationOffset) *
                HexMetrics.elevationStep;
        }
    }

    void Refresh()
    {
        if (chunk) { chunk.Refresh(); }

        for (int i = 0; i < neighbours.Length; i++)
        {
            HexCell neighbour = neighbours[i];

            if (neighbour != null && neighbour.chunk != chunk)
            {
                neighbour.chunk.Refresh();
            }
        }
    }

    void RefreshSelfOnly()
    {
        chunk.Refresh();
    }

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

    public void SetOutgoingRiver(HexDirection direction)
    {
        if (hasOutgoingRiver && outgoingRiver == direction)
        {
            return;
        }

        HexCell neighbour = GetNeighbour(direction);
        if (!neighbour || elevation < neighbour.elevation)
        {
            return;
        }

        RemoveOutgoingRiver();
        if (hasIncomingRiver && incomingRiver == direction)
        {
            RemoveIncomingRiver();
        }

        hasOutgoingRiver = true;
        outgoingRiver = direction;
        RefreshSelfOnly();

        neighbour.RemoveIncomingRiver();
        neighbour.hasIncomingRiver = true;
        neighbour.incomingRiver = direction.Opposite();
        neighbour.RefreshSelfOnly();
    }

    public void RemoveRiver()
    {
        RemoveOutgoingRiver();
        RemoveIncomingRiver();
    }

    public void RemoveOutgoingRiver()
    {
        if (!hasOutgoingRiver)
        {
            return;
        }
        hasOutgoingRiver = false;
        RefreshSelfOnly();

        HexCell neighbor = GetNeighbour(outgoingRiver);
        neighbor.hasIncomingRiver = false;
        neighbor.RefreshSelfOnly();
    }

    public void RemoveIncomingRiver()
    {
        if (!hasIncomingRiver)
        {
            return;
        }
        hasIncomingRiver = false;
        RefreshSelfOnly();

        HexCell neighbor = GetNeighbour(incomingRiver);
        neighbor.hasOutgoingRiver = false;
        neighbor.RefreshSelfOnly();
    }
}
