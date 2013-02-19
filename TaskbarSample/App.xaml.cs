/**************************************************************************\
    Copyright Microsoft Corporation. All Rights Reserved.
\**************************************************************************/

namespace TaskbarSample
{
    using System.Text;
    using System.Windows;
    using Microsoft.Windows.Shell;

    public partial class App : Application
    {
        private void OnJumpItemsRejected(object sender, JumpItemsRejectedEventArgs e)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0} Jump Items Rejected:\n", e.RejectionReasons.Count);
            for (int i = 0; i < e.RejectionReasons.Count; ++i)
            {
                sb.AppendFormat("Reason: {0}\tItem: {1}\n", e.RejectionReasons[i], e.RejectedItems[i]);
            }

            MessageBox.Show(sb.ToString());
        }

        private void OnJumpItemsRemoved(object sender, JumpItemsRemovedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} Jump Items Removed by the user:\n", e.RemovedItems.Count);
            for (int i = 0; i < e.RemovedItems.Count; ++i)
            {
                sb.AppendFormat("{0}\n", e.RemovedItems[i]);
            }

            MessageBox.Show(sb.ToString());
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length > 0)
            {
                MessageBox.Show(e.Args[0], "Application Startup Argument");
            }
        }
    }
}
