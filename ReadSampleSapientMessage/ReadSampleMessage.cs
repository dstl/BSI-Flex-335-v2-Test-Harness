// Crown-owned copyright, 2021-2024
using Microsoft.Extensions.Hosting;
using Sapient.Data;
using System.Reflection;
using System.Text;

namespace SAPIENT.ReadSampleMessage
{
    public class ReadSampleMessage
    {
        public SapientMessage? ReadSampleMessageFromFile(SapientMessage.ContentOneofCase contentOneofCase, string additionText, out string error)
        {
            error = string.Empty;
            SapientMessage? result = null;
            try
            {
                string? executablePath = Path.GetDirectoryName(Path.GetFullPath(Assembly.GetEntryAssembly()?.Location));
                string filePath = this.GetFilePath(contentOneofCase, executablePath, additionText);
                string fileContent  = File.ReadAllText(filePath);
                if (!string.IsNullOrEmpty(fileContent)) 
                {
                    result = SapientMessage.Parser.ParseJson(fileContent);
                }
            }
            catch (Exception e)
            {
                error = e.ToString();
            }
            return result;
        }

        private string GetFilePath(SapientMessage.ContentOneofCase contentOneofCase, string? executablePath, string additionText)
        {
            string filePath = string.Empty;
            if (executablePath != null)
            {
                string fileName = string.Empty;
                switch (contentOneofCase)
                {
                    case SapientMessage.ContentOneofCase.None:
                        break;
                    case SapientMessage.ContentOneofCase.Registration:
                        fileName = "Default.Registration.json";
                        break;
                    case SapientMessage.ContentOneofCase.RegistrationAck:
                        fileName = "Default.RegistrationAck.json";
                        break;
                    case SapientMessage.ContentOneofCase.StatusReport:
                        fileName = "Default.StatusReport.json";
                        break;
                    case SapientMessage.ContentOneofCase.DetectionReport:
                        fileName = "Default.DetectionReport.json";
                        break;
                    case SapientMessage.ContentOneofCase.Task:
                        fileName = "Default.Task.json";
                        break;
                    case SapientMessage.ContentOneofCase.TaskAck:
                        fileName = "Default.TaskAck.json";
                        break;
                    case SapientMessage.ContentOneofCase.Alert:
                        fileName = "Default.Alert.json";
                        break;
                    case SapientMessage.ContentOneofCase.AlertAck:
                        fileName = "Default.AlertAck.json";
                        break;
                    case SapientMessage.ContentOneofCase.Error:
                        fileName = "Default.Error.json";
                        break;
                    default:
                        break;
                }
                if ((!string.IsNullOrEmpty(fileName)) && (!string.IsNullOrEmpty(executablePath)))
                {
                    if (!string.IsNullOrEmpty(additionText))
                    {
                        StringBuilder builder = new StringBuilder(fileName);
                        builder.Replace(".json", $"{additionText}.json");
                        fileName = builder.ToString();  
                    }
                    filePath = Path.Combine(executablePath, fileName);
                }
            }
            return filePath;
        }
    }
}