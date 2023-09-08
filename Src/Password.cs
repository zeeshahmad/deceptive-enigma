using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace deceptive_enigma;

/// <summary>
/// The Password object is akin to the rolling drums in the enigma machine. Its job is to provide 
/// addends in from a recursive sequence based on an initial provided string.
/// </summary>
public class Password
{
    // private static bool initialised = false;//to check whether setallowedchars was called


    /// <summary>
    /// Core string of a Password which is mutated as additives are accessed by user.
    /// </summary>
    private readonly StringBuilder phrase;


    /// <summary>
    /// The number of times the Password has been mutated.
    /// </summary>
    private int mutate_count;

    /// <summary>
    /// Holds predefined character positions in the password string that would be picked in a cyclic manner to serve as the starting index of a password mutation.
    /// </summary>
    private readonly int[] mutate_positions;

    /// <summary>
    /// The index of the character of the password phrase that will be used to provide an additive integer to user.
    /// </summary>
    private int addend_position;//since the password is a finite sequence, it makes sense to store the position from which the additive is retrieved and looping the position over the password

    /// <summary>
    /// Holds the value from CalcLowestCharCode() method.
    /// </summary>
    private int lowestCharCode;

    /// <summary>
    /// The list of valid characters allowed is fetched from config.
    /// </summary>
    private readonly List<char> allowed_chars;

    /// <summary>
    /// Makes a new Password.
    /// </summary>
    /// <param name="phrase">The string of characters serving as the starting point of the unmutated password.
    /// Akin to setting the initial position of the cylinders/drums in enigma.</param>
    public Password(string phrase, IConfigProvider config)
    {
        addend_position = 0;
        this.phrase = new(phrase);
        allowed_chars = config.AllowedChars;

        if (phrase.Length<3) {
            throw new ArgumentException("The password phrase must be atleast 3 characters.");
        } 

        VerifyCharsAllowed(phrase);

        mutate_count = 0;
        mutate_positions = new int[] {phrase.Length-1,2,0};

        CalcLowestCharCode();
    }

    public override string ToString()
    {
        return $"The password is {phrase} after {mutate_count} mutations.";
    }

    /// <summary>
    /// Calculate the next integer addend using the recursive sequence of the Password,
    /// and mutates the password. Addends are calculated based on the index of a password character
    /// in the allowed_characters List.
    /// </summary>
    /// <returns>The next addend</returns>
    public int NextAddend()
    {
        //mutates the password and returns a unique integer based on the mutant
        int return_val = allowed_chars.IndexOf(phrase[addend_position]);

        Mutate();

        addend_position = (addend_position+1) % phrase.Length;

        return return_val;
    }

    /// <summary>
    /// Mutates the phrase of the Password in order to be able to provide the next addend, i.e. step the cylinders/drums in 
    /// enigma once. Each subsequent call starts the mutation (roll) from the next (cyclic) positions in mutate_positions.
    /// A character is mutated by changing it by adding to its index in allowed_characters a predefined value based on the 
    /// initial password. The character's index is cyclic and upon principalisation will trigger the next (toward the left)
    /// character's mutation.
    /// </summary>
    private void Mutate() 
    {
        int pos = mutate_positions[mutate_count%3];
        int i = pos;
        bool moveLeft;

        do {
            int charIndex = allowed_chars.IndexOf(phrase[i]);
            charIndex += lowestCharCode;
            moveLeft = PrincipaliseIndex(ref charIndex);
            phrase[i] = allowed_chars[charIndex];
        } while (moveLeft && (i--) > 0);

        mutate_count++;
    }

    /// <summary>
    /// One time calculation of the lowest index in the initial password.
    /// This is for adding complexity in the mutation.
    /// </summary>
    private void CalcLowestCharCode()
    {
        int lowest = allowed_chars.Count;
        for (int i=0;i< phrase.Length; i++)
        {
            int ind = allowed_chars.IndexOf(phrase[i]);
            if (lowest > ind)
            {
                lowest = ind;
            }
        }
        if (lowest < 1) lowest = 1;
        this.lowestCharCode = lowest;
    }

    
    /// <summary>
    /// Keeps the index of a password's character to within the limits of allowed_chars
    /// </summary>
    /// <param name="val">The value to principalise</param>
    /// <returns>True if the input value needed principalisation. </returns>
    private bool PrincipaliseIndex(ref int val)
    {
        //makes sure when incrementing characters, they begin from start again if out of bounds
        int totalIndices = allowed_chars.Count;
        bool neededFix = false;
        while (val < 0) {
            val += totalIndices;
            neededFix = true;
        }
        while (val >= totalIndices) {
            val -= totalIndices;
            neededFix = true;
        }
        return neededFix;
    }

    /// <summary>
    /// Throws an error if any character in a string is not in allowed_chars.
    /// For internal validation of password.
    /// </summary>
    /// <param name="s">The string to test</param>
    private void VerifyCharsAllowed(string s)
    {
        //verification that the password doesn't have unallowed characters, since the indices of characters in the vector are used to provide addend
        foreach (char c in s)
        {
            if (allowed_chars.IndexOf(c)== -1)
            {
                throw new ArgumentException("Character '{c}' is not allowed in password.");
            }
        }
    }

    

}