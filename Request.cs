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
using System.Security.Cryptography;

namespace Scribd.Net
{
    /// <summary>
    /// Manages the REST request sent to Scribd.
    /// </summary>
    internal sealed class Request : IDisposable
    {
        /// <summary>
        /// ctor
        /// </summary>
        public Request() 
        { 
            this.Parameters = new Dictionary<string, string>();
            this.SpecifiedUser = false;
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="user"></param>
        public Request(User user)
            : this()
        {
            this.User = user;
            this.SpecifiedUser = true;
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="user"></param>
        /// <param name="defaultIfNull"></param>
        public Request(User user, bool defaultIfNull)
            : this()
        {
            this.User = user;
            this.SpecifiedUser = !defaultIfNull;
        }

        /// <summary>
        /// Does this Request specify a specific user
        /// </summary>
        public bool SpecifiedUser { get; private set; }

        /// <summary>
        /// User making call
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Name of the REST method
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// Method parameters
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; }

        /// <summary>
        /// Call to make to Scribd;
        /// </summary>
        public string RESTCall { get { return this.ToString(); } }

        /// <summary>
        /// The RESTCall
        /// </summary>
        /// <returns><see cref="T:System.string"/></returns>
        public override string ToString()
        {
            // Pass the session key if we've got it.
            if (this.User != null && !string.IsNullOrEmpty(this.User.SessionKey))
            {
                if (!this.Parameters.ContainsKey("session_key"))
                    this.Parameters.Add("session_key", this.User.SessionKey);
            }
            else if (
                this.User != null &&
                !string.IsNullOrEmpty(this.User.PhantomName) &&
                this.MethodName.StartsWith("docs.")
                )
            {
                if (!this.Parameters.ContainsKey("my_user_id"))
                    this.Parameters.Add("my_user_id", this.User.PhantomName);
            }

            // Add our API Key
            if (!this.Parameters.ContainsKey("api_key"))
                this.Parameters.Add("api_key", Service.APIKey);

            string _result = @"{0}?method={1}&{2}";

            StringBuilder _builder = new StringBuilder();
            foreach (string _key in this.Parameters.Keys)
            {
                string val = System.Web.HttpUtility.UrlEncode(this.Parameters[_key]);
                _builder.AppendFormat(@"{0}={1}&", _key, val);
            }

            // Escape the params, if needed
            string _params = System.Web.HttpUtility.UrlPathEncode(_builder.ToString());

            // Sign the call, if necessary
            if (Service.EnforceSigning && Service.SecretKeyBytes != null)
            {
                _result += string.Format(@"api_sig={0}", GetAPISig());
            }

            // Create the call
            return string.Format(_result, Service.APIUrl, this.MethodName, _params);

        }

        /// <summary>
        /// Generates the api_sig parameter
        /// </summary>
        /// <returns>The API Signature MD5 hash.</returns>
        internal string GetAPISig()
        {
            MD5 _md5 = MD5.Create();
            string _source = "";

            // The keys need to be sorted before hashing.
            SortedList<string, string> sorted = new SortedList<string, string>();
            sorted.Add("method", this.MethodName);

            foreach (string pname in this.Parameters.Keys)
            {
                // Do not add the file parameter to the signing
                if (pname != "file")
                {
                    sorted.Add(pname, this.Parameters[pname]);
                }
            }

            // now that we have keys in sorted order... build up the string.
            foreach (KeyValuePair<string, string> kvp in sorted)
            {
                _source += kvp.Key + kvp.Value;
            }

            byte[] _data;
            byte[] _key = Service.SecretKeyBytes;
            StringBuilder _builder;

            _data = Encoding.Default.GetBytes(_source);
            
            // create a byte array that begins with the secret-key-bytes followed by the parameter-bytes
            byte[] concat = new byte[_key.Length + _data.Length];
            System.Buffer.BlockCopy(_key, 0, concat, 0, _key.Length);
            System.Buffer.BlockCopy(_data, 0, concat, _key.Length, _data.Length);

            concat = _md5.ComputeHash(concat);

            _builder = new StringBuilder();
            for (int _i = 0; _i < concat.Length; _i++)
            {
                _builder.Append(concat[_i].ToString("x2"));
            }

            return _builder.ToString();
        }

        #region IDisposable Members

        public void Dispose()
        {
            // Sink it.
        }

        #endregion
    }
}