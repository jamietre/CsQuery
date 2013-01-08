using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Tests.Mocks
{
    public class MockAsyncResult: IAsyncResult
    {
        public object AsyncState
        {
            get;
            set;
        }

        public System.Threading.WaitHandle AsyncWaitHandle
        {
            get;
            set;
        }

        public bool CompletedSynchronously
        {
            get;
            set;
        }

        public bool IsCompleted
        {
            get;
            set;
        }
    }
}
