
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Scope = "member", Target = "Standard.WebBrowserEvents+_EventSink.#System.Reflection.IReflect.InvokeMember(System.String,System.Reflection.BindingFlags,System.Reflection.Binder,System.Object,System.Object[],System.Reflection.ParameterModifier[],System.Globalization.CultureInfo,System.String[])")]

// Interface declarations for MSHTML objects.
namespace Standard
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Windows.Controls;

    [
        ComImport, 
        Guid(IID.WebBrowserEvents2), 
        InterfaceType(ComInterfaceType.InterfaceIsIDispatch),
        TypeLibType(TypeLibTypeFlags.FHidden)
    ]
    internal interface DWebBrowserEvents2
    {
        [DispId(102)]
        void StatusTextChange([In] string text);
        [DispId(104)]
        void DownloadComplete();
        [DispId(105)]
        void CommandStateChange([In] long command, [In] bool enable);
        [DispId(106)]
        void DownloadBegin();
        [DispId(108)]
        void ProgressChange([In] int progress, [In] int progressMax);
        [DispId(112)]
        void PropertyChange([In] string szProperty);
        [DispId(113)]
        void TitleChange([In] string text);
        [DispId(225)]
        void PrintTemplateInstantiation([In, MarshalAs(UnmanagedType.IDispatch)] object pDisp);
        [DispId(226)]
        void PrintTemplateTeardown([In, MarshalAs(UnmanagedType.IDispatch)] object pDisp);
        [DispId(227)]
        void UpdatePageStatus([In, MarshalAs(UnmanagedType.IDispatch)] object pDisp, [In] ref object nPage, [In] ref object fDone);
        [DispId(250)]
        void BeforeNavigate2(
            [In, MarshalAs(UnmanagedType.IDispatch)] object pDisp,
            [In] ref object URL,
            [In] ref object flags,
            [In] ref object targetFrameName, 
            [In] ref object postData,
            [In] ref object headers,
            [In, Out] ref bool cancel);
        [DispId(251)]
        void NewWindow2([In, Out, MarshalAs(UnmanagedType.IDispatch)] ref object pDisp, [In, Out] ref bool cancel);
        [DispId(252)]
        void NavigateComplete2([In, MarshalAs(UnmanagedType.IDispatch)] object pDisp, [In] ref object URL);
        [DispId(253)]
        void OnQuit();
        [DispId(254)]
        void OnVisible([In] bool visible);
        [DispId(255)]
        void OnToolBar([In] bool toolBar);
        [DispId(256)]
        void OnMenuBar([In] bool menuBar);
        [DispId(257)]
        void OnStatusBar([In] bool statusBar);
        [DispId(258)]
        void OnFullScreen([In] bool fullScreen);
        [DispId(259)]
        void DocumentComplete([In, MarshalAs(UnmanagedType.IDispatch)] object pDisp, [In] ref object URL);
        [DispId(260)]
        void OnTheaterMode([In] bool theaterMode);
        [DispId(262)]
        void WindowSetResizable([In] bool resizable);
        [DispId(263)]
        void WindowClosing([In] bool isChildWindow, [In, Out] ref bool cancel);
        [DispId(264)]
        void WindowSetLeft([In] int left);
        [DispId(265)]
        void WindowSetTop([In] int top);
        [DispId(266)]
        void WindowSetWidth([In] int width);
        [DispId(267)]
        void WindowSetHeight([In] int height);
        [DispId(268)]
        void ClientToHostWindow([In, Out] ref long cx, [In, Out] ref long cy);
        [DispId(269)]
        void SetSecureLockIcon([In] int secureLockIcon);
        [DispId(270)]
        void FileDownload([In, Out] ref bool activeDocument, [In, Out] ref bool cancel);
        [DispId(271)]
        void NavigateError(
            [In, MarshalAs(UnmanagedType.IDispatch)] object pDisp,
            [In] ref object URL, 
            [In] ref object frame, 
            [In] ref object statusCode, 
            [In, Out] ref bool cancel);
        [DispId(272)]
        void PrivacyImpactedStateChange([In] bool bImpacted);
        [DispId(282)] // IE 7+
        void SetPhishingFilterStatus(uint phishingFilterStatus);
        [DispId(283)] // IE 7+
        void WindowStateChanged(uint dwFlags, uint dwValidFlagsMask);
    }

    [
        ComImport,
        Guid(IID.HtmlDocument2),
        InterfaceTypeAttribute(ComInterfaceType.InterfaceIsDual)
    ]
    internal interface IHtmlDocument2
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        object GetScript();
        [return: MarshalAs(UnmanagedType.Interface)]
        object GetAll();
        [return: MarshalAs(UnmanagedType.Interface)]
        object GetBody();
        [return: MarshalAs(UnmanagedType.Interface)]
        object GetActiveElement();
        [return: MarshalAs(UnmanagedType.Interface)]
        object GetImages();
        [return: MarshalAs(UnmanagedType.Interface)]
        object GetApplets();
        [return: MarshalAs(UnmanagedType.Interface)]
        object GetLinks();
        [return: MarshalAs(UnmanagedType.Interface)]
        object GetForms();
        [return: MarshalAs(UnmanagedType.Interface)]
        object GetAnchors();
        void SetTitle([In, MarshalAs(UnmanagedType.BStr)] string p);
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetTitle();
        [return: MarshalAs(UnmanagedType.Interface)]
        object GetScripts();
        void SetDesignMode([In, MarshalAs(UnmanagedType.BStr)] string p);
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetDesignMode();
        [return: MarshalAs(UnmanagedType.Interface)]
        object GetSelection();
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetReadyState();
        [return: MarshalAs(UnmanagedType.Interface)]
        object GetFrames();
        [return: MarshalAs(UnmanagedType.Interface)]
        object GetEmbeds();
        [return: MarshalAs(UnmanagedType.Interface)]
        object GetPlugins();
        void SetAlinkColor([In, MarshalAs(UnmanagedType.Struct)] object p);
        [return: MarshalAs(UnmanagedType.Struct)]
        object GetAlinkColor();
        void SetBackColor([In, MarshalAs(UnmanagedType.Struct)] object p);
        [return: MarshalAs(UnmanagedType.Struct)]
        object GetBackColor();
        void SetForeColor([In, MarshalAs(UnmanagedType.Struct)] object p);
        [return: MarshalAs(UnmanagedType.Struct)]
        object GetForeColor();
        void SetLinkColor([In, MarshalAs(UnmanagedType.Struct)] object p);
        [return: MarshalAs(UnmanagedType.Struct)]
        object GetLinkColor();
        void SetVlinkColor([In, MarshalAs(UnmanagedType.Struct)] object p);
        [return: MarshalAs(UnmanagedType.Struct)]
        object GetVlinkColor();
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetReferrer();
        [return: MarshalAs(UnmanagedType.Interface)]
        object GetLocation();
    }

    [
        ComImport, 
        DefaultMember("Name"),
        Guid(IID.WebBrowser2),
        InterfaceType(ComInterfaceType.InterfaceIsIDispatch),
    ]
    interface IWebBrowser2
    {
        [DispId(100)]
        void GoBack();
        [DispId(0x65)]
        void GoForward();
        [DispId(0x66)]
        void GoHome();
        [DispId(0x67)]
        void GoSearch();
        [DispId(0x68)]
        void Navigate([MarshalAs(UnmanagedType.BStr)] string URL, [In] ref object Flags, [In] ref object TargetFrameName, [In] ref object PostData, [In] ref object Headers);
        [DispId(-550)]
        void Refresh();
        [DispId(0x69)]
        void Refresh2([In] ref object Level);
        [DispId(0x6a)]
        void Stop();
        [DispId(300)]
        void Quit();
        [DispId(0x12d)]
        void ClientToWindow([In, Out] ref int pcx, [In, Out] ref int pcy);
        [DispId(0x12e)]
        void PutProperty([MarshalAs(UnmanagedType.BStr)] string Property, object vtValue);
        [DispId(0x12f)]
        object GetProperty([MarshalAs(UnmanagedType.BStr)] string Property);
        [DispId(500)]
        void Navigate2([In] ref object URL, [In] ref object Flags, [In] ref object TargetFrameName, [In] ref object PostData, [In] ref object Headers);
        [DispId(0x1f5)]
        OLECMDF QueryStatusWB(OLECMDID cmdID);
        [DispId(0x1f6)]
        void ExecWB(OLECMDID cmdID, OLECMDEXECOPT cmdexecopt, [In] ref object pvaIn, [In, Out] ref object pvaOut);
        [DispId(0x1f7)]
        void ShowBrowserBar([In] ref object pvaClsid, [In] ref object pvarShow, [In] ref object pvarSize);
        bool AddressBar { [return: MarshalAs(UnmanagedType.VariantBool)] [DispId(0x22b)] get; [DispId(0x22b)] set; }
        object Application { [return: MarshalAs(UnmanagedType.IDispatch)] [DispId(200)] get; }
        bool Busy { [return: MarshalAs(UnmanagedType.VariantBool)] [DispId(0xd4)] get; }
        object Container { [return: MarshalAs(UnmanagedType.IDispatch)] [DispId(0xca)] get; }
        object Document { [return: MarshalAs(UnmanagedType.IDispatch)] [DispId(0xcb)] get; }
        string FullName { [return: MarshalAs(UnmanagedType.BStr)] [DispId(400)] get; }
        bool FullScreen { [return: MarshalAs(UnmanagedType.VariantBool)] [DispId(0x197)] get; [DispId(0x197)] set; }
        int Height { [DispId(0xd1)] get; [DispId(0xd1)] set; }
        int HWND { [DispId(-515)] get; }
        int Left { [DispId(0xce)] get; [DispId(0xce)] set; }
        string LocationName { [return: MarshalAs(UnmanagedType.BStr)] [DispId(210)] get; }
        string LocationURL { [return: MarshalAs(UnmanagedType.BStr)] [DispId(0xd3)] get; }
        bool MenuBar { [return: MarshalAs(UnmanagedType.VariantBool)] [DispId(0x196)] get; [DispId(0x196)] set; }
        string Name { [return: MarshalAs(UnmanagedType.BStr)] [DispId(0)] get; }
        bool Offline { [return: MarshalAs(UnmanagedType.VariantBool)] [DispId(550)] get; [DispId(550)] set; }
        object Parent { [return: MarshalAs(UnmanagedType.IDispatch)] [DispId(0xc9)] get; }
        string Path { [return: MarshalAs(UnmanagedType.BStr)] [DispId(0x191)] get; }
        READYSTATE ReadyState { [DispId(-525)] get; }
        bool RegisterAsBrowser { [return: MarshalAs(UnmanagedType.VariantBool)] [DispId(0x228)] get; [DispId(0x228)] set; }
        bool RegisterAsDropTarget { [return: MarshalAs(UnmanagedType.VariantBool)] [DispId(0x229)] get; [DispId(0x229)] set; }
        bool Resizable { [return: MarshalAs(UnmanagedType.VariantBool)] [DispId(0x22c)] get; [DispId(0x22c)] set; }
        bool Silent { [return: MarshalAs(UnmanagedType.VariantBool)] [DispId(0x227)] get; [DispId(0x227)] set; }
        bool StatusBar { [return: MarshalAs(UnmanagedType.VariantBool)] [DispId(0x193)] get; [DispId(0x193)] set; }
        string StatusText { [return: MarshalAs(UnmanagedType.BStr)] [DispId(0x194)] get; [DispId(0x194)] set; }
        bool TheaterMode { [return: MarshalAs(UnmanagedType.VariantBool)] [DispId(0x22a)] get; [DispId(0x22a)] set; }
        int ToolBar { [DispId(0x195)] get; [DispId(0x195)] set; }
        int Top { [DispId(0xcf)] get; [DispId(0xcf)] set; }
        bool TopLevelContainer { [return: MarshalAs(UnmanagedType.VariantBool)] [DispId(0xcc)] get; }
        string Type { [return: MarshalAs(UnmanagedType.BStr)] [DispId(0xcd)] get; }
        bool Visible { [return: MarshalAs(UnmanagedType.VariantBool)] [DispId(0x192)] get; [DispId(0x192)] set; }
        int Width { [DispId(0xd0)] get; [DispId(0xd0)] set; }
    }

    internal class WebBrowserEvents : IDisposable
    {
        private readonly _EventSink _sink;
        private SafeConnectionPointCookie _cookie;

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public WebBrowserEvents(WebBrowser browser)
        {
            if (browser.Document == null)
            {
                throw new InvalidOperationException("Can't add an event sink until the browser's document is non-null");
            }

            var serviceProvider = (IServiceProvider)browser.Document;
            var serviceGuid = new Guid(SID.SWebBrowserApp);
            var iid = new Guid(IID.ConnectionPointContainer);
            var cpc = (IConnectionPointContainer)serviceProvider.QueryService(ref serviceGuid, ref iid);

            _sink = new _EventSink(this);
            _cookie = new SafeConnectionPointCookie(cpc, _sink, new Guid(IID.WebBrowserEvents2));
        }

        // Because the DWebBrowserEvents2 interface is internal, provide our own IDispatch front-end for interop.
        private class _EventSink : DWebBrowserEvents2, IReflect
        {
            private readonly WebBrowserEvents _target;
            private static readonly Dictionary<int, MethodInfo> _dispIdMethodMap = typeof(DWebBrowserEvents2).GetMethods()
                .ToDictionary(mi => ((DispIdAttribute[])mi.GetCustomAttributes(typeof(DispIdAttribute), false))[0].Value);

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            internal _EventSink(WebBrowserEvents target)
            {
                Assert.IsNotNull(target);
                _target = target;
            }

            #region DWebBrowserEvents2 Members

            public void StatusTextChange(string text) {}
            public void DownloadComplete() {}
            public void CommandStateChange(long command, bool enable) {}
            public void DownloadBegin() {}
            public void ProgressChange(int progress, int progressMax) {}
            public void PropertyChange(string szProperty) {}
            public void TitleChange(string text) {}
            public void PrintTemplateInstantiation(object pDisp) {}
            public void PrintTemplateTeardown(object pDisp) {}
            public void UpdatePageStatus(object pDisp, ref object nPage, ref object fDone) {}
            public void BeforeNavigate2(object pDisp, ref object URL, ref object flags, ref object targetFrameName, ref object postData, ref object headers, ref bool cancel) {}
            public void NewWindow2(ref object pDisp, ref bool cancel) {}
            public void NavigateComplete2(object pDisp, ref object URL) {}
            public void OnQuit() {}
            public void OnVisible(bool visible) {}
            public void OnToolBar(bool toolBar) {}
            public void OnMenuBar(bool menuBar) {}
            public void OnStatusBar(bool statusBar) {}
            public void OnFullScreen(bool fullScreen) {}
            public void DocumentComplete(object pDisp, ref object URL) {}
            public void OnTheaterMode(bool theaterMode) {}
            public void WindowSetResizable(bool resizable) {}
            public void WindowClosing(bool isChildWindow, ref bool cancel)
            {
                _target._NotifyWindowClosing();
            }
            public void WindowSetLeft(int left) {}
            public void WindowSetTop(int top) {}
            public void WindowSetWidth(int width) {}
            public void WindowSetHeight(int height) {}
            public void ClientToHostWindow(ref long cx, ref long cy) {}
            public void SetSecureLockIcon(int secureLockIcon) {}
            public void FileDownload(ref bool activeDocument, ref bool cancel) {}
            public void NavigateError(object pDisp, ref object URL, ref object frame, ref object statusCode, ref bool cancel) {}
            public void PrivacyImpactedStateChange(bool bImpacted) {}
            public void SetPhishingFilterStatus(uint phishingFilterStatus) {}
            public void WindowStateChanged(uint dwFlags, uint dwValidFlagsMask) {}

            #endregion

            #region IReflect Members

            FieldInfo IReflect.GetField(string name, BindingFlags bindingAttr) { throw new NotImplementedException(); }
            FieldInfo[] IReflect.GetFields(BindingFlags bindingAttr) { return null; }
            MemberInfo[] IReflect.GetMember(string name, BindingFlags bindingAttr) { throw new NotImplementedException(); }
            MemberInfo[] IReflect.GetMembers(BindingFlags bindingAttr) { throw new NotImplementedException(); }
            MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr) { throw new NotImplementedException(); }
            MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers) { throw new NotImplementedException(); }
            MethodInfo[] IReflect.GetMethods(BindingFlags bindingAttr) { return null; }
            PropertyInfo[] IReflect.GetProperties(BindingFlags bindingAttr) { return null; }
            PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers) { throw new NotImplementedException(); }
            PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr) { throw new NotImplementedException(); }

            object IReflect.InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
            {
                Verify.IsNotNull(name, "name");
                if (name.StartsWith("[DISPID=", StringComparison.OrdinalIgnoreCase))
                {
                    int dispid = int.Parse(name.Substring(8, name.Length - 9), CultureInfo.InvariantCulture);
                    MethodInfo method;
                    if (_dispIdMethodMap.TryGetValue(dispid, out method))
                    {
                        return method.Invoke(this, invokeAttr, binder, args, culture);
                    }
                }
                throw new MissingMethodException(GetType().Name, name);
            }

            Type IReflect.UnderlyingSystemType { get { return typeof(DWebBrowserEvents2); } }

            #endregion
        }

        public event EventHandler WindowClosing;

        private void _NotifyWindowClosing()
        {
            var handler = WindowClosing;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #region IDisposable Pattern

        #if DEBUG
        [SuppressMessage("Microsoft.Performance", "CA1821:RemoveEmptyFinalizers")]
        ~WebBrowserEvents()
        {
            Assert.Fail();
        }
        #endif

        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_cookie")]
        public void Dispose()
        {
            Utility.SafeDispose(ref _cookie);
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    internal static partial class Utility
    {
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static string GetWebPageTitle(WebBrowser browser)
        {
            if (browser.Document == null)
            {
                return "";
            }
            
            return ((IHtmlDocument2)browser.Document).GetTitle();
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void SuppressJavaScriptErrors(WebBrowser browser)
        {
            if (browser.Document != null)
            {
                var serviceProvider = (IServiceProvider)browser.Document;
                var serviceGuid = new Guid(SID.SWebBrowserApp);
                var iid = new Guid(IID.WebBrowser2);
                var webBrowser2 = (IWebBrowser2)serviceProvider.QueryService(ref serviceGuid, ref iid);
                webBrowser2.Silent = true;
            }
        }
    }
}