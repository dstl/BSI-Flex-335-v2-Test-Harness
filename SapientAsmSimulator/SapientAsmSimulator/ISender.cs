// File:              $Workfile: ISender.cs$
// <copyright file="ISender.cs" >
// Crown-owned copyright, 2021-2024
// See Release/Supply Conditions
// </copyright>

namespace SapientServices
{
    /// <summary>
    /// Interface for sending messages
    /// </summary>
    public interface ISender
    {
        /// <summary>
        /// Method to read and send from file.
        /// </summary>
        /// <param name="input_filename">Path to XML file to be loaded.</param>
        void ReadAndSendFile(string input_filename);
    }
}
