using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AbstractBinding.Messages;

namespace AbstractBinding.Tests
{
    [TestClass]
    public class RecipientEventApiTests
    {
        private const string _testCategory = "Recipient Event API";
        private readonly Mock<IRecipientCallback> _callbackMock;
        private readonly Mock<IRegisteredObject> _regObjectMock;

        public RecipientEventApiTests()
        {
            // Initialize service mock
            _callbackMock = new Mock<IRecipientCallback>();

            // Initialize registered object mock
            _regObjectMock = new Mock<IRegisteredObject>();
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void SubscribeTest()
        {
            // Arrange
            var objectId = "objId1";
            EventNotification actualNotif = null;
            _callbackMock.Setup(o => o.Callback(It.IsAny<INotification>())).Callback<INotification>((notif) =>
            {
                actualNotif = notif as EventNotification;
            });
            var server = new Recipient();
            var request = new SubscribeRequest()
            {
                objectId = objectId,
                eventId = nameof(IRegisteredObject.NotifyOnNonDataChanged)
            };

            // Act
            server.Register(objectId, _regObjectMock.Object);
            var response = server.Request(request, _callbackMock.Object) as SubscribeResponse;
            _regObjectMock.Raise(o => o.NotifyOnNonDataChanged += null, EventArgs.Empty);

            // Assert
            _callbackMock.Verify();
            _regObjectMock.Verify();
            
            Assert.AreEqual(NotificationType.eventInvoked, actualNotif.notificationType);
            Assert.AreEqual(request.objectId, actualNotif.objectId);
            Assert.AreEqual(request.eventId, actualNotif.eventId);
            Assert.IsNotNull(actualNotif.eventArgs as EventArgs);
            
            Assert.AreEqual(ResponseType.subscribe, response.responseType);
            Assert.AreEqual(request.objectId, response.objectId);
            Assert.AreEqual(request.eventId, response.eventId);
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void SubscribeWithArgsTest()
        {
            // Arrange
            var objectId = "objId1";
            EventNotification notification = null;
            _callbackMock.Setup(o => o.Callback(It.IsAny<INotification>())).Callback<INotification>((resp) =>
            {
                notification = resp as EventNotification;
            });
            var server = new Recipient();
            var request = new SubscribeRequest()
            {
                objectId = objectId,
                eventId = nameof(IRegisteredObject.NotifyOnDataChanged)
            };
            var expectedEventArgs = new DataChangedEventArgs("eventName", 2.0);

            // Act
            server.Register(objectId, _regObjectMock.Object);
            var response = server.Request(request, _callbackMock.Object) as SubscribeResponse;
            _regObjectMock.Raise(o => o.NotifyOnDataChanged += null, expectedEventArgs);

            // Assert
            _callbackMock.Verify();
            _regObjectMock.Verify();
            
            Assert.AreEqual(NotificationType.eventInvoked, notification.notificationType);
            Assert.AreEqual(request.objectId, notification.objectId);
            Assert.AreEqual(request.eventId, notification.eventId);
            Assert.IsNotNull(notification.eventArgs as DataChangedEventArgs);
            Assert.AreEqual("eventName", (notification.eventArgs as DataChangedEventArgs).Name);
            Assert.AreEqual(2.0, (notification.eventArgs as DataChangedEventArgs).Data);

            Assert.AreEqual(ResponseType.subscribe, response.responseType);
            Assert.AreEqual(request.objectId, response.objectId);
            Assert.AreEqual(request.eventId, response.eventId);
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void UnsubscribeTest()
        {
            // Arrange
            var objectId = "objId1";
            string nullIfPassed = String.Empty;
            _callbackMock.Setup(o => o.Callback(It.IsAny<INotification>())).Callback<INotification>((resp) =>
            {
                nullIfPassed = null;
            });
            var server = new Recipient();
            var subscribeRequestObj = new SubscribeRequest()
            {
                objectId = objectId,
                eventId = nameof(IRegisteredObject.NotifyOnNonDataChanged)
            };
            var unsubscribeRequest = new UnsubscribeRequest()
            {
                objectId = objectId,
                eventId = nameof(IRegisteredObject.NotifyOnNonDataChanged)
            };

            // Act
            server.Register(objectId, _regObjectMock.Object);
            server.Request(subscribeRequestObj, _callbackMock.Object);
            var response = server.Request(unsubscribeRequest, _callbackMock.Object) as UnsubscribeResponse;
            _regObjectMock.Raise(o => o.NotifyOnNonDataChanged += null, EventArgs.Empty);

            // Assert
            _callbackMock.Verify();
            _regObjectMock.Verify();
            
            Assert.AreEqual(ResponseType.unsubscribe, response.responseType);
            Assert.AreEqual(unsubscribeRequest.objectId, response.objectId);
            Assert.AreEqual(unsubscribeRequest.eventId, response.eventId);
        }
    }
}
