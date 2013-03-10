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
    /// Event arguments for document events.
    /// </summary>
    public sealed class DocumentEventArgs : EventArgs
    {
        private Document m_document;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="document">Document related to the event.</param>
        internal DocumentEventArgs(Document document)
            : base()
        {
            m_document = document;
        }

        /// <summary>
        /// Document related to the event.
        /// </summary>
        public Document Document { get { return m_document; } }

        /// <summary>
        /// Cancel the event
        /// </summary>
        public bool Cancel { get; set; }
    }
}