using System;

namespace CSharpDLL
{
    public class CSharpConversion
    {
        unsafe public static void GrayscaleCSharp(byte* pCur, byte* output, int width)
        {
            for (int x = 0; x < width; x++)
            {
                output[x] = (byte)(pCur[0] * 0.2126f + pCur[1] * 0.7152f + pCur[2] * 0.0722f);
                pCur += 3;
            }
        }
    }
}
