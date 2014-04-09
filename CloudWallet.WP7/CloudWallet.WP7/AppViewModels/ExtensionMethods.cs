using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CloudWallet
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Converts bool value to visibility (true for visible, false for collapsed)
        /// </summary>
        /// <param name="val">boolean value</param>
        /// <returns>Visibility</returns>
        public static Visibility ToVisibility(this bool val)
        {
            return (val ? Visibility.Visible : Visibility.Collapsed);
        }
    }
}
