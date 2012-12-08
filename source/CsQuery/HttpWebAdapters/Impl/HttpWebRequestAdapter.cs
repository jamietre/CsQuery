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
using System.Security.Cryptography.X509Certificates;

namespace HttpWebAdapters.Adapters {
	public class HttpWebRequestAdapter : IHttpWebRequest {
		private HttpWebRequest request;

		public HttpWebRequestAdapter(HttpWebRequest request) {
			this.request = request;
		}

		public HttpWebRequestMethod Method {
			get { return HttpWebRequestMethod.Parse(request.Method); }
			set { request.Method = value.ToString(); }
		}

		public IHttpWebResponse GetResponse() {
			return new HttpWebResponseAdapter(request.GetResponse());
		}

        public IAsyncResult BeginGetResponse(AsyncCallback callback, object state) {
            return request.BeginGetResponse(callback, state);
        }

        public IHttpWebResponse EndGetResponse(IAsyncResult result) {
            return new HttpWebResponseAdapter(request.EndGetResponse(result));
        }

        public IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state) {
            return request.BeginGetRequestStream(callback, state);
        }

        public Stream EndGetRequestStream(IAsyncResult result) {
            return request.EndGetRequestStream(result);
        }

		///<summary>
		///Gets a <see cref="T:System.IO.Stream"></see> object to use to write request data.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.IO.Stream"></see> to use to write request data.
		///</returns>
		///
		///<exception cref="T:System.InvalidOperationException">The <see cref="M:System.Net.HttpWebRequest.GetRequestStream"></see> method is called more than once.-or- <see cref="P:System.Net.HttpWebRequest.TransferEncoding"></see> is set to a value and <see cref="P:System.Net.HttpWebRequest.SendChunked"></see> is false. </exception>
		///<exception cref="T:System.ObjectDisposedException">In a .NET Compact Framework application, a request stream with zero content length was not obtained and closed correctly. For more information about handling zero content length requests, see Network Programming in the .NET Compact Framework.</exception>
		///<exception cref="T:System.Net.WebException"><see cref="M:System.Net.HttpWebRequest.Abort"></see> was previously called.-or- The time-out period for the request expired.-or- An error occurred while processing the request. </exception>
		///<exception cref="T:System.NotSupportedException">The request cache validator indicated that the response for this request can be served from the cache; however, requests that write data must not use the cache. This exception can occur if you are using a custom cache validator that is incorrectly implemented. </exception>
		///<exception cref="T:System.Net.ProtocolViolationException">The <see cref="P:System.Net.HttpWebRequest.Method"></see> property is GET or HEAD.-or- <see cref="P:System.Net.HttpWebRequest.KeepAlive"></see> is true, <see cref="P:System.Net.HttpWebRequest.AllowWriteStreamBuffering"></see> is false, <see cref="P:System.Net.HttpWebRequest.ContentLength"></see> is -1, <see cref="P:System.Net.HttpWebRequest.SendChunked"></see> is false, and <see cref="P:System.Net.HttpWebRequest.Method"></see> is POST or PUT. </exception><PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /><IPermission class="System.Net.DnsPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /><IPermission class="System.Net.WebPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /></PermissionSet>
		public Stream GetRequestStream() {
			return request.GetRequestStream();
		}

		///<summary>
		///Cancels a request to an Internet resource.
		///</summary>
		///<PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" /></PermissionSet>
		public void Abort() {
			request.Abort();
		}

		///<summary>
		///Adds a byte range header to the request for a specified range.
		///</summary>
		///
		///<param name="to">The position at which to stop sending data. </param>
		///<param name="from">The position at which to start sending data. </param>
		///<exception cref="T:System.ArgumentException">rangeSpecifier is invalid. </exception>
		///<exception cref="T:System.ArgumentOutOfRangeException">from is greater than to-or- from or to is less than 0. </exception>
		///<exception cref="T:System.InvalidOperationException">The range header could not be added. </exception>
		public void AddRange(int from, int to) {
			request.AddRange(from, to);
		}

		///<summary>
		///Adds a byte range header to a request for a specific range from the beginning or end of the requested data.
		///</summary>
		///
		///<param name="range">The starting or ending point of the range. </param>
		///<exception cref="T:System.ArgumentException">rangeSpecifier is invalid. </exception>
		///<exception cref="T:System.InvalidOperationException">The range header could not be added. </exception>
		public void AddRange(int range) {
			request.AddRange(range);
		}

		///<summary>
		///Adds a range header to a request for a specified range.
		///</summary>
		///
		///<param name="from">The position at which to start sending data. </param>
		///<param name="to">The position at which to stop sending data. </param>
		///<param name="rangeSpecifier">The description of the range. </param>
		///<exception cref="T:System.ArgumentException">rangeSpecifier is invalid. </exception>
		///<exception cref="T:System.ArgumentNullException">rangeSpecifier is null. </exception>
		///<exception cref="T:System.ArgumentOutOfRangeException">from is greater than to-or- from or to is less than 0. </exception>
		///<exception cref="T:System.InvalidOperationException">The range header could not be added. </exception>
		public void AddRange(string rangeSpecifier, int from, int to) {
			request.AddRange(rangeSpecifier, from, to);
		}

		///<summary>
		///Adds a range header to a request for a specific range from the beginning or end of the requested data.
		///</summary>
		///
		///<param name="range">The starting or ending point of the range. </param>
		///<param name="rangeSpecifier">The description of the range. </param>
		///<exception cref="T:System.ArgumentException">rangeSpecifier is invalid. </exception>
		///<exception cref="T:System.ArgumentNullException">rangeSpecifier is null. </exception>
		///<exception cref="T:System.InvalidOperationException">The range header could not be added. </exception>
		public void AddRange(string rangeSpecifier, int range) {
			request.AddRange(rangeSpecifier, range);
		}

		///<summary>
		///Gets or sets a value that indicates whether the request should follow redirection responses.
		///</summary>
		///
		///<returns>
		///true if the request should automatically follow redirection responses from the Internet resource; otherwise, false. The default value is true.
		///</returns>
		///
		public bool AllowAutoRedirect {
			get { return request.AllowAutoRedirect; }
			set { request.AllowAutoRedirect = value; }
		}

		///<summary>
		///Gets or sets a value that indicates whether to buffer the data sent to the Internet resource.
		///</summary>
		///
		///<returns>
		///true to enable buffering of the data sent to the Internet resource; false to disable buffering. The default is true.
		///</returns>
		///
		public bool AllowWriteStreamBuffering {
			get { return request.AllowWriteStreamBuffering; }
			set { request.AllowWriteStreamBuffering = value; }
		}

		///<summary>
		///Gets a value that indicates whether a response has been received from an Internet resource.
		///</summary>
		///
		///<returns>
		///true if a response has been received; otherwise, false.
		///</returns>
		///
		public bool HaveResponse {
			get { return request.HaveResponse; }
		}

		///<summary>
		///Gets or sets a value that indicates whether to make a persistent connection to the Internet resource.
		///</summary>
		///
		///<returns>
		///true if the request to the Internet resource should contain a Connection HTTP header with the value Keep-alive; otherwise, false. The default is true.
		///</returns>
		///
		public bool KeepAlive {
			get { return request.KeepAlive; }
			set { request.KeepAlive = value; }
		}

		///<summary>
		///Gets or sets a value that indicates whether to pipeline the request to the Internet resource.
		///</summary>
		///
		///<returns>
		///true if the request should be pipelined; otherwise, false. The default is true.
		///</returns>
		///
		public bool Pipelined {
			get { return request.Pipelined; }
			set { request.Pipelined = value; }
		}

		///<summary>
		///Gets or sets a value that indicates whether to send an authenticate header with the request.
		///</summary>
		///
		///<returns>
		///true to send a WWW-authenticate HTTP header with requests after authentication has taken place; otherwise, false. The default is false.
		///</returns>
		///
		public bool PreAuthenticate {
			get { return request.PreAuthenticate; }
			set { request.PreAuthenticate = value; }
		}

		///<summary>
		///Gets or sets a value that indicates whether to allow high-speed NTLM-authenticated connection sharing.
		///</summary>
		///
		///<returns>
		///true to keep the authenticated connection open; otherwise, false.
		///</returns>
		///<PermissionSet><IPermission class="System.Net.WebPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /></PermissionSet>
		public bool UnsafeAuthenticatedConnectionSharing {
			get { return request.UnsafeAuthenticatedConnectionSharing; }
			set { request.UnsafeAuthenticatedConnectionSharing = value; }
		}

		///<summary>
		///Gets or sets a value that indicates whether to send data in segments to the Internet resource.
		///</summary>
		///
		///<returns>
		///true to send data to the Internet resource in segments; otherwise, false. The default value is false.
		///</returns>
		///
		///<exception cref="T:System.InvalidOperationException">The request has been started by calling the <see cref="M:System.Net.HttpWebRequest.GetRequestStream"></see>, <see cref="M:System.Net.HttpWebRequest.BeginGetRequestStream(System.AsyncCallback,System.Object)"></see>, <see cref="M:System.Net.HttpWebRequest.GetResponse"></see>, or <see cref="M:System.Net.HttpWebRequest.BeginGetResponse(System.AsyncCallback,System.Object)"></see> method. </exception>
		public bool SendChunked {
			get { return request.SendChunked; }
			set { request.SendChunked = value; }
		}

		///<summary>
		///Gets or sets the type of decompression that is used.
		///</summary>
		///
		///<returns>
		///A T:System.Net.DecompressionMethods object that indicates the type of decompression that is used. 
		///</returns>
		///
		///<exception cref="T:System.InvalidOperationException">The object's current state does not allow this property to be set.</exception>
		public DecompressionMethods AutomaticDecompression {
			get { return request.AutomaticDecompression; }
			set { request.AutomaticDecompression = value; }
		}

		///<summary>
		///Gets or sets the maximum allowed length of the response headers.
		///</summary>
		///
		///<returns>
		///The length, in kilobytes (1024 bytes), of the response headers.
		///</returns>
		///
		///<exception cref="T:System.InvalidOperationException">The property is set after the request has already been submitted. </exception>
		///<exception cref="T:System.ArgumentOutOfRangeException">The value is less than 0 and is not equal to -1. </exception>
		public int MaximumResponseHeadersLength {
			get { return request.MaximumResponseHeadersLength; }
			set { request.MaximumResponseHeadersLength = value; }
		}

		///<summary>
		///Gets or sets the collection of security certificates that are associated with this request.
		///</summary>
		///
		///<returns>
		///The <see cref="T:System.Security.Cryptography.X509Certificates.X509CertificateCollection"></see> that contains the security certificates associated with this request.
		///</returns>
		///
		///<exception cref="T:System.ArgumentNullException">The value specified for a set operation is null. </exception>
		public X509CertificateCollection ClientCertificates {
			get { return request.ClientCertificates; }
			set { request.ClientCertificates = value; }
		}

		///<summary>
		///Gets or sets the cookies associated with the request.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.Net.CookieContainer"></see> that contains the cookies associated with this request.
		///</returns>
		///
		public CookieContainer CookieContainer {
			get { return request.CookieContainer; }
			set { request.CookieContainer = value; }
		}

		///<summary>
		///Gets the original Uniform Resource Identifier (URI) of the request.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.Uri"></see> that contains the URI of the Internet resource passed to the <see cref="M:System.Net.WebRequest.Create(System.String)"></see> method.
		///</returns>
		///
		public Uri RequestUri {
			get { return request.RequestUri; }
		}

		///<summary>
		///Gets or sets the Content-length HTTP header.
		///</summary>
		///
		///<returns>
		///The number of bytes of data to send to the Internet resource. The default is -1, which indicates the property has not been set and that there is no request data to send.
		///</returns>
		///
		///<exception cref="T:System.ArgumentOutOfRangeException">The new <see cref="P:System.Net.HttpWebRequest.ContentLength"></see> value is less than 0. </exception>
		///<exception cref="T:System.InvalidOperationException">The request has been started by calling the <see cref="M:System.Net.HttpWebRequest.GetRequestStream"></see>, <see cref="M:System.Net.HttpWebRequest.BeginGetRequestStream(System.AsyncCallback,System.Object)"></see>, <see cref="M:System.Net.HttpWebRequest.GetResponse"></see>, or <see cref="M:System.Net.HttpWebRequest.BeginGetResponse(System.AsyncCallback,System.Object)"></see> method. </exception>
		public long ContentLength {
			get { return request.ContentLength; }
			set { request.ContentLength = value; }
		}

		///<summary>
		///Gets or sets the time-out value for the <see cref="M:System.Net.HttpWebRequest.GetResponse"></see> and <see cref="M:System.Net.HttpWebRequest.GetRequestStream"></see> methods.
		///</summary>
		///
		///<returns>
		///The number of milliseconds to wait before the request times out. The default is 100,000 milliseconds (100 seconds).
		///</returns>
		///
		///<exception cref="T:System.ArgumentOutOfRangeException">The value specified is less than zero and is not <see cref="F:System.Threading.Timeout.Infinite"></see>.</exception>
		public int Timeout {
			get { return request.Timeout; }
			set { request.Timeout = value; }
		}

		///<summary>
		///Gets or sets a time-out when writing to or reading from a stream.
		///</summary>
		///
		///<returns>
		///The number of milliseconds before the writing or reading times out. The default value is 300,000 milliseconds (5 minutes).
		///</returns>
		///
		///<exception cref="T:System.ArgumentOutOfRangeException">The value specified for a set operation is less than or equal to zero and is not equal to <see cref="F:System.Threading.Timeout.Infinite"></see></exception>
		///<exception cref="T:System.InvalidOperationException">The request has already been sent. </exception>
		public int ReadWriteTimeout {
			get { return request.ReadWriteTimeout; }
			set { request.ReadWriteTimeout = value; }
		}

		///<summary>
		///Gets the Uniform Resource Identifier (URI) of the Internet resource that actually responds to the request.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.Uri"></see> that identifies the Internet resource that actually responds to the request. The default is the URI used by the <see cref="M:System.Net.WebRequest.Create(System.String)"></see> method to initialize the request.
		///</returns>
		///
		public Uri Address {
			get { return request.Address; }
		}

		///<summary>
		///Gets the service point to use for the request.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.Net.ServicePoint"></see> that represents the network connection to the Internet resource.
		///</returns>
		///<PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" /></PermissionSet>
		public ServicePoint ServicePoint {
			get { return request.ServicePoint; }
		}

		///<summary>
		///Gets or sets the maximum number of redirects that the request follows.
		///</summary>
		///
		///<returns>
		///The maximum number of redirection responses that the request follows. The default value is 50.
		///</returns>
		///
		///<exception cref="T:System.ArgumentException">The value is set to 0 or less. </exception>
		public int MaximumAutomaticRedirections {
			get { return request.MaximumAutomaticRedirections; }
			set { request.MaximumAutomaticRedirections = value; }
		}

		///<summary>
		///Gets or sets authentication information for the request.
		///</summary>
		///
		///<returns>
		///An <see cref="T:System.Net.ICredentials"></see> that contains the authentication credentials associated with the request. The default is null.
		///</returns>
		///<PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" /></PermissionSet>
		public ICredentials Credentials {
			get { return request.Credentials; }
			set { request.Credentials = value; }
		}

		///<summary>
		///Gets or sets a <see cref="T:System.Boolean"></see> value that controls whether default credentials are sent with requests.
		///</summary>
		///
		///<returns>
		///true if the default credentials are used; otherwise false. The default value is false.
		///</returns>
		///
		///<exception cref="T:System.InvalidOperationException">You attempted to set this property after the request was sent.</exception><PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="USERNAME" /></PermissionSet>
		public bool UseDefaultCredentials {
			get { return request.UseDefaultCredentials; }
			set { request.UseDefaultCredentials = value; }
		}

		///<summary>
		///Gets or sets the name of the connection group for the request.
		///</summary>
		///
		///<returns>
		///The name of the connection group for this request. The default value is null.
		///</returns>
		///
		public string ConnectionGroupName {
			get { return request.ConnectionGroupName; }
			set { request.ConnectionGroupName = value; }
		}

		///<summary>
		///Specifies a collection of the name/value pairs that make up the HTTP headers.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.Net.WebHeaderCollection"></see> that contains the name/value pairs that make up the headers for the HTTP request.
		///</returns>
		///
		///<exception cref="T:System.InvalidOperationException">The request has been started by calling the <see cref="M:System.Net.HttpWebRequest.GetRequestStream"></see>, <see cref="M:System.Net.HttpWebRequest.BeginGetRequestStream(System.AsyncCallback,System.Object)"></see>, <see cref="M:System.Net.HttpWebRequest.GetResponse"></see>, or <see cref="M:System.Net.HttpWebRequest.BeginGetResponse(System.AsyncCallback,System.Object)"></see> method. </exception><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" /></PermissionSet>
		public WebHeaderCollection Headers {
			get { return request.Headers; }
			set { request.Headers = value; }
		}

		///<summary>
		///Gets or sets proxy information for the request.
		///</summary>
		///
		///<returns>
		///The <see cref="T:System.Net.IWebProxy"></see> object to use to proxy the request. The default value is set by calling the <see cref="P:System.Net.GlobalProxySelection.Select"></see> property.
		///</returns>
		///
		///<exception cref="T:System.Security.SecurityException">The caller does not have permission for the requested operation. </exception>
		///<exception cref="T:System.ArgumentNullException"><see cref="P:System.Net.HttpWebRequest.Proxy"></see> is set to null. </exception>
		///<exception cref="T:System.InvalidOperationException">The request has been started by calling <see cref="M:System.Net.HttpWebRequest.GetRequestStream"></see>, <see cref="M:System.Net.HttpWebRequest.BeginGetRequestStream(System.AsyncCallback,System.Object)"></see>, <see cref="M:System.Net.HttpWebRequest.GetResponse"></see>, or <see cref="M:System.Net.HttpWebRequest.BeginGetResponse(System.AsyncCallback,System.Object)"></see>. </exception><PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" /><IPermission class="System.Net.WebPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /></PermissionSet>
		public IWebProxy Proxy {
			get { return request.Proxy; }
			set { request.Proxy = value; }
		}

		///<summary>
		///Gets or sets the version of HTTP to use for the request.
		///</summary>
		///
		///<returns>
		///The HTTP version to use for the request. The default is <see cref="F:System.Net.HttpVersion.Version11"></see>.
		///</returns>
		///
		///<exception cref="T:System.ArgumentException">The HTTP version is set to a value other than 1.0 or 1.1. </exception>
		public Version ProtocolVersion {
			get { return request.ProtocolVersion; }
			set { request.ProtocolVersion = value; }
		}

		///<summary>
		///Gets or sets the value of the Content-type HTTP header.
		///</summary>
		///
		///<returns>
		///The value of the Content-type HTTP header. The default value is null.
		///</returns>
		///
		public string ContentType {
			get { return request.ContentType; }
			set { request.ContentType = value; }
		}

		///<summary>
		///Gets or sets the media type of the request.
		///</summary>
		///
		///<returns>
		///The media type of the request. The default value is null.
		///</returns>
		///
		public string MediaType {
			get { return request.MediaType; }
			set { request.MediaType = value; }
		}

		///<summary>
		///Gets or sets the value of the Transfer-encoding HTTP header.
		///</summary>
		///
		///<returns>
		///The value of the Transfer-encoding HTTP header. The default value is null.
		///</returns>
		///
		///<exception cref="T:System.InvalidOperationException"><see cref="P:System.Net.HttpWebRequest.TransferEncoding"></see> is set when <see cref="P:System.Net.HttpWebRequest.SendChunked"></see> is false. </exception>
		///<exception cref="T:System.ArgumentException"><see cref="P:System.Net.HttpWebRequest.TransferEncoding"></see> is set to the value "Chunked". </exception>
		public string TransferEncoding {
			get { return request.TransferEncoding; }
			set { request.TransferEncoding = value; }
		}

		///<summary>
		///Gets or sets the value of the Connection HTTP header.
		///</summary>
		///
		///<returns>
		///The value of the Connection HTTP header. The default value is null.
		///</returns>
		///
		///<exception cref="T:System.ArgumentException">The value of <see cref="P:System.Net.HttpWebRequest.Connection"></see> is set to Keep-alive or Close. </exception>
		public string Connection {
			get { return request.Connection; }
			set { request.Connection = value; }
		}

		///<summary>
		///Gets or sets the value of the Accept HTTP header.
		///</summary>
		///
		///<returns>
		///The value of the Accept HTTP header. The default value is null.
		///</returns>
		///
		public string Accept {
			get { return request.Accept; }
			set { request.Accept = value; }
		}

		///<summary>
		///Gets or sets the value of the Referer HTTP header.
		///</summary>
		///
		///<returns>
		///The value of the Referer HTTP header. The default value is null.
		///</returns>
		///
		public string Referer {
			get { return request.Referer; }
			set { request.Referer = value; }
		}

		///<summary>
		///Gets or sets the value of the User-agent HTTP header.
		///</summary>
		///
		///<returns>
		///The value of the User-agent HTTP header. The default value is null.The value for this property is stored in <see cref="T:System.Net.WebHeaderCollection"></see>. If WebHeaderCollection is set, the property value is lost.
		///</returns>
		///
		public string UserAgent {
			get { return request.UserAgent; }
			set { request.UserAgent = value; }
		}

		///<summary>
		///Gets or sets the value of the Expect HTTP header.
		///</summary>
		///
		///<returns>
		///The contents of the Expect HTTP header. The default value is null.The value for this property is stored in <see cref="T:System.Net.WebHeaderCollection"></see>. If WebHeaderCollection is set, the property value is lost.
		///</returns>
		///
		///<exception cref="T:System.ArgumentException">Expect is set to a string that contains "100-continue" as a substring. </exception>
		public string Expect {
			get { return request.Expect; }
			set { request.Expect = value; }
		}

		///<summary>
		///Gets or sets the value of the If-Modified-Since HTTP header.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.DateTime"></see> that contains the contents of the If-Modified-Since HTTP header. The default value is the current date and time.
		///</returns>
		///<PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" /></PermissionSet>
		public DateTime IfModifiedSince {
			get { return request.IfModifiedSince; }
			set { request.IfModifiedSince = value; }
		}
	}
}