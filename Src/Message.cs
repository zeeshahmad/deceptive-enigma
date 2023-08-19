using System;
using System.Collections.Generic;


namespace deceptive_enigma;

public class Message : List<Word>
{
    public static readonly int periodThreshold = 3;
    private static readonly string period = ".";
    private readonly List<string> periodTypes;
    private readonly List<string[]> connectorRules;
    

    public Message(ConfigProvider config) {
        periodTypes = config.PeriodPartsofspeech;
        connectorRules = ParseConnectorRules(config.Connectors);
    }

    public Message(string stringToParse, AppDict dict, ConfigProvider config) {
        periodTypes = config.PeriodPartsofspeech;
        connectorRules = ParseConnectorRules(config.Connectors);

        string[] words = stringToParse.Split(" ");

        foreach(string word in words)
        {
            string wordString = word;

            //decapitalise first letters
            wordString = char.ToLower(wordString[0])+wordString[1..];

            //remove any periods
            wordString = wordString.Replace(period, string.Empty);

            //remove connectors
            if (IsConnector(wordString)) continue;
            
            Word? wordFound = dict.FindWord(wordString) ?? throw new ArgumentException($"The word {word} in the encrypted message could not be found in the dictionary.");
            Add(wordFound);
        }
    }

    public static Message operator +(Message left, Word word)
    {
        left.Add(word);
        return left;
    }

    public override string ToString() {
        string str = "";
        string space;
        string prevWordType = "";
        bool capitaliseNext = true;
        bool doingPeriod;
        string connector;
        int wordCount = 0;

        for (int i=0; i < this.Count; i++)
        {
            space = " ";
            doingPeriod = DoPeriod(this[i].Partofspeech, wordCount);
            connector = MatchConnector(prevWordType, this[i].Partofspeech);

            if (connector != "")
            {
                connector += space;
                if (capitaliseNext) connector = char.ToUpper(connector[0])+connector[1..];

            }

            if (i>=Count-1) space = period;
            else if (doingPeriod) {
                wordCount = 0;
                space = period + " ";
            }
            str+= connector + this[i].ToString(capitaliseNext && connector=="") + space;

            capitaliseNext = doingPeriod;
            prevWordType = this[i].Partofspeech;
            wordCount++;
        }

        return str;
    }

    private static List<string[]> ParseConnectorRules(List<string> rawConnectors)
    {
        return rawConnectors.ConvertAll((ruleline) => ruleline.Split(" "));
    }


    private bool DoPeriod(string t, int wordCount)
    {
        return (periodTypes.IndexOf(t) != -1) && (wordCount >= periodThreshold);
    }

    private string MatchConnector(string prevType, string currType)
    {
        foreach (var rule in connectorRules)
        {
            if (rule[0]==prevType && rule[2]==currType)
                return rule[1];
        }
        return "";
    }

    private bool IsConnector(string word)
    {
        foreach (var rule in connectorRules)
        {
            if (rule[1] == word) return true;
        }
        return false;
    }
}