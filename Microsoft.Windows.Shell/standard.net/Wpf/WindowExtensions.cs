
namespace Standard
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media;

    /// <summary>
    /// Attached properties for WPF Windows.
    /// </summary>
    internal sealed class WindowExtensions
    {
        private IntPtr _hwnd = IntPtr.Zero;
        private Window _window = null;

        private event Action WindowSourceInitialized;

        private WindowExtensions(Window window)
        {
            Assert.IsNotNull(window);
            _window = window;
            _hwnd = new WindowInteropHelper(window).Handle;

            if (_hwnd == IntPtr.Zero)
            {
                _window.SourceInitialized += _OnWindowSourceInitialized;
            }
        }

        private void _OnWindowSourceInitialized(object sender, EventArgs e)
        {
            Assert.AreEqual(sender, _window);

            _window.SourceInitialized -= _OnWindowSourceInitialized;

            _hwnd = new WindowInteropHelper(_window).Handle;
            Assert.IsNotDefault(_hwnd);

            Action handler = WindowSourceInitialized;
            if (handler != null)
            {
                handler();
            }
        }

        private static WindowExtensions _EnsureAttachedExtensions(Window window)
        {
            Assert.IsNotNull(window);

            var ext = (WindowExtensions)window.GetValue(WindowExtensionsProperty);
            if (ext == null)
            {
                ext = new WindowExtensions(window);
                window.SetValue(WindowExtensionsProperty, ext);
            }

            return ext;
        }

        private static readonly DependencyProperty WindowExtensionsProperty = DependencyProperty.RegisterAttached(
            "WindowExtensions",
            typeof(WindowExtensions),
            typeof(WindowExtensions),
            new PropertyMetadata(null));

        // Not bothering with CLR attached property getter/setter since this is a private dependency property.


        public static readonly DependencyProperty HwndBackgroundBrushProperty = DependencyProperty.RegisterAttached(
            "HwndBackgroundBrush", 
            typeof(SolidColorBrush),
            typeof(WindowExtensions),
            new PropertyMetadata(
                Brushes.Pink,
                (d,e) => _OnHwndBackgroundBrushChanged(d)));

        public static SolidColorBrush GetHwndBackgroundBrush(FrameworkElement window)
        {
            Verify.IsNotNull(window, "window");
            return (SolidColorBrush)window.GetValue(HwndBackgroundBrushProperty);
        }

        public static void SetHwndBackgroundBrush(FrameworkElement window, SolidColorBrush value)
        {
			if (!(window is Window))
			{
				return;
			}
            Verify.IsNotNull(window, "window");
            window.SetValue(HwndBackgroundBrushProperty, value);
        }

        private static void _OnHwndBackgroundBrushChanged(DependencyObject d)
        {
            if (DesignerProperties.GetIsInDesignMode(d))
            {
                return;
            }

            var window = d as Window;
            Verify.IsNotNull(window, "window");

            WindowExtensions ext = _EnsureAttachedExtensions(window);

            if (ext._hwnd == IntPtr.Zero)
            {
                ext.WindowSourceInitialized += () => _OnHwndBackgroundBrushChanged(window);
                return;
            }

			SolidColorBrush backgroundBrush = (SolidColorBrush)window.GetValue(HwndBackgroundBrushProperty);
            //if (backgroundBrush == null)
            //{
                // Nothing to change.
            //    return;
            //}

            Color backgroundColor = backgroundBrush.Color;

            // Not really handling errors here, but they shouldn't matter... Might leak an HBRUSH.

            IntPtr hBrush = NativeMethods.CreateSolidBrush(Utility.RGB(backgroundColor));

            // Note that setting this doesn't necessarily repaint the window right away.
            // Since the WPF content should cover the HWND background this doesn't matter.
            // The new background will get repainted when the window is resized.
            IntPtr hBrushOld = NativeMethods.SetClassLongPtr(ext._hwnd, GCLP.HBRBACKGROUND, hBrush);

            if (IntPtr.Zero != hBrushOld)
            {
                NativeMethods.DeleteObject(hBrushOld);
            }
        }
    }
}
