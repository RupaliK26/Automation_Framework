using System;
using OpenQA.Selenium;

/**
 * @author kpace
 */

namespace Automation_Framework.Helpers.Randomness
{
    public class RandomNumbers
    {


        private RandomNumbers()
        {

        }


        /**
         * Returns a pseudo-random number between min and max, inclusive.
         * The difference between min and max can be at most
         * <code>Integer.MAX_VALUE - 1</code>.
         *
         * @param min Minimum value
         * @param max Maximum value.  Must be greater than min.
         * @return Integer between min and max, inclusive.
         */
        public static int getRandomInteger(int min, int max)
        {
            return new Random().Next(min, max + 1);
        }

        /**
         * Returns a pseudo-random number with the specified no of digits
         *
         * @param numDigits int for number of digits (places)
         * @return int pseudo-random
         * @author bcline
         */
        public static int getRandomInteger(int numDigits)
        {

            //int max in Java is 2,147,483,647 (10 digits)
            if (numDigits > 9)
            {
                return getRandomInteger(1000000000, 2147483646);
            }

            //should always return a positive number of digits
            if (numDigits < 1)
            {
                numDigits = 1;
            }

            return new Random().Next((int)Math.Pow(10, numDigits), ((int)Math.Pow(10, numDigits + 1)));
        }

        ///**
        // * Returns a pseudo-random float number between min and max, inclusive.
        // * The difference between min and max can be at most
        // * <code>Float.MAX_VALUE - 1</code>.
        // *
        // * @param min Minimum value
        // * @param max Maximum value.  Must be greater than min.
        // * @return Float between min and max, inclusive.
        // */
        //public static float getRandomFloat(float min, float max) {
        //    return new Random().NextDouble(min, max);
        //}

        ///**
        // * Returns a pseudo-random number between min and max, inclusive and rounded off to {@code roundToDecimalPlace}
        // * The difference between min and max can be at most
        // * <code>Float.MAX_VALUE - 1</code>.
        // *
        // * @param min                 Minimum value
        // * @param max                 Maximum value.  Must be greater than min.
        // * @param roundToDecimalPlace number of decimal places upto which the retruned values must be rounded off
        // * @return Integer between min and max, inclusive.
        // */
        //public static float getRandomFloat(float min, float max, int roundToDecimalPlace) {
        //    return Float.parseFloat(string.ToString("%." + roundToDecimalPlace + "f", RandomUtils.nextFloat(min, max)));
        //}
    }
}
