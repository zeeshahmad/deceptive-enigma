
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;

namespace deceptive_enigma;
public class ConfigProvider: IConfigProvider
{
    static readonly Deserializer yamlDeserializer = new ();

    public ConfigProvider(string pathToRes = "")
    {
        _allowed_chars = FetchAllowedChars($@"{pathToRes}res/inputchars.txt");
        _dictionary = ReadListsOfStrings($@"{pathToRes}res/dictionary.yaml");
        _partsofspeech_rules = ReadListsOfStrings($@"{pathToRes}res/rules.yaml");
        _period_partsofspeech = ReadListOfStrings($@"{pathToRes}res/periodpartsofspeech.txt");
        _connectors = ReadListOfStrings($@"{pathToRes}res/connectors.txt");
    }

    private readonly List<char> _allowed_chars;
    /// <summary>
    /// The list of valid characters that are allowed in a password's phrase.
    /// </summary>
    public List<char> AllowedChars
    {
        get { return _allowed_chars; }   
    }

    private readonly Dictionary<string, List<string>> _dictionary;
    /// <summary>
    /// The dictionary of words categorised by parts of speech.
    /// </summary>
    public Dictionary<string, List<string>> Dictionary
    {
        get { return _dictionary; }   
    }

    private readonly Dictionary<string, List<string>> _partsofspeech_rules;
    /// <summary>
    /// The rules of parts of speech. Each List (value) in this Dictionary is parts of speech 
    /// that can be followed by the part of speech in corresponding key.
    /// </summary>
    public Dictionary<string, List<string>> PartsofspeechRules
    {
        get { return _partsofspeech_rules; }   
    }

    private readonly List<string> _period_partsofspeech;
    /// <summary>
    /// List of parts of speech that can be preceeded by a period in a sentence.
    /// </summary>
    public List<string> PeriodPartsofspeech
    {
        get { return _period_partsofspeech; }   
    }

    private readonly List<string> _connectors;
    /// <summary>
    /// List of connectors sandwiched between the parts of speech that can be validly
    ///  found between in an encrypted message.
    /// </summary>
    public List<string> Connectors
    {
        get { return _connectors; }   
    }

    /// <summary>
    /// Reads the characters that are to be allowed in a password's phrase.
    /// </summary>
    /// <param name="filename">The path to a text file containing allowed characters each separated with a new line / carriage return.</param>
    /// <returns>List of characters read from the file.</returns>
    private static List<char> FetchAllowedChars(string filename)
    {
        var allowed_chars = new List<char>();

        using StreamReader reader = new(filename);
        while (!reader.EndOfStream)
        {
            string? line = reader.ReadLine();
            if (!string.IsNullOrEmpty(line))
            {
                allowed_chars.Add(line[0]);
            }
        } 

        return allowed_chars;
    }


    /// <summary>
    /// Reads in a yaml file containing keyed lists of strings and produces a corresponding Dictionary.
    /// </summary>
    /// <param name="filename">The path to a yaml file. </param>
    /// <returns>Dictionary with keys corresponding to those in yaml and values as Lists</returns>
    private static Dictionary<string, List<string>> ReadListsOfStrings(string filename)
    {
        using StreamReader reader = new(filename);
        string fileContent = reader.ReadToEnd();

        var dict = yamlDeserializer.Deserialize<Dictionary< string, List<string> >>(fileContent);

        return dict;
    }

    /// <summary>
    /// Reads in a txt file containing a list of strings delimited by newline.
    /// </summary>
    /// <param name="filename">Path to a plain text file containing strings delimited with newline.</param>
    /// <returns>List of strings found</returns>
    private static List<string> ReadListOfStrings(string filename)
    {
        var strings = new List<string>();

        using StreamReader reader = new(filename);
        while (!reader.EndOfStream)
        {
            string? line = reader.ReadLine();
            if (!string.IsNullOrEmpty(line))
            {
                strings.Add(line);
            }
        } 

        return strings;
    }


}