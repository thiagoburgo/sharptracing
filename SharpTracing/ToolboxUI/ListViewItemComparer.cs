using System;
using System.Collections;
using System.Windows.Forms;

namespace TooboxUI.Components {
    // This class is an implementation of the 'IComparer' interface.
    public class ListViewItemComparer : IComparer {
        // Specifies the column to be sorted
        private int ColumnToSort;
        // Specifies the order in which to sort (i.e. 'Ascending').
        // Case insensitive comparer object
        private readonly CaseInsensitiveComparer ObjectCompare;
        private SortOrder OrderOfSort;
        // Class constructor, initializes various elements
        public ListViewItemComparer() {
            // Initialize the column to '0'
            this.ColumnToSort = 0;
            // Initialize the sort order to 'none'
            this.OrderOfSort = SortOrder.None;
            // Initialize the CaseInsensitiveComparer object
            this.ObjectCompare = new CaseInsensitiveComparer();
        }

        public int SortColumn {
            set { this.ColumnToSort = value; }
            get { return this.ColumnToSort; }
        }

        // Gets or sets the order of sorting to apply
        // (for example, 'Ascending' or 'Descending').
        public SortOrder Order {
            set { this.OrderOfSort = value; }
            get { return this.OrderOfSort; }
        }

        // This method is inherited from the IComparer interface.
        // 
        // x: First object to be compared
        // y: Second object to be compared
        //
        // The result of the comparison. "0" if equal, 
        // negative if 'x' is less than 'y' and 
        // positive if 'x' is greater than 'y'

        #region IComparer Members

        public int Compare(object x, object y) {
            int compareResult;
            ListViewItem listviewX, listviewY;
            // Cast the objects to be compared to ListViewItem objects
            listviewX = (ListViewItem) x;
            listviewY = (ListViewItem) y;
            // Determine the type being compared
            try {
                compareResult = this.CompareDateTime(listviewX, listviewY);
            } catch {
                try {
                    compareResult = this.CompareDecimal(listviewX, listviewY);
                } catch {
                    compareResult = this.CompareString(listviewX, listviewY);
                }
            }
            // Simple String Compare
            // compareResult = String.Compare (
            // 	listviewX.SubItems[ColumnToSort].Text,
            // 	listviewY.SubItems[ColumnToSort].Text
            // );
            // Calculate correct return value based on object comparison
            if (this.OrderOfSort == SortOrder.Ascending) {
                // Ascending sort is selected, return normal result of compare operation
                return compareResult;
            } else if (this.OrderOfSort == SortOrder.Descending) {
                // Descending sort is selected, return negative result of compare operation
                return (-compareResult);
            } else {
                // Return '0' to indicate they are equal
                return 0;
            }
        }

        #endregion

        public int CompareDateTime(ListViewItem listviewX, ListViewItem listviewY) {
            // Parse the two objects passed as a parameter as a DateTime.
            DateTime firstDate = DateTime.Parse(listviewX.SubItems[this.ColumnToSort].Text);
            DateTime secondDate = DateTime.Parse(listviewY.SubItems[this.ColumnToSort].Text);
            // Compare the two dates.
            int compareResult = DateTime.Compare(firstDate, secondDate);
            return compareResult;
        }

        public int CompareDecimal(ListViewItem listviewX, ListViewItem listviewY) {
            // Parse the two objects passed as a parameter as a DateTime.
            Decimal firstValue = Decimal.Parse(listviewX.SubItems[this.ColumnToSort].Text);
            Decimal secondValue = Decimal.Parse(listviewY.SubItems[this.ColumnToSort].Text);
            // Compare the two dates.
            int compareResult = Decimal.Compare(firstValue, secondValue);
            return compareResult;
        }

        public int CompareString(ListViewItem listviewX, ListViewItem listviewY) {
            // Case Insensitive Compare
            int compareResult = this.ObjectCompare.Compare(listviewX.SubItems[this.ColumnToSort].Text,
                                                           listviewY.SubItems[this.ColumnToSort].Text);
            return compareResult;
        }

        // Gets or sets the number of the column to which to
        // apply the sorting operation (Defaults to '0').
    }
}