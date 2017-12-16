using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AbstractBinding.Messages;

namespace AbstractBinding.Tests
{
    [TestClass]
    public class SenderInitializationTests
    {
        private const string _testCategory = "Sender Initialization";

        private readonly Mock<IAbstractClient> _clientMock;
        private readonly Mock<ISerializer> _serializerMock;
        private readonly Mock<IRegisteredObject> _regObjectMock;

        public SenderInitializationTests()
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
            _serializerMock.Setup(o => o.DeserializeObject<GetBindingDescriptionsResponse>(It.IsAny<string>())).Returns<string>((serObj) =>
            {
                return Serializer.Deserialize<GetBindingDescriptionsResponse>(serObj);
            });

            // Initialize registered object mock
            _regObjectMock = new Mock<IRegisteredObject>();
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void RegisterTypeTest()
        {
            // Arrange
            var sender = new Sender(_clientMock.Object, _serializerMock.Object);

            // Act
            sender.Register<IRegisteredObject>();

            // Assert
            Assert.AreEqual(1, sender.RegisteredTypes.Count());
            Assert.AreEqual(typeof(IRegisteredObject), sender.RegisteredTypes.ElementAt(0));
        }

        [TestMethod]
        [TestCategory(_testCategory)]
        public void SynchronizeBindingsTest()
        {
            // Arrange
            string objectId = "obj1Id";
            var objectDescriptionFactory = new ObjectDescriptionFactory();
            _clientMock.Setup(o => o.Request(It.IsAny<string>())).Returns<string>((req) =>
            {
                var resp = new GetBindingDescriptionsResponse();
                resp.bindings.Add(objectId, objectDescriptionFactory.Create<EmptyInterface>());
                return Serializer.Serialize(resp);
            });
            var sender = new Sender(_clientMock.Object, _serializerMock.Object);

            // Act
            sender.Register<EmptyInterface>();
            sender.SynchronizeBindings();
            var bindings = sender.GetBindingsByType<EmptyInterface>();

            // Assert
            Assert.AreEqual(1, bindings.Keys.Count());
            Assert.AreEqual(objectId, bindings.Keys.ElementAt(0));
        }
    }

    public interface EmptyInterface
    {

    }
}
