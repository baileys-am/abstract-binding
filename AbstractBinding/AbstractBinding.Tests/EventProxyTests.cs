using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AbstractBinding.Messages;

namespace AbstractBinding.Tests
{
    [TestClass]
    public class EventProxyTests
    {
        private const string _testCategory = "Event Proxy";
        private const string _objId1 = "objId1";
        private const string _objId2 = "objId2";

        private readonly Mock<IProxyClient> _clientMock;
        private readonly Sender _sender;

        public EventProxyTests()
        {
            // Initialize client mock
            _clientMock = new Mock<IProxyClient>();
            _clientMock.Setup(o => o.Request(It.IsAny<GetBindingDescriptionsRequest>())).Returns<GetBindingDescriptionsRequest>(req =>
            {
                return new GetBindingDescriptionsResponse()
                {
                    bindings = new Dictionary<string, ObjectDescription>()
                    {
                        { _objId1, ObjectDescriptor.GetObjectDescription<IRegisteredObject>() },
                        { _objId2, ObjectDescriptor.GetObjectDescription<IRegisteredObject2>() }
                    }
                };
            });

            // Initialize sender
            _sender = new Sender(_clientMock.Object);
            _sender.Register<IRegisteredObject>();
            _sender.Register<IRegisteredObject2>();
            _sender.SynchronizeBindings();
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void SubscribeNoArgsTest()
        {
            // Arrange
            var expectedRequest = new SubscribeRequest()
            {
                objectId = _objId1,
                eventId = ObjectDescriptor.GetEventId<IRegisteredObject>(nameof(IRegisteredObject.NotifyOnNonDataChanged))
            };
            SubscribeRequest actualRequest = null;
            _clientMock.Setup(o => o.Request(It.IsAny<SubscribeRequest>())).Returns<SubscribeRequest>(req =>
            {
                actualRequest = req;

                return new SubscribeResponse()
                {
                    objectId = _objId1,
                    eventId = ObjectDescriptor.GetEventId<IRegisteredObject>(nameof(IRegisteredObject.NotifyOnNonDataChanged))
                };
            });

            var objProxy = _sender.GetBindingsByType<IRegisteredObject>()[_objId1];

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
                eventId = ObjectDescriptor.GetEventId<IRegisteredObject>(nameof(IRegisteredObject.NotifyOnDataChanged))
            };
            SubscribeRequest actualRequest = null;
            _clientMock.Setup(o => o.Request(It.IsAny<SubscribeRequest>())).Returns<SubscribeRequest>(req =>
            {
                actualRequest = req;

                return new SubscribeResponse()
                {
                    objectId = objectId,
                    eventId = ObjectDescriptor.GetEventId<IRegisteredObject>(nameof(IRegisteredObject.NotifyOnDataChanged))
                };
            });
            var objProxy = RuntimeProxy.Create<IRegisteredObject>(objectId, _clientMock.Object);

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
            _clientMock.Setup(o => o.Request(It.IsAny<IRequest>())).Returns<IRequest>(req =>
            {
                throw new NotImplementedException();
            });
            var objProxy = RuntimeProxy.Create<IRegisteredObject>(objectId, _clientMock.Object);

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
            _clientMock.Setup(o => o.Request(It.IsAny<IRequest>())).Returns<IRequest>(req =>
            {
                return new ExceptionResponse()
                {
                    exception = new RecipientBindingException()
                };
            });
            var objProxy = RuntimeProxy.Create<IRegisteredObject>(objectId, _clientMock.Object);

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
            _clientMock.Setup(o => o.Request(It.IsAny<IRequest>())).Returns<IRequest>(req =>
            {
                return new UnsubscribeResponse()
                {
                    objectId = objectId,
                    eventId = ObjectDescriptor.GetEventId<IRegisteredObject>(nameof(IRegisteredObject.NotifyOnNonDataChanged))
                };
            });
            var objProxy = RuntimeProxy.Create<IRegisteredObject>(objectId, _clientMock.Object);

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
            _clientMock.Setup(o => o.Request(It.IsAny<SubscribeRequest>())).Returns<SubscribeRequest>(req =>
            {
                return new SubscribeResponse()
                {
                    objectId = objectId,
                    eventId = "wrongEventId"
                };
            });
            var objProxy = RuntimeProxy.Create<IRegisteredObject>(objectId, _clientMock.Object);

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
                eventId = ObjectDescriptor.GetEventId<IRegisteredObject>(nameof(IRegisteredObject.NotifyOnNonDataChanged))
            };
            UnsubscribeRequest actualRequest = null;
            _clientMock.Setup(o => o.Request(It.IsAny<UnsubscribeRequest>())).Returns<UnsubscribeRequest>(req =>
            {
                actualRequest = req;

                return new UnsubscribeResponse()
                {
                    objectId = objectId,
                    eventId = ObjectDescriptor.GetEventId<IRegisteredObject>(nameof(IRegisteredObject.NotifyOnNonDataChanged))
                };
            });
            var objProxy = RuntimeProxy.Create<IRegisteredObject>(objectId, _clientMock.Object);

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
                eventId = ObjectDescriptor.GetEventId<IRegisteredObject>(nameof(IRegisteredObject.NotifyOnDataChanged))
            };
            UnsubscribeRequest actualRequest = null;
            _clientMock.Setup(o => o.Request(It.IsAny<UnsubscribeRequest>())).Returns<UnsubscribeRequest>(req =>
            {
                actualRequest = req;

                return new UnsubscribeResponse()
                {
                    objectId = objectId,
                    eventId = ObjectDescriptor.GetEventId<IRegisteredObject>(nameof(IRegisteredObject.NotifyOnDataChanged))
                };
            });
            var objProxy = RuntimeProxy.Create<IRegisteredObject>(objectId, _clientMock.Object);

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
            _clientMock.Setup(o => o.Request(It.IsAny<IRequest>())).Returns<IRequest>(req =>
            {
                throw new NotImplementedException();
            });
            var objProxy = RuntimeProxy.Create<IRegisteredObject>(objectId, _clientMock.Object);

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
            _clientMock.Setup(o => o.Request(It.IsAny<IRequest>())).Returns<IRequest>(req =>
            {
                return new ExceptionResponse()
                {
                    exception = new RecipientBindingException()
                };
            });
            var objProxy = RuntimeProxy.Create<IRegisteredObject>(objectId, _clientMock.Object);

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
            _clientMock.Setup(o => o.Request(It.IsAny<IRequest>())).Returns<IRequest>(req =>
            {
                return new SubscribeResponse()
                {
                    objectId = objectId,
                    eventId = ObjectDescriptor.GetEventId<IRegisteredObject>(nameof(IRegisteredObject.NotifyOnNonDataChanged))
                };
            });
            var objProxy = RuntimeProxy.Create<IRegisteredObject>(objectId, _clientMock.Object);

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
            _clientMock.Setup(o => o.Request(It.IsAny<IRequest>())).Returns<IRequest>(req =>
            {
                return new UnsubscribeResponse()
                {
                    objectId = objectId,
                    eventId = "wrongEventId"
                };
            });
            var objProxy = RuntimeProxy.Create<IRegisteredObject>(objectId, _clientMock.Object);

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
            _clientMock.Setup(o => o.Request(It.IsAny<IRequest>())).Returns<IRequest>((req) =>
            {
                var resp = new GetBindingDescriptionsResponse();
                resp.bindings.Add(objectId, ObjectDescriptor.GetObjectDescription<IRegisteredObject>());
                return resp;
            });
            var sender = new Sender(_clientMock.Object);
            sender.Register<IRegisteredObject>();
            sender.SynchronizeBindings();
            _clientMock.Setup(o => o.Request(It.IsAny<SubscribeRequest>())).Returns<SubscribeRequest>(req =>
            {
                return new SubscribeResponse()
                {
                    objectId = objectId,
                    eventId = ObjectDescriptor.GetEventId<IRegisteredObject>(nameof(IRegisteredObject.NotifyOnNonDataChanged))
                };
            });
            var objProxy = sender.GetBindingsByType<IRegisteredObject>()[objectId];
            bool argsReceived = false;
            objProxy.NotifyOnNonDataChanged += (s, e) => { argsReceived = e as EventArgs != null; };
            var notification = new EventNotification()
            {
                objectId = objectId,
                eventId = ObjectDescriptor.GetEventId<IRegisteredObject>(nameof(IRegisteredObject.NotifyOnNonDataChanged)),
                eventArgs = EventArgs.Empty
            };

            // Act
            _clientMock.Raise(o => o.NotificationReceived += null, null, new Messages.NotificationEventArgs(notification));

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
            _clientMock.Setup(o => o.Request(It.IsAny<IRequest>())).Returns<IRequest>((req) =>
            {
                var resp = new GetBindingDescriptionsResponse();
                resp.bindings.Add(objectId, ObjectDescriptor.GetObjectDescription<IRegisteredObject>());
                return resp;
            });
            var sender = new Sender(_clientMock.Object);
            sender.Register<IRegisteredObject>();
            sender.SynchronizeBindings();
            _clientMock.Setup(o => o.Request(It.IsAny<SubscribeRequest>())).Returns<SubscribeRequest>(req =>
            {
                return new SubscribeResponse()
                {
                    objectId = objectId,
                    eventId = ObjectDescriptor.GetEventId<IRegisteredObject>(nameof(IRegisteredObject.NotifyOnDataChanged))
                };
            });
            var objProxy = sender.GetBindingsByType<IRegisteredObject>()[objectId];
            bool argsReceived = false;
            objProxy.NotifyOnDataChanged += (s, e) => { argsReceived = e as DataChangedEventArgs != null; };
            var notification = new EventNotification()
            {
                objectId = objectId,
                eventId = ObjectDescriptor.GetEventId<IRegisteredObject>(nameof(IRegisteredObject.NotifyOnDataChanged)),
                eventArgs = new DataChangedEventArgs("dataChanged", 2)
            };

            // Act
            _clientMock.Raise(o => o.NotificationReceived += null, null, new Messages.NotificationEventArgs(notification));

            // Assert
            _clientMock.Verify();
            Assert.IsTrue(argsReceived);
        }
    }
}
