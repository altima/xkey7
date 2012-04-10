using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace xkey7.XKey
{
    public class Controller
    {
        private const string LATEST_VERSION_URL = "http://devfaw.com/latest.php";
        private const string BASE_DATA_URL = "http://{0}/data.xml";
        private const string BASE_LAUNCH_URL = "http://{0}/launchgame.sh?{1}";
        private const string BASE_IMAGE_URL = "http://{0}/covers/{1}.jpg";
        private string _ip;
        private WebClient _webClient;

        public event EventHandler<DataLoadedEventArgs> DataLoaded;
        public event EventHandler<LatestVersionEventArgs> LatestVersionLoaded;
        public event EventHandler<ErrorEventArgs> Error;

        public void OnLatestVersionLoaded(string version)
        {
            EventHandler<LatestVersionEventArgs> handler = LatestVersionLoaded;
            if (handler != null) handler(this, new LatestVersionEventArgs(version));
        }
        private void OnError(ErrorState errorState, string errorMessage)
        {
            EventHandler<ErrorEventArgs> handler = Error;
            if (handler != null) handler(this, new ErrorEventArgs(errorState, errorMessage));
        }
        private void OnDataLoaded(Model datamodel)
        {
            EventHandler<DataLoadedEventArgs> handler = DataLoaded;
            if (handler != null) handler(this, new DataLoadedEventArgs(datamodel));
        }

        public Controller(string ip)
        {
            _ip = ip;
        }

        private void InitWebClient()
        {
            _webClient = new WebClient();
        }

        void WebClientDataLoaded(object sender, DownloadStringCompletedEventArgs e)
        {
            _webClient.DownloadStringCompleted -= WebClientDataLoaded;
            if (e.Error != null)
            {
                HandleError(ErrorState.NoConnection, e.Error.Message);
            }
            else
            {
                HandleDataResponse(e.Result);
            }
        }
        void WebClient_LatestVersion(object sender, DownloadStringCompletedEventArgs e)
        {
            _webClient.DownloadStringCompleted -= WebClient_LatestVersion;
            if (e.Error != null)
            {
                HandleError(ErrorState.NoConnection, e.Error.Message);
            }
            else
            {
                var latestVersion = e.Result.Trim();
                HandleLatestVersionResponse(latestVersion);
            }
        }

        private void HandleDataResponse(string data)
        {
            try
            {
                var xml = XDocument.Parse(data);

                var rootNode = xml.Root;

                var active = (rootNode.Descendants("ACTIVE").ToList().Count > 0) ? rootNode.Descendants("ACTIVE").ToList()[0].Value : string.Empty;

                var mountNodes = rootNode.Descendants("MOUNT").ToList();
                var aboutNode = rootNode.Descendants("ABOUT").ToList()[0];

                var gui = int.Parse(rootNode.Element("GUISTATE").Value);
                var emergancy = int.Parse(rootNode.Element("EMERGENCY").Value);
                var tray = int.Parse(rootNode.Element("TRAYSTATE").Value);

                var m = new Model(gui, emergancy, tray);

                m.Active = active;

                foreach (var mountNode in mountNodes)
                {
                    var name = mountNode.Attribute("NAME").Value;
                    var mount = m.AddMount(name);
                    var isoNodes = mountNode.Elements();
                    foreach (var isoNode in isoNodes)
                    {
                        var title = isoNode.Element("TITLE").Value;
                        var id = isoNode.Element("ID").Value;
                        mount.AddIso(title, id, (!string.IsNullOrEmpty(active) && id == active));
                    }
                }

                foreach (var element in aboutNode.Elements())
                {
                    var name = element.Attribute("NAME").Value;
                    var value = element.Value;
                    m.AddAbout(name, value);
                }
                OnDataLoaded(m);
            }
            catch (Exception e)
            {
                HandleError(ErrorState.ParseError, e.Message);
            }
        }
        private void HandleError(ErrorState state, string message)
        {
            OnError(state, message);
        }

        public void LoadData()
        {
            InitWebClient();
            _webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(WebClientDataLoaded);
            _webClient.DownloadStringAsync(new Uri(string.Format(BASE_DATA_URL, _ip), UriKind.Absolute));
        }
        public void LaunchGame(string id)
        {
            InitWebClient();
            _webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(WebClientDataLoaded);
            _webClient.DownloadStringAsync(new Uri(string.Format(BASE_LAUNCH_URL, _ip, id), UriKind.Absolute));
        }
        public void GetLatestVersion()
        {
            InitWebClient();
            _webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(WebClient_LatestVersion);
            _webClient.DownloadStringAsync(new Uri(LATEST_VERSION_URL, UriKind.Absolute));
        }

        private void HandleLatestVersionResponse(string latestVersion)
        {
            OnLatestVersionLoaded(latestVersion);
        }

        public static string GetImageUrl(string id, string ip)
        {
            return string.Format(BASE_IMAGE_URL, ip, id);
        }
    }

    public class LatestVersionEventArgs : EventArgs
    {
        public string LatestVersion { get; set; }

        public LatestVersionEventArgs(string version)
        {
            LatestVersion = version;
        }
    }
    public class ErrorEventArgs : EventArgs
    {
        public ErrorState ErrorState { get; set; }
        public string ErrorMessage { get; set; }

        public ErrorEventArgs(ErrorState errorState, string errorMessage)
        {
            ErrorState = errorState;
            ErrorMessage = errorMessage;
        }
    }
    public class DataLoadedEventArgs : EventArgs
    {
        public Model DataModel { get; set; }

        public DataLoadedEventArgs(Model datamodel)
        {
            DataModel = datamodel;
        }
    }
}
