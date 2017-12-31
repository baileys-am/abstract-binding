using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AbstractBinding.Messages;

namespace AbstractBinding.Tests
{
    [TestClass]
    public class RecipientInitializationTests
    {
        private const string _testCategory = "Recipient Initialization";
        private readonly Mock<IRecipientCallback> _serviceMock;
        private readonly Mock<IRegisteredObject> _regObjectMock;

        public RecipientInitializationTests()
        {
            // Initialize service mock
            _serviceMock = new Mock<IRecipientCallback>();

            // Initialize registered object mock
            _regObjectMock = new Mock<IRegisteredObject>();
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void RegisterObjectTest()
        {
            // Arrange
            var objectId = "objId1";
            var server = new Recipient();

            // Act
            server.Register(objectId, _regObjectMock.Object);

            // Assert
            _serviceMock.Verify();
            _regObjectMock.Verify();
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void RegisterObjectWithNestedBindingTest()
        {
            // Arrange
            var objectId = "objId1";
            var server = new Recipient();
            var nestedObjectMock = new Mock<INestedObject>();
            _regObjectMock.SetupGet(o => o.NestedObject).Returns(nestedObjectMock.Object);

            // Act
            server.Register(objectId, _regObjectMock.Object, typeof(INestedObject));
            var resp = server.Request(new GetBindingDescriptionsRequest()) as GetBindingDescriptionsResponse;

            // Assert
            _serviceMock.Verify();
            _regObjectMock.Verify();
            Assert.AreEqual(2, resp.bindings.Count);
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void GetBindingsDescriptionRequestTest()
        {
            // Arrange
            var objectId = "objId1";
            var server = new Recipient();
            server.Register(objectId, _regObjectMock.Object);
            var req = new GetBindingDescriptionsRequest();

            // Act
            var resp = server.Request(req) as GetBindingDescriptionsResponse;

            // Assert
            Assert.AreEqual(ResponseType.getBindings, resp.responseType);
            Assert.AreEqual(1, resp.bindings.Count);
            Assert.AreEqual(objectId, resp.bindings.Keys.First());
        }
    }
}
