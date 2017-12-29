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
        private readonly ISerializer _serializer = new Serializer();

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
            var server = new Recipient(_serializer);

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
            var server = new Recipient(_serializer);
            var nestedObjectMock = new Mock<INestedObject>();
            _regObjectMock.SetupGet(o => o.NestedObject).Returns(nestedObjectMock.Object);

            // Act
            server.Register(objectId, _regObjectMock.Object, typeof(INestedObject));
            string resp = server.Request(_serializer.SerializeObject(new GetBindingDescriptionsRequest()));
            GetBindingDescriptionsResponse respObj = _serializer.DeserializeObject<GetBindingDescriptionsResponse>(resp);

            // Assert
            _serviceMock.Verify();
            _regObjectMock.Verify();
            Assert.AreEqual(2, respObj.bindings.Count);
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void GetBindingsDescriptionRequestTest()
        {
            // Arrange
            var objectId = "objId1";
            var server = new Recipient(_serializer);
            server.Register(objectId, _regObjectMock.Object);
            var req = _serializer.SerializeObject(new GetBindingDescriptionsRequest());

            // Act
            var resp = server.Request(req);

            // Assert
            var respObj = _serializer.DeserializeObject<GetBindingDescriptionsResponse>(resp);
            Assert.AreEqual(ResponseType.getBindings, respObj.responseType);
            Assert.AreEqual(1, respObj.bindings.Count);
            Assert.AreEqual(objectId, respObj.bindings.Keys.First());
        }
    }
}
