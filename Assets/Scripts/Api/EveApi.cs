using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace Api
{
    public static class EveApi
    {
        public abstract class ApiSection
        {
            protected TimeSpan CacheDuration { get { return new TimeSpan(0, 15, 0); } }
            protected abstract string SectionUrl { get; }

            private DateTime lastUpdate = DateTime.MinValue;
            private List<Action> waitingCallbacks = new List<Action>();
            private bool waiting = false;

            public ApiSection()
            {
                Update();
            }

            public virtual void Update(Action callback = null)
            {
                if (DateTime.Now > lastUpdate + CacheDuration)
                {
                    // Time for a real update, request new infos from the server and queue the callback
                    waiting = true;
                    waitingCallbacks.Add(callback);
                    ApiRequest(SectionUrl, (str) => { ProcessUpdate(str); ExecuteCallbacks(); });
                    lastUpdate = DateTime.Now;
                }
                else if (waiting)
                {
                    // We are already updating, queue the callback until update is done
                    waitingCallbacks.Add(callback);
                }
                else
                {
                    // Use the cached values
                    if (callback != null)
                        callback();
                }
            }

            private void ExecuteCallbacks()
            {
                waiting = false;
                waitingCallbacks.ForEach((action) => { if (action != null) action(); });
                waitingCallbacks.Clear();
            }

            protected abstract void ProcessUpdate(string updateXML);
        }

        public class ServerStatusApi : ApiSection
        {
            public int OnlinePlayers { get; private set; }

            protected override string SectionUrl
            {
                get
                {
                    return @"https://api.eveonline.com/Server/ServerStatus.xml.aspx";
                }
            }

            protected override void ProcessUpdate(string updateXML)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(updateXML);
                string text = doc.SelectSingleNode("//result/onlinePlayers/text()").Value;
                int playersResult = OnlinePlayers;
                int.TryParse(text, out playersResult);
                OnlinePlayers = playersResult;
            }
        }

        public class PlayerKillsApi : ApiSection
        {
            public List<PlayerKill> LastHourKills { get; private set; }

            public PlayerKillsApi()
            {
                LastHourKills = new List<PlayerKill>();
            }

            protected override string SectionUrl
            {
                get
                {
                    return @"https://zkillboard.com/api/kills/pastSeconds/3600/no-items/no-attackers/xml/";
                }
            }

            protected override void ProcessUpdate(string updateXML)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(updateXML);
                XmlNodeList nodes = doc.SelectNodes("//result/rowset/row");

                LastHourKills = new List<PlayerKill>();
                foreach (XmlNode node in nodes)
                {
                    PlayerKill kill = new PlayerKill();
                    string text = node.Attributes["solarSystemID"].Value;
                    int.TryParse(text, out kill.systemID);
                    kill.characterName = node.FirstChild.Attributes["characterName"].Value;
                    LastHourKills.Add(kill);
                }
            }
        }

        public static ServerStatusApi ServerStatus { get; private set; }
        public static PlayerKillsApi PlayerKills { get; private set; }

        // TODO Super duper dangerous don't publish before this is fixed
        private class TrustAllCertificatesPolicy : ICertificatePolicy
        {
            public bool CheckValidationResult(ServicePoint srvPoint, X509Certificate certificate, WebRequest request, int certificateProblem)
            {
                return true;
            }
        }

        private struct CallbackInfo
        {
            public WebRequest request;
            public Action<string> callback;
        }

        static EveApi()
        {
            ServicePointManager.CertificatePolicy = new TrustAllCertificatesPolicy();

            ServerStatus = new ServerStatusApi();
            PlayerKills = new PlayerKillsApi();
        }

        private static void ApiRequest(string uri, Action<string> callback)
        {
            AsyncCallback requestCallback = new AsyncCallback(ProcessApiResult);
            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
            CallbackInfo info = new CallbackInfo();
            info.request = request;
            info.callback = callback;

            request.UserAgent = @"http://theasuro.de Maintainer: Till W theasuro@gmail.com";

            request.BeginGetResponse(requestCallback, info);
        }

        private static void ProcessApiResult(IAsyncResult result)
        {
            CallbackInfo info = (CallbackInfo)result.AsyncState;
            WebResponse response = info.request.EndGetResponse(result);
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string text = reader.ReadToEnd();
            response.Close();
            if (info.callback != null)
                info.callback(text);
        }
    }

    public struct PlayerKill
    {
        public int systemID;
        public string characterName;
    }
}