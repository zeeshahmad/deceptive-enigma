
namespace deceptive_enigma;

public class MessageTests :IClassFixture<ConfigFixture>
{

    private readonly IConfigProvider config;

    public MessageTests(ConfigFixture fixture)
    {
        config = fixture.MockConfigProvider.Object;
    }

    [Fact]
    public void Normal_Initialization_Without_Params()
    {
        Message message = new(config);

        Assert.NotNull(message);
    }

    [Fact]
    public void Normal_Initialization_With_Params()
    {
        AppDict dict = new(config.Dictionary);
        string[] keys = dict.Keys.ToArray();
        string firstPos = keys[0];
        string firstWord = dict[firstPos][0].Value;
        
        Message message = new(firstWord, dict, config);

        Assert.NotNull(message);
    }

    [Fact]
    public void Initialization_With_Nonexistent_Word()
    {
        AppDict dict = new(config.Dictionary);
        string incorrectWord = "xxx xxx";

        Assert.Throws<ArgumentException>(() => new Message(incorrectWord, dict, config));
    }

    [Fact]
    public void Initialization_RemovesPeriods()
    {
        AppDict dict = new(config.Dictionary);
        dict["NOUN"].Add(new Word("testword","pos"));
        Message message = new("testword......", dict, config);
        string result = message.ToString();
        int periodCount = result.Count(c => c=='.');

        Assert.Equal(1,periodCount);
    }

    [Fact]
    public void OperatorPlus_AddsWordsToMessage()
    {
        Message message = new (config);
        Word word = new ("testword", "NOUN");
        Word anotherword = new ("anotherword", "VERB");
        
        Message result = message + word;
        result += anotherword;

        Assert.Contains(word, result);
        Assert.Contains(anotherword, result);
    }

    [Fact]
    public void Message_ToString()
    {
        Message m = new(config);
        AppDict dict = new(config.Dictionary);
        string firstPos = config.PartsofspeechRules.Keys.ToArray()[0];
        Word word = dict[firstPos][0];
        int numWords = 10;
        for (int i = 0; i < numWords; i++)
        {
            m += word;
            string partofspeech = config.PartsofspeechRules[word.Partofspeech][0];
            word = dict[partofspeech][0];
        }
        string period = ".";

        string result = m.ToString();
        int spaceCount = result.Count(c => c==' ');
        int periodCount = result.Count(c => c==period[0]);

        foreach (Word testword in m)
        {
            Assert.Contains(testword.Value, result, StringComparison.OrdinalIgnoreCase);
        }
        Assert.Contains(period,result);
        Assert.True(spaceCount >= numWords-1);
        Assert.True(periodCount<=Math.Ceiling((float)numWords/Message.periodThreshold));
        Assert.True(periodCount>=numWords/Message.periodThreshold);
    }


}
