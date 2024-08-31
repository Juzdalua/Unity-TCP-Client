using System;

public class SendBuffer
{
    byte[] _buffer;
    int _usedSize = 0;  // ����� ���� ũ��

    // ���� �� �ִ� ���� ��ȯ
    public int FreeSize { get { return _buffer.Length - _usedSize; } }

    // ���� ����
    public SendBuffer(int chunkSize)
    {
        _buffer = new byte[chunkSize];
    }

    // ����� ���� �뷮 �ޱ�
    public ArraySegment<byte> Open(int reserveSize)
    {
        if (reserveSize > FreeSize)
            return new ArraySegment<byte>();

        return new ArraySegment<byte>(_buffer, _usedSize, reserveSize);
    }

    // ����� ���۸�ŭ Ŀ�� �̵�
    public ArraySegment<byte> Close(int usedSize)
    {
        ArraySegment<byte> segment = new ArraySegment<byte>(_buffer, _usedSize, usedSize);
        _usedSize += usedSize;
        return segment;
    }
}
