using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AbstractBinding.Messages;

namespace AbstractBinding.Tests
{
    [TestClass]
    public class SenderInitializationTests
    {
        private const string _testCategory = "Sender Initialization";

        private readonly Mock<IProxyClient> _clientMock;

        public SenderInitializationTests()
        {
            // Initialize client mock
            _clientMock = new Mock<IProxyClient>();
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void RegisterTypeTest()
        {
            // Arrange
            var sender = new Sender(_clientMock.Object);

            // Act
            sender.Register<IRegisteredObject>();

            // Assert
            _clientMock.Verify();

            Assert.AreEqual(1, sender.RegisteredTypes.Count());
            Assert.AreEqual(typeof(IRegisteredObject), sender.RegisteredTypes.ElementAt(0));
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void SynchronizeBindingsTest()
        {
            // Arrange
            string objectId1 = "objId1";
            string objectId2 = "objId2";
            string objectId3 = "objId3";
            var regObjBinding = new ObjectBinding()
            {
                events = ObjectDescriptor.GetObjectDescription<IRegisteredObject>().Events,
                properties = ObjectDescriptor.GetObjectDescription<IRegisteredObject>().Properties.ToDictionary(p => p, p => new NestedObjectBinding()
                {
                }),
                methods = ObjectDescriptor.GetObjectDescription<IRegisteredObject>().Methods
            };
            var reg2ObjBinding = new ObjectBinding()
            {
                events = ObjectDescriptor.GetObjectDescription<IRegisteredObject2>().Events,
                properties = ObjectDescriptor.GetObjectDescription<IRegisteredObject2>().Properties.ToDictionary(p => p, p => new NestedObjectBinding()
                {
                }),
                methods = ObjectDescriptor.GetObjectDescription<IRegisteredObject2>().Methods
            };
            _clientMock.Setup(o => o.Request(It.IsAny<GetBindingDescriptionsRequest>())).Returns<GetBindingDescriptionsRequest>((req) =>
            {
                var resp = new GetBindingDescriptionsResponse();
                resp.bindings.Add(objectId1, regObjBinding);
                resp.bindings.Add(objectId2, regObjBinding);
                resp.bindings.Add(objectId3, reg2ObjBinding);
                return resp;
            });
            var sender = new Sender(_clientMock.Object);

            // Act
            sender.Register<IRegisteredObject>();
            sender.Register<IRegisteredObject2>();
            sender.SynchronizeBindings();
            var bindings1 = sender.GetBindingsByType<IRegisteredObject>();
            var bindings2 = sender.GetBindingsByType<IRegisteredObject2>();

            // Assert
            _clientMock.Verify();

            Assert.AreEqual(2, bindings1.Keys.Count());
            Assert.AreEqual(objectId1, bindings1.Keys.ElementAt(0));
            Assert.AreEqual(objectId2, bindings1.Keys.ElementAt(1));

            Assert.AreEqual(1, bindings2.Keys.Count());
            Assert.AreEqual(objectId3, bindings2.Keys.ElementAt(0));
        }
    }
}
