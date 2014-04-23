using System;

namespace Assets.Scripts
{
    #region Contructs
    // By putting this here, it's available to any class as it's not in a class but the namespace
    // This is a structure. TODO: See if this wouldn't be better served being in the classes that use it.
    public struct Vector2Int // Doing this instead of the unity one as it would return a float
    {
        public int x;
        public int y;

        public Vector2Int(int width, int height)
        {
            x = width;
            y = height;
        }
    }

    public class Utilities
    {


    #endregion

        #region Fields

        static private Random _random;

        #endregion

        #region Constructors

        public Utilities()
        {
            _random = new Random(); // Could pass an int seed through this if i wanted to.
        }

        #endregion

        #region Properties



        #endregion

        #region Methods

        // A nice trick to make it easier to pick random items from an array
        // This is a generic method, it should take any array and return a result
        public T RandArray<T>(T[] array)
        {
            return array[_random.Next(0, array.Length)];
        }


        // Basic clamp function; Provided under the CPOL See http://www.codeproject.com/Articles/23323/A-Generic-Clamp-Function-for-C for details
        // Another generic method, was static but could not unit test it that way?
        public T Clamp<T>(T value, T min, T max)
            where T : IComparable<T>
        {
            T result = value;
            if (value.CompareTo(max) > 0)
                result = max;
            if (value.CompareTo(min) < 0)
                result = min;
            return result;
        }
        #endregion

    }
}
