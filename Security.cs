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
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Scribd.Net
{
    /// <summary>
    /// Security API functions for Scribd.
    /// </summary>
    public sealed class Security
    {

        /// <summary>
        /// This method allows you to disable a user's access to a secure document, 
        /// or to re-enable it after a previous call. It is not necessary to grant 
        /// initial access to a document - that is done through the embed code.
        /// </summary>
        /// <param name="allowed">Disables or enables access to all user's documents.</param>
        /// <returns></returns>
        public static bool SetAccess(bool allowed)
        {
            return Security.SetAccess(allowed, 0);
        }

        /// <summary>
        /// This method allows you to disable a user's access to a secure document, 
        /// or to re-enable it after a previous call. It is not necessary to grant 
        /// initial access to a document - that is done through the embed code.
        /// </summary>
        /// <param name="allowed">Disables or enables access to the given document identifier.</param>
        /// <param name="documentId">The document identifier.</param>
        /// <returns></returns>
        public static bool SetAccess(bool allowed, int documentId)
        {
            bool _result = false;
            // Set up our request
            using (Request _request = new Request())
            {
                _request.MethodName = "security.setAccess";
                _request.Parameters.Add("user_identifier", Service.User.UserName);
                _request.Parameters.Add("allowed", allowed ? "1" : "0");
                if (documentId > 0) { _request.Parameters.Add("doc_id", documentId.ToString()); }

                // Grab our response
                using (Response _response = Service.Instance.PostRequest(_request))
                {
                    // Parse the response
                    if (_response != null && _response.HasChildNodes && _response.ErrorList.Count < 1)
                    {
                        _result = true;
                    }
                }
            }

            return _result;
        }

        /// <summary>
        /// This method can be used for tracking and verification purposes. It gets the list 
        /// of the user_identifiers currently authorized to view the given document.
        /// </summary>
        /// <param name="documentId">The document identifier.</param>
        /// <returns></returns>
        public static List<string> GetDocumentAccessList(int documentId)
        {
            List<string> _result = new List<string>();

            // Generate request
            using (Request _request = new Request(Service.Instance.InternalUser))
            {
                _request.MethodName = "security.getDocumentAccessList";
                _request.Parameters.Add("doc_id", documentId.ToString());

                // Get Results
                using (Response _response = Service.Instance.PostRequest(_request))
                {
                    // Parse response
                    if (_response != null && _response.HasChildNodes && _response.ErrorList.Count < 1)
                    {
                        XmlNodeList _list = _response.GetElementsByTagName("result");
                        if (_list.Count > 0)
                        {
                            foreach (XmlNode _node in _list)
                            {
                                string _item = _node.SelectSingleNode("user_identifier").InnerText;
                                _result.Add(_item);
                            }
                        }
                    }
                }
            }

            return _result;
        }

        /// <summary>
        /// This method can be used for tracking and verification purposes. 
        /// It gets the list of the secure documents that the current user 
        /// is allowed to access.
        /// </summary>
        /// <returns></returns>
        public static List<UserAccessItem> GetUserAccessList()
        {
            List<UserAccessItem> _result = new List<UserAccessItem>();

            // Generate request
            using (Request _request = new Request(Service.Instance.InternalUser))
            {
                _request.MethodName = "security.getUserAccessList";
                _request.Parameters.Add("user_identifier", Service.User.UserName);
                
                // Get Results
                using (Response _response = Service.Instance.PostRequest(_request))
                {
                    // Parse response
                    if (_response != null && _response.HasChildNodes && _response.ErrorList.Count < 1)
                    {
                        XmlNodeList _list = _response.GetElementsByTagName("result");
                        
                        if (_list.Count > 0)
                        {
                            foreach (XmlNode _node in _list)
                            {
                                UserAccessItem _item = new UserAccessItem();
                                _item.DocumentId = int.Parse(_node.SelectSingleNode("doc_id").InnerText);
                                _item.SecretPassword = _node.SelectSingleNode("secret_password").InnerText;
                                _item.AccessKey = _node.SelectSingleNode("access_key").InnerText;
                                _item.Title = _node.SelectSingleNode("title").InnerText.Trim();
                                _item.Description = _node.SelectSingleNode("description").InnerText.Trim();
                                
                                switch (_node.SelectSingleNode("conversion_status").InnerText.Trim().ToLower())
                                {
                                    case "displayable": _item.ConversionStatus = ConversionStatusTypes.Displayable; break;
                                    case "done": _item.ConversionStatus = ConversionStatusTypes.Done; break;
                                    case "error": _item.ConversionStatus = ConversionStatusTypes.Error; break;
                                    case "processing": _item.ConversionStatus = ConversionStatusTypes.Processing; break;
                                    case "published": _item.ConversionStatus = ConversionStatusTypes.Published; break;
                                    default: _item.ConversionStatus = ConversionStatusTypes.None_Specified; break;
                                }

                                _item.PageCount = int.Parse(_node.SelectSingleNode("page_count").InnerText);

                                _result.Add(_item);
                            }
                        }
                    }
                }
            }

            return _result;
        }
    }

    #region User-Access List Item

    /// <summary>
    /// Represents an individual list item.
    /// </summary>
    public sealed class UserAccessItem
    {
        /// <summary>
        /// Ctor
        /// </summary>
        internal UserAccessItem() { }

        /// <summary>
        /// The document identifier.
        /// </summary>
        public int DocumentId { get; internal set; }

        /// <summary>
        /// The password associated to this document.
        /// </summary>
        public string SecretPassword { get; internal set; }

        /// <summary>
        /// The access key to this document.
        /// </summary>
        public string AccessKey { get; internal set; }

        /// <summary>
        /// The title of thid document.
        /// </summary>
        public string Title { get; internal set; }

        /// <summary>
        /// A brief description of this document.
        /// </summary>
        public string Description { get; internal set; }

        /// <summary>
        /// The conversion state of this document.
        /// </summary>
        public ConversionStatusTypes ConversionStatus { get; internal set; }

        /// <summary>
        /// Number of pages associated to this document.
        /// </summary>
        public int PageCount { get; internal set; }
    }

    #endregion

}
