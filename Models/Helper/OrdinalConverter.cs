using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Ark.Models.Helper
{
    public class OrdinalConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            ListBoxItem lbi = value as ListBoxItem;
            int ordinal = 0;

            if (lbi != null)
            {
                ListBox lb = ItemsControl.ItemsControlFromItemContainer(lbi) as ListView;
                ordinal = lb.ItemContainerGenerator.IndexFromContainer(lbi) + 1;
            }

            return ordinal;

        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // This converter does not provide conversion back from ordinal position to list view item
            throw new System.InvalidOperationException();
        }
    }
}
