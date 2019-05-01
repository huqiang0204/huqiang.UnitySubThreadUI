/*  Copyright © 2016 Graeme Collins. All Rights Reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.

3. The name of the author may not be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY GRAEME COLLINS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GifImageData
{
    private GifData _gifData;
    private BitArray _blockBits;
    private int _currentCodeSize;
    private Dictionary<int, GifColor> _colors;

    public int lzwMinimumCodeSize;
    public int endingOffset;
    public List<byte> blockBytes;
    public Dictionary<int, int[]> codeTable;
    public List<int> colorIndices;
    public GifGraphicsControlExtension graphicsControlExt;
    public GifImageDescriptor imageDescriptor;

    public GifImageData(GifData gifData)
    {
        _gifData = gifData;
        _colors = new Dictionary<int, GifColor>(256);

        codeTable = new Dictionary<int, int[]>(4096);
        colorIndices = new List<int>(256);
        blockBytes = new List<byte>(255);
    }

    public void decode()
    {
        // Convert bytes to bits
        _blockBits = new BitArray(blockBytes.ToArray());
        //Debug.Log("Converted " + blockBytes.Count + " bytes into " + _blockBits.Length + " bits.");

        // Translate block
        translateBlock();

        // Prepare colors
        prepareColors();
    }

    private void translateBlock()
    {
        _currentCodeSize = lzwMinimumCodeSize + 1;
        int currentCode;
        int previousCode;
        int bitOffset = _currentCodeSize;
        int iteration = 0;
        int cc = 1 << lzwMinimumCodeSize;
        int eoi = cc + 1;

        //Debug.Log("Starting to translate block. Current code size: " + _currentCodeSize +", CC: " + cc + ", EOI: " + eoi);

        initializeCodeTable();
        currentCode = getCode(_blockBits, bitOffset, _currentCodeSize);
        addToColorIndices(getCodeValues(currentCode));
        previousCode = currentCode;
        bitOffset += _currentCodeSize;

        while (true)
        {
            currentCode = getCode(_blockBits, bitOffset, _currentCodeSize);
            bitOffset += _currentCodeSize;

            // Handle special codes
            if (currentCode == cc)
            {
                //Debug.Log("Encountered CC. Reinitializing code table...");
                _currentCodeSize = lzwMinimumCodeSize + 1;
                initializeCodeTable();
                currentCode = getCode(_blockBits, bitOffset, _currentCodeSize);
                addToColorIndices(getCodeValues(currentCode));
                previousCode = currentCode;
                bitOffset += _currentCodeSize;
                continue;
            }
            else if (currentCode == eoi)
            {
                break;
            }

            // Does code table contain the current code
            if (codeTable.ContainsKey(currentCode))
            {
                int[] newEntry;
                int[] previousValues;
                int[] currentValues;
                
                addToColorIndices(getCodeValues(currentCode));
                previousValues = getCodeValues(previousCode);
                currentValues = getCodeValues(currentCode);
                newEntry = new int[previousValues.Length + 1];

                for (int i = 0; i < previousValues.Length; i++)
                {
                    newEntry[i] = previousValues[i];
                }
                newEntry[newEntry.Length - 1] = currentValues[0];

                addToCodeTable(newEntry);
                previousCode = currentCode;
            }
            else
            {
                int[] previousValues = getCodeValues(previousCode);
                int[] indices = new int[previousValues.Length + 1];

                for (int i = 0; i < previousValues.Length; i++)
                {
                    indices[i] = previousValues[i];
                }
                indices[indices.Length - 1] = previousValues[0];

                addToCodeTable(indices);
                addToColorIndices(indices);
                previousCode = currentCode;
            }

            iteration++;

            // Infinite loop exit check
            if (iteration > 999999)
            {
                throw new Exception("Too many iterations. Infinite loop.");
            }
        }
    }

    private void addToCodeTable(int[] entry)
    {
        string indices = "";

        for (int i = 0; i < entry.Length; i++)
        {
            indices += entry[i];
            indices += (i < entry.Length - 1) ? ", " : "";
        }

        //Debug.Log("Adding code " + codeTable.Count + " to code table with values: " + indices);

        if (codeTable.Count == (1 << _currentCodeSize) - 1)
        {
            _currentCodeSize++;
            //Debug.Log("Increasing current code size to: " + _currentCodeSize);

            if (_currentCodeSize > 12)
            {
                throw new NotImplementedException("Code size larger than max (12). Figure out how to handle this.");
            }
        }

        if (codeTable.Count >= 4096)
        {
            throw new Exception("Exceeded max number of entries in code table.");
        }

        codeTable.Add(codeTable.Count, entry);
    }

    private void addToColorIndices(int[] indices)
    {
        for (int i = 0; i < indices.Length; i++)
        {
            colorIndices.Add(indices[i]);
        }
    }

    private bool isMaxCodeValue(int currentCode, int currentCodeSize)
    {
        return currentCode == (1 << currentCodeSize) - 1;
    }

    private void initializeCodeTable()
    {
        int initialCodeTableSize = (1 << lzwMinimumCodeSize) + 1;

        codeTable.Clear();
        for (int i = 0; i <= initialCodeTableSize; i++)
        {
            codeTable.Add(i, new int[] { i });
        }

        //Debug.Log("Initialized code table. Highest index: " + (codeTable.Count - 1));
    }

    private int getCode(BitArray bits, int bitOffset, int currentCodeSize)
    {
        int value = 0;
        string debugValue = "";

        // Max code size check
        if (currentCodeSize > 12)
        {
            throw new ArgumentOutOfRangeException("currentCodeSize", "Max code size is 12");
        }

        // Calculate value
        for (int i = 0; i < currentCodeSize; i++)
        {
            int index = bitOffset + i;

            if (bits[index])
            {
                value += (1 << i);
                debugValue += "1";
            }
            else
            {
                debugValue += "0";
            }
        }

        //Debug.Log("Read code [" + value + "(" + debugValue + ")] at bit offset [" + bitOffset + "] using code size [" + currentCodeSize + "]");

        return value;
    }

    private int[] getCodeValues(int code)
    {
        if (codeTable.ContainsKey(code))
        {
            return codeTable[code];
        }
        else
        {
            throw new Exception("Code " + code + " does not exist. Code table size: " + codeTable.Count + ". Aborting...");
        }
    }

    private void prepareColors()
    {
        GifColor[] colorTable = imageDescriptor.localColorTableFlag ? imageDescriptor.localColorTable : _gifData.globalColorTable;

        foreach (int index in colorIndices)
        {
            if (!_colors.ContainsKey(index))
            {
                _colors.Add(index, colorTable[index]);
            }
        }
    }

    public GifColor getColor(int index)
    {
        return _colors[index];
    }
}
