﻿using System;
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
            _serializerMock.Setup(o => o.DeserializeObject<UnsubscribeResponse>(It.IsAny<string>())).Returns<string>((serObj) =>
            {
                return Serializer.Deserialize<UnsubscribeResponse>(serObj);
            });
            _serializerMock.Setup(o => o.DeserializeObject<GetBindingDescriptionsResponse>(It.IsAny<string>())).Returns<string>((serObj) =>
            {
                return Serializer.Deserialize<GetBindingDescriptionsResponse>(serObj);
            });
            _serializerMock.Setup(o => o.DeserializeObject<Notification>(It.IsAny<string>())).Returns<string>((serObj) =>
            {
                return Serializer.Deserialize<Notification>(serObj);
            });
            _serializerMock.Setup(o => o.DeserializeObject<EventNotification>(It.IsAny<string>())).Returns<string>((serObj) =>
            {
                return Serializer.Deserialize<EventNotification>(serObj);
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
                actualRequest = Serializer.Deserialize<SubscribeRequest>(req);

                return Serializer.Serialize(new SubscribeResponse()
                {
                    objectId = objectId,
                    eventId = nameof(IRegisteredObject.NotifyOnDataChanged)
                });
            });
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializerMock.Object);
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
        [ExpectedException(typeof(InvalidResponseException))]
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
        [ExpectedException(typeof(InvalidResponseException))]
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
                actualRequest = Serializer.Deserialize<UnsubscribeRequest>(req);

                return Serializer.Serialize(new UnsubscribeResponse()
                {
                    objectId = objectId,
                    eventId = nameof(IRegisteredObject.NotifyOnNonDataChanged)
                });
            });
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializerMock.Object);
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
                actualRequest = Serializer.Deserialize<UnsubscribeRequest>(req);

                return Serializer.Serialize(new UnsubscribeResponse()
                {
                    objectId = objectId,
                    eventId = nameof(IRegisteredObject.NotifyOnDataChanged)
                });
            });
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializerMock.Object);
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
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializerMock.Object);
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
                return Serializer.Serialize(new ExceptionResponse()
                {
                    exception = new RecipientBindingException()
                });
            });
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializerMock.Object);
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
                return Serializer.Serialize(new SubscribeResponse()
                {
                    objectId = objectId,
                    eventId = nameof(IRegisteredObject.NotifyOnNonDataChanged)
                });
            });
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializerMock.Object);
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
                return Serializer.Serialize(new UnsubscribeResponse()
                {
                    objectId = objectId,
                    eventId = "wrongEventId"
                });
            });
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializerMock.Object);
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
                return Serializer.Serialize(resp);
            });
            var sender = new Sender(_clientMock.Object, _serializerMock.Object);
            sender.Register<IRegisteredObject>();
            sender.SynchronizeBindings();
            _clientMock.Setup(o => o.Request(It.IsAny<string>())).Returns<string>(req =>
            {
                return Serializer.Serialize(new SubscribeResponse()
                {
                    objectId = objectId,
                    eventId = nameof(IRegisteredObject.NotifyOnNonDataChanged)
                });
            });
            var objProxy = sender.GetBindingsByType<IRegisteredObject>()[objectId];
            bool eventRaised = false;
            objProxy.NotifyOnNonDataChanged += (s, e) => { eventRaised = true; };

            var notification = new EventNotification()
            {
                objectId = objectId,
                eventId = nameof(IRegisteredObject.NotifyOnNonDataChanged),
                eventArgs = null
            };

            // Act
            _clientMock.Raise(o => o.NotificationReceived += null, new NotificationEventArgs(Serializer.Serialize(notification)));

            // Assert
            _clientMock.Verify();
            Assert.IsTrue(eventRaised);
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
                return Serializer.Serialize(resp);
            });
            var sender = new Sender(_clientMock.Object, _serializerMock.Object);
            sender.Register<IRegisteredObject>();
            sender.SynchronizeBindings();
            _clientMock.Setup(o => o.Request(It.IsAny<string>())).Returns<string>(req =>
            {
                return Serializer.Serialize(new SubscribeResponse()
                {
                    objectId = objectId,
                    eventId = nameof(IRegisteredObject.NotifyOnDataChanged)
                });
            });
            var objProxy = sender.GetBindingsByType<IRegisteredObject>()[objectId];
            bool eventRaised = false;
            objProxy.NotifyOnDataChanged += (s, e) => { eventRaised = true; };
            _serializerMock.Setup(o => o.DeserializeObject<EventNotification>(It.IsAny<string>())).Returns<string>((serObj) =>
            {
                var notif = Serializer.Deserialize<EventNotification>(serObj);
                return new EventNotification()
                {
                    objectId = notif.objectId,
                    eventId = notif.eventId,
                    eventArgs = Serializer.Deserialize<DataChangedEventArgs>(Serializer.Serialize(notif.eventArgs))
                };
            });
            var notification = new EventNotification()
            {
                objectId = objectId,
                eventId = nameof(IRegisteredObject.NotifyOnDataChanged),
                eventArgs = new DataChangedEventArgs("dataChanged", 2)
            };

            // Act
            _clientMock.Raise(o => o.NotificationReceived += null, new NotificationEventArgs(Serializer.Serialize(notification)));

            // Assert
            _clientMock.Verify();
            Assert.IsTrue(eventRaised);
        }
    }
}