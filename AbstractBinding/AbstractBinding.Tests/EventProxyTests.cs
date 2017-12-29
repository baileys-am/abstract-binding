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

        private readonly Mock<ISenderClient> _clientMock;
        private readonly ISerializer _serializer = new Serializer();

        public EventProxyTests()
        {
            // Initialize client mock
            _clientMock = new Mock<ISenderClient>();
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
                actualRequest = _serializer.DeserializeObject<SubscribeRequest>(req);

                return _serializer.SerializeObject(new SubscribeResponse()
                {
                    objectId = objectId,
                    eventId = nameof(IRegisteredObject.NotifyOnNonDataChanged)
                });
            });
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializer);
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
        public void SubscribeWithArgsTest()
        {
            // Arrange
            string objectId = "objId1";
            var expectedRequest = new SubscribeRequest()
            {
                objectId = objectId,
                eventId = nameof(IRegisteredObject.NotifyOnDataChanged)
            };
            SubscribeRequest actualRequest = null;
            _clientMock.Setup(o => o.Request(It.IsAny<string>())).Returns<string>(req =>
            {
                actualRequest = _serializer.DeserializeObject<SubscribeRequest>(req);

                return _serializer.SerializeObject(new SubscribeResponse()
                {
                    objectId = objectId,
                    eventId = nameof(IRegisteredObject.NotifyOnDataChanged)
                });
            });
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializer);
            var objProxy = proxyFactory.Create<IRegisteredObject>(objectId);

            // Act
            objProxy.NotifyOnDataChanged += (s, e) => { };

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
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializer);
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
                return _serializer.SerializeObject(new ExceptionResponse()
                {
                    exception = new RecipientBindingException()
                });
            });
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializer);
            var objProxy = proxyFactory.Create<IRegisteredObject>(objectId);

            // Act
            objProxy.NotifyOnNonDataChanged += (s, e) => { };

            // Assert
            _clientMock.VerifyAll();
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        [ExpectedException(typeof(InvalidResponseException))]
        public void SubscribeNoArgsWithWrongResponseTypeTest()
        {
            // Arrange
            string objectId = "objId1";
            _clientMock.Setup(o => o.Request(It.IsAny<string>())).Returns<string>(req =>
            {
                return _serializer.SerializeObject(new UnsubscribeResponse()
                {
                    objectId = objectId,
                    eventId = nameof(IRegisteredObject.NotifyOnNonDataChanged)
                });
            });
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializer);
            var objProxy = proxyFactory.Create<IRegisteredObject>(objectId);

            // Act
            objProxy.NotifyOnNonDataChanged += (s, e) => { };

            // Assert
            _clientMock.VerifyAll();
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        [ExpectedException(typeof(InvalidResponseException))]
        public void SubscribeNoArgsWithWrongEventIdTest()
        {
            // Arrange
            string objectId = "objId1";
            _clientMock.Setup(o => o.Request(It.IsAny<string>())).Returns<string>(req =>
            {
                return _serializer.SerializeObject(new SubscribeResponse()
                {
                    objectId = objectId,
                    eventId = "wrongEventId"
                });
            });
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializer);
            var objProxy = proxyFactory.Create<IRegisteredObject>(objectId);

            // Act
            objProxy.NotifyOnNonDataChanged += (s, e) => { };

            // Assert
            _clientMock.VerifyAll();
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void UnsubscribeNoArgsTest()
        {
            // Arrange
            string objectId = "objId1";
            var expectedRequest = new UnsubscribeRequest()
            {
                objectId = objectId,
                eventId = nameof(IRegisteredObject.NotifyOnNonDataChanged)
            };
            UnsubscribeRequest actualRequest = null;
            _clientMock.Setup(o => o.Request(It.IsAny<string>())).Returns<string>(req =>
            {
                actualRequest = _serializer.DeserializeObject<UnsubscribeRequest>(req);

                return _serializer.SerializeObject(new UnsubscribeResponse()
                {
                    objectId = objectId,
                    eventId = nameof(IRegisteredObject.NotifyOnNonDataChanged)
                });
            });
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializer);
            var objProxy = proxyFactory.Create<IRegisteredObject>(objectId);

            // Act
            objProxy.NotifyOnNonDataChanged -= (s, e) => { };

            // Assert
            _clientMock.VerifyAll();
            Assert.AreEqual(expectedRequest.objectId, actualRequest.objectId);
            Assert.AreEqual(expectedRequest.eventId, actualRequest.eventId);
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void UnsubscribeWithArgsTest()
        {
            // Arrange
            string objectId = "objId1";
            var expectedRequest = new UnsubscribeRequest()
            {
                objectId = objectId,
                eventId = nameof(IRegisteredObject.NotifyOnDataChanged)
            };
            UnsubscribeRequest actualRequest = null;
            _clientMock.Setup(o => o.Request(It.IsAny<string>())).Returns<string>(req =>
            {
                actualRequest = _serializer.DeserializeObject<UnsubscribeRequest>(req);

                return _serializer.SerializeObject(new UnsubscribeResponse()
                {
                    objectId = objectId,
                    eventId = nameof(IRegisteredObject.NotifyOnDataChanged)
                });
            });
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializer);
            var objProxy = proxyFactory.Create<IRegisteredObject>(objectId);

            // Act
            objProxy.NotifyOnDataChanged -= (s, e) => { };

            // Assert
            _clientMock.VerifyAll();
            Assert.AreEqual(expectedRequest.objectId, actualRequest.objectId);
            Assert.AreEqual(expectedRequest.eventId, actualRequest.eventId);
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        [ExpectedException(typeof(NotImplementedException))]
        public void UnsubscribeNoArgsWithExceptionTest()
        {
            // Arrange
            string objectId = "objId1";
            _clientMock.Setup(o => o.Request(It.IsAny<string>())).Returns<string>(req =>
            {
                throw new NotImplementedException();
            });
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializer);
            var objProxy = proxyFactory.Create<IRegisteredObject>(objectId);

            // Act
            objProxy.NotifyOnNonDataChanged -= (s, e) => { };

            // Assert
            _clientMock.VerifyAll();
        }

        /////

        [TestMethod]
        [TestCategory(_testCategory)]
        [ExpectedException(typeof(RecipientBindingException))]
        public void UnsubscribeNoArgsWithResponseExceptionTest()
        {
            // Arrange
            string objectId = "objId1";
            _clientMock.Setup(o => o.Request(It.IsAny<string>())).Returns<string>(req =>
            {
                return _serializer.SerializeObject(new ExceptionResponse()
                {
                    exception = new RecipientBindingException()
                });
            });
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializer);
            var objProxy = proxyFactory.Create<IRegisteredObject>(objectId);

            // Act
            objProxy.NotifyOnNonDataChanged -= (s, e) => { };

            // Assert
            _clientMock.VerifyAll();
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        [ExpectedException(typeof(InvalidResponseException))]
        public void UnsubscribeNoArgsWithWrongResponseTypeTest()
        {
            // Arrange
            string objectId = "objId1";
            _clientMock.Setup(o => o.Request(It.IsAny<string>())).Returns<string>(req =>
            {
                return _serializer.SerializeObject(new SubscribeResponse()
                {
                    objectId = objectId,
                    eventId = nameof(IRegisteredObject.NotifyOnNonDataChanged)
                });
            });
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializer);
            var objProxy = proxyFactory.Create<IRegisteredObject>(objectId);

            // Act
            objProxy.NotifyOnNonDataChanged -= (s, e) => { };

            // Assert
            _clientMock.VerifyAll();
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        [ExpectedException(typeof(InvalidResponseException))]
        public void UnsubscribeNoArgsWithWrongEventIdTest()
        {
            // Arrange
            string objectId = "objId1";
            _clientMock.Setup(o => o.Request(It.IsAny<string>())).Returns<string>(req =>
            {
                return _serializer.SerializeObject(new UnsubscribeResponse()
                {
                    objectId = objectId,
                    eventId = "wrongEventId"
                });
            });
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializer);
            var objProxy = proxyFactory.Create<IRegisteredObject>(objectId);

            // Act
            objProxy.NotifyOnNonDataChanged -= (s, e) => { };

            // Assert
            _clientMock.VerifyAll();
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void EventNotificationNoArgsTest()
        {
            // Arrange
            string objectId = "objId1";
            var objectDescriptionFactory = new ObjectDescriptionFactory();
            _clientMock.Setup(o => o.Request(It.IsAny<string>())).Returns<string>((req) =>
            {
                var resp = new GetBindingDescriptionsResponse();
                resp.bindings.Add(objectId, objectDescriptionFactory.Create<IRegisteredObject>());
                return _serializer.SerializeObject(resp);
            });
            var sender = new Sender(_clientMock.Object, _serializer);
            sender.Register<IRegisteredObject>();
            sender.SynchronizeBindings();
            _clientMock.Setup(o => o.Request(It.IsAny<string>())).Returns<string>(req =>
            {
                return _serializer.SerializeObject(new SubscribeResponse()
                {
                    objectId = objectId,
                    eventId = nameof(IRegisteredObject.NotifyOnNonDataChanged)
                });
            });
            var objProxy = sender.GetBindingsByType<IRegisteredObject>()[objectId];
            bool argsReceived = false;
            objProxy.NotifyOnNonDataChanged += (s, e) => { argsReceived = e as EventArgs != null; };
            var notification = new EventNotification()
            {
                objectId = objectId,
                eventId = nameof(IRegisteredObject.NotifyOnNonDataChanged),
                eventArgs = EventArgs.Empty
            };

            // Act
            _clientMock.Raise(o => o.NotificationReceived += null, new NotificationEventArgs(_serializer.SerializeObject(notification)));

            // Assert
            _clientMock.Verify();
            Assert.IsTrue(argsReceived);
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void EventNotificationWithArgsTest()
        {
            // Arrange
            string objectId = "objId1";
            var objectDescriptionFactory = new ObjectDescriptionFactory();
            _clientMock.Setup(o => o.Request(It.IsAny<string>())).Returns<string>((req) =>
            {
                var resp = new GetBindingDescriptionsResponse();
                resp.bindings.Add(objectId, objectDescriptionFactory.Create<IRegisteredObject>());
                return _serializer.SerializeObject(resp);
            });
            var sender = new Sender(_clientMock.Object, _serializer);
            sender.Register<IRegisteredObject>();
            sender.SynchronizeBindings();
            _clientMock.Setup(o => o.Request(It.IsAny<string>())).Returns<string>(req =>
            {
                return _serializer.SerializeObject(new SubscribeResponse()
                {
                    objectId = objectId,
                    eventId = nameof(IRegisteredObject.NotifyOnDataChanged)
                });
            });
            var objProxy = sender.GetBindingsByType<IRegisteredObject>()[objectId];
            bool argsReceived = false;
            objProxy.NotifyOnDataChanged += (s, e) => { argsReceived = e as DataChangedEventArgs != null; };
            var notification = new EventNotification()
            {
                objectId = objectId,
                eventId = nameof(IRegisteredObject.NotifyOnDataChanged),
                eventArgs = new DataChangedEventArgs("dataChanged", 2)
            };

            // Act
            _clientMock.Raise(o => o.NotificationReceived += null, new NotificationEventArgs(_serializer.SerializeObject(notification)));

            // Assert
            _clientMock.Verify();
            Assert.IsTrue(argsReceived);
        }
    }
}
