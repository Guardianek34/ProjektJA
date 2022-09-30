using System;

namespace DLLC
{
    public class Grayscale
    {
        public static unsafe void GrayscaleCSharp(byte* pCur, byte* outputPtr, int width)
        {
            for (int x = 0; x < width; x++)
            {
                outputPtr[x] = (byte)(pCur[0] * 0.2126f + pCur[1] * 0.7152f + pCur[2] * 0.0722f);
                pCur += 3;
            }
        }
    }
}
