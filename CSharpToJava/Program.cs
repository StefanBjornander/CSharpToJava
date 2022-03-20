global using System;
global using System.IO;
global using System.Text;
global using System.Linq;
global using System.Numerics;
global using System.Globalization;
global using System.Collections.Generic;

namespace CCompiler {
  public class Start {
    public static void Main(string[] args) {
      FileInfo file1 = new(@"C:\Users\stefa\Documents\A A C_Compiler_CSharp_10_Read\C_Compiler_CSharp_8\Preprocessor.cs");
      StreamReader streamReader1 = new(file1.FullName);
      String inputText = new(streamReader1.ReadToEnd());
      StringBuilder inputBuffer = new(inputText);
      streamReader1.Close();

      List<String> oldList = new();
      StringBuilder? buf = null;
      foreach (String s in inputText.Split("\r\n")) {
        if (s.Trim().StartsWith("//")) {
          if (buf != null) {
            oldList.Add(buf.ToString());
            buf = null;
          }

          oldList.Add(s);
        }
        else {
          if (buf == null) {
            buf = new();
            buf.Append(s + "  ");
          }
          else {
            buf.Append("\r\n" + s + "  ");
          }
        }
      }

      if (buf != null) {
        oldList.Add(buf.ToString());
      }

      Dictionary<String, String> globalMap = new();
      foreach (String s in oldList) {
        String? lastWord = null;
        int index = 0;

        while (index < s.Length) {
          while ((index < s.Length) && Char.IsWhiteSpace(s[index])) {
            ++index;
          }

          if ((index < s.Length) && (Char.IsLetter(s[index]))) {
            int wordIndex = index;
            while (Char.IsLetterOrDigit(s[index]) || (s[index] == '_')) {
              ++index;
            }

            if ((index < s.Length) && s[index] == '?') {
              ++index;
            }

            if ((index < s.Length) && s[index] == '<') {
              while (s[index] != '>') {
                ++index;
              }

              ++index;
            }

            String oldWord = s.Substring(wordIndex, index - wordIndex);
            if ((lastWord != null) && Char.IsUpper(oldWord[0])) {
              switch (lastWord) {
                case "void":
                case "char":
                case "short":
                case "int":
                case "long":
                case "float":
                case "double":
                case "string": {
                    String newWord = Char.ToLower(oldWord[0]) + oldWord.Substring(1);
                    globalMap[oldWord] = newWord;
                  }
                  break;

                default:
                  if (Char.IsUpper(lastWord[0])) {
                    String newWord = Char.ToLower(oldWord[0]) + oldWord.Substring(1);
                    globalMap[oldWord] = newWord;
                  }
                  break;
              }
            }

            lastWord = oldWord;
          }
          else if ((index < s.Length) && (s[index] == '.')) {
            ++index;

            while ((index < s.Length) && Char.IsWhiteSpace(s[index])) {
              ++index;
            }

            if ((index < s.Length) && Char.IsUpper(s[index])) {
              int startIndex = index;
              while (Char.IsLetterOrDigit(s[index]) || (s[index] == '_')) {
                ++index;
              }

              int endIndex = index - 1;
              while ((index < s.Length) && Char.IsWhiteSpace(s[index])) {
                ++index;
              }

              if ((index < s.Length) && s[index] == '(') {
                String oldWord = s.Substring(startIndex, endIndex - startIndex + 1);
                String newWord = Char.ToLower(oldWord[0]) + oldWord.Substring(1);
                globalMap[oldWord] = newWord;
              }
            }
          }
          else if ((index < s.Length) && (s[index] == '\'')) {
            ++index;

            while (s[index] != '\'') {
              if (((s[index] == '\\') && (s[index + 1] == '\'')) ||
                  ((s[index] == '\\') && (s[index + 1] == '\\'))) {
                ++index;
              }

              ++index;
            }

            ++index;
            lastWord = null;
          }
          else if ((index < s.Length) && (s[index] == '\"')) {
            ++index;

            while (s[index] != '\"') {
              if (((s[index] == '\\') && (s[index + 1] == '\"')) ||
                  ((s[index] == '\\') && (s[index + 1] == '\\'))) {
                ++index;
              }

              ++index;
            }

            ++index;
            lastWord = null;
          }
          else if (index < (s.Length - 1) && (s[index] == '/') && (s[index + 1] == '/')) {
            while (index < s.Length) {
              if (s[index] == '\n') {
                break;
              }

              ++index;

              if ((s[index - 1] == '\r') && (index < s.Length) && s[index] == '\n') {
                break;
              }
            }
          }
          else if (index < (s.Length - 1) && (s[index] == '/') && (s[index + 1] == '*')) {
            index += 2;

            while (index < s.Length) {
              ++index;

              if ((s[index - 1] == '*') && (index < s.Length) && s[index] == '/') {
                ++index;
                break;
              }
            }
          }
          else {
            ++index;
            lastWord = null;
          }
        }
      }

      List<String> newList = new();
      foreach (String s in oldList) {
        StringBuilder buffer = new();
        int index = 0;

        while (index < s.Length) {
          if ((index < (s.Length - 3)) && s.Substring(index, 4).Equals("new(")) {
            int insertIndex = index - 1;

            while ((insertIndex > 0) && Char.IsWhiteSpace(s[insertIndex])) {
              --insertIndex;
            }

            if ((insertIndex > 0) && (s[insertIndex] == '=')) {
              --insertIndex;

              while ((insertIndex > 0) && Char.IsWhiteSpace(s[insertIndex])) {
                --insertIndex;
              }

              if ((insertIndex > 0) && (Char.IsLetterOrDigit(s[insertIndex]) || (s[insertIndex] == '_'))) {
                while ((insertIndex > 0) && (Char.IsLetterOrDigit(s[insertIndex]) || (s[insertIndex] == '_'))) {
                  --insertIndex;
                }

                while ((insertIndex > 0) && Char.IsWhiteSpace(s[insertIndex])) {
                  --insertIndex;
                }

                bool generics = false;
                if ((insertIndex > 0) && (s[insertIndex] == '>')) {
                  generics = true;
                  while ((insertIndex > 0) && (s[insertIndex] != '<')) {
                    --insertIndex;
                  }

                  --insertIndex;
                }

                if ((insertIndex > 0) && (Char.IsLetterOrDigit(s[insertIndex]) || (s[insertIndex] == '_'))) {
                  int endIndex = insertIndex;
                  while ((insertIndex > 0) && (Char.IsLetterOrDigit(s[insertIndex]) || (s[insertIndex] == '_'))) {
                    --insertIndex;
                  }

                  String word = s.Substring(insertIndex + 1, endIndex - insertIndex);
                  buffer.Append("new ");
                  buffer.Append(word);
                  if (generics) {
                    buffer.Append("<>");
                  }
                  buffer.Append('(');
                }
              }
            }

            index += 4;
          }
          else if (char.IsLetter(s[index])) {
            int wordIndex = index;
            while (Char.IsLetterOrDigit(s[index]) || (s[index] == '_')) {
              ++index;
            }

            String word = s.Substring(wordIndex, index - wordIndex);

            if (globalMap.ContainsKey(word)) {
              buffer.Append(globalMap[word]);
            }
            else {
              buffer.Append(word);
            }
          }
          else if (s[index] == '\'') {
            buffer.Append(s[index++]);

            while (s[index] != '\'') {
              if (((s[index] == '\\') && (s[index + 1] == '\'')) ||
                  ((s[index] == '\\') && (s[index + 1] == '\\'))) {
                buffer.Append(s[index++]);
              }

              buffer.Append(s[index++]);
            }

            buffer.Append(s[index++]);
          }
          else if (s[index] == '\"') {
            buffer.Append(s[index++]);

            while (s[index] != '\"') {
              if (((s[index] == '\\') && (s[index + 1] == '\"')) ||
                  ((s[index] == '\\') && (s[index + 1] == '\\'))) {
                buffer.Append(s[index++]);
              }

              buffer.Append(s[index++]);
            }

            buffer.Append(s[index++]);
          }
          else if (index < (s.Length - 1) && (s[index] == '/') && (s[index + 1] == '/')) {
            while (index < s.Length) {
              if (s[index] == '\n') {
                break;
              }

              buffer.Append(s[index++]);

              if ((s[index - 1] == '\r') && (index < s.Length) && s[index] == '\n') {
                break;
              }
            }
          }
          else if (index < (s.Length - 1) && (s[index] == '/') && (s[index + 1] == '*')) {
            buffer.Append("/*");
            index += 2;

            while (index < s.Length) {
              buffer.Append(s[index++]);

              if ((s[index - 1] == '*') && (index < s.Length) && s[index] == '/') {
                break;
              }
            }
          }
          else {
            buffer.Append(s[index++]);
          }
        }

        newList.Add(buffer.ToString());
      }

      FileInfo file2 = new(@"C:\Users\stefa\Documents\A A C_Compiler_CSharp_10_Read\C_Compiler_CSharp_8\Preprocessor.java");
      StreamWriter streamWriter2 = new(file2.FullName);

      foreach (String s in newList) {
        streamWriter2.WriteLine(s);
      }
      streamWriter2.Close();
    }
  }
}
