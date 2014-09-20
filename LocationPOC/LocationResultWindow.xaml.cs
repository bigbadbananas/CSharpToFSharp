using DataContracts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace LocationPOC
{
    using LExpression = System.Linq.Expressions;
    /// <summary>
    /// Interaction logic for LocationResultWindow.xaml
    /// </summary>
    public partial class LocationResultWindow : Window
    {
        public LocationResultWindow() : this(new LocationFilterMessage()) { }

        public LocationResultWindow(LocationFilterMessage filter)
        {
            InitializeComponent();

            Type filterType = filter.GetType();
            var props = filterType.GetProperties().Where(p => p.GetValue(filter, null) != null);

            IEnumerable<string> data = props.Select(p => string.Format("{0}: {1}", p.Name, GetStringGayAsBalls(p.GetValue(filter))));
            
            this.ResultsText.Text = string.Join("\n\r", data);
        }


        private static string GetStringGayAsBalls<T>(T obj)
        {
            if (obj is IEnumerable<T> && !(obj is string))
            {
                return string.Join(",", obj as IEnumerable<T>);
            }

            return obj.ToString();
        }


        private static string GetStringOther<T>(IEnumerable<T> obj)
        {
            if (obj is string)
            {
                return obj.ToString();
            }

            return string.Join(",", obj);
            
        }
    }
}
