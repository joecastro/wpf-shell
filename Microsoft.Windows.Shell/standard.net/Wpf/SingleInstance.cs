namespace Standard
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Channels.Ipc;
    using System.Runtime.Serialization.Formatters;
    using System.Threading;

    internal class SingleInstanceEventArgs : EventArgs
    {
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public IList<string> Args { get; internal set; }
    }

    internal static class SingleInstance
    {
        public static event EventHandler<SingleInstanceEventArgs> SingleInstanceActivated;

        private class _IpcRemoteService : MarshalByRefObject
        {
            [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
            public int GetProcessId()
            {
                return System.Diagnostics.Process.GetCurrentProcess().Id;
            }

            /// <summary>Activate the first instance of the application.</summary>
            /// <param name="args">Command line arguemnts to proxy.</param>
            [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
            public void InvokeFirstInstance(IList<string> args)
            {
                if (System.Windows.Application.Current != null && !System.Windows.Application.Current.Dispatcher.HasShutdownStarted)
                {
                    System.Windows.Application.Current.Dispatcher.BeginInvoke((Action<object>)((arg) => SingleInstance._ActivateFirstInstance((IList<string>)arg)), args);
                }
            }

            /// <summary>Overrides the default lease lifetime of 5 minutes so it will ever expire.</summary>
            public override object InitializeLifetimeService()
            {
                return null;
            }
        }

        private const string _RemoteServiceName = "SingleInstanceApplicationService";
        private static Mutex _singleInstanceMutex;
        private static IpcServerChannel _channel;

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool InitializeAsFirstInstance(string applicationName)
        {
            IList<string> commandLineArgs = Environment.GetCommandLineArgs() ?? new string[0];

            // Build a repeatable machine unique name for the channel.
            string appId = applicationName + Environment.UserName;
            string channelName = appId + ":SingleInstanceIPCChannel";

            bool isFirstInstance;
            _singleInstanceMutex = new Mutex(true, appId, out isFirstInstance);
            if (isFirstInstance)
            {
                _CreateRemoteService(channelName);
            }
            else
            {
                _SignalFirstInstance(channelName, commandLineArgs);
            }

            return isFirstInstance;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void Cleanup()
        {
            Utility.SafeDispose(ref _singleInstanceMutex);

            if (_channel != null)
            {
                ChannelServices.UnregisterChannel(_channel);
                _channel = null;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private static void _CreateRemoteService(string channelName)
        {
            _channel = new IpcServerChannel(
                new Dictionary<string, string>
                {
                    { "name", channelName },
                    { "portName", channelName },
                    { "exclusiveAddressUse", "false" },
                },
                new BinaryServerFormatterSinkProvider { TypeFilterLevel = TypeFilterLevel.Full });

            ChannelServices.RegisterChannel(_channel, true);
            RemotingServices.Marshal(new _IpcRemoteService(), _RemoteServiceName);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private static void _SignalFirstInstance(string channelName, IList<string> args)
        {
            var secondInstanceChannel = new IpcClientChannel();
            ChannelServices.RegisterChannel(secondInstanceChannel, true);

            string remotingServiceUrl = "ipc://" + channelName + "/" + _RemoteServiceName;

            // Obtain a reference to the remoting service exposed by the first instance of the application
            var firstInstanceRemoteServiceReference = (_IpcRemoteService)RemotingServices.Connect(typeof(_IpcRemoteService), remotingServiceUrl);

            // Pass along the current arguments to the first instance if it's up and accepting requests.
            if (firstInstanceRemoteServiceReference != null)
            {
                // Allow the first instance to give itself user focus.
                // This could be done with ASFW_ANY if the IPC call is expensive.
                int procId = firstInstanceRemoteServiceReference.GetProcessId();
                NativeMethods.AllowSetForegroundWindow(procId);

                firstInstanceRemoteServiceReference.InvokeFirstInstance(args);
            }
        }

        private static void _ActivateFirstInstance(IList<string> args)
        {
            if (System.Windows.Application.Current != null && !System.Windows.Application.Current.Dispatcher.HasShutdownStarted)
            {
                var handler = SingleInstanceActivated;
                if (handler != null)
                {
                    handler(System.Windows.Application.Current, new SingleInstanceEventArgs { Args = args });
                }
            }
        }
    }
}
