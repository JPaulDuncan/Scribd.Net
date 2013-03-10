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
using System.Xml;
namespace Scribd.Net
{
    /// <summary>
    /// The response from Scribd.
    /// </summary>
    internal sealed class Response : XmlDocument, IDisposable
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Response(string xml)
            : base()
        {
            try
            {
                this.InnerXml = xml;
            }
            catch (System.Exception ex)
            {
                Service.OnErrorOccurred(666, ex.Message, ex);
            }
        }

        /// <summary>
        /// Status of the response
        /// </summary>
        public string Status
        {
            get
            {
                if (this.HasChildNodes)
                {
                    XmlNode _node = this.SelectSingleNode(@"/rsp");
                    return _node.Attributes["stat"].Value;
                }
                return "unknown";
            }
        }

        /// <summary>
        /// Errors associated with the response.
        /// </summary>
        public Dictionary<int, string> ErrorList
        {
            get
            {
                Dictionary<int, string> _result = new Dictionary<int, string>();
                if (this.HasChildNodes)
                {
                    foreach (XmlNode _node in this.GetElementsByTagName("error"))
                    {
                        int _code = int.Parse(_node.Attributes["code"].Value);
                        string _msg = _node.Attributes["message"].Value;
                        _result.Add(_code, _msg);
                    }
                }
                return _result;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            // Sink it.
        }

        #endregion
    }
}