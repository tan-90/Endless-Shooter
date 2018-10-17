using System.Collections;

public static class Util
{
    public static T[] ShuffleArray<T>(T[] Array, int seed)
    {
        System.Random PseudoRandom = new System.Random(seed);

        for(int i = 0; i < Array.Length - 1; i++)
        {
            int RandomIndex = PseudoRandom.Next(i, Array.Length);
            T ElementA = Array[RandomIndex];
            Array[RandomIndex] = Array[i];
            Array[i] = ElementA;
        }

        return Array;
    }
}
