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

using System.Collections.Generic;
using System.Text;
using System.Xml;
using System;
using System.IO;
using System.Drawing;
using System.Net;

namespace Scribd.Net
{
    /// <summary>
    /// A Scribd document 
    /// </summary>
    [Serializable()]
    public sealed class Document
    {
        private byte[] m_thumbnailData;
        private byte[] m_largeImageData;
        private Size m_thumbnailSize = new Size(71, 100);

        #region constructors

        /// <summary>
        /// ctor
        /// </summary>
        public Document() 
        {
            this.TagList = new List<string>();
            this.DownloadFormats = new List<string>();
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="documentId"></param>
        public Document(int documentId)
            : this(documentId, new Size(71, 100)) { }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="thumbnailSize"></param>
        public Document(int documentId, Size thumbnailSize)
            :this()
        {
            Document _temp = Document.Download(documentId);
            this.AccessKey = _temp.AccessKey;
            this.AccessType = _temp.AccessType;
            this.ConversionStatus = _temp.ConversionStatus;
            this.Description = _temp.Description;
            this.DocumentId = _temp.DocumentId;
            this.DocumentType = _temp.DocumentType;
            this.License = _temp.License;
            this.RevisionId = _temp.RevisionId;
            this.SecretPassword = _temp.SecretPassword;
            this.ThumbnailUrl = _temp.ThumbnailUrl;
            this.Title = _temp.Title;
            this.TagList.AddRange(_temp.TagList.ToArray());
            this.m_thumbnailSize = thumbnailSize;
        }

        #endregion constructors

        #region events

        /// <summary>
        /// Occurs while asynchronously uploading a document to Scribd.
        /// </summary>
        public static event EventHandler<UploadProgressChangedEventArgs> UploadProgressChanged;

        /// <summary>
        /// Raised before a document is saved.
        /// </summary>
        public static event EventHandler<DocumentEventArgs> BeforeSave;

        /// <summary>
        /// Raised when a document is saved.
        /// </summary>
        public static event EventHandler<DocumentEventArgs> Saved;

        /// <summary>
        /// Raised when a document is uploaded.
        /// </summary>
        public static event EventHandler<DocumentEventArgs> Uploaded;

        /// <summary>
        /// Raised before a document download is attempted.
        /// </summary>
        public static event EventHandler<DocumentEventArgs> BeforeDownload;

        /// <summary>
        /// Raised when a document is downloaded.
        /// </summary>
        public static event EventHandler<DocumentEventArgs> Downloaded;

        /// <summary>
        /// Raised after a thumbnail has been uploaded.
        /// </summary>
        public static event EventHandler ThumbnailUploaded;

        /// <summary>
        /// Notify subscribers of intent to save.
        /// </summary>
        /// <param name="arguments"></param>
        internal void OnBeforeSave(DocumentEventArgs arguments)
        {
            if (BeforeSave != null)
            {
                BeforeSave(this, arguments);
            }
        }

        /// <summary>
        /// Notify subscribers of save.
        /// </summary>
        internal void OnSaved()
        {
            if (Saved != null)
            {
                Saved(this, new DocumentEventArgs(this));
            }
        }

        /// <summary>
        /// Notify subscribers of progress change.
        /// This only functions on an asynchronous call.
        /// </summary>
        /// <param name="sender">Originator of the call.</param>
        /// <param name="args">The progress arguments. <see cref="UploadProgressChangedEventArgs"/></param>
        internal static void OnUploadProgressChanged(object sender, UploadProgressChangedEventArgs args)
        {
            if (UploadProgressChanged != null)
            {
                UploadProgressChanged(sender, args);
            }
        }

        /// <summary>
        /// Notify subscribers of an upload.
        /// </summary>
        /// <param name="document">Uploaded document</param>
        internal static void OnUploaded(Document document)
        {
            if (Uploaded != null)
            {
                Uploaded(document, new DocumentEventArgs(document));
            }
        }

        /// <summary>
        /// Notify subscribers before download
        /// </summary>
        /// <param name="arguments">Document to download</param>
        internal static void OnBeforeDownload(DocumentEventArgs arguments)
        {
            if (BeforeDownload != null)
            {
                BeforeDownload(arguments.Document, arguments);
            }
        }

        /// <summary>
        /// Notify subscribers of a download.
        /// </summary>
        /// <param name="document">Downloaded document</param>
        internal static void OnDownloaded(Document document)
        {
            if (Downloaded != null)
            {
                Downloaded(document, new DocumentEventArgs(document));
            }
        }

        #endregion events

        #region properties

        /// <summary>
        /// Every document on Scribd has a unique document ID. 
        /// You can use this ID to link to the document, among other things.
        /// </summary>
        public int DocumentId { get; set; }

        /// <summary>
        /// Title of the document
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Description of the document
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Permission of the document 
        /// </summary>
        public AccessTypes AccessType { get; set; }

        /// <summary>
        /// The Creative Commons license type 
        /// </summary>
        public CCLicenseTypes License { get; set; }

        /// <summary>
        /// To show ads or not
        /// </summary>
        [Obsolete("Removed from the API")]
        public ShowAdsTypes ShowAds { get; set; }

        /// <summary>
        /// Status of the document's conversion
        /// </summary>
        public ConversionStatusTypes ConversionStatus { get; set; }

        /// <summary>
        /// Tags associated to the document
        /// </summary>
        public List<string> TagList { get; set; }

        /// <summary>
        /// Original type of document
        /// </summary>
        public string DocumentType { get; set; }

        /// <summary>
        /// Link to a JPG that contains a thumbnail of the document.
        /// </summary>
        public Uri ThumbnailUrl { get; set; }

        /// <summary>
        /// The doc_id to save uploaded file as a revision to.
        /// </summary>
        public int RevisionId { get; set; }

        /// <summary>
        /// Every document on Scribd has a unique, secure access key. 
        /// You must know the access key of a document to embed it.
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// Private documents on Scribd have a secret password. 
        /// You use the secret password of a private document to 
        /// link to the private page of the document on Scribd. 
        /// This parameter will only be present if the document is private.
        /// </summary>
        public string SecretPassword { get; set; }

        /// <summary>
        /// Scribd Qualified Publishers have the option to add links on public 
        /// Scribd documents back to their own website. The link appears above 
        /// the document on the Scribd view page. This API parameter allows you 
        /// to control that link. For the parameter to be effective, you must 
        /// signup and be approved for a qualified publisher account.
        /// </summary>
        public Uri LinkBackURL { get; set; }

        /// <summary>
        /// Link to a JPG that contains a snapshop of the document.
        /// </summary>
        public Uri LargeImageURL { get; set; }

        /// <summary>
        /// The bytes of the thumbnail image.
        /// </summary>
        public byte[] ThumbnailImageData
        {
            get
            {
                try
                {
                    // Only get it once per instance.
                    if (null != this.ThumbnailUrl && m_thumbnailData == null)
                    {
                        using (System.Net.WebClient _client = new WebClient())
                        {
                            if (Service.WebProxy != null) { _client.Proxy = Service.WebProxy; }
                            m_thumbnailData = _client.DownloadData(this.ThumbnailUrl);
                        }
                    }
                }
                catch { }
                return m_thumbnailData;

            }
        }

        /// <summary>
        /// The bytes of the large document snapshot image.
        /// </summary>
        public byte[] LargeImageData
        {
            get
            {
                try
                {
                    // Only get it once per instance.
                    if (null != this.LargeImageURL && m_largeImageData == null)
                    {
                        using (System.Net.WebClient _client = new WebClient())
                        {
                            if (Service.WebProxy != null) { _client.Proxy = Service.WebProxy; }
                            m_largeImageData = _client.DownloadData(this.LargeImageURL);
                        }
                    }
                }
                catch { }
                return m_largeImageData;
            }
        }

        /// <summary>
        /// The thumbnail (if available)
        /// </summary>
        public Bitmap ThumbnailImage
        {
            get
            {
                Bitmap _result = new Bitmap(m_thumbnailSize.Width, m_thumbnailSize.Height);
                byte[] _buffer = this.ThumbnailImageData;
                if (_buffer != null)
                {
                    MemoryStream _stream = new MemoryStream(_buffer);
                    _result = new Bitmap(_stream);
                }
                return _result;
            }
        }

        /// <summary>
        /// The category identifier associated to this document.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// The category associated to this document.
        /// </summary>
        public string Category { get; internal set; }
        
        /// <summary>
        /// The number of pages in this document.
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// The name of the author of this document.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// The name of the publisher of this document.
        /// </summary>
        public string Publisher { get; set; }

        /// <summary>
        /// Date document was originally published.
        /// </summary>
        public DateTime WhenPublished { get; set; }

        /// <summary>
        /// Edition of this document.
        /// </summary>
        public string Edition { get; set; }

        /// <summary>
        /// If this is set to True, the upload link that appears in the iPaper menu will be disabled. The default is False.
        /// </summary>
        public bool DisableUploadLink { get; set; }

        /// <summary>
        /// If this is set to True, the print tool that appears in the iPaper menu will be disabled. The default is False.
        /// </summary>
        public bool DisablePrint { get; set; }

        /// <summary>
        /// If this is set to True, the text selection tool will be disabled. The default is False.
        /// </summary>
        public bool DisableSelectText { get; set; }

        /// <summary>
        /// If this is set to True, the option to show the about iPaper dialog that appears in the iPaper menu will be disabled. The default is False.
        /// </summary>
        public bool DisableAboutDialog { get; set; }

        /// <summary>
        /// If this is set to True, the option to show the "Document Info" dialog that appears in the iPaper menu will be disabled. The default is False.
        /// </summary>
        public bool DisableInfoDialog { get; set; }

        /// <summary>
        /// If this is set to True, the user will not be able to change iPaper view modes ("Book Mode", "List Mode", etc.). The default is False. 
        /// </summary>
        public bool DisableViewModeChange { get; set; }

        /// <summary>
        /// If this is set to True, the related docs panel will not be shown. The default is False.
        /// </summary>
        public bool DisableRelatedDocuments { get; set; }

        /// <summary>
        /// Settings for the document used in a store.
        /// </summary>
        public DocumentStore StoreSettings { get; set; }

        /// <summary>
        /// Download formats available.
        /// </summary>
        public List<string> DownloadFormats { get; set; }

        #endregion properties

        #region Document Management

        /// <summary>
        /// Returns the Flash embedding code for this document.
        /// </summary>
        /// <returns>String</returns>
        public string GetEmbedCode() { return GetEmbedCode(1, ViewMode.List, "", ""); }

        /// <summary>
        /// Returns the Flash embedding code for this document.
        /// </summary>
        /// <param name="startingPage">The page to start on.</param>
        /// <param name="viewMode">The way the pages should be displayed.</param>
        /// <param name="viewHeight">The height of the embeded document.</param>
        /// <param name="viewWidth">The width of the embeded document.</param>
        /// <returns>String</returns>
        public string GetEmbedCode(int startingPage, ViewMode viewMode, string viewHeight, string viewWidth)
        {
            return Document.GetEmbedCode(this, startingPage, viewMode, viewHeight, viewWidth);
        }

        /// <summary>
        /// Add this document to the specified collections, using the current User
        /// </summary>
        /// <param name="collectionIDs">list of collection ids</param>
        public void AddToCollections(ICollection<int> collectionIDs)
        {
            AddToCollections(null, collectionIDs);
        }

        /// <summary>
        /// Add this document to the specified collections
        /// </summary>
        /// <param name="user">make the request on the behalf of this user</param>
        /// <param name="collectionIDs">list of collection ids</param>
        public void AddToCollections(User user, ICollection<int> collectionIDs)
        {
            foreach (int cid in collectionIDs)
            {
                AddToCollection(user, cid);
            }
        }

        /// <summary>
        /// Add this document to the specified collection
        /// </summary>
        /// <param name="user">make the request on the behalf of this user</param>
        /// <param name="collectionId">the collection id</param>
        public void AddToCollection(User user, int collectionId)
        {
            if (collectionId < 1)
            {
                Service.OnErrorOccurred(666, "You must have collectionId indicated.");
                return;
            }

            // Build our request
            using (Request _request = new Request(user, true))
            {
                _request.MethodName = "docs.addToCollection";
            
                _request.Parameters.Add("doc_id", this.DocumentId.ToString());
                _request.Parameters.Add("collection_id", collectionId.ToString());
                
                Response _response = Service.Instance.PostRequest(_request);
                if (_response != null && _response.HasChildNodes && _response.ErrorList.Count < 1)
                {
                    ;
                }
            }
        }

        /// <summary>
        /// Saves a document to Scribd
        /// </summary>
        public void Save()
        {
            if (this.DocumentId == 0)
            {
                Service.OnErrorOccurred(666, "You must have a DocumentId indicated.");
                return;
            }
            DocumentEventArgs _arguments = new DocumentEventArgs(this);
            OnBeforeSave(_arguments);
            if (_arguments.Cancel)
            {
                // Get out!
                return;
            }

            // Build our request
            using (Request _request = new Request())
            {
                _request.MethodName = "docs.changeSettings";

                _request.Parameters.Add("doc_ids", this.DocumentId.ToString());

                if (!string.IsNullOrEmpty(this.Title))          // no sense setting title to null OR ""
                {
                _request.Parameters.Add("title", this.Title);
                }

                if (this.Description != null)                   // null means to leave description unchanged
                {
                _request.Parameters.Add("description", this.Description);
                }

                _request.Parameters.Add("access", this.AccessType == AccessTypes.Public ? "public" : "private");

                // License
                switch (this.License)
                {
                    case CCLicenseTypes.BY: _request.Parameters.Add("license", "by"); break;
                    case CCLicenseTypes.BY_NC: _request.Parameters.Add("license", "by-nc"); break;
                    case CCLicenseTypes.BY_NC_ND: _request.Parameters.Add("license", "by-nc-nd"); break;
                    case CCLicenseTypes.BY_NC_SA: _request.Parameters.Add("license", "by-nc-sa"); break;
                    case CCLicenseTypes.BY_ND: _request.Parameters.Add("license", "by-nd"); break;
                    case CCLicenseTypes.BY_SA: _request.Parameters.Add("license", "by-sa"); break;
                    case CCLicenseTypes.C: _request.Parameters.Add("license", "c"); break;
                    case CCLicenseTypes.PD: _request.Parameters.Add("license", "pd"); break;
                    default: break;
                }

                // Ads
                switch (this.ShowAds)
                {
                    case ShowAdsTypes.Default: _request.Parameters.Add("show_ads", "default"); break;
                    case ShowAdsTypes.True: _request.Parameters.Add("show_ads", "true"); break;
                    case ShowAdsTypes.False: _request.Parameters.Add("show_ads", "false"); break;
                    default: break;
                }

                if (this.LinkBackURL != null && !string.IsNullOrEmpty(this.LinkBackURL.ToString()))
                {
                    _request.Parameters.Add("link_back_url", this.LinkBackURL.ToString());
                }

                // Category
                _request.Parameters.Add("category_id", this.CategoryId.ToString());

                //if (!string.IsNullOrEmpty(this.Category)) { _request.Parameters.Add("category", this.Category); }
                //if (!string.IsNullOrEmpty(this.Subcategory)) { _request.Parameters.Add("subcategory", this.Subcategory); }
              
                // Tags
                if (this.TagList.Count > 0)
                {
                    StringBuilder _tags = new StringBuilder();
                    foreach (string _tag in this.TagList)
                    {
                        // No blank Tags, thankyouverymuch!
                        if (!string.IsNullOrEmpty(_tag))
                            _tags.AppendFormat("{0},", _tag);
                    }
                    _request.Parameters.Add("tags", _tags.ToString().TrimEnd(','));
                }

                if (!string.IsNullOrEmpty(this.Author)) { _request.Parameters.Add("author", this.Author); }
                if (!string.IsNullOrEmpty(this.Publisher)) { _request.Parameters.Add("publisher", this.Publisher); }
                if (!this.WhenPublished.Equals(DateTime.MinValue)) { _request.Parameters.Add("when_published", string.Format("YYYY-MM-DD", this.WhenPublished)); }
                if (!string.IsNullOrEmpty(this.Edition)) { _request.Parameters.Add("edition", this.Edition); }
                _request.Parameters.Add("disable_upload_link", this.DisableUploadLink ? "1" : "0");
                _request.Parameters.Add("disable_print", this.DisablePrint ? "1" : "0");
                _request.Parameters.Add("disable_select_text", this.DisableSelectText ? "1" : "0");
                _request.Parameters.Add("disable_about_dialog", this.DisableAboutDialog ? "1" : "0"); ;
                _request.Parameters.Add("disable_info_dialog", this.DisableInfoDialog ? "1" : "0");
                _request.Parameters.Add("disable_view_mode_change", this.DisableViewModeChange ? "1" : "0");
                _request.Parameters.Add("disable_related_docs", this.DisableRelatedDocuments ? "1" : "0");

                if (this.StoreSettings != null)
                {
                    this.StoreSettings.PopulateParameters(_request.Parameters);
                }

                Response _response = Service.Instance.PostRequest(_request);
                if (_response != null && _response.HasChildNodes && _response.ErrorList.Count < 1)
                {
                    // Notify those who care.
                    OnSaved();
                }
            }
        }

        /// <summary>
        /// This method can be used for tracking and verification purposes. It gets the list 
        /// of the user_identifiers currently authorized to view this document.
        /// </summary>
        /// <returns></returns>
        public List<string> GetDocumentAccessList()
        {
            return Security.GetDocumentAccessList(this.DocumentId);
        }

        #endregion Document Management

        #region Static members

        /// <summary>
        /// Returns the Flash embedding code for this document.
        /// </summary>
        /// <param name="document">The source document to embed.</param>
        /// <param name="startingPage">The page to start on.</param>
        /// <param name="viewMode">The way the pages should be displayed.</param>
        /// <param name="viewHeight">The height of the embeded document.</param>
        /// <param name="viewWidth">The width of the embeded document.</param>
        /// <returns>String</returns>
        public static string GetEmbedCode(Document document, int startingPage, ViewMode viewMode, string viewHeight, string viewWidth)
        {
            string _viewMode = Enum.GetName(typeof(ViewMode), viewMode);
            if (string.IsNullOrEmpty(viewHeight)) { viewHeight = "500"; }
            if (string.IsNullOrEmpty(viewWidth)) { viewWidth = "100%"; }
            if (startingPage < 1) { startingPage = 1; }

            StringBuilder _sb = new StringBuilder();

            _sb.AppendFormat(@"<object codebase=""http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=9,0,0,0"" id=""doc_296323{0}"" name=""doc_296323{0}"" classid=""clsid:d27cdb6e-ae6d-11cf-96b8-444553540000"" align=""middle"" height=""500"" width=""100%"">", document.DocumentId.ToString());
            _sb.AppendFormat(@"<param name=""movie"" value=""http://documents.scribd.com/ScribdViewer.swf?document_id={0}&access_key={1}&page={2}&version={3}&auto_size=true&viewMode={4}"">", document.DocumentId.ToString(), document.AccessKey, startingPage.ToString(), "1", _viewMode);
            _sb.Append(@"<param name=""quality"" value=""high"">");
            _sb.Append(@"<param name=""play"" value=""true"">");
            _sb.Append(@"<param name=""loop"" value=""true"">");
            _sb.Append(@"<param name=""scale"" value=""showall"">");
            _sb.Append(@"<param name=""wmode"" value=""opaque"">");
            _sb.Append(@"<param name=""devicefont"" value=""false"">");
            _sb.Append(@"<param name=""bgcolor"" value=""#ffffff"">");
            _sb.Append(@"<param name=""menu"" value=""true"">");
            _sb.Append(@"<param name=""allowFullScreen"" value=""true"">");
            _sb.Append(@"<param name=""allowScriptAccess"" value=""always"">");
            _sb.Append(@"<param name=""salign"" value="""">");
            _sb.AppendFormat(@"<embed src=""http://documents.scribd.com/ScribdViewer.swf?document_id={0}&access_key={1}&page={2}&version={3}&auto_size=true&viewMode={4}"" quality=""high"" pluginspage=""http://www.macromedia.com/go/getflashplayer"" play=""true"" loop=""true"" scale=""showall"" wmode=""opaque"" devicefont=""false"" bgcolor=""#ffffff"" name=""doc_296323{0}_object"" menu=""true"" allowfullscreen=""true"" allowscriptaccess=""always"" salign="""" type=""application/x-shockwave-flash"" align=""middle""  height=""{5}"" width=""{6}""></embed>", document.DocumentId.ToString(), document.AccessKey, startingPage.ToString(), "1", _viewMode, viewHeight, viewWidth);
            _sb.Append(@"</object>");
            _sb.Append(@"<div style=""font-size:10px;text-align:center;width:100%"">");
            _sb.AppendFormat(@"<a href=""http://www.scribd.com/doc/{0}"">{1}</a> - <a href=""http://www.scribd.com/upload"">Upload a Document to Scribd</a></div><div style=""display:none""> Read this document on Scribd: <a href=""http://www.scribd.com/doc/{0}"">{1}</a> </div>", document.DocumentId.ToString(), document.Title);

            return _sb.ToString();
        }

        /// <summary>
        /// Saves multiple documents with the given template
        /// settings.
        /// </summary>
        /// <param name="documents">Documents to modify and save.</param>
        /// <param name="template"><see cref="T:Scribd.Net.Document"/> template to use.</param>
        public static void BulkSave(List<Document> documents, Document template)
        {
            // Parse out the document id's and call the 
            // overload
            List<int> _idList = new List<int>();
            foreach (Document _document in documents)
            {
                _idList.Add(_document.DocumentId);
            }

            BulkSave(_idList.ToArray(), template);
        }

        /// <summary>
        /// Saves multiple documents with the given template
        /// settings.
        /// </summary>
        /// <param name="documentId">Array of document identifiers to modify and save.</param>
        /// <param name="template"><see cref="T:Scribd.Net.Document"/> template to use.</param>
        public static void BulkSave(int[] documentId, Document template)
        {
            using (Request _request = new Request())
            {
                _request.MethodName = "docs.changeSettings";

                // Concat the doc_id's into CSV
                StringBuilder _idList = new StringBuilder();
                foreach (int _id in documentId)
                {
                    _idList.AppendFormat("{0},", _id.ToString());
                }
                _request.Parameters.Add("doc_ids", _idList.ToString().TrimEnd(','));
                _request.Parameters.Add("title", template.Title);
                _request.Parameters.Add("description", template.Description);
                _request.Parameters.Add("access", template.AccessType == AccessTypes.Public ? "public" : "private");

                // License
                switch (template.License)
                {
                    case CCLicenseTypes.BY: _request.Parameters.Add("license", "by"); break;
                    case CCLicenseTypes.BY_NC: _request.Parameters.Add("license", "by-nc"); break;
                    case CCLicenseTypes.BY_NC_ND: _request.Parameters.Add("license", "by-nc-nd"); break;
                    case CCLicenseTypes.BY_NC_SA: _request.Parameters.Add("license", "by-nc-sa"); break;
                    case CCLicenseTypes.BY_ND: _request.Parameters.Add("license", "by-nd"); break;
                    case CCLicenseTypes.BY_SA: _request.Parameters.Add("license", "by-sa"); break;
                    case CCLicenseTypes.C: _request.Parameters.Add("license", "c"); break;
                    case CCLicenseTypes.PD: _request.Parameters.Add("license", "pd"); break;
                    default: break;
                }

                // Ads
                switch (template.ShowAds)
                {
                    case ShowAdsTypes.Default: _request.Parameters.Add("show-ads", "default"); break;
                    case ShowAdsTypes.True: _request.Parameters.Add("show-ads", "true"); break;
                    case ShowAdsTypes.False: _request.Parameters.Add("show-ads", "false"); break;
                    default: break;
                }

                if (template.LinkBackURL != null && !string.IsNullOrEmpty(template.LinkBackURL.ToString()))
                {
                    _request.Parameters.Add("link_back_url", template.LinkBackURL.ToString());
                }

                if (!string.IsNullOrEmpty(template.Category)) { _request.Parameters.Add("category", template.Category); }

                // Tags
                if (template.TagList.Count > 0)
                {
                    StringBuilder _tags = new StringBuilder();
                    foreach (string _tag in template.TagList)
                    {
                        // No blank Tags, thankyouverymuch!
                        if (!string.IsNullOrEmpty(_tag))
                            _tags.AppendFormat("{0},", _tag);
                    }
                    _request.Parameters.Add("tags", _tags.ToString().TrimEnd(','));
                }

                if (!string.IsNullOrEmpty(template.Author)) { _request.Parameters.Add("author", template.Author); }
                if (!string.IsNullOrEmpty(template.Publisher)) { _request.Parameters.Add("publisher", template.Publisher); }
                if (!template.WhenPublished.Equals(DateTime.MinValue)) { _request.Parameters.Add("when_published", string.Format("YYYY-MM-DD", template.WhenPublished)); }
                if (!string.IsNullOrEmpty(template.Edition)) { _request.Parameters.Add("edition", template.Edition); }
                _request.Parameters.Add("disable_upload_link", template.DisableUploadLink ? "1" : "0");
                _request.Parameters.Add("disable_print", template.DisablePrint ? "1" : "0");
                _request.Parameters.Add("disable_select_text", template.DisableSelectText ? "1" : "0");
                _request.Parameters.Add("disable_about_dialog", template.DisableAboutDialog ? "1" : "0"); ;
                _request.Parameters.Add("disable_info_dialog", template.DisableInfoDialog ? "1" : "0");
                _request.Parameters.Add("disable_view_mode_change", template.DisableViewModeChange ? "1" : "0");
                _request.Parameters.Add("disable_related_docs", template.DisableRelatedDocuments ? "1" : "0");

                if (template.StoreSettings != null)
                {
                    template.StoreSettings.PopulateParameters(_request.Parameters);
                }
                             
                // Post our request.
                Service.Instance.PostRequest(_request);
            }
        }

        /// <summary>
        /// This method retrieves the conversion status of the document
        /// from Scribd.
        /// </summary>
        /// <param name="documentId">Identifier of the document</param>
        /// <returns><see cref="T:Scribd.Net.ConversionStatusTypes"/></returns>
        public static ConversionStatusTypes CheckConversionStatus(int documentId)
        {
            ConversionStatusTypes _result = ConversionStatusTypes.None_Specified;

            // Build our request
            using (Request _request = new Request())
            {
                _request.MethodName = "docs.getConversionStatus";
                _request.Parameters.Add("doc_id", documentId.ToString());

                // Get the response
                Response _response = Service.Instance.PostRequest(_request);
                if (_response != null && _response.HasChildNodes && _response.ErrorList.Count < 1)
                {
                    XmlNode _node = _response.SelectSingleNode(@"/rsp/conversion_status");

                    if (_node != null)
                    {
                        switch (_node.InnerText.Trim().ToLower())
                        {
                            case "displayable": _result = ConversionStatusTypes.Displayable; break;
                            case "done": _result = ConversionStatusTypes.Done; break;
                            case "error": _result = ConversionStatusTypes.Error; break;
                            case "processing": _result = ConversionStatusTypes.Processing; break;
                            case "published": _result = ConversionStatusTypes.Published; break;
                            default: _result = ConversionStatusTypes.None_Specified; break;
                        }
                    }
                }
            }

            return _result;
        }

        #region "Upload Stream"

        /// <summary>
        /// Helper function to store the given stream to a file.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/></param>
        /// <returns>The path of the file.</returns>
        private static string StreamToFile(ref Stream stream)
        {
            // Push stream into a temp file, then push that to Scribd.
            if (stream != null && stream.Length > 0)
            {
                string _tempFile = String.Empty;

                if (String.IsNullOrEmpty(Service.TemporaryDirectoryPath))
                {
                    _tempFile = Path.GetTempFileName();
                }
                else
                {
                    _tempFile = Path.Combine(Service.TemporaryDirectoryPath, Path.GetRandomFileName());
                }
                
                byte[] _buffer = new byte[stream.Length];

                stream.Position = 0; // rewind the pointer.
                stream.Read(_buffer, 0, (int)stream.Length);

                File.WriteAllBytes(_tempFile, _buffer);

                return _tempFile;
            }

            return null;
        }


        /// <summary>
        /// Uploads a document into Scribd
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> to send</param>
        /// <param name="documentType">Type of document</param>
        /// <param name="asynch">Indicates whether to upload file asynchronously.</param>
        /// <returns><see cref="Document"/> instance of the uploaded document.</returns>
        public static Document Upload(Stream stream, string documentType, bool asynch)
        {
            return Upload(stream, AccessTypes.Public, 0, documentType, asynch);
        }

        /// <summary>
        /// Uploads a document into Scribd
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> to send</param>
        /// <param name="accessType">Access permission of document</param>
        /// <param name="asynch">Indicates whether to upload file asynchronously.</param>
        /// <returns><see cref="T:Scribd.Net.Document"/> instance of the uploaded document.</returns>
        public static Document Upload(Stream stream, AccessTypes accessType, bool asynch)
        {
            return Upload(stream, accessType, 0, string.Empty, asynch);
        }

        /// <summary>
        /// Uploads a document into Scribd
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> to send</param>
        /// <param name="revisionNumber">The document id to save uploaded file as a revision to</param>
        /// <param name="asynch">Indicates whether to upload file asynchronously.</param>
        /// <returns><see cref="T:Scribd.Net.Document"/> instance of the uploaded document.</returns>
        public static Document Upload(Stream stream, int revisionNumber, bool asynch)
        {
            return Upload(stream, AccessTypes.Public, revisionNumber, string.Empty, asynch);
        }

        /// <summary>
        /// Uploads a document into Scribd
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> to send</param>
        /// <param name="accessType">Access permission of document</param>
        /// <param name="revisionNumber">The document id to save uploaded file as a revision to</param>
        /// <param name="documentType">Type of document</param>
        /// <param name="asynch">Indicates whether to upload file asynchronously.</param>
        /// <returns><see cref="T:Scribd.Net.Document"/> instance of the uploaded document.</returns>
        public static Document Upload(Stream stream, AccessTypes accessType, int revisionNumber, string documentType, bool asynch)
        {
            Document _result = new Document();
            string _fileName = StreamToFile(ref stream);
            if (!string.IsNullOrEmpty(_fileName))
            {
                _result = Document.Upload(_fileName, accessType, revisionNumber, documentType, asynch);
            }
            else
            {
                _result= null;
            }
            try
            {
                File.Delete(_fileName);
            }
            catch { }
            return _result;
        }
        #endregion "Upload Stream"

        #region Upload path

        /// <summary>
        /// Uploads a document into Scribd
        /// </summary>
        /// <param name="path">Local or Url path to the file</param>
        /// <param name="documentType">Type of document</param>
        /// <param name="asynch">Indicates whether the file should be uploaded asynchronously</param>
        /// <returns><see cref="Document"/> instance of the uploaded document.</returns>
        public static Document Upload(string path, string documentType, bool asynch)
        {
            return Upload(path, AccessTypes.Public, 0, documentType, asynch);
        }

        /// <summary>
        /// Uploads a document into Scribd.
        /// </summary>
        /// <param name="path">Local or Url path to the file</param>
        /// <param name="isPrivate">Access permission of document</param>
        /// <param name="asynch">Indicates whether the file should be uploaded asynchronously</param>
        /// <returns><see cref="T:Scribd.Net.Document"/> instance of the uploaded document.</returns>
        public static Document Upload(string path, bool isPrivate, bool asynch)
        {
            return Upload(path, (isPrivate) ? AccessTypes.Private : AccessTypes.Public, asynch);
        }

        /// <summary>
        /// Uploads a document into Scribd
        /// </summary>
        /// <param name="path">Local or Url path to the file</param>
        /// <param name="accessType">Access permission of document</param>
        /// <param name="asynch">Indicates whether the file should be uploaded asynchronously</param>
        /// <returns><see cref="T:Scribd.Net.Document"/> instance of the uploaded document.</returns>
        public static Document Upload(string path, AccessTypes accessType, bool asynch)
        {
            return Upload(path, accessType, 0, string.Empty, asynch);
        }

        /// <summary>
        /// Uploads a document into Scribd
        /// </summary>
        /// <param name="path">Local or Url path to the file</param>
        /// <param name="revisionNumber">The document id to save uploaded file as a revision to</param>
        /// <param name="asynch">Indicates whether the file should be uploaded asynchronously</param>
        /// <returns><see cref="T:Scribd.Net.Document"/> instance of the uploaded document.</returns>
        public static Document Upload(string path, int revisionNumber, bool asynch)
        {
            return Upload(path, AccessTypes.Public, revisionNumber, string.Empty, asynch);
        }

        /// <summary>
        /// Uploads a document into Scribd
        /// </summary>
        /// <param name="path">Local or Url path to the file</param>
        /// <param name="isPrivate">Access permission of document</param>
        /// <param name="revisionNumber">The document id to save uploaded file as a revision to</param>
        /// <param name="asynch">Indicates whether the file should be uploaded asynchronously</param>
        /// <returns><see cref="T:Scribd.Net.Document"/> instance of the uploaded document.</returns>
        public static Document Upload(string path, bool isPrivate, int revisionNumber, bool asynch)
        {
            return Upload(path, (isPrivate) ? AccessTypes.Private : AccessTypes.Public, revisionNumber, string.Empty, asynch);
        }

        /// <summary>
        /// Uploads a document into Scribd
        /// </summary>
        /// <param name="path">Local or Url path to the file</param>
        /// <param name="accessType">Access permission of document</param>
        /// <param name="revisionNumber">The document id to save uploaded file as a revision to</param>
        /// <param name="documentType">Type of document</param>
        /// <param name="asynch">Synch of Asych upload?</param>
        /// <returns><see cref="T:Scribd.Net.Document"/> instance of the uploaded document.</returns>
        public static Document Upload(string path, AccessTypes accessType, int revisionNumber, string documentType, bool asynch)
        {
            return _Upload(path, accessType, revisionNumber, documentType, false, DownloadAndDRMTypes.Default, asynch);
        }

        /// <summary>
        /// Uploads a document into Scribd
        /// </summary>
        /// <param name="path">Local or Url path to the file</param>
        /// <param name="accessType">Access permission of document</param>
        /// <param name="revisionNumber">The document id to save uploaded file as a revision to</param>
        /// <param name="documentType">Type of document</param>
        /// <param name="downloadType">Download options to support</param>
        /// <param name="asynch">Synch of Asych upload?</param>
        /// <returns><see cref="T:Scribd.Net.Document"/> instance of the uploaded document.</returns>
        public static Document UploadPaidContent(string path, AccessTypes accessType, int revisionNumber,
            string documentType, DownloadAndDRMTypes downloadType, bool asynch)
        {
            return _Upload(path, accessType, revisionNumber, documentType, true, downloadType, asynch);
        }

        /// <summary>
        /// Uploads a document into Scribd
        /// </summary>
        /// <param name="path">Local or Url path to the file</param>
        /// <param name="accessType">Access permission of document</param>
        /// <param name="revisionNumber">The document id to save uploaded file as a revision to</param>
        /// <param name="documentType">Type of document</param>
        /// <param name="paidContent">Is this paid content or not</param>
        /// <param name="downloadType">Download options to support</param>
        /// <param name="asynch">Synch of Asych upload?</param>
        /// <returns><see cref="T:Scribd.Net.Document"/> instance of the uploaded document.</returns>
        private static Document _Upload(string path, AccessTypes accessType, int revisionNumber, string documentType,
            bool paidContent, DownloadAndDRMTypes downloadType, bool asynch)
        {
            Document _result = new Document();

            // Build our request
            using (Request _request = new Request())
            {
                // Is this from a URL?
                if (path.StartsWith(@"http://")  || path.StartsWith(@"https://"))
                {
                    // Upload to Scribd via URL
                    _request.MethodName = "docs.uploadFromUrl";
                    _request.Parameters.Add("url", path);
                }
                else
                {
                    // Don't.
                    _request.MethodName = "docs.upload";
                    _request.Parameters.Add("file", path);

                }

                if (!string.IsNullOrEmpty(documentType))
                {
                    _request.Parameters.Add("doc_type", documentType.ToLower());
                }

                _request.Parameters.Add("access", accessType == AccessTypes.Public ? "public" : "private");

                if (revisionNumber != 0)
                {
                    _request.Parameters.Add("rev_id", revisionNumber.ToString());
                }

                if (paidContent)
                {
                    _request.Parameters.Add("paid_content", "1");
                    if (downloadType != DownloadAndDRMTypes.Default)
                    {
                        switch (downloadType)
                        {
                            case DownloadAndDRMTypes.DownloadDRM:               _request.Parameters.Add("download_and_drm", "download-drm"); break;
                            case DownloadAndDRMTypes.DownloadPDF:               _request.Parameters.Add("download_and_drm", "download-pdf"); break;
                            case DownloadAndDRMTypes.DownloadPDFandOriginal:    _request.Parameters.Add("download_and_drm", "download-pdf-orig"); break;
                            case DownloadAndDRMTypes.ViewOnly:                  _request.Parameters.Add("download_and_drm", "view-only"); break;
                        }
                    }
                }

                if (asynch)
                {
                    // Post it asychronously
                    Service.Instance.PostFileUploadRequest(_request);
                }
                else
                {
                    // Post is sychronously

                    // Get our response
                    Response _response = Service.Instance.PostRequest(_request);

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

                    // Notify subscribers
                    OnUploaded(_result);
                }
            }
            return _result;
        }

        /// <summary>
        /// This method accepts a document thumbnail file.
        /// </summary>
        /// <param name="documentId">The doc_id to attach the uploaded thumbnail to</param>
        /// <param name="path">Local path to the file</param>
        /// <param name="asynch">Synch of Asych upload?</param>
        /// <returns>True on success</returns>
        public static bool UploadThumbnail(int documentId, string path, bool asynch)
        {
            bool _result = false;

            // Build our request
            using (Request _request = new Request())
            {
                // Upload to Scribd via URL
                _request.MethodName = "docs.uploadThumb";
                _request.Parameters.Add("doc_id", documentId.ToString());
                _request.Parameters.Add("file", path);

                if (asynch)
                {
                    // Post it asychronously
                    Service.Instance.PostFileUploadRequest(_request);
                }
                else
                {
                    // Post is sychronously

                    // Get our response
                    Response _response = Service.Instance.PostRequest(_request);

                    if (_response.Status == "ok")
                    {
                        _result = true;

                        // Notify subscribers
                        if (ThumbnailUploaded != null) { ThumbnailUploaded(null, new EventArgs()); }
                    }
                }
            }
            return _result;
        }

        #endregion

        /// <summary>
        /// This method returns a link you can use to download a static version of a document.
        /// </summary>
        /// <param name="documentId">Document ID of the file to download. You must have ownership of this document.</param>
        /// <param name="documentType">The type of file to download. If "original", 
        /// will get a link to the original file uploaded, regardless of its extension.</param>
        /// <returns>A link to the file.</returns>
        public static string GetDownloadURL(int documentId, string documentType)
        {
            string _result = @"http://www.scribd.com";
            // Set up our request
            using (Request _request = new Request())
            {
                _request.MethodName = "docs.getDownloadUrl";
                _request.Parameters.Add("doc_id", documentId.ToString());
                _request.Parameters.Add("doc_type", documentType.ToLower());

                // Grab our response
                using (Response _response = Service.Instance.PostRequest(_request))
                {

                    // Parse the response
                    if (_response != null && _response.HasChildNodes && _response.ErrorList.Count < 1)
                    {
                        XmlNode _node = _response.SelectSingleNode("rsp");

                        // Data
                        _result = _node.SelectSingleNode("download_link").InnerText;
                    }
                }
            }

            return _result;
        }

        /// <summary>
        /// Downloads a document from Scribd.
        /// </summary>
        /// <param name="documentId">Identifier of the document</param>
        /// <returns><see cref="T:Scribd.Net.Document"/></returns>
        /// <example><![CDATA[
        ///		Scribd.Net.Document myDocument = Scribd.Net.Document.Download(39402);
        /// ]]></example>
        public static Document Download(int documentId)
        {
            Document _result = new Document();
            _result.DocumentId = documentId;

            // Give subscribers a chance to bail.
            DocumentEventArgs _arguments = new DocumentEventArgs(_result);
            OnBeforeDownload(_arguments);
            if (_arguments.Cancel)
            {
                return _result;
            }

            // Set up our request
            using (Request _request = new Request())
            {
                _request.MethodName = "docs.getSettings";
                _request.Parameters.Add("doc_id", documentId.ToString());

                // Grab our response
                using (Response _response = Service.Instance.PostRequest(_request))
                {

                    // Parse the response
                    if (_response != null && _response.HasChildNodes && _response.ErrorList.Count < 1)
                    {
                        XmlNode _node = _response.SelectSingleNode("rsp");

                        // Data
                        _result.DocumentId = int.Parse(_node.SelectSingleNode("doc_id").InnerText);
                        _result.Title = _node.SelectSingleNode("title").InnerText.Trim();
                        _result.Description = _node.SelectSingleNode("description").InnerText.Trim();
                        _result.AccessKey = _node.SelectSingleNode("access_key").InnerText;
                        _result.AccessType = _node.SelectSingleNode("access").InnerText == "private" ? AccessTypes.Private : AccessTypes.Public;

                        // Depends on version of the Scribd API ...
                        string _url = _node.SelectSingleNode("thumbnail_url") == null ? string.Empty : _node.SelectSingleNode("thumbnail_url").InnerText;
                        _result.ThumbnailUrl = new Uri(_url);

                        // Link to the large image
                        _result.LargeImageURL = new Uri(_url.Replace("thumb", "large"));

                        // License
                        switch (_node.SelectSingleNode("license").InnerText.ToLower())
                        {
                            case "by": _result.License = CCLicenseTypes.BY; break;
                            case "by-nc": _result.License = CCLicenseTypes.BY_NC; break;
                            case "by-nc-nd": _result.License = CCLicenseTypes.BY_NC_ND; break;
                            case "by-nc-sa": _result.License = CCLicenseTypes.BY_NC_SA; break;
                            case "by-nd": _result.License = CCLicenseTypes.BY_ND; break;
                            case "by-sa": _result.License = CCLicenseTypes.BY_SA; break;
                            case "c": _result.License = CCLicenseTypes.C; break;
                            case "pd": _result.License = CCLicenseTypes.PD; break;
                            default: _result.License = CCLicenseTypes.None_Specified; break;
                        }

                        // 2010.04.06 - JPD - This has been removed from the API.
                        // Show Ads
                        //switch (_node.SelectSingleNode("show_ads").InnerText.ToLower())
                        //{
                        //    case "default": _result.ShowAds = ShowAdsTypes.Default; break;
                        //    case "true": _result.ShowAds = ShowAdsTypes.True; break;
                        //    case "false": _result.ShowAds = ShowAdsTypes.False; break;
                        //    default: _result.ShowAds = ShowAdsTypes.Default; break;
                        //}

                        // Tags
                        if (_node.SelectSingleNode("tags") != null) // Doing this now in case they decide to dump "tags" too...grr
                        {
                            string _tags = _node.SelectSingleNode("tags").InnerText;
                            foreach (string _tag in _tags.Split(','))
                            {
                                _result.TagList.Add(_tag.Trim());
                            }
                        }

                        // Security
                        if (_node.SelectSingleNode("secret_password") != null)
                        {
                            _result.AccessType = AccessTypes.Private;
                            _result.SecretPassword = _node.SelectSingleNode("secret_password").InnerText;
                        }

                        if (_node.SelectSingleNode("link_back_url") != null)
                        {
                            _result.LinkBackURL = new Uri( _node.SelectSingleNode("link_back_url").InnerText);
                        }

                        if (_node.SelectSingleNode("page_count") != null)
                        {
                            //_result.PageCount = int.Parse(_node.SelectSingleNode("page_count").InnerText);
                            int pc;
                            int.TryParse(_node.SelectSingleNode("page_count").InnerText, out pc);
                            _result.PageCount = pc;
                        }

                        // Category
                        if (_node.SelectSingleNode("category_id") != null)
                        {
                            int cat;
                            int.TryParse(_node.SelectSingleNode("category_id").InnerText, out cat);
                            _result.CategoryId = cat;

                            // TODO:  Set Category property
                            
                        }

                        // Download Formats
                        if (_node.SelectSingleNode("download_formats") != null)
                        {
                            _result.DownloadFormats.AddRange(_node.SelectSingleNode("download_formats").InnerText.Split(','));
                        }


                        if (_node.SelectSingleNode("author") != null)
                        {
                            _result.Author = _node.SelectSingleNode("author").InnerText;
                        }

                        if (_node.SelectSingleNode("publisher") != null)
                        {
                            _result.Publisher = _node.SelectSingleNode("publisher").InnerText;
                        }

                        if (_node.SelectSingleNode("when_published") != null)
                        {
                            if (_node.SelectSingleNode("when_published").InnerText != "")
                            {
                                _result.WhenPublished = Convert.ToDateTime(_node.SelectSingleNode("when_published").InnerText);
                            }
                        }

                        if (_node.SelectSingleNode("edition") != null)
                        {
                            _result.Edition = _node.SelectSingleNode("edition").InnerText;  
                        }

                        _result.StoreSettings = new DocumentStore();

                        if (_node.SelectSingleNode("page_restriction_type") != null)
                        {
                            switch (_node.SelectSingleNode("page_restriction_type").InnerText.ToLower())
                            {
                                case "automatic": _result.StoreSettings.RestrictionType = PageRestrictionTypes.Automatic; break;
                                case "max_pages": _result.StoreSettings.RestrictionType = PageRestrictionTypes.MaxPages; break;
                                case "max_percentage": _result.StoreSettings.RestrictionType = PageRestrictionTypes.MaxPercentage; break;
                                case "page_range": _result.StoreSettings.RestrictionType = PageRestrictionTypes.PageRange; break;
                                default: _result.StoreSettings.RestrictionType = PageRestrictionTypes.Automatic; break;
                            }
                        }

                        if (_node.SelectSingleNode("max_pages") != null)
                        {
                            _result.StoreSettings.MaxPages = Convert.ToInt32(_node.SelectSingleNode("max_pages").InnerText);
                        }

                        if (_node.SelectSingleNode("max_percentage") != null)
                        {
                            _result.StoreSettings.MaxPercentage = Convert.ToInt32(_node.SelectSingleNode("max_percentage").InnerText);
                        }

                        if (_node.SelectSingleNode("page_range") != null)
                        {
                            _result.StoreSettings.PageRange =_node.SelectSingleNode("page_range").InnerText;
                        }

                        if (_node.SelectSingleNode("allow_search_targeting") != null)
                        {
                            _result.StoreSettings.AllowSearchTargeting = Convert.ToBoolean(_node.SelectSingleNode("allow_search_targeting").InnerText);
                        }

                        if (_node.SelectSingleNode("obfuscate_numbers") != null)
                        {
                            _result.StoreSettings.ObfuscateNumbers = Convert.ToBoolean(_node.SelectSingleNode("obfuscate_numbers").InnerText);
                        }

                        if (_node.SelectSingleNode("allow_search_indexing") != null)
                        {
                            _result.StoreSettings.AllowSearchIndexing = Convert.ToBoolean(_node.SelectSingleNode("allow_search_indexing").InnerText);
                        }

                        if (_node.SelectSingleNode("price") != null)
                        {
                            _result.StoreSettings.Price = (float)Convert.ToDouble(_node.SelectSingleNode("price").InnerText);
                        }

                        if (_node.SelectSingleNode("min_price") != null)
                        {
                            _result.StoreSettings.MinPrice = (float)Convert.ToDouble(_node.SelectSingleNode("min_price").InnerText);
                        }

                        if (_node.SelectSingleNode("max_price") != null)
                        {
                            _result.StoreSettings.MaxPrice = (float)Convert.ToDouble(_node.SelectSingleNode("max_price").InnerText);
                        }

                    }
                }
                // Notify subscribers
                OnDownloaded(_result);
            }
            // Get out!
            return _result;
        }

        /// <summary>
        /// Deletes a document from Scribd.
        /// </summary>
        /// <param name="documentId">Identifier of the document</param>
        /// <example>
        /// <![CDATA[ Scribd.Net.Document.Delete(3902); ]]>
        /// </example>
        public static void Delete(int documentId)
        {
            // Build our request
            using (Request _request = new Request())
            {
                _request.MethodName = "docs.delete";
                _request.Parameters.Add("doc_id", documentId.ToString());

                // Post our request
                using (Service.Instance.PostRequest(_request)) { }
            }
        }

        /// <summary>
        /// This method retrieves a list of documents for a given user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="includeDetails">Additionally retrieves detailed document information.</param>
        /// <returns></returns>
        public static List<Document> GetList(User user, bool includeDetails)
        {
            return GetList(user, 1000, 1, includeDetails);
        }

        /// <summary>
        /// This method retrieves a list of documents for a given user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="limit">The number of documents to return. You can paginate through the full list using the limit and offset parameters. The maximum limit is 1000. </param>
        /// <param name="offset">The offset into the list of documents. You can paginate through the full list using the limit and offset parameters.</param>
        /// <param name="includeDetails">Additionally retrieves detailed document information.</param>
        /// <returns></returns>
        public static List<Document> GetList(User user, int limit, int offset, bool includeDetails)
        {
            List<Document> _result = new List<Document>();

            // Build the request
            using (Request _request = new Request(user))
            {
                _request.MethodName = "docs.getList";
                
                // Currently the 'use_api_account' parameter isn't working. Since "false == not using the param at all" just
                // comment it out.
                //_request.Parameters.Add("use_api_account", "false");

                _request.Parameters.Add("limit", limit.ToString());
                _request.Parameters.Add("offset", offset.ToString());

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
                                Document _item = new Document();

                                _item.DocumentId = int.Parse(_node.SelectSingleNode("doc_id").InnerText);
                                _item.Title = _node.SelectSingleNode("title").InnerText.Trim();
                                _item.Description = _node.SelectSingleNode("description").InnerText.Trim();
                                _item.AccessKey = _node.SelectSingleNode("access_key").InnerText;

                                switch (_node.SelectSingleNode("conversion_status").InnerText.Trim().ToLower())
                                {
                                    case "displayable": _item.ConversionStatus = ConversionStatusTypes.Displayable; break;
                                    case "done": _item.ConversionStatus = ConversionStatusTypes.Done; break;
                                    case "error": _item.ConversionStatus = ConversionStatusTypes.Error; break;
                                    case "processing": _item.ConversionStatus = ConversionStatusTypes.Processing; break;
                                    case "published": _item.ConversionStatus = ConversionStatusTypes.Published; break;
                                    default: _item.ConversionStatus = ConversionStatusTypes.None_Specified; break;
                                }

                                // We're going to default to public
                                _item.AccessType = AccessTypes.Public;

                                // We've got a password - it's private!
                                if (_node.SelectSingleNode("secret_password") != null)
                                {
                                    _item.AccessType = AccessTypes.Private;
                                    _item.SecretPassword = _node.SelectSingleNode("secret_password").InnerText;
                                }

                                // Get all the properties.
                                Document _temp = Document.Download(_item.DocumentId);

                                _result.Add(_temp);
                            }
                        }
                    }
                }
            }
            return _result;
        }
        #endregion Static members

    }
}