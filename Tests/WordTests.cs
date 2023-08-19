
namespace deceptive_enigma;

public class WordTests
{
    [Fact]
    public void ToString_BeginCapitalTrue()
    {
        var word = new Word("apple", "noun");
        string result = word.ToString(beginCapital: true);

        Assert.Equal("Apple", result);
    }

    [Fact]
    public void ToString_BeginCapitalFalse()
    {
        var word = new Word("banana", "noun");
        string result = word.ToString(beginCapital: false);

        Assert.Equal("banana", result);
    }

    [Fact]
    public void ToString_EmptyWord()
    {
        var word = new Word("", "noun");
        string result = word.ToString(beginCapital: true);

        Assert.Equal("", result);
    }

}
