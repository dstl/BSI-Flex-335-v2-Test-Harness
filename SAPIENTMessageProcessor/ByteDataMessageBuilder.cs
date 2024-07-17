// Crown-owned copyright, 2021-2024
namespace SAPIENT.MessageProcessor.TCP
{
    public class ByteDataMessageBuilder : IByteDataMessageBuilder   
    {
        private byte[] currentData = new byte[0];

        public event ByteDataMessageEventHandler DataReceived;

        public ByteDataMessageBuilder()
        {
        }

        public byte[] AddHeader(byte[] data)
        {
            var dataLength = data.Length;
            byte[] dataLengthInBytes = BitConverter.GetBytes(dataLength);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(dataLengthInBytes);
            }
            return dataLengthInBytes.Concat(data).ToArray();
        }

        public void AddData(byte[] data)
        {
            lock (this)
            {
                if (data.Length > 0)
                {
                    currentData = MergeUsingArrayCopyWithNewArray(currentData, data);
                    ProcessReceivedData();
                }
            }
        }

        protected void OnDataReceived(ByteDataMessageEventArgs e)
        {
            ByteDataMessageEventHandler handler = this.DataReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private byte[] MergeUsingArrayCopyWithNewArray(byte[] firstArray, byte[] secondArray)
        {
            byte[] combinedArray = new byte[firstArray.Length + secondArray.Length];
            Array.Copy(firstArray, combinedArray, firstArray.Length);
            Array.Copy(secondArray, 0, combinedArray, firstArray.Length, secondArray.Length);
            return combinedArray;
        }

        private void ProcessReceivedData()
        {
            int messageLength = GetMessageLengthFromHeader();
            int messagePlusHeaderLength = messageLength + 4;
            int totalDataLength = this.currentData.Length;
            if ((messageLength > 0) && (messagePlusHeaderLength <= totalDataLength))
            {
                byte[] messageData = ExtractMessageDataFromCurrentData(messageLength);
                this.currentData = ExtractRemainingDataFromCurrentData(totalDataLength, messagePlusHeaderLength);
                this.OnDataReceived(new ByteDataMessageEventArgs() { MessageData = messageData });
                if (this.currentData.Length > 0)
                {
                    ProcessReceivedData();
                }
            }
        }

        private byte[] ExtractMessageDataFromCurrentData(int messageLength)
        {

            byte[] messageData = new byte[messageLength];
            Array.Copy(
                this.currentData,
                4,
                messageData,
                0,
                messageLength);
            return messageData;
        }

        private byte[] ExtractRemainingDataFromCurrentData(int totalDataLength, int messagePlusHeaderLength)
        {
            byte[] remainingData;
            int remainingDataLength = totalDataLength - messagePlusHeaderLength;
            if (remainingDataLength > 0)
            {

                remainingData = new byte[remainingDataLength];
                Array.Copy(
                    this.currentData,
                    messagePlusHeaderLength,
                    remainingData,
                    0,
                    remainingDataLength);
            }
            else
            {
                remainingData = new byte[0];
            }
            return remainingData;
        }

        private int GetMessageLengthFromHeader()
        {
            int result = 0;
            if (this.currentData.Length > 4)
            {
                var lengthData = new byte[4];
                Array.Copy(this.currentData, lengthData, 4);
                if (!BitConverter.IsLittleEndian)
                {
                    Array.Reverse<byte>(lengthData);
                }
                result = BitConverter.ToInt32(lengthData, 0);
            }
            return result;
        }
    }
}