using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

public class SortableBindingList<T> : BindingList<T>
{
    private bool isSorted;
    private PropertyDescriptor sortProperty;
    private ListSortDirection sortDirection;

    public SortableBindingList() : base() { }

    public SortableBindingList(IList<T> list) : base(list) { }

    protected override bool SupportsSortingCore => true;
    protected override bool IsSortedCore => isSorted;

    protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
    {
        if (prop.PropertyType.GetInterface("IComparable") != null || typeof(IComparable).IsAssignableFrom(prop.PropertyType))
        {
            List<T> items = this.Items as List<T>;

            if (items != null)
            {
                // Obsługa zagnieżdżonych właściwości
                Func<T, object> keySelector = item =>
                {
                    object value = item;
                    foreach (var part in prop.Name.Split('.'))
                    {
                        if (value == null) break;
                        PropertyInfo propInfo = value.GetType().GetProperty(part);
                        value = propInfo?.GetValue(value, null);
                    }
                    return value;
                };

                if (direction == ListSortDirection.Ascending)
                {
                    items.Sort((x, y) => ComparePropertyValues(keySelector(x), keySelector(y)));
                }
                else
                {
                    items.Sort((x, y) => ComparePropertyValues(keySelector(y), keySelector(x)));
                }
            }

            sortProperty = prop;
            sortDirection = direction;
            isSorted = true;

            // Raise the ListChanged event so bound controls refresh their data.
            this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }
        else
        {
            throw new NotSupportedException("Cannot sort by " + prop.Name + ". This " + prop.PropertyType.ToString() + " does not implement IComparable");
        }
    }

    private int ComparePropertyValues(object value1, object value2)
    {
        if (value1 == null && value2 == null) return 0;
        if (value1 == null) return -1;
        if (value2 == null) return 1;

        if (value1 is IComparable comparable1 && value2 is IComparable comparable2)
        {
            return comparable1.CompareTo(comparable2);
        }
        throw new ArgumentException("Values do not implement IComparable");
    }

    protected override void RemoveSortCore()
    {
        isSorted = false;
    }

    protected override bool SupportsSearchingCore => true;

    protected override int FindCore(PropertyDescriptor prop, object key)
    {
        for (int i = 0; i < this.Count; i++)
        {
            T item = this[i];
            if (prop.GetValue(item).Equals(key))
            {
                return i;
            }
        }
        return -1; // Not found
    }

    public int Find(PropertyDescriptor prop, object key)
    {
        return FindCore(prop, key);
    }
}
