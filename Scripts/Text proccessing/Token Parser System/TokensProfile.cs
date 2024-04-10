using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tokens profile")]
public class TokensProfile : ScriptableObject
{
    public TokenType[] tokens =
    {
        new TokenType
        {
            ID = "InputAct",
            Arguments = new TokenArgumentType[]
            {
                new TokenArgumentType
                {
                    ID = "Action",
                    Type = Types.String
                },
                new TokenArgumentType
                {
                    ID = "keyStatus",
                    Type = Types.String
                }
            }
        },
        new TokenType
        {
            ID = "Img",
            Arguments = new TokenArgumentType[]
            {
                new TokenArgumentType
                {
                    ID = "spriteAsset",
                    Type = Types.String
                },
                new TokenArgumentType
                {
                    ID = "posX",
                    Type = Types.Float
                },
                new TokenArgumentType
                {
                    ID = "posY",
                    Type = Types.Float
                },
                new TokenArgumentType
                {
                    ID = "Width",
                    Type = Types.Float
                },
                new TokenArgumentType
                {
                    ID = "Height",
                    Type = Types.Float
                },
                new TokenArgumentType
                {
                    ID = "spaceBetweenText",
                    Type = Types.Int
                },
                new TokenArgumentType
                {
                    ID = "ObjectToAttachTo",
                    Type= Types.String
                },
                new TokenArgumentType
                {
                    ID = "ParentComponent",
                    Type= Types.String
                },
                new TokenArgumentType
                {
                    ID = "EventName",
                    Type= Types.String
                }
            }
        },
        new TokenType
        {
            ID = "InputImg",
            Arguments = new TokenArgumentType[]
            {
                new TokenArgumentType
                {
                    ID = "action",
                    Type = Types.String
                },
                new TokenArgumentType
                {
                    ID = "keyimgindex",
                    Type = Types.Int
                },
                new TokenArgumentType
                {
                    ID = "posX",
                    Type = Types.Float
                },
                new TokenArgumentType
                {
                    ID = "posY",
                    Type = Types.Float
                },
                new TokenArgumentType
                {
                    ID = "Width",
                    Type = Types.Float
                },
                new TokenArgumentType
                {
                    ID = "Height",
                    Type = Types.Float
                },
                new TokenArgumentType
                {
                    ID = "spaceBetweenText",
                    Type = Types.Int
                },
                new TokenArgumentType
                {
                    ID = "ObjectToAttachTo",
                    Type= Types.String
                },
                new TokenArgumentType
                {
                    ID = "ParentComponent",
                    Type= Types.String
                },
                new TokenArgumentType
                {
                    ID = "EventName",
                    Type= Types.String
                }
            }
        }
    };
}
