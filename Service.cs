// Scribd.NET C# Library
// Copyright (c) 2010 James Paul Duncan and Peter Protebic
// The MIT License (MIT)
// Permission is hereby granted, free of charge, to any person obtaining a 
// copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE. 
// http://www.jpaulduncan.com

using System;
using System.Text;
using System.IO;
using System.Net;
using System.Xml;
namespace Scribd.Net
{
    /// <summary>
    /// The entry point to the Scribd API service
    /// </summary>
    public sealed class Service
    {
        #region constructors

        /// <summary>
        /// private default constructor
        /// </summary>
        private Service() { }

        /// <summary>
        /// Singleton
        /// </summary>
        internal static Service Instance
        {
            get
            {
                return InternalService.m_instance;
            }
        }

        #endregion constructors

        #region Events

        /// <summary>
        /// Raised when an error occurs.
        /// </summary>
        public static event EventHandler<ScribdEventArgs> Error;

        /// <summary>
        /// Occurs before a post is made to Scribd.
        /// </summary>
        public static event EventHandler<ServicePostEventArgs> BeforePost;

        /// <summary>
        /// Occurs after a post is made to Scribd.
        /// </summary>
        public static event EventHandler<ServicePostEventArgs> AfterPost;

        /// <summary>
        /// Notifies subscribers of an error
        /// </summary>
        /// <param name="errorCode">Error code returned by Scribd.</param>
        /// <param name="message">Message associated to the error code.</param>
        internal static void OnErrorOccurred(int errorCode, string message)
        {
            if (Error != null)
            {
                Error(Service.Instance, new ScribdEventArgs(errorCode, message));
            }
        }
        /// <summary>
        /// Notifies subscribers of an error
        /// </summary>
        /// <param name="errorCode">Error code returned by Scribd.</param>
        /// <param name="message">Message associated to the error code.</param>
        /// <param name="ex">Any exception associated with the error.</param>
        internal static void OnErrorOccurred(int errorCode, string message, Exception ex)
        {
            if (Error != null)
            {
                Error(Service.Instance, new ScribdEventArgs(errorCode, message, ex));
            }
        }

        /// <summary>
        /// Notifies subscribers that a post is about to happen.
        /// </summary>
        /// <param name="arguments">Arguments for the call</param>
        internal static void OnBeforePost(ServicePostEventArgs arguments)
        {
            if (BeforePost != null)
            {
                BeforePost(Service.Instance, arguments);
            }
        }

        /// <summary>
        /// Notifies a subscriber that a post has already happened.
        /// </summary>
        /// <param name="arguments">Arguments for the call</param>
        internal static void OnAfterPost(ServicePostEventArgs arguments)
        {
            if (AfterPost != null)
            {
                AfterPost(Service.Instance, arguments);
            }
        }

        #endregion Events

        #region Static settings

        /// <summary>
        /// The version of this assembly.
        /// </summary>
        public static Version Version
        {
            get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version; }
        }

        /// <summary>
        /// Proxy 
        /// </summary>
        public static IWebProxy WebProxy { get; set; }

        /// <summary>
        /// Indicates that all calls will include the 
        /// api_sig parameter.  Ensure Service.SecretKey is specified.
        /// </summary>
        public static bool EnforceSigning { get; set; }

        /// <summary>
        /// URL of the Scribd API service
        /// </summary>
        public static string APIUrl { get { return Properties.Resources.API_URL; } }

        /// <summary>
        /// Scribd API Key
        /// </summary>
        public static string APIKey { get; set; }

        /// <summary>
        /// Your Scribd Publisher Id
        /// </summary>
        public static string PublisherID { get; set; }

        /// <summary>
        /// Secret key for signing calls. Seting value via this setter is less secure than using SecretKeyBytes setter.
        /// </summary>
        public static string SecretKey
        {
            get
            {
                return (SecretKeyBytes != null) ? "do not use" : null;
            }
            set
            {
                SecretKeyBytes = System.Text.Encoding.Default.GetBytes(value);
            }
        }

        /// <summary>
        /// Secret key for signing calls. This is a byte array version of the secret key string.
        /// </summary>
        public static byte[] SecretKeyBytes { get; set; }

        /// <summary>
        /// Location to place temporary files for uploading.
        /// </summary>
        public static string TemporaryDirectoryPath { get; set; }

        /// <summary>
        /// The User-Agent string sent to Scribd.
        /// </summary>
        public static string UserAgent
        {
            get
            {
                return string.Format("Scribd.Net Library {0}", Scribd.Net.Service.Version.ToString());
            }
        }

        /// <summary>
        /// User instance
        /// </summary>
        internal User InternalUser { get; set; }

        /// <summary>
        /// User who is currently logged in.
        /// </summary>
        public static User User
        {
            get
            {
                return Instance.InternalUser;
            }
        }

        /// <summary>
        /// Helper function to "Slurpify" a link.
        /// </summary>
        /// <param name="url">The URL of the document you wish to slurp.</param>
        /// <returns>A Slurpified <see cref="T:System.string"/></returns>
        public static string Slurpify(string url)
        {
            return Slurpify(url, DisplayMode.Scribd, false);
        }

        /// <summary>
        /// Helper function to "Slurpify" a link.
        /// </summary>
        /// <param name="url">The URL of the document you wish to slurp.</param>
        /// <param name="displayMode">How a document is displayed on Scribd</param>
        /// <param name="isPrivate">Public or private display of the document</param>
        /// <returns>A Slurpified <see cref="T:System.string"/></returns>
        public static string Slurpify(string url, DisplayMode displayMode, bool isPrivate)
        {
            if (!string.IsNullOrEmpty(Service.PublisherID))
            {
                string _url = @"http://www.scribd.com/slurp?url={0}&display_mode={1}&privacy={2}&publisher_id={3}";
                return string.Format(_url, url, displayMode.ToString().ToLower(), isPrivate == true ? "private" : "public", Service.PublisherID);
            }

            // Notify users
            OnErrorOccurred(10004, Properties.Resources.ERR_NO_PUBLISHERID);

            return string.Empty;
        }

        #endregion Static Settings

        /// <summary>
        /// Send a request to Scribd 
        /// </summary>
        /// <param name="request">The request data</param>
        /// <returns><see cref="T:Scribd.Net.Response"/></returns>
        internal Response PostRequest(Request request)
        {
            // Get started
            Response _result = null;

            // Set up our Event arguments for subscribers.
            ServicePostEventArgs _args = new ServicePostEventArgs(request.RESTCall, request.MethodName);

            // Give subscribers a chance to stop this before it gets ugly.
            OnBeforePost(_args);

            if (!_args.Cancel)
            {
                // Ensure we have the min. necessary params.
                if (string.IsNullOrEmpty(Service.APIKey))
                {
                    OnErrorOccurred(10000, Properties.Resources.ERR_NO_APIKEY);
                    return _result;
                }
                else if (Service.SecretKeyBytes == null)
                {
                    OnErrorOccurred(10001, Properties.Resources.ERR_NO_SECRETKEY);
                    return _result;
                }

                if (!request.SpecifiedUser)
                {
                // Hook up our current user.
                request.User = InternalUser;
                }

                try
                {
                    // Set up our client to call API
                    using (WebClient _client = new WebClient())
                    {
                        _client.Headers.Add("user-agent", Service.UserAgent);

                        // Set up the proxy, if available.
                        if (Service.WebProxy != null) { _client.Proxy = Service.WebProxy; }

                        // Make the call and get the response.
                        string _xml = string.Empty;
                        // Special case - need to upload multi-part POST
                        if (request.MethodName == "docs.upload")
                        {
                            // Get our filename from the request, then dump it!
                            // (Scribd doesn't like passing the literal "file" param.)
                            string _fileName = request.Parameters["file"];
                            request.Parameters.Remove("file");

                            byte[] _resp = FileUploader(_fileName, request.RESTCall, string.Empty);

                            // Get the response
                            _xml = System.Text.Encoding.Default.GetString(_resp);
                        }
                        else
                        {
                            _xml = _client.DownloadString(request.ToString());
                        }
                        // Load up the resultant XML
                        _result = new Response(_xml);

                        // Are we cool?
                        if (_result.Status != "ok")
                        {
                            foreach (int _code in _result.ErrorList.Keys)
                            {
                                // Let the subscribers know, cuz they care.
                                OnErrorOccurred(_code, _result.ErrorList[_code]);
                            }
                        }

                        // Append the response to the AfterPost event.
                        _args.ResponseXML = _result.InnerXml;
                    }
                }
                catch (Exception ex)
                {
                    // Something unexpected?
                    OnErrorOccurred(666, ex.Message, ex);
                }
                finally
                {
                    OnAfterPost(_args);
                }
            }
            // Load up response here ...
            return _result;
        }

        /// <summary>
        /// Uploads a document into Scribd asynchronously.
        /// </summary>
        /// <param name="request"></param>
        internal void PostFileUploadRequest(Request request)
        {
            // Set up our Event arguments for subscribers.
            ServicePostEventArgs _args = new ServicePostEventArgs(request.RESTCall, request.MethodName);

            // Give subscribers a chance to stop this before it gets ugly.
            OnBeforePost(_args);

            if (!_args.Cancel)
            {
                // Ensure we have the min. necessary params.
                if (string.IsNullOrEmpty(Service.APIKey))
                {
                    OnErrorOccurred(10000, Properties.Resources.ERR_NO_APIKEY);
                    return;
                }
                else if (Service.SecretKeyBytes == null)
                {
                    OnErrorOccurred(10001, Properties.Resources.ERR_NO_SECRETKEY);
                    return;
                }

                if (!request.SpecifiedUser)
                {
                // Hook up our current user.
                request.User = InternalUser;
                }

                try
                {
                    // Set up our client to call API
                    using (WebClient _client = new WebClient())
                    {                       
                        // Set up the proxy, if available.
                        if (Service.WebProxy != null) { _client.Proxy = Service.WebProxy; }

                        // Special case - need to upload multi-part POST
                        if (request.MethodName == "docs.upload" || request.MethodName == "docs.uploadThumb")
                        {
                            // Get our filename from the request, then dump it!
                            // (Scribd doesn't like passing the literal "file" param.)
                            string _fileName = request.Parameters["file"];
                            request.Parameters.Remove("file");

                            // Make that call.
                            _client.UploadProgressChanged += new UploadProgressChangedEventHandler(_client_UploadProgressChanged);
                            _client.UploadFileCompleted += new UploadFileCompletedEventHandler(_client_UploadFileCompleted);
                            _client.UploadFileAsync(new Uri(request.RESTCall), "POST", _fileName);
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Something unexpected?
                    OnErrorOccurred(666, ex.Message, ex);
                }
                finally
                {
                    OnAfterPost(_args);
                }
            }

            return;
        }

        /// <summary>
        /// Parses the response from the async upload.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void _client_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            try
            {
                Document _result = new Document();

                // Get the response
                string _xml = System.Text.Encoding.Default.GetString(e.Result);

                // Load up the resultant XML
                Response _response = new Response(_xml);

                // Are we cool?
                if (_response.Status != "ok")
                {
                    foreach (int _code in _response.ErrorList.Keys)
                    {
                        // Let the subscribers know, cuz they care.
                        OnErrorOccurred(_code, _response.ErrorList[_code]);
                    }
                }
                else
                {
                    // Parse the response
                    if (_response != null && _response.HasChildNodes && _response.ErrorList.Count < 1)
                    {
                        XmlNode _node = _response.SelectSingleNode("rsp");

                        // Data
                        _result.DocumentId = int.Parse(_node.SelectSingleNode("doc_id").InnerText);
                        _result.AccessKey = _node.SelectSingleNode("access_key").InnerText;

                        // Security
                        if (_node.SelectSingleNode("secret_password") != null)
                        {
                            _result.AccessType = AccessTypes.Private;
                            _result.SecretPassword = _node.SelectSingleNode("secret_password").InnerText;
                        }
                    }
                }

                // Notify those who care.
                Document.OnUploaded(_result);
            }
            catch (Exception _ex)
            {
                OnErrorOccurred(666, _ex.Message, _ex);
            }

        }
        internal void _client_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            // Notify subscribers.
            Document.OnUploadProgressChanged(sender, e);
        }

        /// <summary>
        /// Verifies the user is logged in.
        /// </summary>
        /// <returns>True on success</returns>
        public bool IsUserLoggedIn
        {
            get
            {
                return (this.InternalUser != null && (!string.IsNullOrEmpty(this.InternalUser.Name)));
            }
        }

        #region Thread-safe singleton
        /// <summary>
        /// Implements thread-safe singleton pattern
        /// </summary>
        class InternalService
        {
            internal static Service m_instance = new Service();
            static InternalService()
            {
                m_instance.InternalUser = new User();
            }
        }
        #endregion Thread-safe singleton

        #region FileUploader

       
            /// <summary>
            /// Uploads a file.
            /// </summary>
            /// <param name="fileName"></param>
            /// <param name="url"></param>
            /// <param name="contentType"></param>
            /// <returns></returns>
            internal static byte[] FileUploader(string fileName, string url, string contentType)
            {
                return FileUploader(fileName, url, contentType, new CookieContainer());
            }

            /// <summary>
            /// Uploads a file.
            /// </summary>
            /// <param name="fileName"></param>
            /// <param name="url"></param>
            /// <param name="contentType"></param>
            /// <param name="cookies"></param>
            /// <returns></returns>
            internal static byte[] FileUploader(string fileName, string url, string contentType, CookieContainer cookies)
            {

                if ((contentType == null) ||
                    (contentType.Length == 0))
                {
                    contentType = "application/octet-stream";
                }

                Uri _uri = new Uri(url);

                string _boundary = "----------" + DateTime.Now.Ticks.ToString("x");

                HttpWebRequest _request = (HttpWebRequest)WebRequest.Create(_uri);
                _request.CookieContainer = cookies;
                _request.ContentType = "multipart/form-data; boundary=" + _boundary;
                _request.Method = "POST";
                _request.KeepAlive = false;
                _request.AllowWriteStreamBuffering = true;
                _request.Timeout = System.Threading.Timeout.Infinite;
                _request.Accept = "*/*";
                _request.AllowAutoRedirect = false;
                _request.UserAgent = Service.UserAgent;
                _request.ProtocolVersion = HttpVersion.Version10;
                
                // Build up the post message header
                StringBuilder _stringBuilder = new StringBuilder();
                _stringBuilder.Append("--");
                _stringBuilder.Append(_boundary);
                _stringBuilder.Append("\r\n");
                _stringBuilder.Append("Content-Disposition: form-data; name=\"file\"");
                _stringBuilder.Append("; filename=\"");
                _stringBuilder.Append(Path.GetFileName(fileName));
                _stringBuilder.Append("\"\r\n");
                _stringBuilder.Append("Content-Type: ");
                _stringBuilder.Append(contentType);
                _stringBuilder.Append("\r\n\r\n");

                string _postHeader = _stringBuilder.ToString();
                byte[] _postHeaderBytes = Encoding.UTF8.GetBytes(_postHeader);

                // Build the trailing boundary string as a byte array
                // ensuring the boundary appears on a line by itself
                byte[] _boundaryBytes =
                     Encoding.ASCII.GetBytes("\r\n--" + _boundary + "--\r\n");

                FileStream _fileStream = new FileStream(fileName,
                                            FileMode.Open, FileAccess.Read);
                long _length = _postHeaderBytes.Length + _fileStream.Length +
                                                       _boundaryBytes.Length;
                _request.ContentLength = _length;

                Stream _requestStream = _request.GetRequestStream();

            //                _requestStream.Write(_postHeaderBytes, 0, _postHeaderBytes.Length);

                // Write out our post header
                for (int _i = 0; _i < _postHeaderBytes.Length; _i++)
                {
                    _requestStream.WriteByte(_postHeaderBytes[_i]);
                    // Notify subscribers.
                    Document.OnUploadProgressChanged(null, null);
                }

                // Write out the file contents
                byte[] _buffer = new Byte[checked((uint)Math.Min(4096,
                                         (int)_fileStream.Length))];
                int _bytesRead = 0;
                while ((_bytesRead = _fileStream.Read(_buffer, 0, _buffer.Length)) != 0)
                    _requestStream.Write(_buffer, 0, _bytesRead);

                // Write out the trailing boundary
                _requestStream.Write(_boundaryBytes, 0, _boundaryBytes.Length);
                WebResponse _response = _request.GetResponse();
                Stream _responseStream = _response.GetResponseStream();
                StreamReader _streamReader = new StreamReader(_responseStream);

                string _responseData = _streamReader.ReadToEnd();

                ASCIIEncoding _encoding = new ASCIIEncoding();

                return _encoding.GetBytes(_responseData);
            }
        
        #endregion FileUploader
    }
}