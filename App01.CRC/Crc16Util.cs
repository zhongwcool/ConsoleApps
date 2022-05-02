namespace App01.CRC;

public static class Crc16Util
{
    /// <summary>
    ///     指定多项式码来校验对应的接收数据的CRC校验码<br />
    ///     Specifies a polynomial code to validate the corresponding CRC check code for the received data
    /// </summary>
    /// <param name="data">需要校验的数据，带CRC校验码</param>
    /// <param name="pcH">多项式码高位</param>
    /// <param name="pcL">多项式码低位</param>
    /// <returns>返回校验成功与否</returns>
    public static bool CheckCRC16(byte[] data, byte pcH = 0xA0, byte pcL = 0x01)
    {
        if (data == null || data.Length < 2) return false;
        var length = data.Length;
        var numArray1 = new byte[length - 2];
        Array.Copy(data, 0, numArray1, 0, numArray1.Length);
        var numArray2 = CRC16(numArray1, pcH, pcL);
        return numArray2[length - 2] == data[length - 2] &&
               numArray2[length - 1] == data[length - 1];
    }

    private const int SourceIndex = 4;

    /// <summary>
    ///     通过指定多项式码来获取对应的数据的CRC校验码<br />
    ///     The CRC check code of the corresponding data is obtained by specifying the polynomial code
    /// </summary>
    /// <param name="data">需要校验的数据，不包含CRC字节</param>
    /// <param name="pcL">多项式码低位</param>
    /// <param name="pcH">多项式码高位</param>
    /// <param name="preH">预置的高位值</param>
    /// <param name="preL">预置的低位值</param>
    /// <returns>返回带CRC校验码的字节数组，可用于串口发送</returns>
    public static byte[] CRC16(byte[] data, byte pcH = 0xA0, byte pcL = 0x01, byte preH = 0xFF, byte preL = 0xFF)
    {
        if (data.Length <= SourceIndex) return null;
        var preArray = new byte[data.Length - SourceIndex];
        Array.Copy(data, SourceIndex, preArray, 0, preArray.Length);
        var num1 = preL;
        var num2 = preH;
        foreach (var num3 in preArray)
        {
            num1 ^= num3;
            for (var index = 0; index <= 7; ++index)
            {
                var num4 = num2;
                var num5 = num1;
                num2 >>= 1;
                num1 >>= 1;
                if ((num4 & 0x01) == 0x01)
                    num1 |= 0x80;
                if ((num5 & 0x01) == 0x01)
                {
                    num2 ^= pcH;
                    num1 ^= pcL;
                }
            }
        }

        var postArray = new byte[data.Length + 2];
        data.CopyTo(postArray, 0);
        postArray[^2] = num2;
        postArray[^1] = num1;
        return postArray;
    }
}