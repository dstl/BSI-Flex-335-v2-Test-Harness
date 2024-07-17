// Crown-owned copyright, 2021-2024
using NUnit.Framework;
using Sapient.Data;
using SAPIENT.ReadSampleMessage;

namespace SapientServicesValidator.UnitTests
{
    public class SapientServiceValidatorTests
    {
        [Test]
        public void TestRead_WithoutAdditionalText()
        {
            TestRead(Sapient.Data.SapientMessage.ContentOneofCase.Alert, string.Empty);
        }

        [Test]
        public void TestRead_WithAdditionalText()
        {
            TestRead(Sapient.Data.SapientMessage.ContentOneofCase.Alert, "Extra");
        }

        public void TestRead(Sapient.Data.SapientMessage.ContentOneofCase content, string additional)
        {
            string error = string.Empty;
            ReadSampleMessage x = new ReadSampleMessage();
            SapientMessage? m = x.ReadSampleMessageFromFile(Sapient.Data.SapientMessage.ContentOneofCase.Alert, string.Empty, out error);
            Assert.That(error, Is.Empty);
            Assert.That(m, Is.Not.Null);
        }
    }
}