using System;
using System.Collections.Generic;

namespace PongOut
{
    /// <summary>
    /// A sorted list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SortedVector<T> where T: IComparable<T> 
    {
        List<T> list;
        bool allowDuplicates;

        public SortedVector(bool allowDuplicates = true){
            this.allowDuplicates = allowDuplicates;
            list = new List<T>();
        }
        public int Length => list.Count; 
        
        public T this[int index] {
            get
            {
                return list[index];
            }
        }

        public void Add(T obj)
        {
            int index = Find(obj);
            if (index >= 0 && !allowDuplicates)
                throw new ArgumentException("Duplicate item");
            int insertAt = index;
            if(insertAt < 0)
                insertAt = ~index;

            list.Insert(insertAt, obj);
        }

        public int Find(T obj)
        {
            return list.BinarySearch(obj);
        }

        public bool Remove(T obj)
        {
            int idx = Find(obj);
            if (idx < 0)
                return false;

            list.RemoveAt(idx);
            return true;
        }
    }
}
