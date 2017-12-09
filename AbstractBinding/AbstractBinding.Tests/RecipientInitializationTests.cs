using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AbstractBinding.Tests
{
    [TestClass]
    public class RecipientInitializationTests
    {
        private const string _testCategory = "Recipient Initialization";
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

            // Initialize registered object mock
            _regObjectMock = new Mock<IRegisteredObject>();
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void RegisterObjectTest()
        {
            // Arrange
            var objectId = "objId1";
            var server = new Recipient(_serviceMock.Object, _serializerMock.Object);

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
