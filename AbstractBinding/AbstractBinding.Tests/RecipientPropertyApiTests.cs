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
        private readonly Mock<IRecipientCallback> _serviceMock;
        private readonly Mock<IRegisteredObject> _regObjectMock;

        public RecipientPropertyApiTests()
        {
            // Initialize service mock
            _serviceMock = new Mock<IRecipientCallback>();

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
            var recipient = new Recipient();
            var request = new PropertyGetRequest()
            {
                objectId = objectId,
                propertyId = nameof(IRegisteredObject.StringValueProperty)
            };

            // Act
            recipient.Register(objectId, _regObjectMock.Object);
            var response = recipient.Request(request) as PropertyGetResponse;

            // Assert
            _serviceMock.Verify();
            _regObjectMock.Verify();
            
            Assert.AreEqual(ResponseType.propertyGet, response.responseType);
            Assert.AreEqual(request.objectId, response.objectId);
            Assert.AreEqual(request.propertyId, response.propertyId);
            Assert.AreEqual(expectedValue, response.value);
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void GetValueWithExceptionTest()
        {
            // Arrange
            string objectId = "objId1";
            var recipient = new Recipient();
            var request = new PropertyGetRequest()
            {
                objectId = objectId,
                propertyId = nameof(IRegisteredObject.StringValueProperty)
            };
            recipient.Register(objectId, _regObjectMock.Object);
            _regObjectMock.SetupGet(o => o.StringValueProperty).Throws(new NotImplementedException());

            // Act
            var response = recipient.Request(request) as ExceptionResponse;

            // Assert
            _serviceMock.Verify();
            _regObjectMock.Verify();
            
            Assert.AreEqual(ResponseType.exception, response.responseType);
            Assert.IsTrue(response.exception.Message.Contains(objectId) &&
                          response.exception.Message.Contains(nameof(IRegisteredObject.StringValueProperty)));
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void SetValueWithoutExceptionTest()
        {
            // Arrange
            string objectId = "objId1";
            string expectedValue = "toBeExpected";
            _regObjectMock.Setup(o => o.StringValueProperty).Returns(expectedValue);
            var recipient = new Recipient();
            var request = new PropertySetRequest()
            {
                objectId = objectId,
                propertyId = nameof(IRegisteredObject.StringValueProperty),
                value = expectedValue
            };

            // Act
            recipient.Register(objectId, _regObjectMock.Object);
            var response = recipient.Request(request) as PropertySetResponse;

            // Assert
            _serviceMock.Verify();
            _regObjectMock.Verify();

            Assert.AreEqual(expectedValue, _regObjectMock.Object.StringValueProperty);
            
            Assert.AreEqual(ResponseType.propertySet, response.responseType);
            Assert.AreEqual(request.objectId, response.objectId);
            Assert.AreEqual(request.propertyId, response.propertyId);
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void SetValueWithExceptionTest()
        {
            // Arrange
            string objectId = "objId1";
            _regObjectMock.SetupSet(o => o.StringValueProperty = It.IsAny<string>()).Throws(new NotImplementedException());
            var recipient = new Recipient();
            var request = new PropertySetRequest()
            {
                objectId = objectId,
                propertyId = nameof(IRegisteredObject.StringValueProperty),
                value = "toBeExpected"
            };

            // Act
            recipient.Register(objectId, _regObjectMock.Object);
            var response = recipient.Request(request) as ExceptionResponse;

            // Assert
            _serviceMock.Verify();
            _regObjectMock.Verify();
            
            Assert.AreEqual(ResponseType.exception, response.responseType);
            Assert.IsTrue(response.exception.Message.Contains(objectId) &&
                          response.exception.Message.Contains(nameof(IRegisteredObject.StringValueProperty)));
        }
    }
}
