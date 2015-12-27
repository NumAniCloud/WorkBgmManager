//------------------------------------------------------------------------------
// <copyright file="BgmForWorkWindow.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace WorkingBgmManager
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("7d858416-57df-4aa9-9ae5-ce8848c7e8a7")]
    public class BgmForWorkWindow : ToolWindowPane
    {
        private BgmForWorkViewModel viewModel { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BgmForWorkWindow"/> class.
        /// </summary>
        public BgmForWorkWindow() : base(null)
        {
            this.Caption = "BgmForWorkWindow";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            var control = new BgmForWorkWindowControl();
            this.Content = control;

            viewModel = control.ViewModel;
        }

        protected override void OnClose()
        {
            base.OnClose();
            viewModel?.Dispose();
        }
    }
}
