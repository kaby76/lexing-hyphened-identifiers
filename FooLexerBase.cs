using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

public abstract class FooLexerBase : Lexer
{
    protected FooLexerBase(ICharStream input) : base(input) { }

    protected FooLexerBase(ICharStream input, TextWriter output, TextWriter errorOutput)
        : base(input, output, errorOutput) { }

    protected bool IsHyphenedId()
    {
        var input = (ICharStream)InputStream;
        int start = TokenStartCharIndex;
        // Find the whole contiguous run, then look for a declared prefix.
        // A hyphen beyond that prefix is subtraction (e.g. c-4-5).
        int offset = 1;
        while (IsIdentifierContinuation(input.LA(offset)))
            offset++;
        int end = input.Index + offset - 1;

        string candidate = input.GetText(Interval.Of(start, end - 1));
        string preceding = start == 0 ? string.Empty : input.GetText(Interval.Of(0, start - 1));

        // The declaration must be able to lex its own hyphenated identifier.
        if (Regex.IsMatch(preceding, @"(?:^|[^A-Za-z0-9-])var[ \t\r\n]+$"))
            return true;

        foreach (Match declaration in Regex.Matches(preceding,
                     @"(?:^|[^A-Za-z0-9-])var[ \t\r\n]+([A-Za-z][A-Za-z0-9-]*)[ \t\r\n]*;"))
        {
            string declaredName = declaration.Groups[1].Value;
            if (input.Index - start < declaredName.Length &&
                candidate.StartsWith(declaredName, StringComparison.Ordinal) &&
                (candidate.Length == declaredName.Length || candidate[declaredName.Length] == '-'))
                return true;
        }

        return false;
    }

    private static bool IsIdentifierContinuation(int symbol) =>
        symbol >= 'a' && symbol <= 'z' ||
        symbol >= 'A' && symbol <= 'Z' ||
        symbol >= '0' && symbol <= '9' ||
        symbol == '-';
}
