using System;

public static class ArrayExtensions
{
    public static T[] SubArray<T>(this T[] data, int index, int length)
    {
        T[] result = new T[length];
        Array.Copy(data, index, result, 0, length);
        return result;
    }

    public static void SetToNumber(this char[] array, int number, bool seperate = false, char seperator = '.', char emptyPlaceChar = ' ')
    {
        int length = array.Length;
        bool placeSeperator = false;
        int counter = 1;
        for (int i = length - 1; i >= 0; i--)
        {
            if (placeSeperator == true)
            {
                array[i] = seperator;
                placeSeperator = false;
                continue;
            }
            array[i] = (char)((number % 10) + 48);
            number /= 10;
            if (number == 0)
            {
                for (int j = i - 1; j >= 0; j--)
                    array[j] = emptyPlaceChar;
                break;
            }
            if (seperate && counter % 3 == 0)
                placeSeperator = true;
            counter++;
        }
    }

    public static void SetToFloat1Frac(this char[] array, float number)
    {
        int simpleNumber = (int)Math.Truncate(number);
        int fractional = (int)((number - Math.Truncate(number)) * 100);
        int firstFracNumber = fractional / 10;
        array[array.Length - 1] = (char)((firstFracNumber) + 48);
        array[array.Length - 2] = '.';
        for (int i = array.Length - 3; i >= 0; i--)
        {
            array[i] = (char)((simpleNumber % 10) + 48);
            simpleNumber /= 10;
            if (simpleNumber == 0)
            {
                for (int j = i - 1; j >= 0; j--)
                    array[j] = ' ';
                break;
            }
        }
    }

    public static void SetToFloat2Frac(this char[] array, float number)
    {
        int simpleNumber = (int)Math.Truncate(number);
        int fractional = (int)((number - Math.Truncate(number)) * 100);
        int firstFracNumber = fractional / 10;
        int secondFracNumber = fractional % 10;
        array[array.Length - 1] = (char)((secondFracNumber) + 48);
        array[array.Length - 2] = (char)((firstFracNumber) + 48);
        array[array.Length - 3] = '.';
        for (int i = array.Length - 4; i >= 0; i--)
        {
            array[i] = (char)((simpleNumber % 10) + 48);
            simpleNumber /= 10;
            if (simpleNumber == 0)
            {
                for (int j = i - 1; j >= 0; j--)
                    array[j] = ' ';
                break;
            }
        }
    }
}
