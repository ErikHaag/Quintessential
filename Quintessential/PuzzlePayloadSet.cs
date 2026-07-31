using System.Collections.Generic;

namespace Quintessential;

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

public class PuzzlePayloadSet
{
    //public List<Payload> PuzzleInitialization = new();
    public List<Payload> SolutionInitialization = new();
}
