﻿// <copyright file="ChatRoomTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.ChatServer;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Unit tests for the <see cref="ChatRoom"/>.
/// </summary>
[TestFixture]
public class ChatRoomTests
{
    private const string ChatServerHost = "";

    /// <summary>
    /// Tests if a new room returns the specified room id, which was given to the constructor.
    /// </summary>
    [Test]
    public void RoomId()
    {
        const ushort roomId = 4711;
        var room = new ChatRoom(roomId, new NullLogger<ChatRoom>());
        Assert.That(room.RoomId, Is.EqualTo(roomId));
    }

    /// <summary>
    /// Tries to register a client with a different room id. This should fail with an <see cref="ArgumentException"/>.
    /// </summary>
    [Test]
    public void RegisterClientWithDifferentRoomId()
    {
        const ushort roomId = 4711;
        var room = new ChatRoom(roomId, new NullLogger<ChatRoom>());
        var clientId = room.GetNextClientIndex();
        var authenticationInfo = new ChatServerAuthenticationInfo(clientId, roomId - 1, "Bob", ChatServerHost, "123456789");
        Assert.Throws<ArgumentException>(() => room.RegisterClient(authenticationInfo));
    }

    /// <summary>
    /// Tries to register a client with a correct room id.
    /// </summary>
    [Test]
    public void RegisterClientWithCorrectRoomId()
    {
        const ushort roomId = 4711;
        var room = new ChatRoom(roomId, new NullLogger<ChatRoom>());
        var clientId = room.GetNextClientIndex();
        var authenticationInfo = new ChatServerAuthenticationInfo(clientId, roomId, "Bob", ChatServerHost, "123456789");
        Assert.DoesNotThrow(() => room.RegisterClient(authenticationInfo));
    }

    /// <summary>
    /// Tries to join a null client. This should fail with an <see cref="ArgumentNullException"/>.
    /// </summary>
    [Test]
    public async ValueTask TryJoinNullClient()
    {
        const ushort roomId = 4711;
        var room = new ChatRoom(roomId, new NullLogger<ChatRoom>());
        var clientId = room.GetNextClientIndex();
        var authenticationInfo = new ChatServerAuthenticationInfo(clientId, roomId, "Bob", ChatServerHost, "123456789");
        room.RegisterClient(authenticationInfo);
        Assert.ThrowsAsync<ArgumentNullException>(() => room.TryJoinAsync(null!).AsTask());
    }

    /// <summary>
    /// Tries to join the room with an unauthenticated client. This should fail.
    /// </summary>
    [Test]
    public async ValueTask TryJoinWithUnauthenticatedClient()
    {
        const ushort roomId = 4711;
        var room = new ChatRoom(roomId, new NullLogger<ChatRoom>());
        var clientId = room.GetNextClientIndex();
        var authenticationInfo = new ChatServerAuthenticationInfo(clientId, roomId, "Bob", ChatServerHost, "123456789");
        room.RegisterClient(authenticationInfo);
        var chatClient = new Mock<IChatClient>();
        Assert.That(await room.TryJoinAsync(chatClient.Object), Is.False);
    }

    /// <summary>
    /// Tries to join the room with a client with a wrong authentication token. This should fail.
    /// </summary>
    [Test]
    public async ValueTask TryJoinWithAuthenticatedClientButWrongToken()
    {
        const ushort roomId = 4711;
        var room = new ChatRoom(roomId, new NullLogger<ChatRoom>());
        var clientId = room.GetNextClientIndex();
        var authenticationInfo = new ChatServerAuthenticationInfo(clientId, roomId, "Bob", ChatServerHost, "123456789");
        room.RegisterClient(authenticationInfo);
        var chatClient = new Mock<IChatClient>();
        chatClient.Setup(c => c.AuthenticationToken).Returns("987654321");
        Assert.That(await room.TryJoinAsync(chatClient.Object), Is.False);
    }

    /// <summary>
    /// Tries to join the room with an authenticated client. This should be successful.
    /// </summary>
    [Test]
    public async ValueTask TryJoinWithAuthenticatedClient()
    {
        const ushort roomId = 4711;
        var room = new ChatRoom(roomId, new NullLogger<ChatRoom>());
        var clientId = room.GetNextClientIndex();
        var authenticationInfo = new ChatServerAuthenticationInfo(clientId, roomId, "Bob", ChatServerHost, "123456789");
        room.RegisterClient(authenticationInfo);
        var chatClient = new Mock<IChatClient>();
        chatClient.Setup(c => c.AuthenticationToken).Returns(authenticationInfo.AuthenticationToken);
        Assert.That(await room.TryJoinAsync(chatClient.Object), Is.True);
    }

    /// <summary>
    /// Tests if <see cref="IChatClient.SendChatRoomClientListAsync"/> is called after a client successfully joined a room.
    /// </summary>
    [Test]
    public async ValueTask ChatRoomClientListSent()
    {
        const ushort roomId = 4711;
        var room = new ChatRoom(roomId, new NullLogger<ChatRoom>());
        var clientId = room.GetNextClientIndex();
        var authenticationInfo = new ChatServerAuthenticationInfo(clientId, roomId, "Bob", ChatServerHost, "123456789");
        room.RegisterClient(authenticationInfo);
        var chatClient = new Mock<IChatClient>();
        chatClient.Setup(c => c.AuthenticationToken).Returns(authenticationInfo.AuthenticationToken);
        await room.TryJoinAsync(chatClient.Object);
        chatClient.Verify(c => c.SendChatRoomClientListAsync(room.ConnectedClients), Times.Once);
    }

    /// <summary>
    /// Tests if <see cref="ChatRoom.ConnectedClients"/> returns the successfully authenticated and joined client.
    /// </summary>
    [Test]
    public async ValueTask ConnectedClients()
    {
        const ushort roomId = 4711;
        var room = new ChatRoom(roomId, new NullLogger<ChatRoom>());
        var clientId = room.GetNextClientIndex();
        var authenticationInfo = new ChatServerAuthenticationInfo(clientId, roomId, "Bob", ChatServerHost, "123456789");
        room.RegisterClient(authenticationInfo);
        var chatClient = new Mock<IChatClient>();
        chatClient.Setup(c => c.AuthenticationToken).Returns(authenticationInfo.AuthenticationToken);
        await room.TryJoinAsync(chatClient.Object);
        Assert.That(room.ConnectedClients, Has.Count.EqualTo(1));
        Assert.That(room.ConnectedClients, Contains.Item(chatClient.Object));
    }

    /// <summary>
    /// Tests if <see cref="IChatClient.SendChatRoomClientUpdateAsync"/> is called as soon as another client joined the room.
    /// </summary>
    [Test]
    public async ValueTask SendJoinedMessage()
    {
        const ushort roomId = 4711;
        var room = new ChatRoom(roomId, new NullLogger<ChatRoom>());
        var clientId0 = room.GetNextClientIndex();
        var clientId1 = room.GetNextClientIndex();
        var authenticationInfo0 = new ChatServerAuthenticationInfo(clientId0, roomId, "Alice", ChatServerHost, "99999");
        var authenticationInfo1 = new ChatServerAuthenticationInfo(clientId1, roomId, "Bob", ChatServerHost, "123456789");
        room.RegisterClient(authenticationInfo0);
        room.RegisterClient(authenticationInfo1);

        var chatClient0 = new Mock<IChatClient>();
        chatClient0.SetupAllProperties();
        chatClient0.Setup(c => c.AuthenticationToken).Returns(authenticationInfo0.AuthenticationToken);
        var chatClient1 = new Mock<IChatClient>();
        chatClient1.SetupAllProperties();
        chatClient1.Setup(c => c.AuthenticationToken).Returns(authenticationInfo1.AuthenticationToken);

        await room.TryJoinAsync(chatClient0.Object);
        await room.TryJoinAsync(chatClient1.Object);

        chatClient0.Verify(c => c.SendChatRoomClientUpdateAsync(clientId1, authenticationInfo1.ClientName, ChatRoomClientUpdateType.Joined), Times.Once);
    }

    /// <summary>
    /// Tests if <see cref="IChatClient.SendChatRoomClientUpdateAsync"/> is called as soon as another client left the room.
    /// </summary>
    [Test]
    public async ValueTask SendLeftMessage()
    {
        const ushort roomId = 4711;
        var room = new ChatRoom(roomId, new NullLogger<ChatRoom>());
        var clientId0 = room.GetNextClientIndex();
        var clientId1 = room.GetNextClientIndex();
        var authenticationInfo0 = new ChatServerAuthenticationInfo(clientId0, roomId, "Alice", ChatServerHost, "99999");
        var authenticationInfo1 = new ChatServerAuthenticationInfo(clientId1, roomId, "Bob", ChatServerHost, "123456789");
        room.RegisterClient(authenticationInfo0);
        room.RegisterClient(authenticationInfo1);

        var chatClient0 = new Mock<IChatClient>();
        chatClient0.SetupAllProperties();
        chatClient0.Setup(c => c.AuthenticationToken).Returns(authenticationInfo0.AuthenticationToken);

        var chatClient1 = new Mock<IChatClient>();
        chatClient1.SetupAllProperties();
        chatClient1.Setup(c => c.AuthenticationToken).Returns(authenticationInfo1.AuthenticationToken);

        await room.TryJoinAsync(chatClient0.Object);
        await room.TryJoinAsync(chatClient1.Object);

        await room.LeaveAsync(chatClient0.Object);
        chatClient1.Verify(c => c.SendChatRoomClientUpdateAsync(clientId0, authenticationInfo0.ClientName, ChatRoomClientUpdateType.Left), Times.Once);
    }

    /// <summary>
    /// Tests if <see cref="IChatClient.SendMessageAsync"/> is called as soon as a message is sent through the room.
    /// </summary>
    [Test]
    public async ValueTask SendMessage()
    {
        const ushort roomId = 4711;
        const string chatMessage = "foobar1234567890";
        var room = new ChatRoom(roomId, new NullLogger<ChatRoom>());
        var clientId0 = room.GetNextClientIndex();
        var clientId1 = room.GetNextClientIndex();
        var authenticationInfo0 = new ChatServerAuthenticationInfo(clientId0, roomId, "Alice", ChatServerHost, "99999");
        var authenticationInfo1 = new ChatServerAuthenticationInfo(clientId1, roomId, "Bob", ChatServerHost, "123456789");
        room.RegisterClient(authenticationInfo0);
        room.RegisterClient(authenticationInfo1);

        var chatClient0 = new Mock<IChatClient>();
        chatClient0.Setup(c => c.AuthenticationToken).Returns(authenticationInfo0.AuthenticationToken);

        var chatClient1 = new Mock<IChatClient>();
        chatClient1.Setup(c => c.AuthenticationToken).Returns(authenticationInfo1.AuthenticationToken);

        await room.TryJoinAsync(chatClient0.Object);
        await room.TryJoinAsync(chatClient1.Object);

        await room.SendMessageAsync(clientId1, chatMessage);
        chatClient0.Verify(c => c.SendMessageAsync(clientId1, chatMessage), Times.Once);
        chatClient1.Verify(c => c.SendMessageAsync(clientId1, chatMessage), Times.Once);
    }

    /// <summary>
    /// Tests if <see cref="ChatRoom.RoomClosed"/> is fired as soon as all connected clients left the room.
    /// </summary>
    [Test]
    public async ValueTask RoomClosedEvent()
    {
        const ushort roomId = 4711;
        var room = new ChatRoom(roomId, new NullLogger<ChatRoom>());
        var clientId0 = room.GetNextClientIndex();
        var clientId1 = room.GetNextClientIndex();
        var authenticationInfo0 = new ChatServerAuthenticationInfo(clientId0, roomId, "Alice", ChatServerHost, "99999");
        var authenticationInfo1 = new ChatServerAuthenticationInfo(clientId1, roomId, "Bob", ChatServerHost, "123456789");
        room.RegisterClient(authenticationInfo0);
        room.RegisterClient(authenticationInfo1);

        var chatClient0 = new Mock<IChatClient>();
        chatClient0.Setup(c => c.AuthenticationToken).Returns(authenticationInfo0.AuthenticationToken);

        var chatClient1 = new Mock<IChatClient>();
        chatClient1.Setup(c => c.AuthenticationToken).Returns(authenticationInfo1.AuthenticationToken);

        var eventCalled = false;
        room.RoomClosed += (sender, e) => eventCalled = true;
        await room.TryJoinAsync(chatClient0.Object);
        await room.TryJoinAsync(chatClient1.Object);
        await room.LeaveAsync(chatClient0.Object);
        await room.LeaveAsync(chatClient1.Object);
        Assert.That(eventCalled, Is.True);
    }
}