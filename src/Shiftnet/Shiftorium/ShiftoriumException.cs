using System;

namespace Shiftnet.Shiftorium
{
    public class ShiftoriumException : Exception
    {
        public ShiftoriumException(ShiftoriumResponse response, Exception innerException) : base(
            "A request to the Shiftorium API has failed.", innerException)
        {
            this.Response = response;
        }
        
        public ShiftoriumResponse Response { get; }
    }
}