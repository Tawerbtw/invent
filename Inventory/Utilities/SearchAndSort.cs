using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;

namespace Inventory.Utilities
{
    public class SearchAndSort
    {
        public static void SearchListView<T>(ListView listView, T model, string searchText)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(listView.ItemsSource);
            view.Filter = item =>
            {
                if (item is T yourItem)
                {
                    if (string.IsNullOrWhiteSpace(searchText))
                        return true;

                    string lowerSearchText = searchText.ToLower();
                    Type itemType = yourItem.GetType();
                    PropertyInfo[] properties = itemType.GetProperties();

                    foreach (PropertyInfo property in properties)
                    {
                        if (property.PropertyType == typeof(string))
                        {
                            string propertyValue = (string)property.GetValue(yourItem, null);
                            if (!string.IsNullOrEmpty(propertyValue) && propertyValue.ToLower().Contains(lowerSearchText))
                                return true;
                        }
                    }

                    return false;
                }
                return false;
            };
        }

        public static void SortListView(ListView listView, bool ascending)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(listView.ItemsSource);
            view.SortDescriptions.Clear();
            if (ascending)
                view.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Ascending));
            else
                view.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Descending));
        }
    }
}
