using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GifImageData
{
    private GifData _gifData;
    private BitArray _blockBits;
    private int _currentCodeSize;

    public int lzwMinimumCodeSize;
    public int endingOffset;
    public List<byte> blockBytes;
    public List<int[]> codeTable;
    public List<int> colorIndices;
    public GifGraphicsControlExtension graphicsControlExt;
    public GifImageDescriptor imageDescriptor;
    public Color[] colors;
    public GifImageData(GifData gifData)
    {
        _gifData = gifData;

        codeTable = new List<int[]>(4096);
        colorIndices = new List<int>(256);
        blockBytes = new List<byte>(255);
    }

    public void decode()
    {
        // Convert bytes to bits
        _blockBits = new BitArray(blockBytes.ToArray());

        // Translate block
        translateBlock();
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

        initializeCodeTable();
        currentCode = getCode(_blockBits, bitOffset, _currentCodeSize);
        colorIndices.AddRange(codeTable[currentCode]);
        previousCode = currentCode;
        bitOffset += _currentCodeSize;

        while (true)
        {

            // Calculate value
            currentCode = 0;
            for (int i = 0; i < _currentCodeSize; i++)
            {
                int index = bitOffset + i;
                if (_blockBits[index])
                    currentCode += (1 << i);
            }
            bitOffset += _currentCodeSize;

            if (currentCode == cc)
            {
                _currentCodeSize = lzwMinimumCodeSize + 1;
                int initialCodeTableSize = (1 << lzwMinimumCodeSize) + 1;
                codeTable.Clear();
                for (int i = 0; i <= initialCodeTableSize; i++)
                    codeTable.Add(new int[] { i });
                
                currentCode = 0;
                for (int i = 0; i < _currentCodeSize; i++)
                {
                    int index = bitOffset + i;
                    if (_blockBits[index])
                        currentCode += (1 << i);
                }
                colorIndices.AddRange(codeTable[currentCode]);
                previousCode = currentCode;
                bitOffset += _currentCodeSize;
                continue;
            }
            else if (currentCode == eoi)
            {
                break;
            }
            int c = codeTable.Count;
            if (currentCode < c)
            {
                int[] newEntry;
                int[] previousValues;
                int[] currentValues;

                colorIndices.AddRange(codeTable[currentCode]);
                previousValues = codeTable[previousCode];
                currentValues = codeTable[currentCode];
                newEntry = new int[previousValues.Length + 1];

                for (int i = 0; i < previousValues.Length; i++)
                {
                    newEntry[i] = previousValues[i];
                }
                newEntry[newEntry.Length - 1] = currentValues[0];

                if (codeTable.Count == (1 << _currentCodeSize) - 1)
                {
                    _currentCodeSize++;
                }
                codeTable.Add(newEntry);
                previousCode = currentCode;
            }
            else
            {
                int[] previousValues = codeTable[previousCode];
                int[] indices = new int[previousValues.Length + 1];

                for (int i = 0; i < previousValues.Length; i++)
                {
                    indices[i] = previousValues[i];
                }
                indices[indices.Length - 1] = previousValues[0];

                if (codeTable.Count == (1 << _currentCodeSize) - 1)
                {
                    _currentCodeSize++;
                }
                codeTable.Add(indices);
                colorIndices.AddRange(indices);
                previousCode = currentCode;
            }
            iteration++;
        }
    }

    private void addToCodeTable(int[] entry)
    {
        if (codeTable.Count == (1 << _currentCodeSize) - 1)
        {
            _currentCodeSize++;
        }
        codeTable.Add(entry);
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
            codeTable.Add(new int[] { i });
        }
    }

    private int getCode(BitArray bits, int bitOffset, int currentCodeSize)
    {
        int value = 0;
        for (int i = 0; i < currentCodeSize; i++)
        {
            int index = bitOffset + i;

            if (bits[index])
            {
                value += (1 << i);
            }
        }
        return value;
    }

}
