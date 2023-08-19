
namespace deceptive_enigma;
public record Word(string Value, string Partofspeech) {


    public string ToString(bool beginCapital)
    {
        string s = this.Value;
        if (string.IsNullOrEmpty(s)) return s;
        else if (beginCapital) s = char.ToUpper(s[0]) + s[1..];
        return s;
    }

}