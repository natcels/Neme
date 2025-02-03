using cleddmessenger.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;



using WebSocketSharp;
using WebSocketSharp.Server;

namespace cleddmessenger.Tests
{
    [TestClass]
    public class SignalingServerTests
    {
        private const int TestPort = 8081;
        private const string TestAddress = "ws://127.0.0.1:8081/signaling";
        private WebSocketServer server;

        [TestInitialize]
        public void Setup()
        {
            // Initialize and start the signaling server
            server = new WebSocketServer(TestPort);
            server.AddWebSocketService<SignalingServer>("/signaling");
            server.Start();
            Assert.IsTrue(server.IsListening, "Server failed to start.");
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Stop the signaling server
            server.Stop();
        }

        [TestMethod]
        public async Task ClientCanConnectAndSendMessages()
        {
            using (var client = new ClientWebSocket())
            {
                await client.ConnectAsync(new Uri(TestAddress), CancellationToken.None);
                Assert.AreEqual(WebSocketState.Open, client.State, "Client failed to connect.");

                string testMessage = "Hello, WebSocket!";
                var messageBuffer = System.Text.Encoding.UTF8.GetBytes(testMessage);

                // Send message
                await client.SendAsync(new ArraySegment<byte>(messageBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                // Receive broadcasted message
                var receiveBuffer = new byte[1024];
                var result = await client.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);

                string receivedMessage = System.Text.Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
                Assert.AreEqual(testMessage, receivedMessage, "Message mismatch.");
            }
        }

        [TestMethod]
        public async Task MultipleClientsCanCommunicate()
        {
            using (var client1 = new ClientWebSocket())
            using (var client2 = new ClientWebSocket())
            {
                // Connect both clients
                await client1.ConnectAsync(new Uri(TestAddress), CancellationToken.None);
                await client2.ConnectAsync(new Uri(TestAddress), CancellationToken.None);

                Assert.AreEqual(WebSocketState.Open, client1.State, "Client 1 failed to connect.");
                Assert.AreEqual(WebSocketState.Open, client2.State, "Client 2 failed to connect.");

                string client1Message = "Hello from Client 1!";
                string client2Message = "Hello from Client 2!";

                var client1Buffer = System.Text.Encoding.UTF8.GetBytes(client1Message);
                var client2Buffer = System.Text.Encoding.UTF8.GetBytes(client2Message);

                // Send messages
                await client1.SendAsync(new ArraySegment<byte>(client1Buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                await client2.SendAsync(new ArraySegment<byte>(client2Buffer), WebSocketMessageType.Text, true, CancellationToken.None);

                // Receive messages
                var receiveBuffer = new byte[1024];
                var result1 = await client1.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                var result2 = await client2.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);

                string receivedByClient1 = System.Text.Encoding.UTF8.GetString(receiveBuffer, 0, result1.Count);
                string receivedByClient2 = System.Text.Encoding.UTF8.GetString(receiveBuffer, 0, result2.Count);

                CollectionAssert.Contains(new[] { client1Message, client2Message }, receivedByClient1, "Client 1 did not receive the correct message.");
                CollectionAssert.Contains(new[] { client1Message, client2Message }, receivedByClient2, "Client 2 did not receive the correct message.");
            }
        }

        [TestMethod]
        public void ServerLogsStartAndStopEvents()
        {
            // Validate that server logs events
            Assert.IsTrue(SignalingServer.IsServerRunning(), "Server should be running.");

            SignalingServer.StopServer();
            Assert.IsFalse(SignalingServer.IsServerRunning(), "Server should not be running.");
        }
    }
}
