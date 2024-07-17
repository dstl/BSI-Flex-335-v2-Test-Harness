// Crown-owned copyright, 2021-2024
namespace SAPIENT.MessageProcessor
{
    public interface IByteDataMessageBuilder
    {
        public event ByteDataMessageEventHandler DataReceived;

        public byte[] AddHeader(byte[] data);

        public void AddData(byte[] data);
    }
}