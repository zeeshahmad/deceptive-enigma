using System.Collections.Generic;
using System.Linq;

namespace deceptive_enigma;
public class AppDict : Dictionary<string, List<Word> >
{

    public AppDict(Dictionary<string, List<string>> rawDict)
    {
        ConsolidateRawDict(rawDict);
    }
    

    public void ConsolidateRawDict(Dictionary<string, List<string>> rawDict)
    {
        foreach (var kvp in rawDict){
            this[kvp.Key] = kvp.Value.Select(
                wordString => new Word(Value: wordString, Partofspeech: kvp.Key)
            ).ToList();
        }
    }


    public Word FindWord(string matchValue)
    {
        foreach (var type in this)
        {
            foreach (var wrd in type.Value) {
                if (wrd.Value == matchValue) {
                    return wrd;
                }
            }
        }
        return null!;
    }
}