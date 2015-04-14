using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Web
{
    public class ConnectionHub : Hub
    {
        public void Update(List<string> contentList,
            List<long> intervalList,
            List<List<string>> currentFunctionList,
            List<List<string>> jsPathList,
            List<List<string>> cssPathList)
        {
            Clients.All.updatesequence(contentList, intervalList, currentFunctionList, jsPathList, cssPathList);
        }
    }
}