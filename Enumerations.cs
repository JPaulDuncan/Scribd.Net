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

namespace Scribd.Net
{
    /// <summary>
    /// Status of a conversion
    /// </summary>
    public enum ConversionStatusTypes
    {
        /// <summary>
        /// No conversion type is specified.
        /// </summary>
        None_Specified = 0,
        /// <summary>
        /// The document is now displayable.
        /// </summary>
        Displayable,
        /// <summary>
        /// Conversion was completed
        /// </summary>
        Done,
        /// <summary>
        /// Problem occurred during conversion
        /// </summary>
        Error,
        /// <summary>
        /// Still processing the conversion
        /// </summary>
        Processing,
        /// <summary>
        /// The document was published
        /// </summary>
        Published
    }

    /// <summary>
    /// Accessibility to a document
    /// </summary>
    public enum AccessTypes
    {
        /// <summary>
        /// The document is publicly viewable
        /// </summary>
        Public = 0,
        /// <summary>
        /// The document is private and not publicly viewable
        /// </summary>
        Private
    }

    /*
     * "download-drm", "download-pdf", "download-pdf-orig", "view-only
     */

    /// <summary>
    /// Download Options for Sellable documents
    /// </summary>
    public enum DownloadAndDRMTypes
    {
        /// <summary>
        /// Default download options, currently 'DownloadPDF', but could change
        /// </summary>
        Default = 0,

        /// <summary>
        /// Allow downloading of document as a PDF
        /// </summary>
        DownloadPDF = 1,

        /// <summary>
        /// Allow downloading of document in original format and as a PDF
        /// </summary>
        DownloadPDFandOriginal = 2,

        /// <summary>
        /// Allow downloading of DRM protected version
        /// </summary>
        DownloadDRM = 3,

        /// <summary>
        /// Document only viewable on Scribd website
        /// </summary>
        ViewOnly = 4
    }

    /// <summary>
    /// Show advertising in documents
    /// </summary>
    public enum ShowAdsTypes
    {
        /// <summary>
        /// The default setting on Scribd
        /// </summary>
        Default = 0,
        /// <summary>
        /// Yes, show advertisements
        /// </summary>
        True,
        /// <summary>
        /// No, do not show advertisements
        /// </summary>
        False
    }

    /// <summary>
    /// Creative Commons License types
    /// </summary>
    public enum CCLicenseTypes
    {
        /// <summary>
        /// No Creative Comment License was specified
        /// </summary>
        None_Specified = 0,
        /// <summary>
        /// Attribution
        /// </summary>
        BY,
        /// <summary>
        /// Attribution, Non-Commercial
        /// </summary>
        BY_NC,
        /// <summary>
        /// Attribution, Non-Commercial, No Distribution
        /// </summary>
        BY_NC_ND,
        /// <summary>
        /// Attribution, Non-Commericial, Share Alike
        /// </summary>
        BY_NC_SA,
        /// <summary>
        /// Attribution, No Distribution
        /// </summary>
        BY_ND,
        /// <summary>
        /// Attribution, Share Alike
        /// </summary>
        BY_SA,
        /// <summary>
        /// Copyright
        /// </summary>
        C,
        /// <summary>
        /// Public-Domain
        /// </summary>
        PD
    }

    /// <summary>
    /// Scope of search
    /// </summary>
    public enum SearchScope
    {
        /// <summary>
        /// Searches all users on Scribd
        /// </summary>
        All = 0,
        /// <summary>
        /// Searches only the current user
        /// </summary>
        User,

        /// <summary>
        /// All documents uploaded by that API account will be searched
        /// </summary>
        Account

    }

    /// <summary>
    /// How a document is displayed on Scribd
    /// </summary>
    public enum DisplayMode
    {
        /// <summary>
        /// The document will be shown in fullscreen mode
        /// </summary>
        Fullscreen = 0,
        /// <summary>
        /// The document will be shown as a view document page on Scribd.
        /// </summary>
        Scribd
    }

    /// <summary>
    /// Various ways to display your QuickSwitch 
    /// Scribd documents.
    /// </summary>
    public enum QuickSwitchMode
    {
        /// <summary>
        /// Fullscreen iPaper links hosted on Scribd.
        /// </summary>
        Fullscreen = 0,
        /// <summary>
        /// Scribd document page links.
        /// </summary>
        Scribd,
        /// <summary>
        /// Embedded iPapers directly on page.
        /// </summary>
        Embedded,
        /// <summary>
        /// Custom page with embedded iPaper hosted on your site. 
        /// </summary>
        Custom
    }

    /// <summary>
    /// Indicates various ways to view a document's pages.
    /// </summary>
    public enum ViewMode
    {
        /// <summary>
        /// Shows the pages in a list format.
        /// </summary>
        List = 0,

        /// <summary>
        /// Shows the pages in a book format.
        /// </summary>
        Book,

        /// <summary>
        /// Shows the pages in a slideshow format.
        /// </summary>
        Slideshow,

        /// <summary>
        /// Shows the pages in a tiled format.
        /// </summary>
        Tiled
    }


    /// <summary>
    /// Restrictions to the preview pages that Scribd shows.
    /// </summary>
    public enum PageRestrictionTypes
    {
        /// <summary>
        /// Scribd will automatically determine the ideal page restriction by default
        /// </summary>
        Automatic = 0,

        /// <summary>
        /// Specifies the maximum number of pages that will be shown in the document preview. Scribd will determine which pages to show based on the purchaser's history and search queries.
        /// </summary>
        MaxPages,

        /// <summary>
        /// Specifies the maximum percentage of content that will be shown in the document preview. Scribd will determine which pages to show based on the purchaser's history and search queries.
        /// </summary>
        MaxPercentage,

        /// <summary>
        /// Scribd will show only the pages that you've specified. Scribd page counts begin from the absolute first page of the document 
        /// </summary>
        PageRange
    }

}