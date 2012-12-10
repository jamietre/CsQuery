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

namespace HttpWebAdapters {

    /// <summary>
    /// Interface for an HTTP web response.
    /// </summary>

	public interface IHttpWebResponse : IDisposable {
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
		string GetResponseHeader(string headerName);

		/// <summary>
		///Gets or sets the cookies that are associated with this response.
		/// </summary>
		///
		/// <returns>
		///A <see cref="T:System.Net.CookieCollection"></see> that contains the cookies that are associated with this response.
		/// </returns>
		///
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception>
		CookieCollection Cookies { get; set; }

		/// <summary>
		///Gets the method that is used to encode the body of the response.
		/// </summary>
		///
		/// <returns>
		///A string that describes the method that is used to encode the body of the response.
		/// </returns>
		///
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception>
		string ContentEncoding { get; }

		/// <summary>
		///Gets the character set of the response.
		/// </summary>
		///
		/// <returns>
		///A string that contains the character set of the response.
		/// </returns>
		///
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" /></PermissionSet>
		string CharacterSet { get; }

		/// <summary>
		///Gets the name of the server that sent the response.
		/// </summary>
		///
		/// <returns>
		///A string that contains the name of the server that sent the response.
		/// </returns>
		///
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception>
		string Server { get; }

		/// <summary>
		///Gets the last date and time that the contents of the response were modified.
		/// </summary>
		///
		/// <returns>
		///A <see cref="T:System.DateTime"></see> that contains the date and time that the contents of the response were modified.
		/// </returns>
		///
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" /></PermissionSet>
		DateTime LastModified { get; }

		/// <summary>
		///Gets the status of the response.
		/// </summary>
		///
		/// <returns>
		///One of the <see cref="T:System.Net.HttpStatusCode"></see> values.
		/// </returns>
		///
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception>
		HttpStatusCode StatusCode { get; }

		/// <summary>
		///Gets the status description returned with the response.
		/// </summary>
		///
		/// <returns>
		///A string that describes the status of the response.
		/// </returns>
		///
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception>
		string StatusDescription { get; }

		/// <summary>
		///Gets the version of the HTTP protocol that is used in the response.
		/// </summary>
		///
		/// <returns>
		///A <see cref="T:System.Version"></see> that contains the HTTP protocol version of the response.
		/// </returns>
		///
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception>
		Version ProtocolVersion { get; }

		/// <summary>
		///Gets the method that is used to return the response.
		/// </summary>
		///
		/// <returns>
		///A string that contains the HTTP method that is used to return the response.
		/// </returns>
		///
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed. </exception>
		string Method { get; }

		/// <summary>
		///When overridden by a descendant class, closes the response stream.
		/// </summary>
		///
		/// <exception cref="T:System.NotSupportedException">Any attempt is made to access the method, when the method is not overridden in a descendant class. </exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" /></PermissionSet>
		void Close();

		/// <summary>
		///When overridden in a descendant class, returns the data stream from the Internet resource.
		/// </summary>
		///
		/// <returns>
		///An instance of the <see cref="T:System.IO.Stream"></see> class for reading data from the Internet resource.
		/// </returns>
		///
		/// <exception cref="T:System.NotSupportedException">Any attempt is made to access the method, when the method is not overridden in a descendant class. </exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" /></PermissionSet>
		Stream GetResponseStream();

		/// <summary>
		///Gets a <see cref="T:System.Boolean"></see> value that indicates whether this response was obtained from the cache.
		/// </summary>
		///
		/// <returns>
		///true if the response was taken from the cache; otherwise, false.
		/// </returns>
		///
		bool IsFromCache { get; }

		/// <summary>
		///Gets a <see cref="T:System.Boolean"></see> value that indicates whether mutual authentication occurred.
		/// </summary>
		///
		/// <returns>
		///true if both client and server were authenticated; otherwise, false.
		/// </returns>
		///
		bool IsMutuallyAuthenticated { get; }

		/// <summary>
		///When overridden in a descendant class, gets or sets the content length of data being received.
		/// </summary>
		///
		/// <returns>
		///The number of bytes returned from the Internet resource.
		/// </returns>
		///
		/// <exception cref="T:System.NotSupportedException">Any attempt is made to get or set the property, when the property is not overridden in a descendant class. </exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" /></PermissionSet>
		long ContentLength { get; set; }

		/// <summary>
		///When overridden in a derived class, gets or sets the content type of the data being received.
		/// </summary>
		///
		/// <returns>
		///A string that contains the content type of the response.
		/// </returns>
		///
		/// <exception cref="T:System.NotSupportedException">Any attempt is made to get or set the property, when the property is not overridden in a descendant class. </exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" /></PermissionSet>
		string ContentType { get; set; }

		/// <summary>
		///When overridden in a derived class, gets the URI of the Internet resource that actually responded to the request.
		/// </summary>
		///
		/// <returns>
		///An instance of the <see cref="T:System.Uri"></see> class that contains the URI of the Internet resource that actually responded to the request.
		/// </returns>
		///
		/// <exception cref="T:System.NotSupportedException">Any attempt is made to get or set the property, when the property is not overridden in a descendant class. </exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" /></PermissionSet>
		Uri ResponseUri { get; }

		/// <summary>
		///When overridden in a derived class, gets a collection of header name-value pairs associated with this request.
		/// </summary>
		///
		/// <returns>
		///An instance of the <see cref="T:System.Net.WebHeaderCollection"></see> class that contains header values associated with this response.
		/// </returns>
		///
		/// <exception cref="T:System.NotSupportedException">Any attempt is made to get or set the property, when the property is not overridden in a descendant class. </exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" /></PermissionSet>
		WebHeaderCollection Headers { get; }
	}
}