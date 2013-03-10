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
    /// Encapsulates scribd's notion of a category: id, name, parent, and child categories.
    /// </summary>
    public sealed class Category
    {
        #region constructors

        /// <summary>
        /// ctor
        /// </summary>
        private Category()
        {
        }

        private Category(int id, string name, Category parent, ICollection<Category> subCategories)
            : this()
        {
            this.ID = id;
            this.Name = name;
            this.Parent = parent;
            this.SubCategories = subCategories;
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
        /// The Parent category of the category (can be null)
        /// </summary>
        public Category Parent { get; private set; }

        /// <summary>
        /// Subcategories (can be null)
        /// </summary>
        public ICollection<Category> SubCategories { get; private set; }

        #endregion

        #region retrieve categories

        /// <summary>
        /// This method retrieves a list of scribd 'categories'.
        /// </summary>
        /// <param name="includeSubcategories"></param>
        /// <returns></returns>
        public static ICollection<Category> GetCategories(bool includeSubcategories)
        {
            return Category.GetCategories(true, 0, includeSubcategories);
        }

        /// <summary>
        /// This method retrieves a list of scribd 'sub-categories' for the specified parent category.
        /// </summary>
        /// <param name="parentCategoryId"></param>
        /// <returns></returns>
        public static ICollection<Category> GetCategories(int parentCategoryId)
        {
            return Category.GetCategories(false, parentCategoryId, false);
        }

        /// <summary>
        /// This method retrieves a list of scribd 'categories'.
        /// </summary>
        /// <param name="topLevel"></param>
        /// <param name="parentCategoryId"></param>
        /// <param name="includeSubcategories"></param>
        /// <returns></returns>
        private static ICollection<Category> GetCategories(bool topLevel, int parentCategoryId, bool includeSubcategories)
        {
            List<Category> _result = new List<Category>();

            // Build the request
            using (Request _request = new Request())
            {
                _request.MethodName = "docs.getCategories";

                if (!topLevel)
                {
                    _request.Parameters.Add("category_id", parentCategoryId.ToString());
                }

                if (includeSubcategories)
                {
                    _request.Parameters.Add("with_subcategories", "true");
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
                                Category cat = Category.Parse(_node, null);
                                _result.Add(cat);
                            }
                        }
                    }
                }
            }

            return _result;
        }

        #endregion

        #region private

        private static Category Parse(XmlNode node, Category parent)
        {
            int id = int.Parse(node.SelectSingleNode("id").InnerText);
            string name = node.SelectSingleNode("name").InnerText.Trim();

            Category cat = new Category(id, name, parent, null);

            ICollection<Category> subCategories = null;
            XmlNode subs = node.SelectSingleNode("subcategories");
            if ((subs != null) && subs.HasChildNodes)
            {
                subCategories = new List<Category>();
                foreach (XmlNode child in subs.ChildNodes)
                {
                    Category subCat = Category.Parse(child, cat);
                    subCategories.Add(subCat);
                }
                if (subCategories.Count == 0)
                {
                    subCategories = null;
                }

                cat.SubCategories = subCategories;
            }

            return cat;
        }

        #endregion
    }
}
