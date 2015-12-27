//------------------------------------------------------------------------------
// <copyright file="BgmForWorkWindowControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace WorkingBgmManager
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for BgmForWorkWindowControl.
    /// </summary>
    public partial class BgmForWorkWindowControl : UserControl
    {
        public BgmForWorkViewModel ViewModel { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BgmForWorkWindowControl"/> class.
        /// </summary>
        public BgmForWorkWindowControl()
        {
            this.InitializeComponent();

            ViewModel = new BgmForWorkViewModel();
            ViewModel.Initialize();
            DataContext = ViewModel;
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()),
                "BgmForWorkWindow");
        }
    }
}