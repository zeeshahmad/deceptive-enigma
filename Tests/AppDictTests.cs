
namespace deceptive_enigma;

public class AppDictTests
{
    [Fact]
    public void ConsolidateRawDict_ConsolidatesCorrectly()
    {
        var rawDict = new Dictionary<string, List<string>>
        {
            { "noun", new List<string> { "apple", "banana" } },
            { "verb", new List<string> { "run", "jump" } }
        };

        var appDict = new AppDict(rawDict);

        Assert.True(appDict.ContainsKey("noun"));
        Assert.True(appDict.ContainsKey("verb"));
        Assert.Equal(2, appDict["noun"].Count);
        Assert.Equal(2, appDict["verb"].Count);
    }

    [Fact]
    public void FindWord_WordExists_ReturnsCorrectWord()
    {
        var appDict = new AppDict(new Dictionary<string, List<string>>
        {
            { "noun", new List<string> { "apple", "banana" } },
            { "verb", new List<string> { "run", "jump" } }
        });

        Word result = appDict.FindWord("run");

        Assert.NotNull(result);
        Assert.Equal("run", result.Value);
        Assert.Equal("verb", result.Partofspeech);
    }

    [Fact]
    public void FindWord_WordDoesNotExist_ReturnsNull()
    {
        var appDict = new AppDict(new Dictionary<string, List<string>>
        {
            { "noun", new List<string> { "apple", "banana" } },
            { "verb", new List<string> { "run", "jump" } }
        });

        Word result = appDict.FindWord("orange");

        Assert.Null(result);
    }
}
