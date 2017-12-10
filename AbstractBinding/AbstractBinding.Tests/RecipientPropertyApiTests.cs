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

            // Initialize registered object mock
            _regObjectMock = new Mock<IRegisteredObject>();
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void GetValueWithoutExceptionTest()
        {
            Assert.Fail();
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
