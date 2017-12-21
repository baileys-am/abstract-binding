using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AbstractBinding.Messages;
using AbstractBinding.SenderInternals;

namespace AbstractBinding.Tests
{
    [TestClass]
    public class PropertyProxyTests
    {
        private const string _testCategory = "Property Proxy Tests";

        private readonly Mock<IAbstractClient> _clientMock;
        private readonly Mock<ISerializer> _serializerMock;

        public PropertyProxyTests()
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
            _serializerMock.Setup(o => o.DeserializeObject<PropertyGetResponse>(It.IsAny<string>())).Returns<string>((serObj) =>
            {
                return Serializer.Deserialize<PropertyGetResponse>(serObj);
            });
            _serializerMock.Setup(o => o.DeserializeObject<PropertySetResponse>(It.IsAny<string>())).Returns<string>((serObj) =>
            {
                return Serializer.Deserialize<PropertySetResponse>(serObj);
            });
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void PropertyGetValue()
        {
            // Arrange
            string objectId = "obj1Id";
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializerMock.Object);
            var proxyObj = proxyFactory.Create<IRegisteredObject>(objectId);
            string expectedValue = "actual value";
            _clientMock.Setup(o => o.Request(It.IsAny<string>())).Returns<string>(req =>
            {
                return Serializer.Serialize(new PropertyGetResponse()
                {
                    objectId = objectId,
                    propertyId = nameof(IRegisteredObject.StringValueProperty),
                    value = expectedValue
                });
            });

            // Act
            var actualValue = proxyObj.StringValueProperty;

            // Assert
            _clientMock.VerifyAll();
            Assert.AreEqual(expectedValue, actualValue);
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void PropertySetValue()
        {
            // Arrange
            string objectId = "obj1Id";
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializerMock.Object);
            var proxyObj = proxyFactory.Create<IRegisteredObject>(objectId);
            string expectedValue = "actual value";
            string actualValue = null;
            _clientMock.Setup(o => o.Request(It.IsAny<string>())).Returns<string>(req =>
            {
                actualValue = Serializer.Deserialize<PropertySetRequest>(req).value as string;
                return Serializer.Serialize(new PropertySetResponse()
                {
                    objectId = objectId,
                    propertyId = nameof(IRegisteredObject.StringValueProperty)
                });
            });

            // Act
            proxyObj.StringValueProperty = expectedValue;

            // Assert
            _clientMock.VerifyAll();
            Assert.AreEqual(expectedValue, actualValue);
        }
    }
}
