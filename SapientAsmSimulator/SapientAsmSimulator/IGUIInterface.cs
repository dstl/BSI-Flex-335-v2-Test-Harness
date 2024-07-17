// File:              $Workfile: IGUIInterface.cs$
// <copyright file="IGUIInterface.cs" >
// Crown-owned copyright, 2021-2024
// See Release/Supply Conditions
// </copyright>

namespace SapientASMsimulator
{
    /// <summary>
    /// Interface to user interface
    /// </summary>
    public interface IGUIInterface
    {
        /// <summary>
        /// method to Update Output Window that can be called from outside the UI thread
        /// </summary>
        /// <param name="message">message to update with</param>
        void UpdateOutputText(string message);

        /// <summary>
        /// Update ASM ID text
        /// </summary>
        /// <param name="text">ASM ID text</param>
        void UpdateASMText(string text);
    }
}
