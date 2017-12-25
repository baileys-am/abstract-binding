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
        private readonly ISerializer _serializer = new Serializer();

        public PropertyProxyTests()
        {
            // Initialize client mock
            _clientMock = new Mock<IAbstractClient>();
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void PropertyGetValue()
        {
            // Arrange
            string objectId = "obj1Id";
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializer);
            var proxyObj = proxyFactory.Create<IRegisteredObject>(objectId);
            string expectedValue = "actual value";
            _clientMock.Setup(o => o.Request(It.IsAny<string>())).Returns<string>(req =>
            {
                return _serializer.SerializeObject(new PropertyGetResponse()
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
            var proxyFactory = new RuntimeProxyFactory(_clientMock.Object, _serializer);
            var proxyObj = proxyFactory.Create<IRegisteredObject>(objectId);
            string expectedValue = "actual value";
            string actualValue = null;
            _clientMock.Setup(o => o.Request(It.IsAny<string>())).Returns<string>(req =>
            {
                actualValue = _serializer.DeserializeObject<PropertySetRequest>(req).value as string;
                return _serializer.SerializeObject(new PropertySetResponse()
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
