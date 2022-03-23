global using System;
global using System.IO;
global using System.Text;
global using System.Linq;
global using System.Numerics;
global using System.Globalization;
global using System.Collections.Generic;

namespace CCompiler {
  public class Start {
    private static string m_inPath = @"C:\Users\stefa\Documents\A A C_Compiler_CSharp_10\C_Compiler_CSharp_8\";
    //private static string m_outPath = @"C:\Users\stefa\Documents\A A C_Compiler_CSharp_10_Read\C_Compiler_CSharp_8\";
    private static string m_outPath = @"C:\JavaX\MyProject\";
    //private static StreamWriter m_logStream;
    private static Dictionary<String, String> m_globalMap = new();

    public static void Main(string[] args) {
      string[] nameList = {"AssemblyCode", "AssemblyCodeGenerator", "AssemblyOperator",
                           "ConstantExpression", "Declarator", "Error", "Expression",
                           /*"ExpressionParser", "ExpressionScanner",*/ "Global", "Graph",
                           "GraphPair", "IfElseChain", "Initializer", "Linker", "Macro",
                           "Main", /*"MainParser", "MainScanner",*/ "Mask", "Message",
                           "MiddleCode", "MiddleCodeGenerator","MiddleCodeOptimizer",
                           "MiddleOperator", "ModifyInitializer", "ObjectCodeComparer",
                           "ObjectCodeInfo", /*"ObjectCodeTable",*/ "Pair",
                           "PartialExpressionParser", "PartialMainParser",
                           "PartialPreprocessorParser", /*"PreParser",*/ "Preprocessor",
                           /*"PreScanner",*/ "Register", "RegisterAllocator", "Scope",
                           "Slash", "Sort", "Specifier", "Statement", "StaticBase",
                           "StaticExpression", "StaticSymbol", "StaticSymbolLinux",
                           "StaticSymbolWindows", "Storage", "Symbol", "SymbolTable",
                           "Token", "Track", "Type", "TypeCast", "TypeSize"};

      foreach (String name in nameList) {
        Console.Out.WriteLine("Reading " + name);
        readFile(name);
      }
      Console.Out.WriteLine();

      //m_logStream = new(m_inPath + "log");
      foreach (String name in nameList) {
        Console.Out.WriteLine("Writing " + name);
        writeFile(name);
      }
      //m_logStream.Close();
    }

    private static void readFile(string name) {
      StreamReader streamReader = new(m_inPath + name + ".cs");
      String inText = streamReader.ReadToEnd();
      String? lastWord = null;

      int index = 0, line = 1;
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
          if ((inText[index] == '(') && (lastWord != null) && Char.IsUpper(oldWord[0])) {
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
        else if ((index < (inText.Length - 1)) && (inText[index] == '@') && (inText[index + 1] == '\'')) {
          index += 2;

          while (inText[index] != '\'') {
            ++index;
          }

          ++index;
          lastWord = null;
        }
        else if ((index < (inText.Length - 1)) && (inText[index] == '@') && (inText[index + 1] == '\"')) {
          index += 2;

          while (inText[index] != '\"') {
            ++index;
          }

          ++index;
          lastWord = null;
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

          ++line;
        }
        else if (index < (inText.Length - 1) && (inText[index] == '/') && (inText[index + 1] == '*')) {
          ++index;

          while (index < inText.Length) {
            if (inText[line] == '\n') {
              ++line;
            }
            ++index;

            if ((inText[index - 1] == '*') && (index < inText.Length) && inText[index] == '/') {
              ++index;
              break;
            }
          }
        }
        else if (index < inText.Length) {
          if (inText[index] == '\n') {
            ++line;
          }

          ++index;
          lastWord = null;
        }
      }

      streamReader.Close();
    }

    private static void writeFile(string name) {
      StreamReader streamReader = new(m_inPath + name + ".cs");
      String preText = streamReader.ReadToEnd();
      
      StringBuilder preBuffer = new();
      for (int preIndex = 0; preIndex < preText.Length; ++preIndex) {
        preBuffer.Append(preText[preIndex]);

        if ((preIndex < (preText.Length - 2)) && (preText[preIndex] == '\n') &&
            (preText[preIndex + 1] == ' ') && (preText[preIndex + 2] == ' ')) {
          preIndex += 2;
        }
      }

      String inText = preBuffer.ToString();
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

            /*if (Char.IsUpper(word[0])) {
              m_logStream.WriteLine(word);
            }*/
          }
        }
        else if ((index < (inText.Length - 1)) && (inText[index] == '@') && (inText[index + 1] == '\'')) {
          outBuffer.Append(inText[index++]);
          outBuffer.Append(inText[index++]);

          while (inText[index] != '\'') {
            outBuffer.Append(charToOctal(inText[index++]));
          }

          outBuffer.Append(inText[index++]);
        }
        else if ((index < (inText.Length - 1)) && (inText[index] == '@') && (inText[index + 1] == '\"')) {
          outBuffer.Append(inText[index++]);
          outBuffer.Append(inText[index++]);

          while (inText[index] != '\"') {
            outBuffer.Append(charToOctal(inText[index++]));
          }

          outBuffer.Append(inText[index++]);
        }
        else if (inText[index] == '\'') {
          outBuffer.Append(inText[index++]);

          while (inText[index] != '\'') {
            if (((inText[index] == '\\') && (inText[index + 1] == '\'')) ||
                ((inText[index] == '\\') && (inText[index + 1] == '\\'))) {
              outBuffer.Append(charToOctal(inText[index++]));
            }

            outBuffer.Append(charToOctal(inText[index++]));
          }

          outBuffer.Append(inText[index++]);
        }
        else if (inText[index] == '\"') {
          outBuffer.Append(inText[index++]);

          while (inText[index] != '\"') {
            if (((inText[index] == '\\') && (inText[index + 1] == '\"')) ||
                ((inText[index] == '\\') && (inText[index + 1] == '\\'))) {
              outBuffer.Append(charToOctal(inText[index++]));
            }

            outBuffer.Append(charToOctal(inText[index++]));
          }

          outBuffer.Append(inText[index++]);
        }
        else if (index < (inText.Length - 1) && (inText[index] == '/') && (inText[index + 1] == '/')) {
          outBuffer.Append(inText[index++]);
          outBuffer.Append(inText[index++]);

          while (index < inText.Length) {
            if (inText[index] == '\n') {
              break;
            }

            if ((index < (inText.Length - 1) && (inText[index] == '\r') && inText[index + 1] == '\n')) {
              break;
            }

            outBuffer.Append(charToOctal(inText[index++]));
          }
        }
        else if (index < (inText.Length - 1) && (inText[index] == '/') && (inText[index + 1] == '*')) {
          outBuffer.Append("/*");
          index += 2;

          while (index < inText.Length) {
            if (inText[index] == '\n') {
              outBuffer.Append('\n');
              ++index;
            }
            else if ((index < (inText.Length - 1) && (inText[index] == '\r') && inText[index + 1] == '\n')) {
              outBuffer.Append('\r');
              outBuffer.Append('\n');
              index += 2;
            }
            else if ((index < (inText.Length - 1)) && (inText[index] == '*') && inText[index + 1] == '/') {
              break;
            }
            else {
              outBuffer.Append(charToOctal(inText[index++]));
            }
          }
        }
        else {
          outBuffer.Append(inText[index++]);
        }
      }

      outBuffer.Replace(" object ", " Object ");
      outBuffer.Replace(" register ", " Register ");
      outBuffer.Replace(" const ", " final ");
      outBuffer.Replace(" RegularFrameRegister ", " RegularFrameRegister ");
      outBuffer.Replace("namespace CCompiler {", "package java.CCompiler;\r\n");
      outBuffer.Replace("using System.Numerics;", "import java.math.*;");
      outBuffer.Replace("using System.Collections.Generic;", "import java.util.*;");
      outBuffer.Replace(" foreach ", " for ");
      outBuffer.Replace(" in ", " : ");
      outBuffer.Replace(" IDictionary ", " Map ");
      outBuffer.Replace(" Dictionary ", " HashMap ");
      outBuffer.Replace("\r\n}\r\n}", "\r\n}");



      StreamWriter streamWriter = new(m_outPath + name + ".java");
      streamWriter.Write(outBuffer);
      streamWriter.Close();
    }

    private static String charToOctal(char c) {
      int asciiValue = (int) c;
      int digit2 = (asciiValue / 64),
          digit1 = ((asciiValue % 64) / 8),
          digit0 = asciiValue % 8;
      return "\\" + digit2.ToString() + digit1.ToString() + digit0.ToString();
    }
  }
}
