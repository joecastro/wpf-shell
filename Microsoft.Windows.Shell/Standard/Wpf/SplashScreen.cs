
namespace Standard
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Resources;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Threading;

    // Current issues with this implementation:
    // *  FadeOutDuration will pop the splashscreen in front of the main window.  This can be partially managed
    //        by using IsTopMost, but that has other effects.  I should be able to hook the WndProc to keep this
    //        window from going inactive.
    // * FadeInDuration doesn't work because this is being created on the main UI thread.  For multiple reasons we
    //        should probably create this window on a background thread.
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class SplashScreen
    {
        private static readonly BLENDFUNCTION _BaseBlendFunction = new BLENDFUNCTION
        {
            BlendOp = AC.SRC_OVER,
            BlendFlags = 0,
            SourceConstantAlpha = 255,
            AlphaFormat = AC.SRC_ALPHA,
        };

        private MessageWindow _hwndWrapper;
        private SafeHBITMAP _hBitmap;
        private DispatcherTimer _dt;
        private DateTime _fadeOutEnd;
        private DateTime _fadeInEnd;
        private ResourceManager _resourceManager;
        private string _resourceName;
        private Dispatcher _dispatcher;
        private Assembly _resourceAssembly;
        private bool _isClosed = false;

        private void _VerifyMutability()
        {
            if (_hwndWrapper != null)
            {
                throw new InvalidOperationException("Splash screen has already been shown.");
            }
        }

        public SplashScreen() { }

        public Assembly ResourceAssembly
        { 
            get { return _resourceAssembly; }
            set
            {
                _VerifyMutability();

                Verify.IsNotNull(value, "value");

                _resourceAssembly = value;
                AssemblyName name = new AssemblyName(_resourceAssembly.FullName);
                _resourceManager = new ResourceManager(name.Name + ".g", _resourceAssembly);
            }
        }

        public string ResourceName
        {
            get { return _resourceName ?? ""; }
            set
            {
                Verify.IsNeitherNullNorEmpty(value, "value");
                _resourceName = value.ToLowerInvariant();
            }
        }

        public string ImageFileName { get; set; }
        public bool IsTopMost { get; set; }
        public bool CloseOnMainWindowCreation { get; set; }
        public TimeSpan FadeOutDuration { get; set; }
        public TimeSpan FadeInDuration { get; set; }

        public void Show()
        {
            _VerifyMutability();

            Stream imageStream = null;
            try
            {
                // Try to use the filepath first.  If it's not provided or not available, use the embedded resource.
                if (!string.IsNullOrEmpty(ImageFileName) && File.Exists(ImageFileName))
                {
                    try
                    {
                        imageStream = new FileStream(ImageFileName, FileMode.Open);
                    }
                    catch (IOException) { }
                }

                if (imageStream == null)
                {
                    imageStream = _resourceManager.GetStream(ResourceName, CultureInfo.CurrentUICulture);
                    if (imageStream == null)
                    {
                        throw new IOException("The resource could not be found.");
                    }
                }

                Size bitmapSize;
                _hBitmap = _CreateHBITMAPFromImageStream(imageStream, out bitmapSize);

                Point location = new Point(
                    (NativeMethods.GetSystemMetrics(SM.CXSCREEN) - bitmapSize.Width) / 2,
                    (NativeMethods.GetSystemMetrics(SM.CYSCREEN) - bitmapSize.Height) / 2);

                // Pass a null WndProc.  Let the MessageWindow use DefWindowProc.
                _hwndWrapper = new MessageWindow(
                    CS.HREDRAW | CS.VREDRAW,
                    WS.POPUP | WS.VISIBLE,
                    WS_EX.WINDOWEDGE | WS_EX.TOOLWINDOW | WS_EX.LAYERED | (IsTopMost ? WS_EX.TOPMOST : 0),
                    new Rect(location, bitmapSize),
                    "Splash Screen",
                    null);

                byte opacity = (byte)(FadeInDuration > TimeSpan.Zero ? 0 : 255);

                using (SafeDC hScreenDC = SafeDC.GetDesktop())
                {
                    using (SafeDC memDC = SafeDC.CreateCompatibleDC(hScreenDC))
                    {
                        IntPtr hOldBitmap = NativeMethods.SelectObject(memDC, _hBitmap);

                        RECT hwndRect = NativeMethods.GetWindowRect(_hwndWrapper.Handle);

                        POINT hwndPos = hwndRect.Position;
                        SIZE hwndSize = hwndRect.Size;
                        POINT origin = new POINT();
                        BLENDFUNCTION bf = _BaseBlendFunction;
                        bf.SourceConstantAlpha = opacity;

                        NativeMethods.UpdateLayeredWindow(_hwndWrapper.Handle, hScreenDC, ref hwndPos, ref hwndSize, memDC, ref origin, 0, ref bf, ULW.ALPHA);
                        NativeMethods.SelectObject(memDC, hOldBitmap);
                    }
                }


                if (CloseOnMainWindowCreation)
                {
                    Dispatcher.CurrentDispatcher.BeginInvoke(
                        DispatcherPriority.Loaded,
                        (DispatcherOperationCallback)delegate(object splashObj)
                        {
                            var splashScreen = (SplashScreen)splashObj;
                            if (!splashScreen._isClosed)
                            {
                                splashScreen.Close();
                            }
                            return null;
                        },
                        this);
                }

                _dispatcher = Dispatcher.CurrentDispatcher;
                if (FadeInDuration > TimeSpan.Zero)
                {
                    _fadeInEnd = DateTime.UtcNow + FadeInDuration;
                    _dt = new DispatcherTimer(FadeInDuration, DispatcherPriority.Normal, _FadeInTick, _dispatcher);
                    _dt.Start();
                }
            }
            finally
            {
                Utility.SafeDispose(ref imageStream);
            }
        }

        public void Close()
        {
            if (!_dispatcher.CheckAccess())
            {
                _dispatcher.Invoke(DispatcherPriority.Normal, (Action)Close);
                return;
            }

            if (_isClosed)
            {
                throw new InvalidOperationException("Splash screen was already closed");
            }

            _isClosed = true;

            if (FadeOutDuration <= TimeSpan.Zero)
            {
                _DestroyResources();
                return;
            }

            try
            {
                NativeMethods.SetActiveWindow(_hwndWrapper.Handle);
            }
            catch
            {
                // SetActiveWindow fails if the application is not in the foreground.
                // If this is the case, don't bother animating the fade out.
                _DestroyResources();
                return;
            }

            _fadeOutEnd = DateTime.UtcNow + FadeOutDuration;
            if (_dt != null)
            {
                _dt.Stop();
            }
            _dt = new DispatcherTimer(TimeSpan.FromMilliseconds(30), DispatcherPriority.Normal, _FadeOutTick, _dispatcher);
            _dt.Start();

            return;
        }

        private void _FadeOutTick(object unused, EventArgs args)
        {
            DateTime dtNow = DateTime.UtcNow;
            if (dtNow >= _fadeOutEnd)
            {
                _DestroyResources();
            }
            else
            {
                double progress = (_fadeOutEnd - dtNow).TotalMilliseconds / FadeOutDuration.TotalMilliseconds;
                BLENDFUNCTION bf = _BaseBlendFunction;
                bf.SourceConstantAlpha = (byte)(255 * progress);
                NativeMethods.UpdateLayeredWindow(_hwndWrapper.Handle, 0, ref bf, ULW.ALPHA);
            }
        }

        private void _FadeInTick(object unused, EventArgs args)
        {
            DateTime dtNow = DateTime.UtcNow;
            if (dtNow >= _fadeInEnd)
            {
                _DestroyResources();
            }
            else
            {
                double progress = 1 - (_fadeInEnd - dtNow).TotalMilliseconds / FadeInDuration.TotalMilliseconds;
                progress = Math.Max(0, Math.Min(progress, 1));
                BLENDFUNCTION bf = _BaseBlendFunction;
                bf.SourceConstantAlpha = (byte)(int)(255 * progress);
                NativeMethods.UpdateLayeredWindow(_hwndWrapper.Handle, 0, ref bf, ULW.ALPHA);
            }
        }

        private void _DestroyResources()
        {
            if (_dt != null)
            {
                _dt.Stop();
                _dt = null;
            }
            Utility.SafeDispose(ref _hwndWrapper);
            Utility.SafeDispose(ref _hBitmap);
            if (_resourceManager != null)
            {
                _resourceManager.ReleaseAllResources();
            }
        }

        private static SafeHBITMAP _CreateHBITMAPFromImageStream(Stream imgStream, out Size bitmapSize)
        {
            IWICImagingFactory pImagingFactory = null;
            IWICBitmapDecoder pDecoder = null;
            IWICStream pStream = null;
            IWICBitmapFrameDecode pDecodedFrame = null;
            IWICFormatConverter pBitmapSourceFormatConverter = null;
            IWICBitmapFlipRotator pBitmapFlipRotator = null;

            SafeHBITMAP hbmp = null;
            try
            {
                using (var istm = new ManagedIStream(imgStream))
                {
                    pImagingFactory = CLSID.CoCreateInstance<IWICImagingFactory>(CLSID.WICImagingFactory);
                    pStream = pImagingFactory.CreateStream();
                    pStream.InitializeFromIStream(istm);

                    // Create an object that will decode the encoded image
                    Guid vendor = Guid.Empty;
                    pDecoder = pImagingFactory.CreateDecoderFromStream(pStream, ref vendor, WICDecodeMetadata.CacheOnDemand);

                    pDecodedFrame = pDecoder.GetFrame(0);
                    pBitmapSourceFormatConverter = pImagingFactory.CreateFormatConverter();

                    // Convert the image from whatever format it is in to 32bpp premultiplied alpha BGRA
                    Guid pixelFormat = WICPixelFormat.WICPixelFormat32bppPBGRA;
                    pBitmapSourceFormatConverter.Initialize(pDecodedFrame, ref pixelFormat, WICBitmapDitherType.None, IntPtr.Zero, 0, WICBitmapPaletteType.Custom);

                    pBitmapFlipRotator = pImagingFactory.CreateBitmapFlipRotator();
                    pBitmapFlipRotator.Initialize(pBitmapSourceFormatConverter, WICBitmapTransform.FlipVertical);

                    int width, height;
                    pBitmapFlipRotator.GetSize(out width, out height);

                    bitmapSize = new Size { Width = width, Height = height };

                    var bmi = new BITMAPINFO
                    {
                        bmiHeader = new BITMAPINFOHEADER
                        {
                            biSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER)),
                            biWidth = width,
                            biHeight = height,
                            biPlanes = 1,
                            biBitCount = 32,
                            biCompression = BI.RGB,
                            biSizeImage = (width * height * 4),
                        },
                    };

                    // Create a 32bpp DIB.  This DIB must have an alpha channel for UpdateLayeredWindow to succeed.
                    IntPtr pBitmapBits;
                    hbmp = NativeMethods.CreateDIBSection(null, ref bmi, out pBitmapBits, IntPtr.Zero, 0);

                    // Copy the decoded image to the new buffer which backs the HBITMAP
                    var rect = new WICRect { X = 0, Y = 0, Width = width, Height = height };
                    pBitmapFlipRotator.CopyPixels(ref rect, width * 4, bmi.bmiHeader.biSizeImage, pBitmapBits);

                    var ret = hbmp;
                    hbmp = null;
                    return ret;
                }
            }
            finally
            {
                Utility.SafeRelease(ref pImagingFactory);
                Utility.SafeRelease(ref pDecoder);
                Utility.SafeRelease(ref pStream);
                Utility.SafeRelease(ref pDecodedFrame);
                Utility.SafeRelease(ref pBitmapFlipRotator);
                Utility.SafeRelease(ref pBitmapSourceFormatConverter);
                Utility.SafeDispose(ref hbmp);
            }
        }
    }
}