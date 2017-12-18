using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AbstractBinding.Messages;
using AbstractBinding.SenderInternals;

namespace AbstractBinding.Tests
{
    [TestClass]
    public class EventProxyTests
    {
        private const string _testCategory = "Event Proxy";

        private readonly Mock<IAbstractClient> _clientMock;
        private readonly Mock<ISerializer> _serializerMock;

        public EventProxyTests()
        {
            // Initialize client mock
            _clientMock = new Mock<IAbstractClient>();

            // Initialize serializer mock
            _serializerMock = new Mock<ISerializer>();
            _serializerMock.Setup(o => o.SerializeObject(It.IsAny<object>())).Returns<object>(obj =>
            {
                return Serializer.Serialize(obj);
            });
            _serializerMock.Setup(o => o.DeserializeObject<Response>(It.IsAny<string>())).Returns<string>((serObj) =>
            {
                return Serializer.Deserialize<Response>(serObj);
            });
            _serializerMock.Setup(o => o.DeserializeObject<ExceptionResponse>(It.IsAny<string>())).Returns<string>((serObj) =>
            {
                return Serializer.Deserialize<ExceptionResponse>(serObj);
            });
            _serializerMock.Setup(o => o.DeserializeObject<SubscribeResponse>(It.IsAny<string>())).Returns<string>((serObj) =>
            {
                return Serializer.Deserialize<SubscribeResponse>(serObj);
            });
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void SubscribeNoArgsTest()
        {
            // Arrange
            string objectId = "objId1";
            var expectedRequest = new SubscribeRequest()
            {
                objectId = objectId,
                eventId = nameof(IRegisteredObject.NotifyOnNonDataChanged)
            };
            SubscribeRequest actualRequest = null;
            _clientMock.Setup(o => o.Request(It.IsAny<string>())).Returns<string>(req =>
            {
                actualRequest = Serializer.Deserialize<SubscribeRequest>(req);

                return Serializer.Serialize(new SubscribeResponse()
                {
                    objectId = objectId,
                    eventId = nameof(IRegisteredObject.NotifyOnNonDataChanged)
                });
            });
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializerMock.Object);
            var objProxy = proxyFactory.Create<IRegisteredObject>(objectId);

            // Act
            objProxy.NotifyOnNonDataChanged += (s, e) => { };

            // Assert
            _clientMock.VerifyAll();
            Assert.AreEqual(expectedRequest.objectId, actualRequest.objectId);
            Assert.AreEqual(expectedRequest.eventId, actualRequest.eventId);
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        [ExpectedException(typeof(NotImplementedException))]
        public void SubscribeNoArgsWithExceptionTest()
        {
            // Arrange
            string objectId = "objId1";
            _clientMock.Setup(o => o.Request(It.IsAny<string>())).Returns<string>(req =>
            {
                throw new NotImplementedException();
            });
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializerMock.Object);
            var objProxy = proxyFactory.Create<IRegisteredObject>(objectId);

            // Act
            objProxy.NotifyOnNonDataChanged += (s, e) => { };

            // Assert
            _clientMock.VerifyAll();
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        [ExpectedException(typeof(RecipientBindingException))]
        public void SubscribeNoArgsWithResponseExceptionTest()
        {
            // Arrange
            string objectId = "objId1";
            _clientMock.Setup(o => o.Request(It.IsAny<string>())).Returns<string>(req =>
            {
                return Serializer.Serialize(new ExceptionResponse()
                {
                    exception = new RecipientBindingException()
                });
            });
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializerMock.Object);
            var objProxy = proxyFactory.Create<IRegisteredObject>(objectId);

            // Act
            objProxy.NotifyOnNonDataChanged += (s, e) => { };

            // Assert
            _clientMock.VerifyAll();
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        [ExpectedException(typeof(RecipientBindingException))]
        public void SubscribeNoArgsWithWrongResponseTypeTest()
        {
            // Arrange
            string objectId = "objId1";
            _clientMock.Setup(o => o.Request(It.IsAny<string>())).Returns<string>(req =>
            {
                return Serializer.Serialize(new UnsubscribeResponse()
                {
                    objectId = objectId,
                    eventId = nameof(IRegisteredObject.NotifyOnNonDataChanged)
                });
            });
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializerMock.Object);
            var objProxy = proxyFactory.Create<IRegisteredObject>(objectId);

            // Act
            objProxy.NotifyOnNonDataChanged += (s, e) => { };

            // Assert
            _clientMock.VerifyAll();
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        [ExpectedException(typeof(RecipientBindingException))]
        public void SubscribeNoArgsWithWrongEventIdTest()
        {
            // Arrange
            string objectId = "objId1";
            _clientMock.Setup(o => o.Request(It.IsAny<string>())).Returns<string>(req =>
            {
                return Serializer.Serialize(new SubscribeResponse()
                {
                    objectId = objectId,
                    eventId = "wrongEventId"
                });
            });
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializerMock.Object);
            var objProxy = proxyFactory.Create<IRegisteredObject>(objectId);

            // Act
            objProxy.NotifyOnNonDataChanged += (s, e) => { };

            // Assert
            _clientMock.VerifyAll();
        }
    }
}
