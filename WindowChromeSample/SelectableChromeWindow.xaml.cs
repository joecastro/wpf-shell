/**************************************************************************\
    Copyright Microsoft Corporation. All Rights Reserved.
\**************************************************************************/

namespace WindowChromeSample
{
    using System.Windows;
    using System.Windows.Input;
    using Microsoft.Windows.Shell;

    public partial class SelectableChromeWindow
    {
        public SelectableChromeWindow()
        {
            InitializeComponent();
        }

        private void _OnStandardChromeClicked(object sender, RoutedEventArgs e)
        {
            this.Style = null;
        }

        private void _OnGradientChromeClicked(object sender, RoutedEventArgs e)
        {
            var style = (Style)Resources["GradientStyle"];
            this.Style = style;
        }

        private void _OnGlassyChromeClicked(object sender, RoutedEventArgs e)
        {
            var style = (Style)Resources["GlassStyle"];
            this.Style = style;
        }

        private void _OnSystemCommandCloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow((Window)e.Parameter);
        }
    }
}
