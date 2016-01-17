using System;
using System.Collections.Generic;
using System.Linq;

namespace FxcCompilerGui.Utils
{
    public class OutputLog
    {
        public OutputLog()
        {
            _lines = new List<string>();
        }

        public string Buffer
        {
            get
            {
                if (_bufferDirty)
                {
                    _finalBuffer = _lines.Aggregate("", (current, s) => current + s + '\n');
                    _bufferDirty = false;
                }
                return _finalBuffer;
            }
        }

        public void WriteLine(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            _lines.Add(text);
            OnBufferChanged();
        }

        public void Clear()
        {
            _lines.Clear();
            OnBufferChanged();
        }

        private void OnBufferChanged()
        {
            _bufferDirty = true;

            var handler = BufferChanged;
            handler?.Invoke(null, EventArgs.Empty);
        }

        public event EventHandler BufferChanged;

        private readonly List<string> _lines;
        private string _finalBuffer;
        private bool _bufferDirty = true;
    }
}