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
/// Information related to a purchasable document.
/// </summary>
    [Serializable()]
    public sealed class DocumentStore
    {
        #region constructors

        /// <summary>
        /// Default ctor.
        /// </summary>
        public DocumentStore()
        {
            this.RestrictionType = PageRestrictionTypes.Automatic;
            this.MaxPages = 0;
            this.MaxPercentage = 0;
            this.PageRange = string.Empty;
            this.AllowSearchTargeting = true;
            this.ObfuscateNumbers = false;
            this.AllowSearchIndexing = true;
            this.Price = 0.00f;
            this.MaxPrice = 0.00f;
        }
        #endregion

        #region methods
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public void PopulateParameters(Dictionary<string, string> parameters)
        {
            switch (this.RestrictionType)
            {
                case PageRestrictionTypes.Automatic: 
                    parameters.Add("page_restriction_type", "automatic"); 
                    break;
                case PageRestrictionTypes.MaxPages: 
                    parameters.Add("page_restriction_type", "max_pages");
                    break;
                case PageRestrictionTypes.MaxPercentage: 
                    parameters.Add("page_restriction_type", "max_percentage");
                    break;
                case PageRestrictionTypes.PageRange: 
                    parameters.Add("page_restriction_type", "page_range");
                    break;
                default: 
                    break;
            }

            parameters.Add("max_pages", this.MaxPages.ToString());
            parameters.Add("max_percentage", this.MaxPercentage.ToString());
            if (!string.IsNullOrEmpty(this.PageRange))
            {
                parameters.Add("page_range", this.PageRange);
            }

            parameters.Add("allow_search_targeting", this.AllowSearchTargeting ? "true" : "false");
            parameters.Add("obfuscate_numbers", this.ObfuscateNumbers ? "true" : "false");
            parameters.Add("allow_search_indexing", this.AllowSearchIndexing ? "true" : "false");
            parameters.Add("price", this.Price == 0.00f ? "auto" : this.Price.ToString());
            parameters.Add("min_price", this.MinPrice.ToString());
            parameters.Add("max_price", this.MaxPrice.ToString());
            parameters.Add("list_price", this.ListPrice.ToString());
        }

        #endregion

        #region properties
        /// <summary>
        /// This parameter lets you add additional restrictions to the preview pages that Scribd shows. 
        /// Scribd will do this automatically, but if you wish, you can set a maximum number of pages to 
        /// show (max_pages), a maximum percentage of the pages to show (max_percentage), or an 
        /// explicit page_range such as 1-3,5,8. See the parameters "max_pages", "max_percentage", 
        /// and "page_range" to actually set the data. 
        /// </summary>
        public PageRestrictionTypes RestrictionType { get; set; }

        /// <summary>
        ///  Corresponds to the "max_pages" page_restriction_type. This is the maximum number of pages 
        ///  that will ever be shown in one document preview. 
        /// </summary>
        public int MaxPages { get; set; }

        /// <summary>
        /// Corresponds to the "max_percentage" page_restriction_type. This is the maximum percentage of the
        /// document that will ever be shown in one document preview. 
        /// </summary>
        public int MaxPercentage { get; set; }

        /// <summary>
        /// Corresponds to the "page_range" page_restriction_type. Any document preview will show at most a subset of these pages
        /// </summary>
        public string PageRange { get; set; }

        /// <summary>
        /// Allow Scribd to vary the preview pages it shows to target users' search queries. Note that you 
        /// can still control the pages Scribd shows with the page_restriction_type parameter, which 
        /// overrides Scribd's search targeting algorithm. Default: "true". Setting this to "false" is NOT recommended. 
        /// </summary>
        public bool AllowSearchTargeting { get; set; }

        /// <summary>
        /// If true, all numbers in the document will be blurred out in previews. This is good for technical 
        /// information and market research reports where the numbers are the key value. Default: false. 
        /// </summary>
        public bool ObfuscateNumbers { get; set; }

        /// <summary>
        /// If true, major search engines will index the full text of the document which will drive targeted traffic 
        /// to the document and increase sales. Note that even when true, search engines never receive a 
        /// human-readable version of the text. Default: true. 
        /// </summary>
        public bool AllowSearchIndexing { get; set; }

        /// <summary>
        /// The price at which this document will be sold. 
        /// </summary>
        public float Price { get; set; }

        /// <summary>
        /// If non-zero then this let purchasers know how much they are saving (when compared to the Price)
        /// </summary>
        public float ListPrice { get; set; }
        
        /// <summary>
        /// Obsolete
        ///  Relevant if "price" is set to auto. If you set a minimum price, Scribd will continue to bid on your 
        ///  behalf but will not go below the specified minimum. 
        /// </summary>
        public float MinPrice{get;set;}

        /// <summary>
        /// Obsolete
        /// Shown in the price box to let purchasers know how much they are saving. A value of 0.00 indicates no list price. 
        /// </summary>
        public float MaxPrice { get; set; }

        #endregion
    }
}
