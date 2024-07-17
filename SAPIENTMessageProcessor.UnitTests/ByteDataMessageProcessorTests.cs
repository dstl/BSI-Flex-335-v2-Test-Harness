// Crown-owned copyright, 2021-2024
using NUnit.Framework;
using SAPIENT.MessageProcessor;
using SAPIENT.MessageProcessor.TCP;

namespace SAPIENT.Message.Processor.UnitTests
{
    public class ByteDataMessageBuilderTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void MessageBuilder_AddHeader_Node()
        {
            ByteDataMessageBuilder messageBuilder = new ByteDataMessageBuilder();
            byte[] inputData = new byte[0] {};
            byte[] result = messageBuilder.AddHeader(inputData);
            byte[] outputData = new byte[4] { 0, 0, 0, 0 };
            Assert.That(outputData.Length, Is.EqualTo(4));
            Assert.That(result, Is.EqualTo(outputData));
        }

        [Test]
        public void MessageBuilder_AddHeader_Small()
        {
            ByteDataMessageBuilder messageBuilder = new ByteDataMessageBuilder();
            byte[] inputData = new byte[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            byte[] result = messageBuilder.AddHeader(inputData);
            byte[] outputData = new byte[14] { 10, 0, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            Assert.That(outputData.Length, Is.EqualTo(14));
            Assert.That(result, Is.EqualTo(outputData));
        }

        [Test]
        public void MessageBuilder_AddHeader_Big()
        {
            ByteDataMessageBuilder messageBuilder = new ByteDataMessageBuilder();
            byte[] inputData = new byte[10000000];
            byte[] result = messageBuilder.AddHeader(inputData);
            byte[] header = new byte[4] { 128, 150, 152, 0};
            byte[] outputData = header.Concat(inputData).ToArray();
            Assert.That(outputData.Length, Is.EqualTo(10000004));
            Assert.That(result, Is.EqualTo(outputData));
        }

        [Test]
        public void MessageBuilder_AddData_None()
        {
            ByteDataMessageBuilder messageBuilder = new ByteDataMessageBuilder();
            messageBuilder.DataReceived += new ByteDataMessageEventHandler((object sender, ByteDataMessageEventArgs e) => 
            {
                Assert.Fail();
            });
            messageBuilder.AddData(new byte[0]);
            Assert.Pass();
        }

        [Test]
        public void MessageBuilder_AddData_Small()
        {
            ByteDataMessageBuilder messageBuilder = new ByteDataMessageBuilder();
            messageBuilder.DataReceived += new ByteDataMessageEventHandler((object sender, ByteDataMessageEventArgs e) =>
            {
                Assert.Fail();
            });
            byte[] rawData = new byte[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            byte[] inputData = messageBuilder.AddHeader(rawData);
            byte[] makeSmall = inputData.Take(10).ToArray();

            messageBuilder.AddData(makeSmall);
            Assert.Pass();
        }

        [Test]
        public void MessageBuilder_AddData_Exact()
        {
            byte[] inputData = new byte[14] { 10, 0, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            byte[] outputData = new byte[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            byte[] result = null;
            ByteDataMessageBuilder messageBuilder = new ByteDataMessageBuilder();
            messageBuilder.DataReceived += new ByteDataMessageEventHandler((object sender, ByteDataMessageEventArgs e) =>
            {
                result = e.MessageData;
            });
            messageBuilder.AddData(inputData);
            Assert.NotNull(result);
            Assert.That(outputData.Length, Is.EqualTo(10));
            Assert.That(result, Is.EqualTo(outputData));
        }

        [Test]
        public void MessageBuilder_AddData_Big()
        {
            byte[] inputData = new byte[18] { 10, 0, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };
            byte[] outputData = new byte[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            byte[] result = null;
            ByteDataMessageBuilder messageBuilder = new ByteDataMessageBuilder();
            messageBuilder.DataReceived += new ByteDataMessageEventHandler((object sender, ByteDataMessageEventArgs e) =>
            {
                result = e.MessageData;
            });
            messageBuilder.AddData(inputData);
            Assert.NotNull(result);
            Assert.That(outputData.Length, Is.EqualTo(10));
            Assert.That(result, Is.EqualTo(outputData));
        }

        [Test]
        public void MessageBuilder_AddData_TwoMessages()
        {
            byte[] inputData = new byte[28] { 10, 0, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 0, 0, 0, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, };
            byte[] outputData1 = new byte[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            byte[] outputData2 = new byte[10] { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            byte[] result1 = null;
            byte[] result2 = null;
            ByteDataMessageBuilder messageBuilder = new ByteDataMessageBuilder();
            messageBuilder.DataReceived += new ByteDataMessageEventHandler((object sender, ByteDataMessageEventArgs e) =>
            {
                if (result1 == null)
                {
                    result1 = e.MessageData;
                }
                else
                {
                    result2 = e.MessageData;
                }
            });
            messageBuilder.AddData(inputData);
            Assert.NotNull(result1);
            Assert.That(outputData1.Length, Is.EqualTo(10));
            Assert.That(result1, Is.EqualTo(outputData1));
            Assert.NotNull(result2);
            Assert.That(outputData2.Length, Is.EqualTo(10));
            Assert.That(result2, Is.EqualTo(outputData2));
        }

        [Test]
        public void MessageBuilder_AddData_ExtraBig()
        {
            byte[] inputData = new byte[30] { 10, 0, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 0, 0, 0, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 30, 31};
            byte[] outputData1 = new byte[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            byte[] outputData2 = new byte[10] { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            byte[] result1 = null;
            byte[] result2 = null;
            ByteDataMessageBuilder messageBuilder = new ByteDataMessageBuilder();
            messageBuilder.DataReceived += new ByteDataMessageEventHandler((object sender, ByteDataMessageEventArgs e) =>
            {
                if (result1 == null)
                {
                    result1 = e.MessageData;
                }
                else
                {
                    result2 = e.MessageData;
                }
            });
            messageBuilder.AddData(inputData);
            Assert.NotNull(result1);
            Assert.That(outputData1.Length, Is.EqualTo(10));
            Assert.That(result1, Is.EqualTo(outputData1));
            Assert.NotNull(result2);
            Assert.That(outputData2.Length, Is.EqualTo(10));
            Assert.That(result2, Is.EqualTo(outputData2));
        }
    }
}