// Crown-owned copyright, 2021-2024
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NUnit.Framework;
using Sapient.Data;
using SapientServices.Data.Validation;
using System.Diagnostics;
using System.Reflection;

namespace SapientServicesValidator.UnitTests
{
    public class SapientServiceValidatorTests
    {
        [Test]
        public void TestMessagesGood()
        {
            TestMessage(true);
        }

        [Test]
        public void TestMessagesBad() 
        {
            TestMessage(false);
        }

        private void TestMessage(bool good)
        { 
            int count = 0;
            Assert.Multiple(() =>
            {
                string message = string.Empty;
                string subfolder = "SapientServicesValidator.UnitTests";
                var assembly = Assembly.GetExecutingAssembly();
                foreach (var name in assembly.GetManifestResourceNames())
                {
                    if (!name.Contains("Backup"))
                    {
                        if (name.StartsWith(subfolder))
                        {
                            if ((good && name.Contains("True")) || (!good && name.Contains("False")))
                            {
                                count++;
                                Debug.WriteLine("------------------------------------------");
                                Console.WriteLine("------------------------------------------");
                                Debug.WriteLine("Run test for: " + name.Replace(subfolder + ".", string.Empty));
                                Console.WriteLine("Run test for: " + name.Replace(subfolder + ".", string.Empty));
                                using (var stream = assembly.GetManifestResourceStream(name))
                                {
                                    using (var reader = new StreamReader(stream))
                                    {
                                        message = reader.ReadToEnd();
                                        Assert.That(message, Is.Not.Null);
                                        Assert.That(message, Is.Not.Empty);
                                        SapientMessage? parsedMessage = null;
                                        try
                                        {
                                            parsedMessage = SapientMessage.Parser.ParseJson(message);
                                            Assert.That(parsedMessage, Is.Not.Null);
                                        }
                                        catch (Google.Protobuf.InvalidProtocolBufferException e)
                                        {
                                            if (name.Contains("True"))
                                            {
                                                Assert.Fail(e.ToString());
                                            }
                                            else
                                            {
                                                Debug.WriteLine("Result: " + e.Message);
                                                Console.WriteLine("Result: " + e.Message);
                                            }
                                        }
                                        if (parsedMessage != null)
                                        {
                                            var v = new SapientMainMessageValidator();
                                            var a = v.Validate(parsedMessage);
                                            bool t = a.IsValid;
                                            Debug.WriteLine("Is Valid: " + t);
                                            Console.WriteLine("Is Valid: " + t);
                                            Debug.WriteLine("Result: " + a.ToString());
                                            Console.WriteLine("Result: " + a.ToString());
                                            if (name.Contains("True"))
                                            {
                                                Assert.That(a.ToString(), Is.EqualTo(string.Empty));
                                                Assert.That(t, Is.True);
                                            }
                                            else
                                            {
                                                Debug.WriteLine(a.ToString());
                                                Console.WriteLine(a.ToString());
                                                Assert.That(a.ToString(), Is.Not.EqualTo(string.Empty));
                                                Assert.That(t, Is.False);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            });
            Debug.WriteLine($"Test count: {count}.");
            Console.WriteLine($"Test count: {count}.");
        }
    }
}