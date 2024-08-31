using System;

public class SendBuffer
{
    byte[] _buffer;
    int _usedSize = 0;  // 사용한 버퍼 크기

    // 받을 수 있는 공간 반환
    public int FreeSize { get { return _buffer.Length - _usedSize; } }

    // 버퍼 설정
    public SendBuffer(int chunkSize)
    {
        _buffer = new byte[chunkSize];
    }

    // 사용할 버퍼 용량 받기
    public ArraySegment<byte> Open(int reserveSize)
    {
        if (reserveSize > FreeSize)
            return new ArraySegment<byte>();

        return new ArraySegment<byte>(_buffer, _usedSize, reserveSize);
    }

    // 사용한 버퍼만큼 커서 이동
    public ArraySegment<byte> Close(int usedSize)
    {
        ArraySegment<byte> segment = new ArraySegment<byte>(_buffer, _usedSize, usedSize);
        _usedSize += usedSize;
        return segment;
    }
}
