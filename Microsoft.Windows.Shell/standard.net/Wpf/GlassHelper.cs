namespace Standard
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media;

    internal class GlassHelper
    {
        // Test Notes:
        // Things to manually verify when making changes to this class.
        // * Do modified windows look correct in non-composited themes?
        // * Does changing the theme back and forth leave the window in a visually ugly state?
        //     * Does it matter which theme was used first?
        // * Does glass extension work properly in high-dpi?
        // * Which of SetWindowThemeAttribute and ExtendGlassFrame are set first shouldn't matter.
        //   The hooks injected by one should not block the hooks of the other.
        // * Do captions and icons always show up when composition is disabled?
        //
        // There are not automated unit tests for this class ( Boo!!! :( )
        // Be careful not to break things...

        private static readonly Dictionary<IntPtr, HwndSourceHook> _extendedWindows = new Dictionary<IntPtr, HwndSourceHook>();

        // TODO:
        // Verify that this really is sufficient.  There are DWMWINDOWATTRIBUTEs as well, so this may
        // be able to be turned off on a per-HWND basis, but I never see comments about that online...
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool IsCompositionEnabled
        {
            get
            {
                if (!Utility.IsOSVistaOrNewer)
                {
                    return false;
                }

                return NativeMethods.DwmIsCompositionEnabled();
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool ExtendGlassFrameComplete(Window window)
        {
            return ExtendGlassFrame(window, new Thickness(-1));
        }

        /// <summary>
        /// Extends the glass frame of a window.  Only works on operating systems that support composition.
        /// </summary>
        /// <param name="window">The window to modify.</param>
        /// <param name="margin">The margins of the new frame.</param>
        /// <returns>Whether the frame was successfully extended.</returns>
        /// <remarks>
        /// This function adds hooks to the Window to respond to changes to whether composition is enabled.
        /// </remarks>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool ExtendGlassFrame(Window window, Thickness margin)
        {
            Verify.IsNotNull(window, "window");

            window.VerifyAccess();

            IntPtr hwnd = new WindowInteropHelper(window).Handle;

            if (_extendedWindows.ContainsKey(hwnd))
            {
                // The hook into the HWND's WndProc has the original margin cached.
                // Don't want to support dynamically adjusting that unless there's a need.
                throw new InvalidOperationException("Multiple calls to this function for the same Window are not supported.");
            }

            return _ExtendGlassFrameInternal(window, margin);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private static bool _ExtendGlassFrameInternal(Window window, Thickness margin)
        {
            Assert.IsNotNull(window);
            Assert.IsTrue(window.CheckAccess());

            // Expect that this might be called on OSes other than Vista.
            if (!Utility.IsOSVistaOrNewer)
            {
                // Not an error.  Just not on Vista so we're not going to get glass.
                return false;
            }

            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            if (IntPtr.Zero == hwnd)
            {
                throw new InvalidOperationException("Window must be shown before extending glass.");
            }

            HwndSource hwndSource = HwndSource.FromHwnd(hwnd);

            bool isGlassEnabled = NativeMethods.DwmIsCompositionEnabled();

            if (!isGlassEnabled)
            {
                window.Background = SystemColors.WindowBrush;
                hwndSource.CompositionTarget.BackgroundColor = SystemColors.WindowColor;
            }
            else
            {
                // Apply the transparent background to both the Window and the HWND
                window.Background = Brushes.Transparent;
                hwndSource.CompositionTarget.BackgroundColor = Colors.Transparent;

                // Thickness is going to be DIPs, need to convert to system coordinates.
                Point deviceTopLeft = DpiHelper.LogicalPixelsToDevice(new Point(margin.Left, margin.Top));
                Point deviceBottomRight = DpiHelper.LogicalPixelsToDevice(new Point(margin.Right, margin.Bottom));

                var dwmMargin = new MARGINS
                {
                    // err on the side of pushing in glass an extra pixel.
                    cxLeftWidth = (int)Math.Ceiling(deviceTopLeft.X),
                    cxRightWidth = (int)Math.Ceiling(deviceBottomRight.X),
                    cyTopHeight = (int)Math.Ceiling(deviceTopLeft.Y),
                    cyBottomHeight = (int)Math.Ceiling(deviceBottomRight.Y),
                };

                NativeMethods.DwmExtendFrameIntoClientArea(hwnd, ref dwmMargin);
            }

            // Even if glass isn't currently enabled, add the hook so we can appropriately respond
            // if that changes.

            bool addHook = !_extendedWindows.ContainsKey(hwnd);

            if (addHook)
            {
                HwndSourceHook hook = delegate(IntPtr innerHwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
                {
                    if (WM.DWMCOMPOSITIONCHANGED == (WM)msg)
                    {
                        _ExtendGlassFrameInternal(window, margin);
                        handled = false;
                    }
                    return IntPtr.Zero;
                };

                _extendedWindows.Add(hwnd, hook);
                hwndSource.AddHook(hook);
                window.Closing += _OnExtendedWindowClosing;
            }

            return isGlassEnabled;
        }

        /// <summary>
        /// Handler for the Closing event on a Window with an extended glass frame.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// When a Window with an extended glass frame closes, removes any local references to it.
        /// </remarks>
        // BUGBUG: Doesn't handle if the Closing gets canceled.
        static void _OnExtendedWindowClosing(object sender, CancelEventArgs e)
        {
            var window = sender as Window;
            Assert.IsNotNull(window);

            IntPtr hwnd = new WindowInteropHelper(window).Handle;

            // We use the Closing rather than the Closed event to ensure that we can get this value.
            Assert.AreNotEqual(IntPtr.Zero, hwnd);

            HwndSource hwndSource = HwndSource.FromHwnd(hwnd);

            Assert.IsTrue(_extendedWindows.ContainsKey(hwnd));

            hwndSource.RemoveHook(_extendedWindows[hwnd]);
            _extendedWindows.Remove(hwnd);

            window.Closing -= _OnExtendedWindowClosing;
        }

        private static readonly Dictionary<IntPtr, HwndSourceHook> _attributedWindows = new Dictionary<IntPtr, HwndSourceHook>();

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool SetWindowThemeAttribute(Window window, bool showCaption, bool showIcon)
        {
            Verify.IsNotNull(window, "window");

            window.VerifyAccess();

            IntPtr hwnd = new WindowInteropHelper(window).Handle;

            if (_attributedWindows.ContainsKey(hwnd))
            {
                // The hook into the HWND's WndProc has the original settings cached.
                // Don't want to support dynamically adjusting that unless there's a need.
                throw new InvalidOperationException("Multiple calls to this function for the same Window are not supported.");
            }

            return _SetWindowThemeAttribute(window, showCaption, showIcon);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private static bool _SetWindowThemeAttribute(Window window, bool showCaption, bool showIcon)
        {
            bool isGlassEnabled;

            Assert.IsNotNull(window);
            Assert.IsTrue(window.CheckAccess());

            // This only is expected to work if Aero glass is enabled.
            try
            {
                isGlassEnabled = NativeMethods.DwmIsCompositionEnabled();
            }
            catch (DllNotFoundException)
            {
                // Not an error.  Just not on Vista so we're not going to get glass.
                return false;
            }

            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            if (IntPtr.Zero == hwnd)
            {
                throw new InvalidOperationException("Window must be shown before we can modify attributes.");
            }

            var options = new WTA_OPTIONS
            {
                dwMask = (WTNCA.NODRAWCAPTION | WTNCA.NODRAWICON)
            };
            if (isGlassEnabled)
            {
                if (!showCaption)
                {
                    options.dwFlags |= WTNCA.NODRAWCAPTION;
                }
                if (!showIcon)
                {
                    options.dwFlags |= WTNCA.NODRAWICON;
                }
            }

            NativeMethods.SetWindowThemeAttribute(hwnd, WINDOWTHEMEATTRIBUTETYPE.WTA_NONCLIENT, ref options, WTA_OPTIONS.Size);

            bool addHook = !_attributedWindows.ContainsKey(hwnd);

            if (addHook)
            {
                HwndSourceHook hook = delegate(IntPtr unusedHwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
                {
                    if (WM.DWMCOMPOSITIONCHANGED == (WM)msg)
                    {
                        _SetWindowThemeAttribute(window, showCaption, showIcon);
                        handled = false;
                    }
                    return IntPtr.Zero;
                };

                _attributedWindows.Add(hwnd, hook);
                HwndSource.FromHwnd(hwnd).AddHook(hook);
                window.Closing += _OnAttributedWindowClosing;
            }

            return isGlassEnabled;
        }

        static void _OnAttributedWindowClosing(object sender, CancelEventArgs e)
        {
            var window = sender as Window;
            Assert.IsNotNull(window);

            IntPtr hwnd = new WindowInteropHelper(window).Handle;

            // We use the Closing rather than the Closed event to ensure that we can get this value.
            Assert.AreNotEqual(IntPtr.Zero, hwnd);

            HwndSource hwndSource = HwndSource.FromHwnd(hwnd);

            Assert.IsTrue(_attributedWindows.ContainsKey(hwnd));

            hwndSource.RemoveHook(_attributedWindows[hwnd]);
            _attributedWindows.Remove(hwnd);

            window.Closing -= _OnExtendedWindowClosing;
        }

    }
}
