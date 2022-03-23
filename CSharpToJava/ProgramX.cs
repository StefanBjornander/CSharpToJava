global using System;
global using System.IO;
global using System.Text;
global using System.Linq;
global using System.Numerics;
global using System.Globalization;
global using System.Collections.Generic;

namespace CCompiler {
  public class Start {
    private string m_path = @"C:\Users\stefa\Documents\A A C_Compiler_CSharp_10_Read\C_Compiler_CSharp_8\";
    Dictionary<String, String> m_globalMap = new();

    public static void Main(string[] args) {
      string name = "Preprocessor";
      FileInfo file1 = new(m_path + name + ".cs");
      readFile(name);
      writeFile(name);
    }

    private static readFile(string name) {
      StreamReader streamReader = new(m_path + name + ".cs");
      String inText = streamReader.ReadToEnd();

      /*List<String> oldList = new();
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
      }*/

      String? lastWord = null;
      int index = 0;

      while (index < inText.Length) {
        if 
      }
       
      String? lastWord = null;
      int index = 0;

      while (index < inText.Length) {
        while ((index < inText.Length) && Char.IsWhiteSpace(inText[index])) {
          ++index;
        }

        if ((index < inText.Length) && (Char.IsLetter(inText[index]))) {
          int wordIndex = index;
          while (Char.IsLetterOrDigit(inText[index]) || (inText[index] == '_')) {
            ++index;
          }

          if ((index < inText.Length) && inText[index] == '?') {
            ++index;
          }

          if ((index < inText.Length) && inText[index] == '<') {
            while (inText[index] != '>') {
              ++index;
            }

            ++index;
          }

          String oldWord = inText.Substring(wordIndex, index - wordIndex);
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
                  m_globalMap[oldWord] = newWord;
                }
                break;

              default:
                if (Char.IsUpper(lastWord[0])) {
                  String newWord = Char.ToLower(oldWord[0]) + oldWord.Substring(1);
                  m_globalMap[oldWord] = newWord;
                }
                break;
            }
          }

          lastWord = oldWord;
        }
        else if ((index < inText.Length) && (inText[index] == '.')) {
          ++index;

          while ((index < inText.Length) && Char.IsWhiteSpace(inText[index])) {
            ++index;
          }

          if ((index < inText.Length) && Char.IsUpper(inText[index])) {
            int startIndex = index;
            while (Char.IsLetterOrDigit(inText[index]) || (inText[index] == '_')) {
              ++index;
            }

            int endIndex = index - 1;
            while ((index < inText.Length) && Char.IsWhiteSpace(inText[index])) {
              ++index;
            }

            if ((index < inText.Length) && inText[index] == '(') {
              String oldWord = inText.Substring(startIndex, endIndex - startIndex + 1);
              String newWord = Char.ToLower(oldWord[0]) + oldWord.Substring(1);
              m_globalMap[oldWord] = newWord;
            }
          }
        }
        else if ((index < inText.Length) && (inText[index] == '\'')) {
          ++index;

          while (inText[index] != '\'') {
            if (((inText[index] == '\\') && (inText[index + 1] == '\'')) ||
                ((inText[index] == '\\') && (inText[index + 1] == '\\'))) {
              ++index;
            }

            ++index;
          }

          ++index;
          lastWord = null;
        }
        else if ((index < inText.Length) && (inText[index] == '\"')) {
          ++index;

          while (inText[index] != '\"') {
            if (((inText[index] == '\\') && (inText[index + 1] == '\"')) ||
                ((inText[index] == '\\') && (inText[index + 1] == '\\'))) {
              ++index;
            }

            ++index;
          }

          ++index;
          lastWord = null;
        }
        else if (index < (inText.Length - 1) && (inText[index] == '/') && (inText[index + 1] == '/')) {
          while (index < inText.Length) {
            if (inText[index] == '\n') {
              break;
            }

            ++index;

            if ((inText[index - 1] == '\r') && (index < inText.Length) && inText[index] == '\n') {
              break;
            }
          }
        }
        else if (index < (inText.Length - 1) && (inText[index] == '/') && (inText[index + 1] == '*')) {
          index += 2;

          while (index < inText.Length) {
            ++index;

            if ((inText[index - 1] == '*') && (index < inText.Length) && inText[index] == '/') {
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

      streamReader.Close();
    }

    private static readFile(string name) {
      StreamReader streamReader = new(m_path + name + ".cs");
      String inText = streamReader.ReadToEnd();
      StringBuilder outBuffer = new();
      int index = 0;

      while (index < inText.Length) {
        if ((index < (inText.Length - 3)) && inText.Substring(index, 4).Equals("new(")) {
          int insertIndex = index - 1;

          while ((insertIndex > 0) && Char.IsWhiteSpace(inText[insertIndex])) {
            --insertIndex;
          }

          if ((insertIndex > 0) && (inText[insertIndex] == '=')) {
            --insertIndex;

            while ((insertIndex > 0) && Char.IsWhiteSpace(inText[insertIndex])) {
              --insertIndex;
            }

            if ((insertIndex > 0) && (Char.IsLetterOrDigit(inText[insertIndex]) || (inText[insertIndex] == '_'))) {
              while ((insertIndex > 0) && (Char.IsLetterOrDigit(inText[insertIndex]) || (inText[insertIndex] == '_'))) {
                --insertIndex;
              }

              while ((insertIndex > 0) && Char.IsWhiteSpace(inText[insertIndex])) {
                --insertIndex;
              }

              bool generics = false;
              if ((insertIndex > 0) && (inText[insertIndex] == '>')) {
                generics = true;
                while ((insertIndex > 0) && (inText[insertIndex] != '<')) {
                  --insertIndex;
                }

                --insertIndex;
              }

              if ((insertIndex > 0) && (Char.IsLetterOrDigit(inText[insertIndex]) || (inText[insertIndex] == '_'))) {
                int endIndex = insertIndex;
                while ((insertIndex > 0) && (Char.IsLetterOrDigit(inText[insertIndex]) || (inText[insertIndex] == '_'))) {
                  --insertIndex;
                }

                String word = inText.Substring(insertIndex + 1, endIndex - insertIndex);
                outBuffer.Append("new ");
                outBuffer.Append(word);
                if (generics) {
                  outBuffer.Append("<>");
                }
                outBuffer.Append('(');
              }
            }
          }

          index += 4;
        }
        else if (char.IsLetter(inText[index])) {
          int wordIndex = index;
          while (Char.IsLetterOrDigit(inText[index]) || (inText[index] == '_')) {
            ++index;
          }

          String word = inText.Substring(wordIndex, index - wordIndex);

          if (m_globalMap.ContainsKey(word)) {
            outBuffer.Append(m_globalMap[word]);
          }
          else {
            outBuffer.Append(word);

            if (Char.IsUpper(word[0])) {
              //Console.Out.WriteLine(word);
            }
          }
        }
        else if (inText[index] == '\'') {
          outBuffer.Append(inText[index++]);

          while (inText[index] != '\'') {
            if (((inText[index] == '\\') && (inText[index + 1] == '\'')) ||
                ((inText[index] == '\\') && (inText[index + 1] == '\\'))) {
              outBuffer.Append(inText[index++]);
            }

            outBuffer.Append(inText[index++]);
          }

          outBuffer.Append(inText[index++]);
        }
        else if (inText[index] == '\"') {
          outBuffer.Append(inText[index++]);

          while (inText[index] != '\"') {
            if (((inText[index] == '\\') && (inText[index + 1] == '\"')) ||
                ((inText[index] == '\\') && (inText[index + 1] == '\\'))) {
              outBuffer.Append(inText[index++]);
            }

            outBuffer.Append(inText[index++]);
          }

          outBuffer.Append(inText[index++]);
        }
        else if (index < (inText.Length - 1) && (inText[index] == '/') && (inText[index + 1] == '/')) {
          while (index < inText.Length) {
            if (inText[index] == '\n') {
              break;
            }

            outBuffer.Append(inText[index++]);

            if ((inText[index - 1] == '\r') && (index < inText.Length) && inText[index] == '\n') {
              break;
            }
          }
        }
        else if (index < (inText.Length - 1) && (inText[index] == '/') && (inText[index + 1] == '*')) {
          outBuffer.Append("/*");
          index += 2;

          while (index < inText.Length) {
            outBuffer.Append(inText[index++]);

            if ((inText[index - 1] == '*') && (index < inText.Length) && inText[index] == '/') {
              break;
            }
          }
        }
        else {
          outBuffer.Append(inText[index++]);
        }
      }

      StreamWriter streamWriter = new(m_path + name + ".java");
      streamWriter.Write(outBuffer);
      streamWriter.Close();
    }
  }
}
