// Crown-owned copyright, 2021-2024
using Sapient.Data;

namespace SapientServices.Communication
{ 
    public class SapientMessageEventArgs: EventArgs  
    {
        public SapientMessage Message { get; set; }

        public string Error { get; set; }
    }
}
