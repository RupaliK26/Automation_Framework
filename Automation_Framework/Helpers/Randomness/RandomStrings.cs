using System;


/**
 * Utility methods to generate random strings.
 * <br>
 *
 * @author uvonhke
 * @since 9/11/15
 */
namespace Automation_Framework.Helpers.Randomness
{
    public static class RandomStrings {
    // Always use the accessors to access these members
    private static Random rng;
    private static char[] lettersAtoZ;

    /**
     * This method will get random word with specific length
     *
     * @param lengthOfWord length of the word
     * @return random word
     */
    public static string GenerateRandomWord(int lengthOfWord) {
        return generateRandomWordUsingAlphabet(lengthOfWord, getLettersAtoZ());
    }

    /**
     * This method will get random word with specific length and with specified characters
     *
     * @param lengthOfWord length od the word
     * @param alphabet     character sequence
     * @return random word
     */
    public static string generateRandomWordUsingAlphabet(int lengthOfWord, char[] alphabet) {
        Random rng = getRng();
        char[] randomWord = new char[lengthOfWord];
        for (int i = 0; i < lengthOfWord; i++)
            randomWord[i] = alphabet[rng.Next(alphabet.Length)];

        return new string(randomWord);
    }

    /**
     * Returns random word within min and max length
     *
     * @param lengthMin minimum length
     * @param lengthMax maximum length
     * @return return random word
     */
    public static string generateRandomName(int lengthMin, int lengthMax) {
        int min = Math.Min(lengthMin, lengthMax) > 0 ? Math.Min(lengthMin, lengthMax) : 1;
        int max = Math.Max(lengthMin, lengthMax) > 0 ? Math.Max(lengthMin, lengthMax) : 1;
        int range = max - min;
        int length = getRng().Next(min, max);
            string name = GenerateRandomWord(length);
        return name.Substring(0, 1).ToUpper() + name.Substring(1);
    }

    private static Random getRng() {
        if (null == rng)
            rng = new Random();

        return rng;
    }

    private static char[] getLettersAtoZ() {
        if (null == lettersAtoZ) {
            lettersAtoZ = new char['z' - 'a' + 1];
            int i = 0;
            for (char c = 'a'; c <= 'z'; c++, i++)
                lettersAtoZ[i] = c;
        }

        return lettersAtoZ;
    }

}
}
