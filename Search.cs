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
    /// Searches Scribd for documents.
    /// </summary>
    public static class Search
    {
        #region Criteria

        /// <summary>
        /// Criteria used for a search.
        /// </summary>
        public sealed class Criteria
        {
            /// <summary>
            /// ctor
            /// </summary>
            public Criteria() { }

            /// <summary>
            /// Terms to search for.
            /// </summary>
            public string Query { get; set; }

            /// <summary>
            /// Scope of search.
            /// </summary>
            public SearchScope Scope { get; set; }

            /// <summary>
            /// Maximum number of results desired.
            /// </summary>
            public int MaxResults { get; set; }

            /// <summary>
            /// Document index of results you wish to start 
            /// from.
            /// </summary>
            public int StartIndex { get; set; }
        }

        #endregion Criteria

        #region Results

        /// <summary>
        /// The results of a search call to Scribd.
        /// </summary>
        public sealed class Result
        {
            #region private members

            private Criteria m_criteria = null;
            private int m_totalAvailable = 0;
            private int m_firstResultIndex = 0;
            private List<Document> m_documentList = new List<Document>();

            #endregion private members

            /// <summary>
            /// ctor
            /// </summary>
            /// <param name="criteria"><see cref="Criteria"/> used in search.</param>
            /// <param name="documents">Documents returned from search.</param>
            /// <param name="totalAvailable">Number of documents available from search.</param>
            /// <param name="firstResultIndex">Index of the first document result in the search.</param>
            internal Result(Criteria criteria, List<Document> documents, int totalAvailable, int firstResultIndex)
            {
                m_criteria = criteria;
                m_documentList = documents;
                m_totalAvailable = totalAvailable;
                m_firstResultIndex = firstResultIndex;
            }

            /// <summary>
            /// Query used in search.
            /// </summary>
            public Criteria Criteria { get { return m_criteria; } }

            /// <summary>
            /// Documents returned from search.
            /// </summary>
            public List<Document> Documents { get { return m_documentList; } }

            /// <summary>
            /// Number of documents available from search.
            /// </summary>
            public int TotalAvailable { get { return m_totalAvailable; } }

            /// <summary>
            /// Index of the first document result in the search.
            /// </summary>
            public int FirstResultIndex { get { return m_firstResultIndex; } }

        }

        #endregion Results

        #region Static methods

        /// <summary>
        /// Indicates the size of the thumbnail image set on each document 
        /// returned in the search result.
        /// </summary>
        public static System.Drawing.Size ThumbnailSize{get;set;}

        /// <summary>
        /// This method searches for the specified query within the scribd documents
        /// </summary>
        /// <param name="criteria">A <see cref="T:Search.Criteria"/> used in search</param>
        /// <returns>The <see cref="T:Search.Result"/></returns>
        public static Result Find(Criteria criteria)
        {
            return InnerFind(criteria.Query, criteria.Scope, criteria.MaxResults, criteria.StartIndex);
        }

        /// <summary>
        /// This method searches for the specified query within the scribd documents
        /// </summary>
        /// <param name="query">Criteria used in search</param>
        /// <returns>The <see cref="T:Search.Result"/></returns>
        public static Result Find(string query)
        {
            return InnerFind(query, SearchScope.All, 10, 1);
        }

        /// <summary>
        /// This method searches for the specified query within the scribd documents
        /// </summary>
        /// <param name="query">Criteria used in search</param>
        /// <param name="scope">Whether to search all of scribd OR just within one 
        /// user/'s documents. If scope is set to "user" and session_key is not provided, 
        /// the documents uploaded by the API user will be searched. If scope is set 
        /// to "user" and session_key is given, the documents uploaded by the session 
        /// user will be searched. If scope is "all", only public documents will be 
        /// searched. Set to "user" by default.</param>
        /// <returns>The <see cref="T:Search.Result"/></returns>
        public static Result Find(string query, SearchScope scope)
        {
            return InnerFind(query, scope, 10, 1);
        }

        /// <summary>
        /// This method searches for the specified query within the scribd documents
        /// </summary>
        /// <param name="query">Criteria used in search</param>
        /// <param name="scope">Whether to search all of scribd OR just within one 
        /// user/'s documents. If scope is set to "user" and session_key is not provided, 
        /// the documents uploaded by the API user will be searched. If scope is set 
        /// to "user" and session_key is given, the documents uploaded by the session 
        /// user will be searched. If scope is "all", only public documents will be 
        /// searched. Set to "user" by default.</param>
        /// <param name="maxResults">Number of results to return: Default 10, max 1000</param>
        /// <param name="startIndex">Number to start at: Default 1. Cannot exceed 1000</param>
        /// <returns>The <see cref="T:Search.Result"/></returns>
        public static Result Find(string query, SearchScope scope, int maxResults, int startIndex)
        {
            return InnerFind(query, scope, maxResults, startIndex);
        }

        /// <summary>
        /// This method searches for the specified query within the scribd documents
        /// </summary>
        /// <param name="query">Criteria used in search</param>
        /// <param name="scope">Whether to search all of scribd OR just within one 
        /// user/'s documents. If scope is set to "user" and session_key is not provided, 
        /// the documents uploaded by the API user will be searched. If scope is set 
        /// to "user" and session_key is given, the documents uploaded by the session 
        /// user will be searched. If scope is "all", only public documents will be 
        /// searched. Set to "user" by default.</param>
        /// <param name="maxResults">Number of results to return: Default 10, max 1000</param>
        /// <param name="startIndex">Number to start at: Default 1. Cannot exceed 1000</param>
        /// <returns>The <see cref="T:Search.Result"/></returns>
        internal static Result InnerFind(string query, SearchScope scope, int maxResults, int startIndex)
        {
            if (Search.ThumbnailSize == null) { Search.ThumbnailSize = new System.Drawing.Size(71, 100); }

            // Validate params
            if (maxResults > 1000) { maxResults = 1000; }
            if (startIndex < 1 || startIndex > 1000) { startIndex = 1; }

            List<Document> _documents = new List<Document>();
            int _totalAvailable = 0, _firstResultIndex = 0;

            // Generate request
            using (Request _request = new Request(Service.Instance.InternalUser))
            {
                _request.MethodName = "docs.search";
                _request.Parameters.Add("query", query);
                _request.Parameters.Add("num_results", maxResults.ToString());
                _request.Parameters.Add("num_start", startIndex.ToString());
                _request.Parameters.Add("scope", Enum.GetName(typeof(SearchScope),scope).ToLower());

                // Get Results
                using (Response _response = Service.Instance.PostRequest(_request))
                {
                    // Parse response
                    if (_response != null && _response.HasChildNodes && _response.ErrorList.Count < 1)
                    {
                        _totalAvailable = int.Parse(_response.SelectSingleNode(@"/rsp/result_set").Attributes["totalResultsAvailable"].Value);
                        _firstResultIndex = int.Parse(_response.SelectSingleNode(@"/rsp/result_set").Attributes["firstResultPosition"].Value);

                        XmlNodeList _list = _response.GetElementsByTagName("result");
                        if (_list.Count > 0)
                        {
                            foreach (XmlNode _node in _list)
                            {
                                Document _item = new Document(0, Search.ThumbnailSize);
                                _item.DocumentId = int.Parse(_node.SelectSingleNode("doc_id").InnerText);
                                _item.Title = _node.SelectSingleNode("title").InnerText.Trim();
                                _item.Description = _node.SelectSingleNode("description").InnerText.Trim();
                                _item.ThumbnailUrl = new Uri(_node.SelectSingleNode("thumbnail_url").InnerText);

                                if (_node.SelectSingleNode("access_key") != null)
                                {
                                    _item.AccessKey = _node.SelectSingleNode("access_key").InnerText;
                                }

                                // Tags
                                string _tags = _node.SelectSingleNode("tags").InnerText;
                                foreach (string _tag in _tags.Split(','))
                                {
                                    _item.TagList.Add(_tag.Trim());
                                }

                                _documents.Add(_item);
                            }
                        }
                    }
                }
            }

            // Set the criteria used to the result
            Criteria _criteria = new Criteria();
            _criteria.Query = query;
            _criteria.Scope = scope;
            _criteria.MaxResults = maxResults;
            _criteria.StartIndex = startIndex;

            return new Result(_criteria, _documents, _totalAvailable, _firstResultIndex);
        }

        #endregion Static methods
    }
}