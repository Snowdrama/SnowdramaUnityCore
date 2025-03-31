using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable enable
public class TableListItem<L, R>
{
    public L? Left = default;
    public R? Right = default;
}

public class TableList<L, R> : IEnumerable<TableListItem<L, R>>
{
    public List<L> left;
    public List<R> right;
    public TableList()
    {
        left = new List<L>();
        right = new List<R>();
    }
    public TableList(int initialCount)
    {
        left = new List<L>(initialCount);
        right = new List<R>(initialCount);
    }

    public void Add(L leftItem, R rightItem)
    {
        left.Add(leftItem);
        right.Add(rightItem);
    }

    public (L, R) Get(int index)
    {
        return (left[index], right[index]);
    }

    public L GetLeft(int index)
    {
        return left[index];
    }
    public R GetRight(int index)
    {
        return right[index];
    }

    public IEnumerator<TableListItem<L, R>> GetEnumerator()
    {
        TableListItem<L, R>[] items = new TableListItem<L, R>[left.Count];
        for (var i = 0; i < left.Count; ++i)
        {
            items[i] = new TableListItem<L, R>() { Left = left[i], Right = right[i] };
        }
        return new Enumerator(items.ToList());
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        TableListItem<L, R>[] items = new TableListItem<L, R>[left.Count];
        for (var i = 0; i < left.Count; ++i)
        {
            items[i] = new TableListItem<L, R>() { Left = left[i], Right = right[i] };
        }
        return new Enumerator(items.ToList());
    }

    public struct Enumerator : IEnumerator<TableListItem<L, R>>
    {
        private List<TableListItem<L, R>> _list;
        private int _index;
        private TableListItem<L, R>? _current;

        public Enumerator(List<TableListItem<L, R>> list)
        {
            _list = list;
            _index = 0;
            _current = default;
        }

        public object? Current
        {
            get
            {
                if (_index == 0 || _index == _list.Count + 1)
                {
                    throw new Exception("Can't enumerate");
                }
                return Current;
            }
        }

        TableListItem<L, R> IEnumerator<TableListItem<L, R>>.Current => _current!;

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            List<TableListItem<L, R>> localList = _list;

            if (_index < localList.Count)
            {
                _current = localList[_index];
                _index++;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            _index = 0;
            _current = default;
        }
    }
}
#nullable disable