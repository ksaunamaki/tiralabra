using Tiracompress.Algorithms;

namespace Tiracompress.Tests;

public class HuffmanTests
{
    /*  Sample input from https://www.programiz.com/dsa/huffman-coding example

        Expected results:

        Byte    Frequency   Code
        0xA     5           11
        0xB     1           100
        0xC     6           0
        0xD     3           101

    */ 
    private byte[] _testinput_1 = new byte[] 
    {
        0xB,0xC,0xA,0xA,0xD,0xD,0xD,0xC,0xC,0xA,0XC,0xA,0XC,0xA,0XC
    };

    /*  Sample input from https://www.geeksforgeeks.org/huffman-coding-greedy-algo-3/ example

        Expected results:

        Byte    Frequency   Code
        0xA     5           1100
        0xB     9           1101
        0xC     12          100
        0xD     13          101
        0xE     16          111
        0xF     45          0

    */ 
    private byte[] _testinput_2 = new byte[] 
    {
        0xA,0xA,0xA,0xA,0xA,
        0xB,0xB,0xB,0xB,0xB,0XB,0xB,0XB,0xB,
        0xC,0xC,0xC,0xC,0xC,0xC,0xC,0xC,0xC,0xC,0xC,0xC,
        0xD,0xD,0xD,0xD,0xD,0xD,0xD,0xD,0xD,0xD,0xD,0xD,0xD,
        0xE,0xE,0xE,0xE,0xE,0xE,0xE,0xE,0xE,0xE,0xE,0xE,0xE,0xE,0xE,0xE,
        0xF,0xF,0xF,0xF,0xF,0xF,0xF,0xF,0xF,0xF,0xF,0xF,0xF,0xF,0xF,0xF,0xF,0xF,0xF,0xF,
        0xF,0xF,0xF,0xF,0xF,0xF,0xF,0xF,0xF,0xF,0xF,0xF,0xF,0xF,0xF,0xF,0xF,0xF,0xF,0xF,
        0xF,0xF,0xF,0xF,0xF
    };

    private bool IsBitSet(uint code, int bit)
    {
        var bitValue = (uint)Math.Pow(2, bit - 1);

        return (code & bitValue) == bitValue;
    }

    [Fact]
    public void TestSymbolFrequencies_Input1()
    {
        var huffman = new Huffman();

        using var memoryStream = new MemoryStream(_testinput_1);

        var frequencies = huffman.BuildSymbolFrequencies(memoryStream);

        Assert.Equal(4, frequencies.Count);

        foreach (var (symbol, frequency) in frequencies)
        {
            switch (symbol)
            {
                case 0xA:
                    Assert.Equal(5, (int)frequency);
                    break;
                case 0xB:
                    Assert.Equal(1, (int)frequency);
                    break;
                case 0xC:
                    Assert.Equal(6, (int)frequency);
                    break;
                case 0xD:
                    Assert.Equal(3, (int)frequency);
                    break;
                default:
                    Assert.Fail("Unknown symbol in frequencies table");
                    break;
            }
        }
    }

    [Fact]
    public void TestSymbolTree_Input1()
    {
        var huffman = new Huffman();

        using var memoryStream = new MemoryStream(_testinput_1);

        var frequencies = huffman.BuildSymbolFrequencies(memoryStream);
        var rootNode = huffman.BuildHuffmanTree(frequencies);

        // Test tree's correctness
        Assert.Null(rootNode.Symbol);

        Assert.NotNull(rootNode.Left!.Symbol);
        Assert.Equal(0xC, (int)rootNode.Left.Symbol);
        Assert.Equal(6, (int)rootNode.Left.Frequency);

        Assert.Null(rootNode.Right!.Symbol);

        Assert.Null(rootNode.Right.Left!.Symbol);

        Assert.NotNull(rootNode.Right.Left.Left!.Symbol);
        Assert.Equal(0xB, (int)rootNode.Right.Left.Left.Symbol);
        Assert.Equal(1, (int)rootNode.Right.Left.Left.Frequency);

        Assert.NotNull(rootNode.Right.Left.Right!.Symbol);
        Assert.Equal(0xD, (int)rootNode.Right.Left.Right.Symbol);
        Assert.Equal(3, (int)rootNode.Right.Left.Right.Frequency);

        Assert.NotNull(rootNode.Right.Right!.Symbol);
        Assert.Equal(0xA, (int)rootNode.Right.Right.Symbol);
        Assert.Equal(5, (int)rootNode.Right.Right.Frequency);
    }

    [Fact]
    public void TestCodeTable_Input1()
    {
        var huffman = new Huffman();

        using var memoryStream = new MemoryStream(_testinput_1);

        var frequencies = huffman.BuildSymbolFrequencies(memoryStream);
        var rootNode = huffman.BuildHuffmanTree(frequencies);
        var table = huffman.BuildCodeTable(rootNode);

        Assert.Equal(4, table.Count);

        Assert.Contains<byte, (uint, int)>(0xA, table);

        var codeForA = table[0xA];

        Assert.True(IsBitSet(codeForA.Item1, 1));
        Assert.True(IsBitSet(codeForA.Item1, 2));

        Assert.Contains<byte, (uint, int)>(0xB, table);

        var codeForB = table[0xB];

        Assert.False(IsBitSet(codeForB.Item1, 1));
        Assert.False(IsBitSet(codeForB.Item1, 2));
        Assert.True(IsBitSet(codeForB.Item1, 3));

        Assert.Contains<byte, (uint, int)>(0xC, table);

        var codeForC = table[0xC];

        Assert.False(IsBitSet(codeForC.Item1, 1));

        Assert.Contains<byte, (uint, int)>(0xD, table);

        var codeForD = table[0xD];

        Assert.True(IsBitSet(codeForD.Item1, 1));
        Assert.False(IsBitSet(codeForD.Item1, 2));
        Assert.True(IsBitSet(codeForD.Item1, 3));
    }

    [Fact]
    public void TestSymbolFrequencies_Input2()
    {
        var huffman = new Huffman();

        using var memoryStream = new MemoryStream(_testinput_2);

        var frequencies = huffman.BuildSymbolFrequencies(memoryStream);

        Assert.Equal(6, frequencies.Count);

        foreach (var (symbol, frequency) in frequencies)
        {
            switch (symbol)
            {
                case 0xA:
                    Assert.Equal(5, (int)frequency);
                    break;
                case 0xB:
                    Assert.Equal(9, (int)frequency);
                    break;
                case 0xC:
                    Assert.Equal(12, (int)frequency);
                    break;
                case 0xD:
                    Assert.Equal(13, (int)frequency);
                    break;
                case 0xE:
                    Assert.Equal(16, (int)frequency);
                    break;
                case 0xF:
                    Assert.Equal(45, (int)frequency);
                    break;
                default:
                    Assert.Fail("Unknown symbol in frequencies table");
                    break;
            }
        }
    }

    [Fact]
    public void TestSymbolTree_Input2()
    {
        var huffman = new Huffman();

        using var memoryStream = new MemoryStream(_testinput_2);

        var frequencies = huffman.BuildSymbolFrequencies(memoryStream);
        var rootNode = huffman.BuildHuffmanTree(frequencies);

        // Test tree's correctness
        Assert.Null(rootNode.Symbol);

        Assert.NotNull(rootNode.Left!.Symbol);
        Assert.Equal(0xF, (int)rootNode.Left.Symbol);
        Assert.Equal(45, (int)rootNode.Left.Frequency);

        Assert.Null(rootNode.Right!.Symbol);

        Assert.Null(rootNode.Right.Left!.Symbol);
        Assert.Null(rootNode.Right.Right!.Symbol);

        Assert.NotNull(rootNode.Right.Left.Left!.Symbol);
        Assert.Equal(0xC, (int)rootNode.Right.Left.Left.Symbol);
        Assert.Equal(12, (int)rootNode.Right.Left.Left.Frequency);

        Assert.NotNull(rootNode.Right.Left.Right!.Symbol);
        Assert.Equal(0xD, (int)rootNode.Right.Left.Right.Symbol);
        Assert.Equal(13, (int)rootNode.Right.Left.Right.Frequency);

        Assert.Null(rootNode.Right.Right.Left!.Symbol);

        Assert.NotNull(rootNode.Right.Right.Left.Left!.Symbol);
        Assert.Equal(0xA, (int)rootNode.Right.Right.Left.Left.Symbol!);
        Assert.Equal(5, (int)rootNode.Right.Right.Left.Left.Frequency);

        Assert.NotNull(rootNode.Right.Right.Left.Right!.Symbol);
        Assert.Equal(0xB, (int)rootNode.Right.Right.Left.Right.Symbol!);
        Assert.Equal(9, (int)rootNode.Right.Right.Left.Right.Frequency);

        Assert.NotNull(rootNode.Right.Right.Right!.Symbol);
        Assert.Equal(0xE, (int)rootNode.Right.Right.Right.Symbol);
        Assert.Equal(16, (int)rootNode.Right.Right.Right.Frequency);
    }

    [Fact]
    public void TestCodeTable_Input2()
    {
        var huffman = new Huffman();

        using var memoryStream = new MemoryStream(_testinput_2);

        var frequencies = huffman.BuildSymbolFrequencies(memoryStream);
        var rootNode = huffman.BuildHuffmanTree(frequencies);
        var table = huffman.BuildCodeTable(rootNode);

        Assert.Equal(6, table.Count);

        Assert.Contains<byte, (uint, int)>(0xA, table);

        var codeForA = table[0xA];

        // 1100
        Assert.False(IsBitSet(codeForA.Item1, 1));
        Assert.False(IsBitSet(codeForA.Item1, 2));
        Assert.True(IsBitSet(codeForA.Item1, 3));
        Assert.True(IsBitSet(codeForA.Item1, 4));

        Assert.Contains<byte, (uint, int)>(0xB, table);

        var codeForB = table[0xB];

        // 1101
        Assert.True(IsBitSet(codeForB.Item1, 1));
        Assert.False(IsBitSet(codeForB.Item1, 2));
        Assert.True(IsBitSet(codeForB.Item1, 3));
        Assert.True(IsBitSet(codeForB.Item1, 4));

        Assert.Contains<byte, (uint, int)>(0xC, table);

        var codeForC = table[0xC];

        // 100
        Assert.False(IsBitSet(codeForC.Item1, 1));
        Assert.False(IsBitSet(codeForC.Item1, 2));
        Assert.True(IsBitSet(codeForC.Item1, 3));

        Assert.Contains<byte, (uint, int)>(0xD, table);

        var codeForD = table[0xD];

        // 101
        Assert.True(IsBitSet(codeForD.Item1, 1));
        Assert.False(IsBitSet(codeForD.Item1, 2));
        Assert.True(IsBitSet(codeForD.Item1, 3));

        Assert.Contains<byte, (uint, int)>(0xE, table);

        var codeForE = table[0xE];

        // 111
        Assert.True(IsBitSet(codeForE.Item1, 1));
        Assert.True(IsBitSet(codeForE.Item1, 2));
        Assert.True(IsBitSet(codeForE.Item1, 3));

        Assert.Contains<byte, (uint, int)>(0xF, table);

        var codeForF = table[0xF];

        // 0
        Assert.False(IsBitSet(codeForF.Item1, 1));
    }
}