# Lexing hyphened identifiers

The hyphen in `a-2` could be part of an identifier or a subtraction operator.
This example makes that choice using declarations earlier in the input: a
hyphened name is an identifier only if it was declared with `var`.

`FooLexer.g4` calls `IsHyphenedId()` before consuming each hyphen in an
`IDENTIFIER`. The predicate in `FooLexerBase.cs` looks backward through the
input for declarations of the form `var name;`, where `name` starts with an
ASCII letter and continues with ASCII letters, digits, or hyphens. While
lexing the name in a `var` declaration, it allows the hyphen by default.
Elsewhere, it looks ahead across the contiguous identifier-like text and
allows a hyphen only while it belongs to a previously declared name. A later
hyphen can therefore remain the `MINUS` token.

For `t1.txt`, the significant tokens are:

| Input | Tokens |
| --- | --- |
| `a-2;` | `IDENTIFIER(a) MINUS NUMBER(2) SEMI` |
| `b-3;` | `IDENTIFIER(b) MINUS NUMBER(3) SEMI` |
| `var c-4;` | `VAR_ IDENTIFIER(c-4) SEMI` |
| `var c-4-6;` | `VAR_ IDENTIFIER(c-4-6) SEMI` |
| `c-4-5;` | `IDENTIFIER(c-4) MINUS NUMBER(5) SEMI` |
| `c-4-6;` | `IDENTIFIER(c-4-6) SEMI` |

The lookup uses the text before the current token, not parser scope or symbol
resolution. In particular, it does not distinguish declarations inside comments
or strings; this small grammar has neither.

