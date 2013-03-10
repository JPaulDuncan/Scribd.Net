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

namespace Scribd.Net
{
    /// <summary>
    /// Event argument for Service posting events.
    /// </summary>
    public sealed class ServicePostEventArgs : EventArgs
    {
        private string m_responseXml;
        private string m_restCall;
        private string m_methodName;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="restCall">Call being made to the service</param>
        /// <param name="methodName">Name of the method being called</param>
        internal ServicePostEventArgs(string restCall, string methodName)
            : base()
        {
            m_restCall = restCall;
            m_methodName = methodName;
        }

        /// <summary>
        /// Cancel a post
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Call being made to the service.
        /// </summary>
        public string RESTCall { get { return m_restCall; } }

        /// <summary>
        /// Method being called.
        /// </summary>
        public string MethodName { get { return m_methodName; } }

        /// <summary>
        /// Response document from the service
        /// </summary>
        public string ResponseXML { get { return m_responseXml; } internal set { m_responseXml = value; } }

    }
}