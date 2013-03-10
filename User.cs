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
    /// A Scribd User
    /// </summary>
    public sealed class User
    {
        private bool m_isLoggedIn;
        private List<Document> m_cachedList;

        #region Constructors

        internal User() { }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Raised before a log in is attempted.
        /// </summary>
        public static event EventHandler<UserEventArgs> BeforeLogin;

        /// <summary>
        /// Raised when a user is successfully logged into Scribd.
        /// </summary>
        public static event EventHandler<UserEventArgs> LoggedIn;

        /// <summary>
        /// Raised when a user fails to log in.
        /// </summary>
        public static event EventHandler<UserEventArgs> LoginFailed;

        /// <summary>
        /// Raised prior to submitting a user for sign up.
        /// </summary>
        public static event EventHandler<UserEventArgs> BeforeSignUp;

        /// <summary>
        /// Raised after submitting a user for sign up.
        /// </summary>
        public static event EventHandler<UserEventArgs> AfterSignUp;

        /// <summary>
        /// Raised prior to logging out the current user.
        /// </summary>
        public static event EventHandler<UserEventArgs> BeforeLogout;

        /// <summary>
        /// Raised after logging out the current user.
        /// </summary>
        public static event EventHandler<UserEventArgs> AfterLogout;

        internal void OnLoggedIn(UserEventArgs args)
        {
            if (LoggedIn != null)
            {
                LoggedIn(this, args);
            }
        }

        internal void OnLoginFailed(UserEventArgs args)
        {
            if (LoginFailed != null)
            {
                LoginFailed(this, args);
            }
        }

        #endregion Events

        #region User Management

        /// <summary>
        /// Validates the specified credentials against the Scribd service.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns>An initialized User object for the specified user. Null if credentials are not valid.</returns>
        public static User ValidateCredentials(string userName, string password)
        {
            User user;
            _Login(userName, password, true, out user);
            return user;
        }

        /// <summary>
        /// Logs a user into the service.
        /// </summary>
        /// <param name="userName">Username associated with your Scribd account</param>
        /// <param name="password">Password associated with your Scribd account</param>
        /// <returns>True on success</returns>
        public static bool Login(string userName, string password)
        {
            User notUsed;
            return _Login(userName, password, false, out notUsed);
        }

        /// <summary>
        /// Logs a user into the service.
        /// </summary>
        /// <param name="userName">Username associated with your Scribd account</param>
        /// <param name="password">Password associated with your Scribd account</param>
        /// <param name="verifyOnly">Just verifying credentials, don't change state of 'logged in' user</param>
        /// <param name="newUser">(out) Can be used by called to get the newly created User</param>
        /// <returns>True on success</returns>
        private static bool _Login(string userName, string password, bool verifyOnly, out User newUser)
        {
            UserEventArgs _args = new UserEventArgs();
            if (BeforeLogin != null)
            {
                _args.Success = false;
                _args.User.UserName = userName;
            }

            newUser = null;

            if (!_args.Cancel)
            {
                using (Request _request = new Request())
                {
                    _request.MethodName = "user.login";
                    _request.Parameters.Add("username", userName);
                    _request.Parameters.Add("password", password);

                    using (Response _response = Service.Instance.PostRequest(_request))
                    {
                        // Parse response
                        User _result = new User();
                        if (_response != null && _response.HasChildNodes && _response.ErrorList.Count < 1)
                        {
                            XmlNodeList _list = _response.GetElementsByTagName("rsp");
                            if (_list.Count > 0)
                            {
                                foreach (XmlNode _node in _list)
                                {
                                    _result.Name = _node.SelectSingleNode("name").InnerText.Trim();
                                    _result.SessionKey = _node.SelectSingleNode("session_key").InnerText.Trim();
                                    _result.UserId = int.Parse(_node.SelectSingleNode("user_id").InnerText.Trim());
                                    _result.UserName = _node.SelectSingleNode("username").InnerText.Trim();
                                    _result.m_isLoggedIn = true;

                                    newUser = _result;

                                    _args.Success = true;
                                    _args.User = _result;

                                    if (!verifyOnly)
                                    {
                                        Service.Instance.InternalUser = _result;
                                        Service.Instance.InternalUser.OnLoggedIn(_args);
                                    }

                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            if (!verifyOnly)
            {
                // ??? It seems wrong to leave any previous 'logged in' user in tact. Code setting 'logged_in' flag to false.
                // but that flag doesn't do anything. Seems like clearing the Service.InternalUser is the right thing to do.
            Service.Instance.InternalUser.m_isLoggedIn = false;
            }

            _args.Success = false;

            // Notify subscribers of failure.
            Service.Instance.InternalUser.OnLoginFailed(_args);

            return false;
        }

        /// <summary>
        /// Clear the currently logged in user state.
        /// </summary>
        /// <returns>true means there no currently logged in user.</returns>
        public static bool Logout()
        {
            UserEventArgs _args = new UserEventArgs();

            if (BeforeLogout != null)
            {
                User.BeforeLogout(null, _args);
            }

            if (!_args.Cancel)
            {
                Service.Instance.InternalUser = new User();
                _args.Success = true;
            }
            else
            {
                _args.Success = false;
            }

            if (AfterLogout != null)
            {
                User.AfterLogout(null, _args);
            }

            return _args.Success;
        }

        /// <summary>
        /// Signs a user up for Scribd.  Doing so will log the 
        /// current user profile out and replace it with this one.
        /// </summary>
        /// <param name="userName">Username to associate with your Scribd account</param>
        /// <param name="password">Password to associate with your Scribd account</param>
        /// <param name="email">Your email address</param>
        /// <param name="name">Your real name</param>
        /// <returns>True on success</returns>
        public static bool Signup(string userName, string password, string email, string name)
        {
            UserEventArgs _args = new UserEventArgs();
            if (BeforeSignUp != null)
            {
                _args.Success = false;
                _args.User.UserName = userName;
                _args.User.Name = name;
            }

            if (!_args.Cancel)
            {
                using (Request _request = new Request())
                {
                    _request.MethodName = "user.signup";
                    _request.Parameters.Add("username", userName);
                    _request.Parameters.Add("password", password);
                    _request.Parameters.Add("email", email);
                    _request.Parameters.Add("name", name);

                    User cachedUser = Service.Instance.InternalUser;
                    Service.Instance.InternalUser = null;

                    using (Response _response = Service.Instance.PostRequest(_request))
                    {
                        Service.Instance.InternalUser = cachedUser;

                        // Parse response
                        User _result = new User();
                        if (_response != null && _response.HasChildNodes && _response.ErrorList.Count < 1)
                        {
                            XmlNodeList _list = _response.GetElementsByTagName("rsp");
                            if (_list.Count > 0)
                            {
                                foreach (XmlNode _node in _list)
                                {
                                    _result.Name = _node.SelectSingleNode("name").InnerText.Trim();
                                    _result.SessionKey = _node.SelectSingleNode("session_key").InnerText.Trim();
                                    _result.UserId = int.Parse(_node.SelectSingleNode("user_id").InnerText.Trim());
                                    _result.UserName = _node.SelectSingleNode("username").InnerText.Trim();
                                    _result.m_isLoggedIn = true;

                                    // Notify subscribers of sign-up success.
                                    _args.Success = true;
                                    _args.User = _result;
                                    if (User.AfterSignUp != null)
                                    {
                                        User.AfterSignUp(null, _args);
                                    }

                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            _args.Success = false;
            if (User.AfterSignUp != null)
            {
                User.AfterSignUp(null, _args);
            }

            return false;
        }

        /// <summary>
        /// This method returns a URL that, when visited, will automatically sign in the 
        /// current user and then redirect to the URL you provide.
        /// </summary>
        /// <param name="nextUrl">The URL or path portion of a Scribd URL to redirect to.</param>
        /// <returns>The auto-signin URL.</returns>
        public static string GetAutoSigninUrl(string nextUrl)
        {
            string _result = nextUrl;
            // Set up our request
            using (Request _request = new Request())
            {
                _request.MethodName = "user.getAutoSigninUrl";
                _request.Parameters.Add("next_url", nextUrl);

                // Grab our response
                using (Response _response = Service.Instance.PostRequest(_request))
                {

                    // Parse the response
                    if (_response != null && _response.HasChildNodes && _response.ErrorList.Count < 1)
                    {
                        XmlNode _node = _response.SelectSingleNode("rsp");

                        // Data
                        _result = _node.SelectSingleNode("url").InnerText;
                    }
                }
            }

            return _result;
        }

        #endregion User Managment

        #region Properties

        /// <summary>
        /// Has the user logged in?
        /// </summary>
        public bool IsLoggedIn { get { return m_isLoggedIn; } }

        /// <summary>
        /// The session_key is a string uniquely associated with this 
        /// user and your API account. You can use the session key 
        /// with most of the methods of the API to upload, delete, 
        /// etc. documents as the user you signed in as. 
        /// </summary>
        public string SessionKey { get; set; }

        /// <summary>
        /// The name of the user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The username of the user.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///	A local user identifier for 3rd party integrations.
        ///	See: http://www.scribd.com/platform/documentation?subtab=api&amp;method_name=Authentication
        /// </summary>
        public string PhantomName { get; set; }

        /// <summary>
        /// Every user on Scribd has a user ID. 
        /// You can use this user ID to link to the profile of 
        /// a user or for a number of other pages on Scribd which 
        /// take it as a parameter. 
        /// </summary>
        public int UserId { get; set; }

        #endregion Properties

        #region Document methods

        /// <summary>
        /// This method retrieves a list of documents for a given user.
        /// </summary>
        /// <returns>A list of Documents associated to this user's Scribd account.</returns>
        public List<Document> Documents
        {
            get
            {
                if (this.m_cachedList == null)
                {
                    // We're not already loaded ...
                    this.m_cachedList = new List<Document>();

                    // Pull the list and add it to our cache.
                    this.m_cachedList.AddRange(Document.GetList(this, true));
                    
                }
                return this.m_cachedList;
            }
        }

        /// <summary>
        /// Re-Pulls Documents from Scribd instead of the 
        /// locally cached collection.
        /// </summary>
        public void ReloadDocuments()
        {
            this.m_cachedList = null;
        }

        #endregion Document methods
    }
}