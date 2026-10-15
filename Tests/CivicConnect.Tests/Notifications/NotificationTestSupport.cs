using CivicConnect.Common;
using System;
using System.Collections.Generic;

namespace CivicConnect.Tests.Notifications;

internal sealed class RecordingNotificationService : INotificationService
{
    public List<NotificationMessage> Messages { get; } = new();
    public void Notify(NotificationMessage message) => Messages.Add(message);
}

internal sealed class ThrowingNotificationService : INotificationService
{
    public void Notify(NotificationMessage message) => throw new InvalidOperationException("popup failed");
}
