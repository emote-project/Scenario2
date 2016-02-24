using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EuxHelpers
{
    /// <summary>
    /// Abstract class with all the methods for notifying properties changes.
    /// This can be inherit by all those classess used as DataContext
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged!=null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
#if DEBUG
            System.Reflection.PropertyInfo property = this.GetType().GetProperty(propertyName);
            var value = "still null";
            if (property!=null) value = property.GetValue(this, null).ToString();
            Console.WriteLine(propertyName + ": " + value);
#endif
        }
    }
}
