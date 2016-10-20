using System;
using System.Security.Cryptography;

namespace PasswordGenerator.Random
{
    public class RandomProvider : IRandomProvider
    {
        public RandomProvider()
        {
            RandomGenerator = new RNGCryptoServiceProvider();
        }

        private RNGCryptoServiceProvider RandomGenerator { get; }

        public int Next(int length)
        {
            return Next((byte)length);
        }
        public int Next(byte length)
        {
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            if (length == 1)
            {
                return 0;
            }

            byte[] randomNumber = new byte[1];
            do
            {
                RandomGenerator.GetBytes(randomNumber);
            } while (!IsEvenProbability(randomNumber[0], length));

            return randomNumber[0] % length;
        }

        private bool IsEvenProbability(byte value, byte length)
        {
            // There are MaxValue / numSides full sets of numbers that can come up
            // in a single byte.  For instance, if we have a length of 6, there are
            // 42 full sets of 1-6 that come up.  The 43rd set is incomplete.
            int fullSetsOfValues = Byte.MaxValue / length;

            // If the length is 6, values between 0 and 251 are allowed.  (We use
            // < rather than <= since the = portion allows through an extra 0 value).
            // 252 through 255 would provide an extra 0, 1, 2, 3 so they are not fair
            // to use.
            bool isEvenProbability = value < length * fullSetsOfValues;

            return isEvenProbability;
        }
    }
}