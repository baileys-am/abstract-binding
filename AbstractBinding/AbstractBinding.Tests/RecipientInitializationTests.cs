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
        private Mock<IRecipientCallback> _serviceMock;
        private Mock<ISerializer> _serializerMock;
        private Mock<IRegisteredObject> _regObjectMock;
        
        public RecipientInitializationTests()
        {
            // Initialize service mock
            _serviceMock = new Mock<IRecipientCallback>();

            // Initialize serializer  mock
            _serializerMock = new Mock<ISerializer>();
            _serializerMock.Setup(o => o.SerializeObject(It.IsAny<object>())).Returns<object>(obj =>
            {
                return Serializer.Serialize(obj);
            });
            _serializerMock.Setup(o => o.DeserializeObject<Request>(It.IsAny<string>())).Returns<string>((serObj) =>
            {
                return Serializer.Deserialize<Request>(serObj);
            });
            _serializerMock.Setup(o => o.DeserializeObject<GetBindingDescriptionsRequest>(It.IsAny<string>())).Returns<string>((serObj) =>
            {
                return Serializer.Deserialize<GetBindingDescriptionsRequest>(serObj);
            });

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

        [TestMethod]
        [TestCategory(_testCategory)]
        public void GetBindingsDescriptionRequestTest()
        {
            // Arrange
            var objectId = "objId1";
            var server = new Recipient(_serializerMock.Object);
            server.Register(objectId, _regObjectMock.Object);
            var req = Serializer.Serialize(new GetBindingDescriptionsRequest());

            // Act
            var resp = server.Request(req);

            // Assert
            var respObj = Serializer.Deserialize<GetBindingDescriptionsResponse>(resp);
            Assert.AreEqual(ResponseType.getBindings, respObj.responseType);
            Assert.AreEqual(1, respObj.bindings.Count);
            Assert.AreEqual(objectId, respObj.bindings.Keys.First());
        }
    }
}
