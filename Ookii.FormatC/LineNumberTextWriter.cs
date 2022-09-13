using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ookii.FormatC
{
    internal class LineNumberTextWriter : TextWriter
    {
        #region Nested types

        // We want to use the same code for either Write(char[]) and Write(string), without allocating a new char[] or string for either, so we provide this helper to access one or the other
        private struct StringBuffer
        {
            private readonly string _stringValue;
            private readonly char[] _charArrayValue;

            public StringBuffer(string stringValue)
            {
                _stringValue = stringValue;
                _charArrayValue = null;
            }

            public StringBuffer(char[] charArrayValue)
            {
                _stringValue = null;
                _charArrayValue = charArrayValue;
            }

            public char this[int index]
            {
                get { return _stringValue == null ? _charArrayValue[index] : _stringValue[index]; }
            }

            public char[] Characters => _charArrayValue ?? _stringValue.ToCharArray();

            public int IndexOfLineBreak(int index, int end)
            {
                if (_stringValue == null)
                {
                    for (int x = index; x < end; ++x)
                    {
                        if (_charArrayValue[x] == '\r' || _charArrayValue[x] == '\n')
                            return x;
                    }
                    return -1;
                }
                else
                    return _stringValue.IndexOfAny(_lineBreakCharacters, index, end - index);
            }

            public int SkipLineBreak(int index, int end)
            {
                Debug.Assert(index < end);
                Debug.Assert(this[index] == '\r' || this[index] == '\n');
                if (this[index] == '\r' && index + 1 < end && this[index + 1] == '\n')
                    return index + 2; // Windows line ending
                else
                    return index + 1;
            }

            public void WriteTo(TextWriter writer, int index, int count)
            {
                if (_stringValue == null)
                    writer.Write(_charArrayValue, index, count);
                else
                    writer.Write(_stringValue.Substring(index, count));
            }

            public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
            {
                if (_stringValue == null)
                    Array.Copy(_charArrayValue, sourceIndex, destination, destinationIndex, count);
                else
                    _stringValue.CopyTo(sourceIndex, destination, destinationIndex, count);
            }
        }

        private enum State
        {
            StartOfLine,
            PartialLineBreak,
            InLine
        }

        #endregion

        private static readonly char[] _lineBreakCharacters = { '\r', '\n' };
        private readonly TextWriter _baseWriter;
        private State _state;
        private int _lineNumber;

        public LineNumberTextWriter(TextWriter baseWriter)
        {
            _baseWriter = baseWriter;
        }

        public override Encoding Encoding => _baseWriter.Encoding;

        public string LineNumberFormat { get; set; }

        public string LineNumberClassName { get; set; }

        public override void Write(string value)
        {
            if (value != null)
            {
                WriteCore(new StringBuffer(value), 0, value.Length);
            }
        }

        public override void Write(char[] buffer, int index, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if ((buffer.Length - index) < count)
                throw new ArgumentException();

            WriteCore(new StringBuffer(buffer), index, count);
        }

        public override void Write(char value)
        {
            switch (value)
            {
            case '\n':
                _baseWriter.WriteLine();
                _state = State.StartOfLine;
                break;

            case '\r':
                _state = State.PartialLineBreak;
                break;

            default:
                if (_state == State.PartialLineBreak)
                {
                    _baseWriter.WriteLine();
                    _state = State.StartOfLine;
                }

                WriteLineNumberIfNeeded();
                _baseWriter.Write(value);
                break;
            }
        }

        private void WriteCore(StringBuffer buffer, int index, int count)
        {
            // We've seen a \r so the next char might be \n for the same line break.
            var end = index + count;
            if (_state == State.PartialLineBreak)
            {
                if (count > 0 && buffer[index] == '\n')
                    index += 1;

                _baseWriter.WriteLine();
                _state = State.StartOfLine;
            }

            while (index < end)
            {
                WriteLineNumberIfNeeded();

                int lineEnd = buffer.IndexOfLineBreak(index, end);
                if (lineEnd < 0)
                    lineEnd = end;

                buffer.WriteTo(_baseWriter, index, lineEnd - index);
                if (lineEnd == end - 1 && buffer[lineEnd] == '\r')
                {
                    _state = State.PartialLineBreak;
                    break;
                }
                else if (lineEnd < end)
                {
                    index = buffer.SkipLineBreak(lineEnd, end);
                    _baseWriter.WriteLine();
                    _state = State.StartOfLine;
                }
                else
                    break;
            }
        }

        private void WriteLineNumberIfNeeded()
        {
            if (_state == State.StartOfLine)
            {
                if (LineNumberFormat != null)
                {
                    ++_lineNumber;
                    _baseWriter.WriteStartElement(LineNumberClassName);
                    _baseWriter.Write(LineNumberFormat, _lineNumber);
                    _baseWriter.WriteEndElement();
                }

                _state = State.InLine;
            }
        }
    }
}
