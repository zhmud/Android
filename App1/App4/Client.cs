using System;
using System.Net.Sockets;

using Android.App;
using Android.OS;
using Android.Widget;
using Microsoft.AspNet.SignalR.Client;

namespace App4
{
    class Client
    {
        private readonly string ip;
        private readonly int port;
        private HubConnection hubConnection;
        private IHubProxy hubProxy;
        private string access_Token = string.Empty;

        public Client(string ip = "127.0.0.1", int port = 9595)
        {
            this.ip = ip;
            this.port = port;
            hubConnection = new HubConnection(@"http://" + ip + ":" + port + "/Server");
            hubProxy = hubConnection.CreateHubProxy("connectionHub");
            hubConnection.Start().Wait();
        }

        public void Authorization(string login, string password)
        {
            access_Token = hubProxy.Invoke<string>("AuthorizationAsync", login, password).Result;
        }

        public void Send(string d_id, double lon, double lat, int accurecy, int battery, DateTime dt, bool sos)
        {
           var str = hubProxy.Invoke<string>("SetDeviceInfoAsync", access_Token, d_id, lon, lat, accurecy, battery, dt, sos).Result;
        }
    }
}