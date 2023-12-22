using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class TokenParser : Instancable<TokenParser>
{
    public TokensProfile profile;

    /// <summary>
    /// looks for a patern that looks like this <[-{ID: argument:value, argument2:value2 }-]>
    /// </summary>
    private const string RegexPattern = @"<\[-{(?<ID>\s*\w+?)\s*:\s*(?<args>(?:\s*(?<arg>\w+?)\s*:\s*(?<value>[^,\n]+?)(?:,\s*)?)*?)\s*}-\]>";

    /// <summary>
    /// Finds the created token instances from the profile in the given input text
    /// </summary>
    /// <param name="toParse">The string to parse</param>
    /// <returns>A list of found tokens</returns>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public List<Token> Parse(string toParse)
    {
        Regex regex = new Regex(RegexPattern);
        MatchCollection collection = regex.Matches(toParse);

        List<Token> toReturn = new List<Token>();

        foreach (Match match in collection)
        {
            string identifier = match.Groups["ID"].Value;
            identifier = identifier.Replace(" ", "");

            foreach (var tokens in profile.tokens)
            {
                if (tokens.ID.Equals(identifier))
                {
                    Token token = new Token { ID = identifier, Arguments = new List<TokenArgument>() };

                    for (int i = 0; i < match.Groups["arg"].Captures.Count; i++)
                    {
                        string argName = match.Groups["arg"].Captures[i].Value;
                        string argValue = match.Groups["value"].Captures[i].Value;

                        argName = argName.Replace(" ", "");
                        foreach (TokenArgumentType argument in tokens.Arguments)
                        {
                            if (argument.ID.Equals(argName))
                            {
                                token.Arguments.Add(new TokenArgument()
                                {
                                    ID = argName,
                                    value = argValue,
                                    Type = argument.Type
                                });
                            }
                        }
                    }
                    toReturn.Add(token);
                }
            }
        }

        return toReturn;
    }

    /// <summary>
    /// Removes the token instances from the input text
    /// </summary>
    /// <param name="toParse">The string to remove tokens from</param>
    /// <returns>A new text with the token removed</returns>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public string RemoveTokens(string toParse)
    {
        return Regex.Replace(toParse, RegexPattern, "");
    }

    /// <summary>
    /// Splits the input text every time a token instance is found
    /// </summary>
    /// <param name="toParse">The string to split</param>
    /// <returns>A array of strings based on the split text</returns>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public string[] SplitFromTokens(string toParse)
    {
        var text = Regex.Replace(toParse, RegexPattern, "<!$SPLIT$!>");
        return text.Split("<!$SPLIT$!>");
    }

    /// <summary>
    /// Replaces all tokens inside of a text with what ever is given to it in an array
    /// </summary>
    /// <param name="input">the text</param>
    /// <param name="replacements">an array of replacements</param>
    /// <returns>The input text with its tokens replaced with the replacements in order</returns>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public string ReplaceTokens(string input, IEnumerable<string> replacements)
    {
        try
        {
            // Create a new Regex object with the specified pattern
            Regex regex = new Regex(RegexPattern);

            // Get all matches in the input text
            MatchCollection matches = regex.Matches(input);

            // Convert the replacements to an array for easy access
            string[] replacementArray = replacements.ToArray();

            // Replace each match with the corresponding replacement
            int i = 0;
            string result = regex.Replace(input, match => {
                string replacement = replacementArray[i];
                i = (i + 1) % replacementArray.Length;
                return replacement;
            });

            return result;
        }
        catch (ArgumentException ex)
        {
            // Handle any invalid regular expression patterns
            Console.WriteLine("Error: " + ex.Message);
            return input; // Return the original text if an error occurs
        }
    }

    /// <summary>
    /// Get's the string index of each token inside of the text
    /// </summary>
    /// <param name="toParse">The text to find indexes for</param>
    /// <returns>A list of numeric index's</returns>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public List<int> GetTokenIndexes(string toParse)
    {
        Regex regex = new Regex(RegexPattern);
        MatchCollection collection = regex.Matches(toParse);

        List<int> indexes = new List<int>();

        foreach (Match match in collection)
        {
            indexes.Add(match.Index);
        }

        return indexes;
    }
}
