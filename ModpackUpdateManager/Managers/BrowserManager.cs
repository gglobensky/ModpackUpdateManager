using CefSharp;
using CefSharp.Handler;
using CefSharp.WinForms;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ModpackUpdateManager.Managers
{
    public static class BrowserManager
    {
        public static string Address { get; private set; }

        private static ChromiumWebBrowser browser;
        private static Form1 MainForm;

        public delegate IResourceRequestHandler OnBeforeRequest(IRequest request);
        public static void Initialize(string startingUrl, ModOperationManager _modOperationManager, Form1 _MainForm, Action<DownloadItem> _onDownloadUpdated, OnBeforeRequest _onBeforeRequest)
        {
            MainForm = _MainForm;
            Cef.Initialize(new CefSettings());
            browser = new ChromiumWebBrowser(startingUrl);
            browser.AddressChanged += Browser_AddressChanged;
            browser.RequestHandler = new ModpackUpdateRequestHandler(_onBeforeRequest);
            browser.DownloadHandler = new ModpackUpdateDownloadHandler(_onDownloadUpdated);
            browser.Dock = System.Windows.Forms.DockStyle.Fill;

            MainForm.AddControl(browser);
        }

        private static void Browser_AddressChanged(object sender, AddressChangedEventArgs e)
        {
            Address = e.Address;
            MainForm.SetURLText(e.Address);
        }

        public async static Task LoadUrl(string url)
        {
            if (!PersistentVariables.GetIsAutoModePaused())
            {
                await Task.Delay(2000);
                // Avoid too many requests
                await browser.LoadUrlAsync(url);
            }
        }

        public static void Back()
        {
            browser.Back();
        }

        public static void Forward()
        {
            browser.Forward();
        }

        public class ModpackUpdateRequestHandler : CefSharp.Handler.RequestHandler
        {
            private OnBeforeRequest onBeforeRequest;
            public ModpackUpdateRequestHandler(OnBeforeRequest _onBeforeRequest)
            {
                onBeforeRequest = _onBeforeRequest;
            }
            protected override bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
            {
                return base.OnBeforeBrowse(chromiumWebBrowser, browser, frame, request, userGesture, isRedirect);
            }
            protected override IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
            {
                return onBeforeRequest.Invoke(request);
            }
        }

        public static string GetRequest(string url)
        {
            // Create a new WebClient instance
            WebClient client = new WebClient();

            // Send a GET request to the API endpoint and store the response as a string
            return client.DownloadString(url);
        }

        public class ModpackUpdateDownloadHandler : DownloadHandler
        {
            private Action<DownloadItem> onDownloadUpdated;
            public ModpackUpdateDownloadHandler(Action<DownloadItem> _onDownloadUpdated)
            {
                onDownloadUpdated = _onDownloadUpdated;
            }

            protected override void OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
            {
                if (!callback.IsDisposed)
                {
                    using (callback)
                    {
                        System.IO.Directory.CreateDirectory(System.IO.Path.Combine(PersistentVariables.GetOutputModPath()));
                        callback.Continue(System.IO.Path.Combine(System.IO.Path.GetFullPath(PersistentVariables.GetOutputModPath()), downloadItem.SuggestedFileName), showDialog: false);
                    }
                }
            }

            protected override void OnDownloadUpdated(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
            {
                onDownloadUpdated.Invoke(downloadItem);
            }

        }

        public static async Task<T> ExecuteJavascript<T>(string script)
        {
            T result = default(T);

            await browser.GetMainFrame().EvaluateScriptAsync(@script).ContinueWith(x =>
            {
                var response = x.Result;
                bool success = response.Success && response.Result != null;

                if (success)
                {
                    if (!Utilities.TryParseJson<T>(JsonConvert.SerializeObject(response.Result), out result))
                    {
                        LogFile.LogMessage($"Error, could not parse javascript execution result for script : {Environment.NewLine}{@script}");
                    }
                }
                else
                {
                    LogFile.LogMessage(Newtonsoft.Json.JsonConvert.SerializeObject(x));
                }
            });

            return result;

        }

        public static void Dispose()
        {
            browser.AddressChanged -= Browser_AddressChanged;

            Cef.Shutdown();
        }
    }

}
