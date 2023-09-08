
using System.Text.RegularExpressions;

namespace deceptive_enigma;

public class PasswordTests: IClassFixture<ConfigFixture>
{

    private readonly IConfigProvider config;

    public PasswordTests(ConfigFixture fixture)
    {
        config = fixture.MockConfigProvider.Object;
    }

    [Fact]
    public void Password_Initialization_Normal()
    {
        string initialPhrase = "InitialPhrase123";
        Password password = new (initialPhrase, config);

        Assert.NotNull(password);
    }

    [Fact]
    public void Password_Initialization_Short_Phrase()
    {
        string shortPhrase = "xa";

        Assert.Throws<ArgumentException>(() => new Password(shortPhrase, config));
    }

    [Fact]
    public void Password_Initialization_Invalid_Chars()
    {
        string invalidPhrase = "£";

        Assert.Throws<ArgumentException>(() =>  new Password(invalidPhrase, config));
    }

    [Fact]
    public void NextAddend_Return_Integer()
    {
        Password password = new ("InitialPhrase", config);
        
        int nextAddend = password.NextAddend();

        Assert.IsType<int>(nextAddend);
    }

    [Fact]
    public void Password_ToString()
    {
        Password password = new ("InitialPhrase", config);

        string result = password.ToString();

        Assert.Contains("The password is InitialPhrase after 0 mutations.", result);
    }

    [Fact]
    public void Password_Phrase_Different_When_NextAddend()
    {
        string initialPhrase = "InitialPhrase";
        Password password = new (initialPhrase, config);

        for (int i=0; i< 5; i++)
        {
            password.NextAddend();
            string result = password.ToString();
            Match match = Regex.Match(result, @"The password is (.{3,}) after (\d+) mutations.");
            string phraseAfterMutation = match.Groups[1].Value;
            Assert.NotEqual(phraseAfterMutation, initialPhrase);
        }
    }

    [Fact]
    public void Password_Mutate_Count_When_NextAddend()
    {
        Password password = new ("InitialPhrase", config);
        int numAddend = 10;
        for (int i=0; i< numAddend; i++)
        {
            password.NextAddend();
        }
        string result = password.ToString();
        Match match = Regex.Match(result, @"The password is (.{3,}) after (\d+) mutations.");
        int numMutations = int.Parse(match.Groups[2].Value);
        Assert.Equal(numMutations, numAddend);
    }

    // Add more tests to cover edge cases and different scenarios

}
