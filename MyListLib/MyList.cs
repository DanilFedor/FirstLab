using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyListLib
{
    public class MyList<T> : IEnumerable<T> where T : IComparable
    {
        public delegate void ListHandler(object sender, ListEventArgs<T> e);
        public event ListHandler Notify;
        public int Size { get; private set; } = 0;

        private Item<T> Head;
        private Item<T> Tail;

        public MyList() { }
        public MyList(T data) => SetHeadItem(data);

        private void SetHeadItem(T data)
        {
            Head = Tail = new Item<T>(data);
            Size = 1;
        }

        public void Add(T data)
        {
            if (Size == 0)
            {
                SetHeadItem(data);
                return;
            }

            var temp = new Item<T>(data);
            temp.Previous = Tail;
            Tail.Next = temp;
            Tail = temp;
            Size++;
        }

        public bool Contains(T data)
        {
            foreach (var item in this)
            {
                if (item.CompareTo(data) == 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Clear all collection
        /// </summary>
        public void Clear()
        {
            Head = Tail = null;
            Size = 0;
        }

        /// <summary>
        /// Removes the first occurrence of the specified object from the collection
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Remove(T data)
        {
            if (Size == 0) throw new NullReferenceException();

            var temp = Head;

            for (int i = 0; i < Size; i++)
            {
                if (temp.Data.CompareTo(data) == 0)
                {
                    if (Size == 1)
                    {
                        Clear();
                    }
                    else if (temp == Head)
                    {
                        Head = Head.Next;
                        Head.Previous = null;
                        Size--;
                    }
                    else if (temp == Tail)
                    {
                        DeleteLast();
                    }
                    else
                    {
                        temp.Previous.Next = temp.Next;
                        Size--;
                    }

                    Notify?.Invoke(this, new ListEventArgs<T>("Delete element", data));
                    return true;
                }

                temp = temp.Next;
            }
            return false;
        }

        public void DeleteLast()
        {
            if (Size == 1)
            {
                Clear();
                return;
            }

            var result = Tail.Data;
            Tail = Tail.Previous;
            Tail.Next = null;
            Size--;
            Notify?.Invoke(this, new ListEventArgs<T>("Delete last element", result));
        }

        public T[] ToArray()
        {
            if (Size == 0) throw new InvalidOperationException("The list is empty");

            var arr = new T[Size];
            var temp = Head;

            for (int i = 0; i < Size; i++)
            {
                arr[i] = temp.Data;
                temp = temp.Next;
            }

            return arr;
        }

        public bool IsEmpty() => Size == 0;

        public DequeEnum<T> GetEnumerator()
        {
            return new DequeEnum<T>(Head);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return (IEnumerator<T>)GetEnumerator(); ;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (this as IEnumerable<T>).GetEnumerator();
        }
    }

    public class DequeEnum<T> : IEnumerator<T>
    {
        public Item<T> head;

        public Item<T> position = new Item<T>();

        public DequeEnum(Item<T> item)
        {
            head = item;
            position.Next = head;
        }

        public bool MoveNext()
        {
            position = position.Next;
            return position != null;
        }

        public void Reset()
        {
            position.Next = head;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public T Current
        {
            get
            {
                try
                {
                    return position.Data;
                }
                catch (NullReferenceException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        void IDisposable.Dispose() { }
    }

    public class ListEventArgs<T>
    {
        public string Message { get; }
        public T Data { get; }

        public ListEventArgs(string mes, T data)
        {
            Message = mes;
            Data = data;
        }
    }
}
