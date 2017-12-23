﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AbstractBinding.Tests
{
    [TestClass]
    public class RecipientInitializationTests
    {
        private const string _testCategory = "Recipient Initialization";
        private Mock<IRecipientCallback> _serviceMock;
        private Mock<ISerializer> _serializerMock;
        private Mock<IRegisteredObject> _regObjectMock;
        
        public RecipientInitializationTests()
        {
            // Initialize service mock
            _serviceMock = new Mock<IRecipientCallback>();

            // Initialize serializer  mock
            _serializerMock = new Mock<ISerializer>();

            // Initialize registered object mock
            _regObjectMock = new Mock<IRegisteredObject>();
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void RegisterObjectTest()
        {
            // Arrange
            var objectId = "objId1";
            var server = new Recipient(_serializerMock.Object);

            // Act
            server.Register(objectId, _regObjectMock.Object);

            // Assert
            //No exceptions thrown
            _serviceMock.Verify();
            _serializerMock.Verify();
            _regObjectMock.Verify();
        }
    }
}
