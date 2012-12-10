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
using HttpWebAdapters.Adapters;

namespace HttpWebAdapters {

    /// <summary>
    /// Defaut HTTP web request factory; creates instances of .NET framework classes.
    /// </summary>

	public class HttpWebRequestFactory : IHttpWebRequestFactory {

        /// <summary>
        /// Creates an HttpWebRequestAdapter wrapping a .NET framework HttpWebRequest object.
        /// </summary>
        ///
        /// <param name="url">
        /// URL of the document.
        /// </param>
        ///
        /// <returns>
        /// A new HttpWebRequestAdapter
        /// </returns>

		public IHttpWebRequest Create(string url) {
			return new HttpWebRequestAdapter((HttpWebRequest) WebRequest.Create(url));
		}

        /// <summary>
        /// Creates an HttpWebRequestAdapter wrapping a .NET framework HttpWebRequest object.
        /// </summary>
        ///
        /// <param name="url">
        /// URL of the document.
        /// </param>
        ///
        /// <returns>
        /// A new HttpWebRequestAdapter
        /// </returns>

		public IHttpWebRequest Create(Uri url) {
			return new HttpWebRequestAdapter((HttpWebRequest) WebRequest.Create(url));
		}
	}
}