namespace UNetLib.Channel;

internal class FragmentedMessage
{
    private readonly Dictionary<byte, byte[]> _fragments = new();

    private readonly byte _totalFragments;

    private int _receivedFragments;

    private bool IsComplete => _receivedFragments == _totalFragments;

    public FragmentedMessage(byte totalFragments)
    {
        _totalFragments = totalFragments;
    }

    /// <summary>
    /// Adds a fragment to the message.
    /// </summary>
    /// <param name="index">The index of the fragment.</param>
    /// <param name="data">The data of the fragment.</param>
    /// <returns>True if the message is complete after adding this fragment.</returns>
    public bool AddFragment(byte index, byte[] data)
    {
        if (!_fragments.TryAdd(index, data))
        {
            // We already have this fragment
            return IsComplete;
        }

        _receivedFragments++;
        return IsComplete;
    }

    /// <summary>
    /// Reassembles the complete message from its fragments.
    /// </summary>
    /// <returns>The reassembled message.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the message is not complete.</exception>
    public byte[] Reassemble()
    {
        if (!IsComplete)
        {
            throw new InvalidOperationException("Cannot reassemble incomplete message.");
        }

        // Calculate total size and create a buffer
        var totalSize = _fragments.Sum(kvp => kvp.Value.Length);
        var buffer = new byte[totalSize];
        var offset = 0;

        // Copy fragments in the correct order
        for (byte i = 0; i < _totalFragments; i++)
        {
            var fragmentData = _fragments[i];
            Buffer.BlockCopy(fragmentData, 0, buffer, offset, fragmentData.Length);
            offset += fragmentData.Length;
        }

        return buffer;
    }
}