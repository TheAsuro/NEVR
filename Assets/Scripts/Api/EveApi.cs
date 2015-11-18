using System;
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
            protected abstract string SectionUrl { get; }

            public ApiSection()
            {
                Update();
            }

            public virtual void Update()
            {
                ApiRequest(SectionUrl, ProcessUpdate);
            }

            public virtual void Update(Action callback)
            {
                ApiRequest(SectionUrl, (str) => { ProcessUpdate(str); callback(); });
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
                string test = doc.SelectSingleNode("//result/onlinePlayers/text()").Value;
                int playersResult = OnlinePlayers;
                int.TryParse(test, out playersResult);
                OnlinePlayers = playersResult;
            }
        }

        public static ServerStatusApi ServerStatus { get; private set; }

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
        }

        private static void ApiRequest(string uri, Action<string> callback)
        {
            AsyncCallback requestCallback = new AsyncCallback(ProcessApiResult);
            WebRequest request = WebRequest.Create(uri);
            CallbackInfo info = new CallbackInfo();
            info.request = request;
            info.callback = callback;

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
}