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

namespace HttpWebAdapters {
	public class HttpWebRequestMethod {
		private string method;
		private static readonly string SGET = "GET";
		private static readonly string SPOST = "POST";
		public static readonly HttpWebRequestMethod GET = new HttpWebRequestMethod(SGET);
		public static readonly HttpWebRequestMethod POST = new HttpWebRequestMethod(SPOST);

		private HttpWebRequestMethod(string m) {
			method = m;
		}

		public override string ToString() {
			return method;
		}

		public static HttpWebRequestMethod Parse(string s) {
			if (s == SGET)
				return GET;
			return POST;
		}
	}
}