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
    /// Encapsulates a Scribd Collection. Id, name, number of documents
    /// </summary>
    public class Collection
    {
        #region constructors

        /// <summary>
        /// ctor
        /// </summary>
        private Collection()
        {
        }

        private Collection(int id, string name, int count)
            : this()
        {
            this.ID = id;
            this.Name = name;
            this.docCount = count;
        }

        #endregion

        #region properties

        /// <summary>
        /// The 'id' of the category
        /// </summary>
        public int ID { get; private set;  }

        /// <summary>
        /// The Name of the category
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// number of documents in this category
        /// </summary>
        public int docCount { get; private set; }

        #endregion

        #region retrieve collections

        /// <summary>
        /// Defines the scope of collections
        /// </summary>
        public enum Scope
        {
            /// <summary>
            /// Indicates this collection is publicly visible.
            /// </summary>
            Public,
            /// <summary>
            /// Indicates this collection is privately visible.
            /// </summary>
            Private,
            /// <summary>
            /// Indicates this collection is publicly and privately visible.
            /// </summary>
            Both
        }

        /// <summary>
        /// This method retrieves a list of scribd 'Collections' associated with current user.
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static ICollection<Collection> GetCollections(Scope scope)
        {
            return GetCollections(null, scope);
        }

        /// <summary>
        /// This method retrieves a list of scribd 'Collections' associated with the user.
        /// </summary>
        /// <param name="user">if null then the current user is assumed</param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static ICollection<Collection> GetCollections(User user, Scope scope)
        {
            ICollection<Collection> _result = new List<Collection>();

            // Build the request
            using (Request _request = new Request(user, true))
            {
                _request.MethodName = "docs.getCollections";

                if (scope == Scope.Public)
                {
                    _request.Parameters.Add("scope", "public");
                }
                else if (scope == Scope.Private)
                {
                    _request.Parameters.Add("scope", "private");
                }

                // Get the response
                using (Response _response = Service.Instance.PostRequest(_request))
                {
                    // Parse the response
                    if (_response != null && _response.HasChildNodes && _response.ErrorList.Count < 1)
                    {
                        XmlNodeList _list = _response.GetElementsByTagName("result");
                        if (_list.Count > 0)
                        {
                            foreach (XmlNode _node in _list)
                            {
                                // this call to Parse will recurse into any subcategories
                                Collection col = Collection.Parse(_node, null);
                                _result.Add(col);
                            }
                        }
                    }
                }
            }

            return _result;
        }

        #endregion

        #region private

        private static Collection Parse(XmlNode node, Category parent)
        {
            int id = int.Parse(node.SelectSingleNode("collection_id").InnerText);
            string name = node.SelectSingleNode("collection_name").InnerText.Trim();
            int count = int.Parse(node.SelectSingleNode("doc_count").InnerText);

            Collection collection = new Collection(id, name, count);
            return collection;
        }

        #endregion  
    }
}
