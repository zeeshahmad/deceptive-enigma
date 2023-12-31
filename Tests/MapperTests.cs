

namespace deceptive_enigma;

public class MapperTests: IClassFixture<ConfigFixture>
{
    private readonly IConfigProvider config;

    public MapperTests(ConfigFixture fixture) {
        config = fixture.MockConfigProvider.Object;
    }

    [Fact]
    public void Normal_Initialization()
    {
        Mapper mapper = new(config);
        AppDict dict = new(config.Dictionary);

        Assert.NotNull(mapper);
        Assert.Equal(dict, mapper.dict);
    }


    [Fact]
    public void Rewind()
    {
        Mapper mapper = new(config);
        AppDict dict = new(config.Dictionary);

        List<string> results = new();

        for (int i =0; i<10; i++)
        {
            results.Add(mapper.Encrypt("test in message", "testpass"));
            mapper.Rewind();
        }

        Assert.True(results.All(s => s == results[0])); // we should definitively get unchanged encrypted string
    }

    [Fact]
    public void Encrypt()
    {
        Mapper mapper = new(config);

        string encrypted1 = mapper.Encrypt("test one", "pass1");
        string encrypted2 = mapper.Encrypt("test2", "pass2");
        string encrypted3 = mapper.Encrypt("test2", "pass2");

        Assert.Equal(encrypted3, encrypted2); // mapper was rewinded
        Assert.Equal("Altogether yet happily yet voluntarily yet dreamily yet instantly yet upside-down yet absentmindedly yet awkwardly.", encrypted1);
        Assert.Equal("Altogether yet happily yet generally yet dreamily yet well.", encrypted2);
    }

    [Fact]
    public void Decrypt()
    {
        Mapper mapper = new(config);

        string decrypted1 = mapper.Decrypt("Altogether yet happily yet voluntarily yet dreamily yet instantly yet upside-down yet absentmindedly yet awkwardly.", "pass1");
        string decrypted2 = mapper.Decrypt("Altogether yet happily yet generally yet dreamily yet well.", "pass2");
        string decrypted3 = mapper.Decrypt("Altogether yet happily yet generally yet dreamily yet well.", "pass2");

        Assert.Equal(decrypted3, decrypted2); // mapper was rewinded
        Assert.Equal("test one", decrypted1);
        Assert.Equal("test2", decrypted2);
    }

    [Fact]
    public void EncryptDecryptRecovery()
    {
        Mapper mapper = new(config);
        string originalMessage = "this is an original message. wow";
        string encrypted = mapper.Encrypt(originalMessage, "some password");
        string decrypted = mapper.Decrypt(encrypted, "some password");

        Assert.Equal(originalMessage, decrypted);
    }

    [Fact]
    public void EncryptedWordsAreFromDictionary()
    {
        Mapper mapper = new(config);
        string originalMessage = "this is an original message. wow";
        string encrypted = mapper.Encrypt(originalMessage, "some password");
        
        List<string> dict = config.Dictionary.Values.SelectMany(list=> list).ToList();
        List<string> conn = config.Connectors.Select(c => c.Split(" ")[1]).ToList();

        string[] encryptedWords = encrypted.Replace(".", string.Empty).Split(" ");
        foreach (string word in encryptedWords) 
        {
            Assert.Contains(word.ToLower(), dict.Concat(conn));
        }
    }

    [Fact]
    public void PartsofspeechRulesFollowed()
    {
        Mapper mapper = new(config);
        string originalMessage = "this is an original message. wow";
        string encrypted = BareWords(
            mapper.Encrypt(originalMessage, "some password"));
        

        string[] words = encrypted.Split(" ");
        
        string prevWord = words[0];
        foreach (string word in words[1..])
        {
            Assert.Contains(PartofspeechOfWord(word), 
                config.PartsofspeechRules[PartofspeechOfWord(prevWord)]);
            prevWord = word;
        }
    }

    private string BareWords(string encrypted)
    {
        encrypted = encrypted.ToLower().Replace(".", string.Empty); //remove periods

        List<string> conn = config.Connectors.Select(c => c.Split(" ")[1]).ToList();
        foreach (string connector in conn) //remove connectors
            encrypted = encrypted.Replace($" {connector}", string.Empty);
        return encrypted;
    }

    private string PartofspeechOfWord(string word)
    {
        AppDict dict = new(config.Dictionary);
        Word word_ = dict.FindWord(word);
        return word_.Partofspeech;
    }

}
