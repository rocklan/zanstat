﻿using System;
using System.Collections.Generic;

namespace Rocklan.Zanstat
{
    public class Huffman
    {
        private class HuffmanNode
        {
            public HuffmanNode zero;
            public HuffmanNode one;
            public byte value;
            public double freq;
        };

        private class HuffmanTable
        {
            public long bits;
            public int length;
        }

        private HuffmanNode huffmanTree;
        private HuffmanTable[] huffmanLookup;

        private byte[] masks = new byte[] { 0x1, 0x2, 0x4, 0x8, 0x10, 0x20, 0x40, 0x80 };
        private double[] frequencies = new double[]
        {
            0.14473691, 0.01147017, 0.00167522, 0.03831121, 0.00356579,
            0.03811315, 0.00178254, 0.00199644, 0.00183511, 0.00225716,
            0.00211240, 0.00308829, 0.00172852, 0.00186608, 0.00215921,
            0.00168891, 0.00168603, 0.00218586, 0.00284414, 0.00161833,
            0.00196043, 0.00151029, 0.00173932, 0.00218370, 0.00934121,
            0.00220530, 0.00381211, 0.00185456, 0.00194675, 0.00161977,
            0.00186680, 0.00182071, 0.06421956, 0.00537786, 0.00514019,
            0.00487155, 0.00493925, 0.00503143, 0.00514019, 0.00453520,
            0.00454241, 0.00485642, 0.00422407, 0.00593387, 0.00458130,
            0.00343687, 0.00342823, 0.00531592, 0.00324890, 0.00333388,
            0.00308613, 0.00293776, 0.00258918, 0.00259278, 0.00377105,
            0.00267488, 0.00227516, 0.00415997, 0.00248763, 0.00301555,
            0.00220962, 0.00206990, 0.00270369, 0.00231694, 0.00273826,
            0.00450928, 0.00384380, 0.00504728, 0.00221251, 0.00376961,
            0.00232990, 0.00312574, 0.00291688, 0.00280236, 0.00252436,
            0.00229461, 0.00294353, 0.00241201, 0.00366590, 0.00199860,
            0.00257838, 0.00225860, 0.00260646, 0.00187256, 0.00266552,
            0.00242641, 0.00219450, 0.00192082, 0.00182071, 0.02185930,
            0.00157439, 0.00164353, 0.00161401, 0.00187544, 0.00186248,
            0.03338637, 0.00186968, 0.00172132, 0.00148509, 0.00177749,
            0.00144620, 0.00192442, 0.00169683, 0.00209439, 0.00209439,
            0.00259062, 0.00194531, 0.00182359, 0.00159096, 0.00145196,
            0.00128199, 0.00158376, 0.00171412, 0.00243433, 0.00345704,
            0.00156359, 0.00145700, 0.00157007, 0.00232342, 0.00154198,
            0.00140730, 0.00288807, 0.00152830, 0.00151246, 0.00250203,
            0.00224420, 0.00161761, 0.00714383, 0.08188576, 0.00802537,
            0.00119484, 0.00123805, 0.05632671, 0.00305156, 0.00105584,
            0.00105368, 0.00099246, 0.00090459, 0.00109473, 0.00115379,
            0.00261223, 0.00105656, 0.00124381, 0.00100326, 0.00127550,
            0.00089739, 0.00162481, 0.00100830, 0.00097229, 0.00078864,
            0.00107240, 0.00084409, 0.00265760, 0.00116891, 0.00073102,
            0.00075695, 0.00093916, 0.00106880, 0.00086786, 0.00185600,
            0.00608367, 0.00133600, 0.00075695, 0.00122077, 0.00566955,
            0.00108249, 0.00259638, 0.00077063, 0.00166586, 0.00090387,
            0.00087074, 0.00084914, 0.00130935, 0.00162409, 0.00085922,
            0.00093340, 0.00093844, 0.00087722, 0.00108249, 0.00098598,
            0.00095933, 0.00427593, 0.00496661, 0.00102775, 0.00159312,
            0.00118404, 0.00114947, 0.00104936, 0.00154342, 0.00140082,
            0.00115883, 0.00110769, 0.00161112, 0.00169107, 0.00107816,
            0.00142747, 0.00279804, 0.00085922, 0.00116315, 0.00119484,
            0.00128559, 0.00146204, 0.00130215, 0.00101551, 0.00091756,
            0.00161184, 0.00236375, 0.00131872, 0.00214120, 0.00088875,
            0.00138570, 0.00211960, 0.00094060, 0.00088083, 0.00094564,
            0.00090243, 0.00106160, 0.00088659, 0.00114514, 0.00095861,
            0.00108753, 0.00124165, 0.00427016, 0.00159384, 0.00170547,
            0.00104431, 0.00091395, 0.00095789, 0.00134681, 0.00095213,
            0.00105944, 0.00094132, 0.00141883, 0.00102127, 0.00101911,
            0.00082105, 0.00158448, 0.00102631, 0.00087938, 0.00139290,
            0.00114658, 0.00095501, 0.00161329, 0.00126542, 0.00113218,
            0.00123661, 0.00101695, 0.00112930, 0.00317976, 0.00085346,
            0.00101190, 0.00189849, 0.00105728, 0.00186824, 0.00092908,
            0.00160896
        };

        public Huffman()
	    {
            huffmanLookup = new HuffmanTable[256];
		    for(int i = 0;i< 256;i++)
		    {
                huffmanLookup[i] = new HuffmanTable();
            }

            BuildTree();
        }

        public byte[] Decode(byte[] input)
        {
            int bit, bitCount;
            HuffmanNode tmp;

            int first = input[0];
            input = input[1..];

            // If 0xff, input data is uncompressed.
            if (first == 0xFF)
                return input;

            List<byte> output = new List<byte>();
            bitCount = (input.Length * 8) - first;
            bit = 0;

            while (bit < bitCount)
            {
                tmp = huffmanTree;
                do
                {
                    if (GetBit(input, bit))
                        tmp = tmp.one;
                    else
                        tmp = tmp.zero;
                    bit++;
                }
                while (tmp.zero != null);

                output.Add(tmp.value);
            }

            return output.ToArray();
        }

        public byte[] Encode(byte[] input)
        {
            byte[] output = new byte[20000];
            int i, j, bitat, outlen;
            long t;
            byte firstByte;

            bitat = 0;

            for (i = 0; i < input.Length; i++)
            {
                t = huffmanLookup[input[i]].bits;
                for (j = 0; j < huffmanLookup[input[i]].length; j++)
                {
                    PutBit(output, bitat + huffmanLookup[input[i]].length - j - 1, t & 1);
                    t >>= 1;
                }
                bitat += huffmanLookup[input[i]].length;
            }

            outlen = 1 + (bitat + 7) / 8;
            firstByte = (byte)(8 * (outlen - 1) - bitat);

            byte[] result;

            if (outlen >= input.Length + 1)
            {
                // Compressed is larger than uncompressed, so just return uncompressed.
                result = new byte[input.Length + 1];
                result[0] = 0xFF;
                Array.Copy(input, 0, result, 1, input.Length);
            }
            else
            {
                result = new byte[outlen];
                result[0] = firstByte;
                Array.Copy(output, 0, result, 1, outlen - 1);
            }

            return result;
        }

        private bool MakeTable(HuffmanNode node, int length, long bits)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if (node.zero != null)
            {
                if (node.one == null)
                    throw new Exception("No 'one' in node");

                if (length >= 32)
                    throw new Exception("Invalid length");

                if (!MakeTable(node.zero, length + 1, bits << 1))
                    return false;

                if (!MakeTable(node.one, length + 1, (bits << 1) | 1))
                    return false;

                return true;
            }

            huffmanLookup[node.value].length = length;
            huffmanLookup[node.value].bits = bits;

            return true;
        }

        private void PutBit(byte[] buf, int pos, long bit)
        {
            if (bit != 0)
            {
                buf[pos / 8] |= masks[pos % 8];
            }
            else
            {
                buf[pos / 8] &= (byte)(~masks[pos % 8]);
            }
        }

        private bool GetBit(byte[] buf, int pos)
        {
            return (buf[pos / 8] & masks[pos % 8]) != 0;
        }

        private void BuildTree()
        {
            double min1, min2;
            int i, j, minat1, minat2;
            HuffmanNode[] work = new HuffmanNode[256];
            HuffmanNode tmp = new HuffmanNode();

            for (i = 0; i < 256; i++)
            {
                work[i] = new HuffmanNode();
                work[i].value = (byte)i;
                work[i].freq = frequencies[i];
                work[i].zero = null;
                work[i].one = null;
                huffmanLookup[i].length = 0;
            }

            for (i = 0; i < 255; i++)
            {
                minat1 = -1;
                minat2 = -1;
                min1 = 1E30;
                min2 = 1E30;

                for (j = 0; j < 256; j++)
                {
                    if (work[j] == null)
                        continue;

                    if (work[j].freq < min1)
                    {
                        minat2 = minat1;
                        min2 = min1;
                        minat1 = j;
                        min1 = work[j].freq;
                    }
                    else if (work[j].freq < min2)
                    {
                        minat2 = j;
                        min2 = work[j].freq;
                    }
                }

                if (minat1 < 0)
                    throw new Exception($"Failed to build Huffman tree: minat1 is {minat1}");

                if (minat2 < 0)
                    throw new Exception($"Failed to build Huffman tree: minat2 is {minat2}");

                tmp = new HuffmanNode();
                tmp.zero = work[minat2];
                tmp.one = work[minat1];
                tmp.freq = work[minat2].freq + work[minat1].freq;
                tmp.value = 0xFF;
                work[minat1] = tmp;
                work[minat2] = null;
            }

            huffmanTree = tmp;
            MakeTable(huffmanTree, 0, 0);
        }
    }
}
