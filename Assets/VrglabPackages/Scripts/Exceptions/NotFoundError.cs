using System;
using UnityEngine;

public class NotFoundError : Exception
{

    public NotFoundError() : base("Requested data not found") {  }
    public NotFoundError(string message) : base(message) {  }
}
