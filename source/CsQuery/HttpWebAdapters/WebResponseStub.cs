#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Net;

namespace HttpWebAdapters {

    /// <summary>
    /// A basic implementation for a WebResponse
    /// </summary>

	public class WebResponseStub : WebResponse, IHttpWebResponse {
		private CookieCollection cookies;
		private string contentEncoding;
		private string characterSet;
		private string server;
		private DateTime lastModified;
		private HttpStatusCode statusCode;
		private string statusDescription;
		private Version protocolVersion;
		private string method;

		/// <summary>
		///Gets the contents of a header that was returned with the response.
		/// </summary>
		///
		/// <returns>
		///The contents of the specified header.
		/// </returns>
		///
		/// <param name="headerName">The header value to return. </param>
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception>
		public string GetResponseHeader(string headerName) {
			throw new NotImplementedException();
		}

		/// <summary>
		///Gets or sets the cookies that are associated with this response.
		/// </summary>
		///
		/// <returns>
		///A <see cref="T:System.Net.CookieCollection"></see> that contains the cookies that are associated with this response.
		/// </returns>
		///
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception>
		public CookieCollection Cookies {
			get { return cookies; }
			set { cookies = value; }
		}

		/// <summary>
		///Gets the method that is used to encode the body of the response.
		/// </summary>
		///
		/// <returns>
		///A string that describes the method that is used to encode the body of the response.
		/// </returns>
		///
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception>
		public string ContentEncoding {
			get { return contentEncoding; }
			set { contentEncoding = value; }
		}

		/// <summary>
		///Gets the character set of the response.
		/// </summary>
		///
		/// <returns>
		///A string that contains the character set of the response.
		/// </returns>
		///
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" /></PermissionSet>
		public string CharacterSet {
			get { return characterSet; }
			set { characterSet = value; }
		}

		/// <summary>
		///Gets the name of the server that sent the response.
		/// </summary>
		///
		/// <returns>
		///A string that contains the name of the server that sent the response.
		/// </returns>
		///
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception>
		public string Server {
			get { return server; }
			set { server = value; }
		}

		/// <summary>
		///Gets the last date and time that the contents of the response were modified.
		/// </summary>
		///
		/// <returns>
		///A <see cref="T:System.DateTime"></see> that contains the date and time that the contents of the response were modified.
		/// </returns>
		///
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" /></PermissionSet>
		public DateTime LastModified {
			get { return lastModified; }
			set { lastModified = value; }
		}

		/// <summary>
		///Gets the status of the response.
		/// </summary>
		///
		/// <returns>
		///One of the <see cref="T:System.Net.HttpStatusCode"></see> values.
		/// </returns>
		///
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception>
		public HttpStatusCode StatusCode {
			get { return statusCode; }
			set { statusCode = value; }
		}

		/// <summary>
		///Gets the status description returned with the response.
		/// </summary>
		///
		/// <returns>
		///A string that describes the status of the response.
		/// </returns>
		///
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception>
		public string StatusDescription {
			get { return statusDescription; }
			set { statusDescription = value; }
		}

		/// <summary>
		///Gets the version of the HTTP protocol that is used in the response.
		/// </summary>
		///
		/// <returns>
		///A <see cref="T:System.Version"></see> that contains the HTTP protocol version of the response.
		/// </returns>
		///
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception>
		public Version ProtocolVersion {
			get { return protocolVersion; }
			set { protocolVersion = value; }
		}

		/// <summary>
		///Gets the method that is used to return the response.
		/// </summary>
		///
		/// <returns>
		///A string that contains the HTTP method that is used to return the response.
		/// </returns>
		///
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception>
		public string Method {
			get { return method; }
			set { method = value; }
		}
	}
}