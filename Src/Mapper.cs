
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace deceptive_enigma;

/// <summary>
/// The mapper is what is used to Encrypt and Decrypt messages. It plays the role of the connection of the keyboard of enigma 
/// machine to the rolling cylinders/drums and then to the output words.
/// The Mapper produces a recursive sequence of Words for some chosen initial settings
/// and an initial password phrase,... just like a Password produces a recursive sequence of integers.
/// </summary>
public class Mapper 
{
    /// <summary>
    /// A dictionary containing lists of Words categorised by part of speech.
    /// </summary>
    public AppDict dict;

    /// <summary>
    /// The wordCount keeps track of the number of words during encryption/decryption to help produce the next word
    /// in the recursive sequence.
    /// </summary>
    private int wordCount;

    /// <summary>
    /// A temporary handle for the last Word in the sequence to help produce the next.
    /// </summary>
    private Word? currentWord;//temporarily stores to words 

    /// <summary>
    /// Stores the characters that are valid for the original message.
    /// </summary>
    private readonly List<char> input_chars;//stores valid input chars

    /// <summary>
    /// Stores the rules of parts of speech, i.e. which part of speech can be followed by which other part of speech.
    /// </summary>
    private readonly Dictionary<string, List<string> > partsofspeechRules ; //stores the rules - which type can follow which

    private readonly ConfigProvider config;

    public Mapper(ConfigProvider config)
    {
        this.config = config;
        input_chars = config.AllowedChars;
        dict = new(config.Dictionary);
        partsofspeechRules = config.PartsofspeechRules;

    }

    /// <summary>
    /// Resets the Mapper to begin a fresh encryption. That is, in terms of the recursive sequence, it start the sequency again before
    /// the first term.
    /// </summary>
    public void Rewind()
    {
        List<string> keys = dict.Keys.ToList();
        keys.Sort();

        currentWord = dict[keys.First()][0];
        wordCount = 0;
    }


    /// <summary>
    /// Provides a sequence of words, using two strings of valid letters (original message and password)
    /// </summary>
    /// <param name="inMessage">The string of letters to encrypt.</param>
    /// <param name="password">The string of letters that acts as password.</param>
    /// <returns>A concatenated sequence of Words, serving as the encrypted message.</returns>
    public string Encrypt(string inMessage, string password)
    {
        Rewind();
        Message outMessage = new(config);

        Password p = new(password, config);

        for (int i=0; i<inMessage.Length; i++)
        {
            int charIndex = input_chars.IndexOf(inMessage[i]);
            if (charIndex ==-1) Console.WriteLine("cpp: errors::disallowed_chars(inMessage[i]);");
            int map_sum = p.NextAddend() + charIndex;
            //Console.WriteLine("mapsum:" + map_sum + ", charindex:" + charIndex);
            Word nextWord = GetNextWord(map_sum);

            outMessage += nextWord;
            currentWord = nextWord;
        }
        Console.WriteLine("------encryption complete------");
        return outMessage.ToString();
    }

    /// <summary>
    /// Does the reverse of Encrypt() method. Attempts to use a concatenate sequency of words, to recover an original message string of letters
    /// based on the presumably correct password string.
    /// </summary>
    /// <param name="encryptedString">A concatenated sequence of Words to decrypt.</param>
    /// <param name="password">The password to use for attempting decryption.</param>
    /// <returns>Recovered original message based on the password, whose correctness depends on the password.</returns>
    public string Decrypt(string encryptedString, string password)
    {
        Rewind();
        Password p = new (password, config);
        Message message = new (encryptedString, dict, config);
        StringBuilder original = new("");
        for (int i=0; i<message.Count; i++)
        {
            int nextIndex = GetNextIndex(message[i]);
            int map_diff = nextIndex - p.NextAddend();
            Console.WriteLine("map_diff:"+map_diff+", nextIndex: "+nextIndex);
            original.Append(input_chars[map_diff]);
            currentWord = message[i];
        }
        return original.ToString();
    }


    /// <summary>
    /// What should be the part of speech of the next word
    /// </summary>
    /// <returns>returns the next possible part of speech which could follow currentWord</returns>
    private List<string> NextWordTypes() {
        //
        if (currentWord == null) throw new NullReferenceException("Cannot calculate next word in mapping as there is no current word to go off of");
        List<string> followers = partsofspeechRules[currentWord.Partofspeech];
        int shiftIndex = wordCount %  followers.Count;
        List<string> types = new ();
        // Console.WriteLine("followers:");
        for (int i=0; i<followers.Count; i++)
        {
            int j = i + shiftIndex;
            while (j >= followers.Count) j -= followers.Count;
            types.Add(followers[j]);
            // Console.WriteLine(followers[j]);
        }
        return types;
    }

    /// <summary>
    /// The next word based on current word
    /// </summary>
    /// <param name="index">Sum of password (addend) and index of a character in message</param>
    /// <returns>Next word</returns>
    private Word GetNextWord(int index) {
        //returns the next word based on an index (sum of password and char in message)
        wordCount++;
        List<string> followers = NextWordTypes();
        int grandIndexCount = 0;
        for (int i=0; i< followers.Count; i++)
        {
            string followerType = followers[i];
            List<Word> wordsindictionary = dict[followerType];
            for (int j=0; j< wordsindictionary.Count; j++)
            {
                if (index == grandIndexCount) return wordsindictionary[j];
                grandIndexCount++;
            }
        }
        Console.WriteLine("cpp: Unexpected: Could not build up encrypted sequence of words.");
        return new Word("null","null");
    }

    /// <summary>
    /// This is used for decryption.
    /// </summary>
    /// <param name="word">diff of password and index of word</param>
    /// <returns>Returns an index based on a given Word which corresponds to the difference between
    /// password addend an index.</returns>
    private int GetNextIndex(Word word)
    {
        // based on the word (diff of password and index of word)
        wordCount++;
        List<string> followers = NextWordTypes();
        int grandIndexCount = 0;
        for (int i=0; i<followers.Count; i++)
        {
            string followerType = followers[i];
            List<Word> wordsindictionary = dict[followerType];
            for (int j=0; j<wordsindictionary.Count; j++)
            {
                if (wordsindictionary[j].Value == word.Value) {
                    return grandIndexCount;
                }
                grandIndexCount++;
            }
        }
        return -1;
    }


}
