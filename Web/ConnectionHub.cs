using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.Model;
using Microsoft.AspNet.SignalR;

namespace Web
{
    public class ConnectionHub : Hub
    {
        public void Update(SequenceSetup setup)
        {
            Clients.All.updatesequence(setup);
        }

        public void UpdateGroup(SequenceSetup setup, string groupId)
        {
            Clients.Group(groupId).updatesequence(setup);
        }
        public void JoinRoom(string roomName)
        {
            Groups.Add(Context.ConnectionId, roomName);
        }
    }
}