global using System;
global using System.IO;
global using System.Text;
global using System.Linq;
global using System.Numerics;
global using System.Globalization;
global using System.Collections.Generic;

namespace CCompiler {
  public class Start {
    private static string m_outPath = @"C:\JavaX\MyProject\";
    private static Dictionary<String, String> m_globalMap = new();

    public static void Main(string[] args) {
      string[] nameList = {"AssemblyCode", "AssemblyCodeGenerator", "AssemblyOperator",
                           "ConstantExpression", "Declarator", "Error", "Expression",
                           /*"ExpressionParser", "ExpressionScanner",*/ "Global", "Graph",
                           /*"GraphPair",*/ "IfElseChain", "Initializer", "Linker", "Macro",
                           "Main", /*"MainParser", "MainScanner",*/ "Mask", "Message",
                           "MiddleCode", "MiddleCodeGenerator","MiddleCodeOptimizer",
                           "MiddleOperator", "ModifyInitializer", "ObjectCodeComparer",
                           "ObjectCodeInfo", /*"ObjectCodeTable", "Pair",*/
                           "PartialExpressionParser", "PartialMainParser",
                           "PartialPreprocessorParser", /*"PreParser",*/ "Preprocessor",
                           /*"PreScanner",*/ "Register", "RegisterAllocator", "Scope",
                           "Slash", "Sort", "Specifier", "Statement", "StaticBase",
                           "StaticExpression", "StaticSymbol", "StaticSymbolLinux",
                           "StaticSymbolWindows", "Storage", "Symbol", "SymbolTable",
                           "Token", "Track", "Type", "TypeCast", "TypeSize"};

      foreach (String name in nameList) {
        Console.Out.WriteLine("Slaching " + name);
        doSlash(name);
      }
    }

    private static void doSlash(string name) {
      StreamReader streamReader = new(m_outPath + name + ".java");
      String inText = streamReader.ReadToEnd();
      streamReader.Close();

      int index = 0;
      StringBuilder outBuffer = new();
      while (index < inText.Length) {
        if (inText[index] == '\\') {
          int asciiValue = (64 * int.Parse(inText[index + 1].ToString())) +
                           (8 * int.Parse(inText[index + 2].ToString())) +
                           int.Parse(inText[index + 3].ToString());
          outBuffer.Append((char) asciiValue);
          index += 3;
        }
        else {
          outBuffer.Append(inText[index]);
        }

        ++index;
      }

      StreamWriter streamWriter = new(m_outPath + name + ".java");
      streamWriter.Write(outBuffer);
      streamWriter.Close();
    }
    private static char octalToChar(String text) {
      int asciiValue = (64 * ((int) text[1])) +
                       (8 * ((int) text[2])) +
                       ((int) text[2]);
      return ((char) asciiValue);
    }

  }
}
