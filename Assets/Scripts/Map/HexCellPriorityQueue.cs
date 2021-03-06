using System.Collections.Generic;

public class HexCellPriorityQueue
{
    private List<HexCell> _list = new List<HexCell>();
    private int _count = 0;
    private int _minimum = int.MaxValue;

    public int Count => _count;

    public void Enqueue(HexCell cell)
    {
        _count += 1;
        int priority = cell.SearchPriority;

        if (priority < _minimum)
        {
            _minimum = priority;
        }
        while (priority >= _list.Count)
        {
            _list.Add(null);
        }

        cell.NextWithSamePriority = _list[priority];
        _list[priority] = cell;
    }

    public HexCell Dequeue()
    {
        _count -= 1;

        for (; _minimum < _list.Count; _minimum++)
        {
            HexCell cell = _list[_minimum];

            if (cell != null)
            {
                _list[_minimum] = cell.NextWithSamePriority;
                return cell;
            }
        }

        return null;
    }

    public void Change(HexCell cell, int oldPriority)
    {
        HexCell current = _list[oldPriority];
        HexCell next = current.NextWithSamePriority;

        if (current == cell)
        {
            _list[oldPriority] = next;
        }
        else
        {
            while (next != cell)
            {
                current = next;
                next = current.NextWithSamePriority;
            }
            current.NextWithSamePriority = cell.NextWithSamePriority;
        }

        Enqueue(cell);
        _count -= 1;
    }

    public void Clear()
    {
        _list.Clear();
        _count = 0;
        _minimum = int.MaxValue;
    }
}