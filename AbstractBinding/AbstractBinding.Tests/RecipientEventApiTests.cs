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
        private readonly ISerializer _serializer = new Serializer();

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
            string notification = null;
            _callbackMock.Setup(o => o.Callback(It.IsAny<string>())).Callback<string>((resp) =>
            {
                notification = resp;
            });
            var server = new Recipient(_serializer);
            var requestObj = new SubscribeRequest()
            {
                objectId = objectId,
                eventId = nameof(IRegisteredObject.NotifyOnNonDataChanged)
            };

            // Act
            server.Register(objectId, _regObjectMock.Object);
            string response = server.Request(_serializer.SerializeObject(requestObj), _callbackMock.Object);
            _regObjectMock.Raise(o => o.NotifyOnNonDataChanged += null, EventArgs.Empty);

            // Assert
            _callbackMock.Verify();
            _regObjectMock.Verify();

            var notificationObj = _serializer.DeserializeObject<EventNotification>(notification);
            Assert.AreEqual(NotificationType.eventInvoked, notificationObj.notificationType);
            Assert.AreEqual(requestObj.objectId, notificationObj.objectId);
            Assert.AreEqual(requestObj.eventId, notificationObj.eventId);
            Assert.IsTrue(Serializer.JsonCompare(EventArgs.Empty, notificationObj.eventArgs));

            var responseObj = _serializer.DeserializeObject<SubscribeResponse>(response);
            Assert.AreEqual(ResponseType.subscribe, responseObj.responseType);
            Assert.AreEqual(requestObj.objectId, responseObj.objectId);
            Assert.AreEqual(requestObj.eventId, responseObj.eventId);
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void SubscribeWithArgsTest()
        {
            // Arrange
            var objectId = "objId1";
            string notification = null;
            _callbackMock.Setup(o => o.Callback(It.IsAny<string>())).Callback<string>((resp) =>
            {
                notification = resp;
            });
            var server = new Recipient(_serializer);
            var requestObj = new SubscribeRequest()
            {
                objectId = objectId,
                eventId = nameof(IRegisteredObject.NotifyOnDataChanged)
            };
            var expectedEventArgs = new DataChangedEventArgs("eventName", 2.0);

            // Act
            server.Register(objectId, _regObjectMock.Object);
            string response = server.Request(_serializer.SerializeObject(requestObj), _callbackMock.Object);
            _regObjectMock.Raise(o => o.NotifyOnDataChanged += null, expectedEventArgs);

            // Assert
            _callbackMock.Verify();
            _regObjectMock.Verify();

            var notificationObj = _serializer.DeserializeObject<EventNotification>(notification);
            Assert.AreEqual(NotificationType.eventInvoked, notificationObj.notificationType);
            Assert.AreEqual(requestObj.objectId, notificationObj.objectId);
            Assert.AreEqual(requestObj.eventId, notificationObj.eventId);
            Assert.IsTrue(Serializer.JsonCompare(expectedEventArgs, notificationObj.eventArgs));

            var responseObj = _serializer.DeserializeObject<SubscribeResponse>(response);
            Assert.AreEqual(ResponseType.subscribe, responseObj.responseType);
            Assert.AreEqual(requestObj.objectId, responseObj.objectId);
            Assert.AreEqual(requestObj.eventId, responseObj.eventId);
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void UnsubscribeTest()
        {
            // Arrange
            var objectId = "objId1";
            string nullIfPassed = String.Empty;
            _callbackMock.Setup(o => o.Callback(It.IsAny<string>())).Callback<string>((resp) =>
            {
                nullIfPassed = null;
            });
            var server = new Recipient(_serializer);
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
            server.Request(_serializer.SerializeObject(subscribeRequestObj), _callbackMock.Object);
            string response = server.Request(_serializer.SerializeObject(unsubscribeRequest), _callbackMock.Object);
            _regObjectMock.Raise(o => o.NotifyOnNonDataChanged += null, EventArgs.Empty);

            // Assert
            _callbackMock.Verify();
            _regObjectMock.Verify();

            var responseObj = _serializer.DeserializeObject<UnsubscribeResponse>(response);
            Assert.AreEqual(ResponseType.unsubscribe, responseObj.responseType);
            Assert.AreEqual(unsubscribeRequest.objectId, responseObj.objectId);
            Assert.AreEqual(unsubscribeRequest.eventId, responseObj.eventId);
        }
    }
}
