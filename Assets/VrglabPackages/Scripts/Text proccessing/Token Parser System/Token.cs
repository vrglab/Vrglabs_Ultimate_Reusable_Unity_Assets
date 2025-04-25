using System;
using System.Collections.Generic;

[Serializable]
public class Token
{
    public string ID;
    public List<TokenArgument> Arguments;
}

[Serializable]
public class TokenArgument
{
    public Types Type;
    public string ID;
    public string value;
}


[Serializable]
public class TokenType
{
    public string ID;
    public TokenArgumentType[] Arguments;
}

[Serializable]
public class TokenArgumentType
{
    public Types Type;
    public string ID;
}

public enum Types
{
    Object,
    Boolean,
    Byte,
    Int,
    Single,
    Float,
    String = 18
}