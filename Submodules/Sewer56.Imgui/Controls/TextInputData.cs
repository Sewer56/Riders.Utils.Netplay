﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using DearImguiSharp;
using Reloaded.Memory.Interop;
using Reloaded.Memory.Pointers;
using Sewer56.Imgui.Misc;

namespace Sewer56.Imgui.Controls
{
    /// <summary>
    /// Encapsulates the creation of native memory to store text input data.
    /// </summary>
    public unsafe class TextInputData
    {
        public sbyte* Pointer => _textInput.Pointer;
        public string Text => GetText();
        public ulong SizeOfData { get; private set; }

        private Pinnable<sbyte> _textInput;

        public TextInputData(int maxCharacters, int characterwidth = sizeof(int))
        {
            SizeOfData = (ulong)(maxCharacters * characterwidth + 1);
            _textInput = new Pinnable<sbyte>(new sbyte[SizeOfData]);
        }

        public TextInputData(string text, int maxCharacters, int characterwidth = sizeof(int))
        {
            SizeOfData = (ulong)(maxCharacters * characterwidth + 1);
            _textInput = new Pinnable<sbyte>(new sbyte[SizeOfData]);

            if (!string.IsNullOrEmpty(text))
            {
                if (text.Length > maxCharacters)
                    throw new ArgumentException("Text length cannot exceed number of characters.");

                var bytes = Encoding.UTF8.GetBytes(text);
                new FixedArrayPtr<byte>((ulong)_textInput.Pointer, (int)SizeOfData).CopyFrom(bytes, bytes.Length);
            }
        }

        public string GetText()
        {
            var text = Encoding.UTF8.GetString((byte*)Pointer, (int)SizeOfData);
            int index = text.IndexOf('\0');
            if (index >= 0)
                text = text.Remove(index);

            return text;
        }

        public void Render(string label = "", ImGuiInputTextFlags flags = 0, ImGuiInputTextCallback callback = null, IntPtr userData = default)
        {
            ImGui.InputText(label, Pointer, (IntPtr)SizeOfData, (int) flags, callback, userData);
        }

        /// <summary>
        /// Custom filter for functions such as <see cref="ImGui.InputText"/>
        /// </summary>
        public int FilterValidPathCharacters(IntPtr ptr)
        {
            var data = new ImGuiInputTextCallbackData();
            return CharacterFilter.IsPathCharacterValid(GetEventCharacter(data)) ? 0 : 1;
        }

        /// <summary>
        /// Custom filter for functions such as <see cref="ImGui.InputText"/>
        /// </summary>
        public int FilterIPAddress(IntPtr ptr)
        {
            const char dot = '.';
            const int charsBetweenDot = 3;
            var data = new ImGuiInputTextCallbackData((void*)ptr);

            var text     = this.GetText();
            var dotIndex = text.LastIndexOf(dot);
            int charsAfterDelim = text.Length;

            if (dotIndex > 0)
                charsAfterDelim = (text.Length - 1) - dotIndex;

            // Dot as 4th Character
            if (charsAfterDelim == charsBetweenDot && GetEventCharacter(data) == dot)
                return 0;

            // Characters after 3rd
            if (charsAfterDelim >= charsBetweenDot)
                return 1;

            return CharacterFilter.IsIPCharacterValid(GetEventCharacter(data)) ? 0 : 1;
        }

        private static char GetEventCharacter(ImGuiInputTextCallbackData data) => Encoding.UTF8.ToCharacter(data.EventChar);
        public static class CharacterFilter
        {
            public static readonly char[] PathInvalidCharacters   = Path.GetInvalidPathChars().Union(Path.GetInvalidFileNameChars()).ToArray();
            public static readonly char[] IPAddressCharacters = "0123456789.".ToCharArray();

            public static bool IsPathCharacterValid(char character) => !PathInvalidCharacters.Contains(character);
            public static bool IsIPCharacterValid(char character)   => IPAddressCharacters.Contains(character);
        }
    }
}
