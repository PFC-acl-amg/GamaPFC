using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Core.Controls
{
    public class UnselectableListBox : ListBox
    {
        public UnselectableListBox() : base()
        {
            SelectionChanged += new SelectionChangedEventHandler((sender, e) =>
            {
                if (e.AddedItems.Count > 0)
                {
                    var last = e.AddedItems[0];
                    foreach (var item in new ArrayList(SelectedItems))
                        if (item != last) SelectedItems.Remove(item);
                }
            });
        }

        static UnselectableListBox()
        {
            SelectionModeProperty.OverrideMetadata(typeof(UnselectableListBox),
                new FrameworkPropertyMetadata(SelectionMode.Multiple));
        }
    }
}
