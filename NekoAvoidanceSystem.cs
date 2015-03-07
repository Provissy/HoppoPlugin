using Fiddler;
using Grabacr07.KanColleViewer.Models;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

/*
 * Copyright 2015 @Provissy.
 * DO NOT modify or distribute the code.
 * DO NOT sublicense the code.
 */

namespace HoppoPlugin
{
    class NekoAvoidanceSystem
    {
        public static void Startup()
        {
            FiddlerApplication.BeforeRequest += FiddlerApplication_BeforeRequest;
        }

        static void FiddlerApplication_BeforeRequest(Session oSession)
        {
            try
            {
                if (!HoppoPluginSettings.Current.EnableNAS)
                    return;
                if (oSession.isHTTPS)
                    return;
                if (oSession.fullUrl.IndexOf("/kcsapi/") == -1)
                    return;

                // Let's go.

                // First, get the request body and header.
                byte[] requestDataBytes = oSession.requestBodyBytes;
                WebHeaderCollection headers = new WebHeaderCollection();
                foreach (HTTPHeaderItem headerItem in oSession.oRequest.headers)
                {
                    headers.Add(headerItem.Name, headerItem.Value);
                }

                // Abort default Fiddler gateway.
                oSession.utilCreateResponseAndBypassServer();

                while (!postData(headers, oSession.requestBodyBytes, oSession))
                {
                    // This row for my HoppoPlugin.
                    MainView.Instance.Dispatcher.Invoke(new Action(() => MainView.Instance.Tbl_NekoCount.Text = (int.Parse(MainView.Instance.Tbl_NekoCount.Text) + 1).ToString()));
                    // Hmmm, just wait a minute.
                    Thread.Sleep(2000);
                    // If postData returns false, it means error occured, we need post again.
                    if (postData(headers, oSession.requestBodyBytes, oSession))
                        break;
                }
            }
            catch
            {
                // Oops what happend !
                // Exception should not be catched here.
                // So just do nothing XD
            }
        }

        static bool postData(WebHeaderCollection headers, byte[] body, Session oSession)
        {
            try
            {
                var myRequest = (HttpWebRequest)WebRequest.Create(oSession.fullUrl);

                // Modify our request.
                // It's very important.
                if (headers != null)
                {
                    foreach (var key in headers.AllKeys)
                    {
                        switch (key.ToLower())
                        {
                            case "accept":
                                myRequest.Accept = headers[key];
                                break;
                            case "connection":
                                myRequest.KeepAlive = false;
                                break;
                            case "content-type":
                                myRequest.ContentType = headers[key];
                                break;
                            case "proxy-connection":
                                myRequest.KeepAlive = false;
                                break;
                            case "if-modified-since":
                                DateTime result;
                                if (DateTime.TryParse(headers[key], out result))
                                {
                                    myRequest.IfModifiedSince = result;
                                }
                                break;
                            case "range":
                                string[] range = headers[key].Split('=');
                                string[] bytes = range[1].Split('-');
                                if (bytes[1] == "")
                                {
                                    myRequest.AddRange(range[0], int.Parse(bytes[0]));
                                }
                                else
                                {
                                    myRequest.AddRange(range[0], int.Parse(bytes[0]), int.Parse(bytes[1]));
                                }
                                break;
                            case "referer":
                                myRequest.Referer = headers[key];
                                break;
                            case "user-agent":
                                myRequest.UserAgent = headers[key];
                                break;
                            case "transfer-enconding":
                                myRequest.TransferEncoding = headers[key];
                                break;
                            case "content-length":
                            case "expect":
                            case "host":
                                break;
                            default:
                                myRequest.Headers[key] = headers[key];
                                break;
                        }
                    }
                }
                myRequest.AllowAutoRedirect = false;
                myRequest.ContentLength = body.Length;
                myRequest.Method = "POST";
                // For very very bad network environment :D
                myRequest.Timeout = 10000;

                // If user set proxy for KCV, use it.
                if (Grabacr07.KanColleViewer.Models.Settings.Current.ProxySettings.IsEnabled)
                    myRequest.Proxy = new WebProxy(Settings.Current.ProxySettings.Host, Settings.Current.ProxySettings.Port);

                // Write body to our request from Fiddler.
                Stream requestStream = myRequest.GetRequestStream();
                requestStream.Write(body, 0, body.Length);
                requestStream.Close();

                // Get response.
                var myResponse = (HttpWebResponse)myRequest.GetResponse();
                StreamReader sr = new StreamReader(myResponse.GetResponseStream());
                byte[] myResponseBodyByte = Encoding.UTF8.GetBytes(sr.ReadToEnd());
                sr.Close();

                // Hmmm, we need to modify one response headers.
                WebHeaderCollection myResponseHeaders = adjustHeaders(myResponse.Headers);
                foreach(string headerKey in myResponseHeaders.AllKeys)
                {
                    oSession.oResponse.headers[headerKey] = myResponseHeaders[headerKey];
                }
                
                // Set our own response to Fiddler.
                oSession.ResponseBody = myResponseBodyByte;
                // All done here !
                return true;
            }
            catch(Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                return false;
            }
        }

        static WebHeaderCollection adjustHeaders(WebHeaderCollection header)
        {
            WebHeaderCollection headers = new WebHeaderCollection();
            foreach (var key in header.AllKeys)
            {
                switch (key.ToLower())
                {
                    // We need close the connection(maybe).
                    case "connection":
                        headers.Add(key, "Close");
                        break;
                    default:
                        headers.Add(key, header[key]);
                        break;
                }
            }
            return headers;
        }

        public static void Shutdown()
        {
            FiddlerApplication.BeforeRequest -= FiddlerApplication_BeforeRequest;
        }
    }
}
