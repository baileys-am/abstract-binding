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
        private readonly Mock<IProxyClient> _clientMock;

        public PropertyProxyTests()
        {
            // Initialize client mock
            _clientMock = new Mock<IProxyClient>();
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void PropertyGetValue()
        {
            // Arrange
            string objectId = "obj1Id";
            var proxyObj = RuntimeProxy.Create<IRegisteredObject>(objectId, _clientMock.Object);
            string expectedValue = "actual value";
            _clientMock.Setup(o => o.Request(It.IsAny<PropertyGetRequest>())).Returns<PropertyGetRequest>(req =>
            {
                return new PropertyGetResponse()
                {
                    objectId = objectId,
                    propertyId = ObjectDescriptor.GetPropertyId<IRegisteredObject>(nameof(IRegisteredObject.StringValueProperty)),
                    value = expectedValue
                };
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
            var proxyObj = RuntimeProxy.Create<IRegisteredObject>(objectId, _clientMock.Object);
            string expectedValue = "actual value";
            string actualValue = null;
            _clientMock.Setup(o => o.Request(It.IsAny<PropertySetRequest>())).Returns<PropertySetRequest>(req =>
            {
                actualValue = req.value as string;
                return new PropertySetResponse()
                {
                    objectId = objectId,
                    propertyId = ObjectDescriptor.GetPropertyId<IRegisteredObject>(nameof(IRegisteredObject.StringValueProperty))
                };
            });

            // Act
            proxyObj.StringValueProperty = expectedValue;

            // Assert
            _clientMock.VerifyAll();
            Assert.AreEqual(expectedValue, actualValue);
        }
    }
}
