using System;
using System.Collections.Generic;
namespace Commune.Basis
{
	/// <summary>
	/// Summary description for SynchronizedCollection.
	/// </summary>
	public abstract class SynchronizedCollection<T>:
		System.Collections.ICollection,
    System.Collections.IList,
    IEnumerable<T>
	{
		#region ICollection Members
		List<T> _InnerList = new List<T>();
		protected List<T> InnerList
		{
			get 
			{
				Synchronize(_InnerList);
				return _InnerList;
			}
			set 
			{
				InnerList = value;
			}
		}
		protected abstract void Synchronize(List<T> items);
		public bool IsSynchronized
		{
			get	{ return false;	}
		}

		public int Count
		{
			get { return InnerList.Count; }
		}

		public void CopyTo(Array array, int index)
		{
      for (int i = 0; i < InnerList.Count; ++i)
        array.SetValue(InnerList[i], i + index);
    }

    object sync = new object();
    public object SyncRoot
    {
			get { return sync;}
		}

		#endregion

		#region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
			return InnerList.GetEnumerator();
		}

		#endregion

    public T this[int index]
    {
      get
      {
        return InnerList[index];
      }
      set
      {
        InnerList[index] = value;
      }
    }
    public int IndexOf(T item)
    {
      return InnerList.IndexOf(item);
    }


    #region IEnumerable<T> Members

    public IEnumerator<T> GetEnumerator()
    {
      return InnerList.GetEnumerator();
    }

#endregion

    #region IList Members

    public int Add(object value)
    {
      return 0;
    }

    public void Clear()
    {
    }

    public bool Contains(object value)
    {
      return InnerList.Contains((T)value);
    }

    int System.Collections.IList.IndexOf(object value)
    {
      return InnerList.IndexOf((T)value);
    }

    public void Insert(int index, object value)
    {
    }
    public bool IsFixedSize
    {
      get { return false; }
    }

    public bool IsReadOnly
    {
      get { return true; }
    }


    public void Remove(object value)
    {
    }

    public void RemoveAt(int index)
    {
    }
    object System.Collections.IList.this[int index]
    {
      get
      {
        return InnerList[index];
      }

      set
      {
        InnerList[index] = (T)value;
      }
    }


#endregion
  }


}
