using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AbstractBinding.Messages;

namespace AbstractBinding.Tests
{
    [TestClass]
    public class RecipientPropertyApiTests
    {
        private const string _testCategory = "Recipient Property API";
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
            _serializerMock.Setup(o => o.SerializeObject(It.IsAny<object>())).Returns<object>(obj =>
            {
                return Serializer.Serialize(obj);
            });
            _serializerMock.Setup(o => o.DeserializeObject<Request>(It.IsAny<string>())).Returns<string>((serObj) =>
            {
                return Serializer.Deserialize<Request>(serObj);
            });
            _serializerMock.Setup(o => o.DeserializeObject<PropertyGetRequest>(It.IsAny<string>())).Returns<string>(serObj =>
            {
                return Serializer.Deserialize<PropertyGetRequest>(serObj);
            });

            // Initialize registered object mock
            _regObjectMock = new Mock<IRegisteredObject>();
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void GetValueWithoutExceptionTest()
        {
            // Arrange
            string objectId = "objId1";
            string expectedValue = "toBeExpected";
            _regObjectMock.Setup(o => o.StringValueProperty).Returns(expectedValue);
            var recipient = new Recipient(_serviceMock.Object, _serializerMock.Object);
            var requestObj = new PropertyGetRequest()
            {
                objectId = objectId,
                propertyId = nameof(IRegisteredObject.StringValueProperty)
            };

            // Act
            recipient.Register(objectId, _regObjectMock.Object);
            string response = recipient.Request(Serializer.Serialize(requestObj));

            // Assert
            var responseObj = Serializer.Deserialize<PropertyGetResponse>(response);
            Assert.AreEqual(requestObj.objectId, responseObj.objectId);
            Assert.AreEqual(requestObj.propertyId, responseObj.propertyId);
            Assert.AreEqual(expectedValue, responseObj.value);
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void GetValueWithExceptionTest()
        {
            Assert.Fail();
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void SetValueWithoutExceptionTest()
        {
            Assert.Fail();
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void SetValueWithExceptionTest()
        {
            Assert.Fail();
        }
    }
}
