using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Web
{
    public class ConnectionHub : Hub
    {
        public void Update()
        {
            Clients.All.updatesequence("[[function(){alert(\"hello\")}],[[function(){alert(\"hello\")}]]");
        }
    }
}