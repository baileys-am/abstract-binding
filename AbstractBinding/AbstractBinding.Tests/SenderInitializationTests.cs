﻿using System;
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

        private readonly Mock<ISenderClient> _clientMock;
        private readonly ISerializer _serializer = new Serializer();

        public SenderInitializationTests()
        {
            // Initialize client mock
            _clientMock = new Mock<ISenderClient>();
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void RegisterTypeTest()
        {
            // Arrange
            var sender = new Sender(_clientMock.Object, _serializer);

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
            var objectDescriptionFactory = new ObjectDescriptionFactory();
            _clientMock.Setup(o => o.Request(It.IsAny<string>())).Returns<string>((req) =>
            {
                var resp = new GetBindingDescriptionsResponse();
                resp.bindings.Add(objectId1, objectDescriptionFactory.Create<IRegisteredObject>());
                resp.bindings.Add(objectId2, objectDescriptionFactory.Create<IRegisteredObject>());
                resp.bindings.Add(objectId3, objectDescriptionFactory.Create<IRegisteredObject2>());
                return _serializer.SerializeObject(resp);
            });
            var sender = new Sender(_clientMock.Object, _serializer);

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
