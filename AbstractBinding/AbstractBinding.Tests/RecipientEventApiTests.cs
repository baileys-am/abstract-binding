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
        private Mock<IAbstractService> _serviceMock;
        private Mock<ISerializer> _serializerMock;
        private Mock<IRegisteredObject> _regObjectMock;

        [TestInitialize()]
        public void Initialize()
        {
            // Initialize service mock
            _serviceMock = new Mock<IAbstractService>();

            // Initialize serializer  mock
            _serializerMock = new Mock<ISerializer>();
            _serializerMock.Setup(o => o.SerializeObject(It.IsAny<object>())).Returns<object>(obj =>
            {
                return Serializer.Serialize(obj);
            });
            _serializerMock.Setup(o => o.DeserializeObject<Request>(It.IsAny<string>())).Returns<string>((serObj) =>
            {
                return Serializer.Deserialize<Request>(serObj);
            });
            _serializerMock.Setup(o => o.DeserializeObject<SubscribeRequest>(It.IsAny<string>())).Returns<string>((serObj) =>
            {
                return Serializer.Deserialize<SubscribeRequest>(serObj);
            });
            _serializerMock.Setup(o => o.DeserializeObject<UnsubscribeRequest>(It.IsAny<string>())).Returns<string>((serObj) =>
            {
                return Serializer.Deserialize<UnsubscribeRequest>(serObj);
            });
            _serializerMock.Setup(o => o.DeserializeObject<InvokeRequest>(It.IsAny<string>())).Returns<string>((serObj) =>
            {
                return Serializer.Deserialize<InvokeRequest>(serObj);
            });

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
            _serviceMock.Setup(o => o.Callback(It.IsAny<string>())).Callback<string>((resp) =>
            {
                notification = resp;
            });
            var server = new Recipient(_serviceMock.Object, _serializerMock.Object);
            var requestObj = new SubscribeRequest()
            {
                objectId = objectId,
                eventId = "NotifyOnNonDataChanged"
            };

            // Act
            server.Register(objectId, _regObjectMock.Object);
            string response = server.Request(Serializer.Serialize(requestObj));
            _regObjectMock.Raise(o => o.NotifyOnNonDataChanged += null, EventArgs.Empty);

            // Assert
            _serializerMock.Verify();
            _serviceMock.Verify();
            _regObjectMock.Verify();

            var notificationObj = Serializer.Deserialize<EventNotification>(notification);
            Assert.AreEqual(NotificationType.eventInvoked, notificationObj.notificationType);
            Assert.AreEqual(requestObj.objectId, notificationObj.objectId);
            Assert.AreEqual(requestObj.eventId, notificationObj.eventId);
            Assert.IsTrue(Serializer.JsonCompare(EventArgs.Empty, notificationObj.eventArgs));

            var responseObj = Serializer.Deserialize<SubscribeResponse>(response);
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
            _serviceMock.Setup(o => o.Callback(It.IsAny<string>())).Callback<string>((resp) =>
            {
                nullIfPassed = null;
            });
            var server = new Recipient(_serviceMock.Object, _serializerMock.Object);
            var subscribeRequestObj = new SubscribeRequest()
            {
                objectId = objectId,
                eventId = "NotifyOnNonDataChanged"
            };
            var unsubscribeRequest = new UnsubscribeRequest()
            {
                objectId = objectId,
                eventId = "NotifyOnNonDataChanged"
            };

            // Act
            server.Register(objectId, _regObjectMock.Object);
            server.Request(Serializer.Serialize(subscribeRequestObj));
            string response = server.Request(Serializer.Serialize(unsubscribeRequest));
            _regObjectMock.Raise(o => o.NotifyOnNonDataChanged += null, EventArgs.Empty);

            // Assert
            _serializerMock.Verify();
            _serviceMock.Verify();
            _regObjectMock.Verify();

            var responseObj = Serializer.Deserialize<UnsubscribeResponse>(response);
            Assert.AreEqual(ResponseType.unsubscribe, responseObj.responseType);
            Assert.AreEqual(unsubscribeRequest.objectId, responseObj.objectId);
            Assert.AreEqual(unsubscribeRequest.eventId, responseObj.eventId);
        }
    }
}
