using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Linq;
using System.Text;
using CsQuery.ExtensionMethods;
using System.Diagnostics;

namespace CsQuery.Utility.StringScanner
{
    
    // Not implemented - intended to update the scanning code in Selector engine and maybe the HTML parser
    public interface IStringScanner
    {

        string Text {get;set;}
        char[] Chars {get;set;}
        bool IgnoreWhitespace { get; set; }
        int Length {get;}
        int Pos {get;set;}
        int LastPos { get; set;}
        char NextChar {get;}
        char Peek();
        string NextCharOrEmpty {get;}
        string Match {get;}
        string LastMatch {get;}
        bool Finished {get;}
        bool AtEnd {get;}

        bool Success {get;}
        string LastError {get;}
        
        bool AllowQuoting();
        ICharacterInfo Info {get; }

        void SkipWhitespace();
        void NextNonWhitespace();
        bool Next(int count=1);
        bool Prev(int count=1);
        void Undo();
        void End();

        void AssertFinished();
        void AssertNotFinished();
        void Reset();

        bool Is(string text);
        bool IsOneOf(params string[] text);
        bool IsOneOf(IEnumerable<string> text);

        IStringScanner ToNewScanner();
        IStringScanner ToNewScanner(string format);
        IStringScanner Expect(string text);
        IStringScanner Expect(IExpectPattern pattern);
        IStringScanner Expect(Func<int, char, bool> validate);
        IStringScanner ExpectChar(char character);
        IStringScanner ExpectChar(string characters);
        IStringScanner ExpectChar(params char[] characters);
        IStringScanner ExpectChar(IEnumerable<char> characters);
        IStringScanner ExpectNumber();
        IStringScanner ExpectAlpha();
        IStringScanner ExpectBoundedBy(string start, string end, bool allowQuoting=false);
        IStringScanner ExpectBoundedBy(char bound, bool allowQuoting = false);

        bool TryGet(IEnumerable<string> stringList, out string result);
        bool TryGet(IExpectPattern pattern, out string result);
        bool TryGet(Func<int, char, bool> validate, out string result);
        bool TryGetChar(char character, out string result);
        bool TryGetChar(string characters, out string result);
        bool TryGetChar(IEnumerable<char> characters, out string result);
        bool TryGetNumber(out string result);
        bool TryGetNumber<T>(out T result) where T : IConvertible;
        bool TryGetNumber(out int result);
        bool TryGetAlpha(out string result);
        bool TryGetBoundedBy(string start, string end, bool allowQuoting, out string result);

        string Get(params string[] values);
        string Get(IEnumerable<string> stringList);
        string Get(IExpectPattern pattern);
        string Get(Func<int, char, bool> validate);
        string GetNumber();
        string GetAlpha();
        string GetBoundedBy(string start, string end, bool allowQuoting=false);
        string GetBoundedBy(char bound, bool allowQuoting=false);

        char GetChar(char character);
        char GetChar(string characters);
        char GetChar(params char[] characters);
        char GetChar(IEnumerable<char> characters);

        void Expect(params string[] values);
        void Expect(IEnumerable<string> stringList);
    }


}