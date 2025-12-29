using System.Collections.Generic;

namespace Quintessential;
public class Payloads
{
    public class Payload
    {
        public string Address { get; }
        public string Data { get; }

        public Payload(string address, string data)
        {
            Address = address;
            Data = data;
        }
    }

    //public List<Payload> PuzzleInitialization = new();
    public List<Payload> SolutionInitialization = new();
}
