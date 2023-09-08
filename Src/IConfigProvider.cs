
using System.Collections.Generic;

namespace deceptive_enigma;
public interface IConfigProvider
{

    /// <summary>
    /// The list of valid characters that are allowed in a password's phrase.
    /// </summary>
    public List<char> AllowedChars
    {
        get ;
    }

    /// <summary>
    /// The dictionary of words categorised by parts of speech.
    /// </summary>
    public Dictionary<string, List<string>> Dictionary
    {
        get ;
    }

    /// <summary>
    /// The rules of parts of speech. Each List (value) in this Dictionary is parts of speech 
    /// that can be followed by the part of speech in corresponding key.
    /// </summary>
    public Dictionary<string, List<string>> PartsofspeechRules
    {
        get ;  
    }

    /// <summary>
    /// List of parts of speech that can be preceeded by a period in a sentence.
    /// </summary>
    public List<string> PeriodPartsofspeech
    {
        get ;  
    }

    /// <summary>
    /// List of connectors sandwiched between the parts of speech that can be validly
    ///  found between in an encrypted message.
    /// </summary>
    public List<string> Connectors
    {
        get ;
    }

}