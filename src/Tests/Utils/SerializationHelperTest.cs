using System.Collections.Generic;
using Bootstrap.Installer.Utils;
using NUnit.Framework;
using Newtonsoft.Json;

namespace Bootstrap.Tests.Utils
{
    [TestFixture]
    public class SerializationHelperTest
    {
        [Test]
        public void CanConvertIntIntDictionaryToJason()
        {
            var dictionary = new Dictionary<int, int> {{1, 10}, {2, 20}, {3, 30}};
            var json = dictionary.DictionaryToJson();
            Assert.IsNotNullOrEmpty(json);
        }

        [Test]
        public void CanConvertJsonToIntIntDictionary()
        {
            var json = "{\"1\":10,\"2\":20,\"3\":30}";
            var dictionary = json.JsonToDictionary<Dictionary<int, int>>();
            Assert.IsNotNull(dictionary);
            Assert.AreEqual(dictionary.GetType(), typeof(Dictionary<int, int>));
        }

        [Test]
        [ExpectedException(typeof(JsonReaderException))]
        public void CannotConvertJsonToDictionaryIfDataIsWrong()
        {
            "this is a test".JsonToDictionary<Dictionary<int, int>>();
        }

        [Test]
        public void CannotConvertJsonToDictionaryIfDataIsEmpty()
        {
            var dictionary = "".JsonToDictionary<Dictionary<int, int>>();
            Assert.IsNull(dictionary);
        }
    }
}
