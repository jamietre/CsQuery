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
using System.IO;
using System.Net;

namespace HttpWebAdapters.Adapters {

    /// <summary>
    /// HTTP web response adapter
    /// </summary>

	public class HttpWebResponseAdapter : IHttpWebResponse {
		private WebResponse response;

        /// <summary>
        /// Create a new HttpWebResponseAdapter from a .NET WebResponse
        /// </summary>
        ///
        /// <param name="response">
        /// The HttpWebResponseAdapter.
        /// </param>

		public HttpWebResponseAdapter(WebResponse response) {
			this.response = response; // TODO use DuckTyping!
		}

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
			try {
				return ((HttpWebResponse) response).GetResponseHeader(headerName);
			} catch (InvalidCastException) {
				return ((WebResponseStub) response).GetResponseHeader(headerName);
			}
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
			get {
				try {
					return ((HttpWebResponse) response).Cookies;
				} catch (InvalidCastException) {
					return ((WebResponseStub) response).Cookies;
				}
			}
			set {
				try {
					((HttpWebResponse) response).Cookies = value;
				} catch (InvalidCastException) {
					((WebResponseStub) response).Cookies = value;
				}
			}
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
			get {
				try {
					return ((HttpWebResponse) response).ContentEncoding;
				} catch (InvalidCastException) {
					return ((WebResponseStub) response).ContentEncoding;
				}
			}
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
			get {
				try {
					return ((HttpWebResponse) response).CharacterSet;
				} catch (InvalidCastException) {
					return ((WebResponseStub) response).CharacterSet;
				}
			}
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
			get {
				try {
					return ((HttpWebResponse) response).Server;
				} catch (InvalidCastException) {
					return ((WebResponseStub) response).Server;
				}
			}
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
			get {
				try {
					return ((HttpWebResponse) response).LastModified;
				} catch (InvalidCastException) {
					return ((WebResponseStub) response).LastModified;
				}
			}
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
			get {
				try {
					return ((HttpWebResponse) response).StatusCode;
				} catch (InvalidCastException) {
					return ((WebResponseStub) response).StatusCode;
				}
			}
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
			get {
				try {
					return ((HttpWebResponse) response).StatusDescription;
				} catch (InvalidCastException) {
					return ((WebResponseStub) response).StatusDescription;
				}
			}
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
			get {
				try {
					return ((HttpWebResponse) response).ProtocolVersion;
				} catch (InvalidCastException) {
					return ((WebResponseStub) response).ProtocolVersion;
				}
			}
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
			get {
				try {
					return ((HttpWebResponse) response).Method;
				} catch (InvalidCastException) {
					return ((WebResponseStub) response).Method;
				}
			}
		}

		/// <summary>
		///When overridden by a descendant class, closes the response stream.
		/// </summary>
		///
		/// <exception cref="T:System.NotSupportedException">Any attempt is made to access the method, when the method is not overridden in a descendant class. </exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" /></PermissionSet>
		public void Close() {
			response.Close();
		}

		/// <summary>
		///When overridden in a descendant class, returns the data stream from the Internet resource.
		/// </summary>
		///
		/// <returns>
		///An instance of the <see cref="T:System.IO.Stream"></see> class for reading data from the Internet resource.
		/// </returns>
		///
		/// <exception cref="T:System.NotSupportedException">Any attempt is made to access the method, when the method is not overridden in a descendant class. </exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" /></PermissionSet>
		public Stream GetResponseStream() {
			return response.GetResponseStream();
		}

		/// <summary>
		///Gets a <see cref="T:System.Boolean"></see> value that indicates whether this response was obtained from the cache.
		/// </summary>
		///
		/// <returns>
		///true if the response was taken from the cache; otherwise, false.
		/// </returns>
		///
		public bool IsFromCache {
			get { return response.IsFromCache; }
		}

		/// <summary>
		///Gets a <see cref="T:System.Boolean"></see> value that indicates whether mutual authentication occurred.
		/// </summary>
		///
		/// <returns>
		///true if both client and server were authenticated; otherwise, false.
		/// </returns>
		///
		public bool IsMutuallyAuthenticated {
			get { return response.IsMutuallyAuthenticated; }
		}

		/// <summary>
		///When overridden in a descendant class, gets or sets the content length of data being received.
		/// </summary>
		///
		/// <returns>
		///The number of bytes returned from the Internet resource.
		/// </returns>
		///
		/// <exception cref="T:System.NotSupportedException">Any attempt is made to get or set the property, when the property is not overridden in a descendant class. </exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" /></PermissionSet>
		public long ContentLength {
			get { return response.ContentLength; }
			set { response.ContentLength = value; }
		}

		/// <summary>
		///When overridden in a derived class, gets or sets the content type of the data being received.
		/// </summary>
		///
		/// <returns>
		///A string that contains the content type of the response.
		/// </returns>
		///
		/// <exception cref="T:System.NotSupportedException">Any attempt is made to get or set the property, when the property is not overridden in a descendant class. </exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" /></PermissionSet>
		public string ContentType {
			get { return response.ContentType; }
			set { response.ContentType = value; }
		}

		/// <summary>
		///When overridden in a derived class, gets the URI of the Internet resource that actually responded to the request.
		/// </summary>
		///
		/// <returns>
		///An instance of the <see cref="T:System.Uri"></see> class that contains the URI of the Internet resource that actually responded to the request.
		/// </returns>
		///
		/// <exception cref="T:System.NotSupportedException">Any attempt is made to get or set the property, when the property is not overridden in a descendant class. </exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" /></PermissionSet>
		public Uri ResponseUri {
			get { return response.ResponseUri; }
		}

		/// <summary>
		///When overridden in a derived class, gets a collection of header name-value pairs associated with this request.
		/// </summary>
		///
		/// <returns>
		///An instance of the <see cref="T:System.Net.WebHeaderCollection"></see> class that contains header values associated with this response.
		/// </returns>
		///
		/// <exception cref="T:System.NotSupportedException">Any attempt is made to get or set the property, when the property is not overridden in a descendant class. </exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" /></PermissionSet>
		public WebHeaderCollection Headers {
			get { return response.Headers; }
		}

		/// <summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose() {
            response.Close();
			//response.GetType().GetMethod("Dispose", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(response, null);
		}
	}
}