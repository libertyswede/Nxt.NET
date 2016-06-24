using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Nxt.NET.Util
{
    public class PropertiesReader
    {
        private const int MATCH_end_of_input = 1;
        private const int MATCH_terminator = 2;
        private const int MATCH_whitespace = 3;
        private const int MATCH_any = 4;

        private const int ACTION_add_to_key = 1;
        private const int ACTION_add_to_value = 2;
        private const int ACTION_store_property = 3;
        private const int ACTION_escape = 4;
        private const int ACTION_ignore = 5;

        private const int STATE_start = 0;
        private const int STATE_comment = 1;
        private const int STATE_key = 2;
        private const int STATE_key_escape = 3;
        private const int STATE_key_ws = 4;
        private const int STATE_before_separator = 5;
        private const int STATE_after_separator = 6;
        private const int STATE_value = 7;
        private const int STATE_value_escape = 8;
        private const int STATE_value_ws = 9;
        private const int STATE_finish = 10;

        private static string[] stateNames = new string[]
        { "STATE_start", "STATE_comment", "STATE_key", "STATE_key_escape", "STATE_key_ws",
            "STATE_before_separator", "STATE_after_separator", "STATE_value", "STATE_value_escape",
            "STATE_value_ws", "STATE_finish" };

        private static readonly int[][] states = new int[][] {
            new int[]{//STATE_start
                MATCH_end_of_input, STATE_finish,           ACTION_ignore,
                MATCH_terminator,   STATE_start,            ACTION_ignore,
                '#',                STATE_comment,          ACTION_ignore,
                '!',                STATE_comment,          ACTION_ignore,
                MATCH_whitespace,   STATE_start,            ACTION_ignore,
                '\\',               STATE_key_escape,       ACTION_escape,
                ':',                STATE_after_separator,  ACTION_ignore,
                '=',                STATE_after_separator,  ACTION_ignore,
                MATCH_any,          STATE_key,              ACTION_add_to_key,
            },
            new int[]{//STATE_comment
                MATCH_end_of_input, STATE_finish,           ACTION_ignore,
                MATCH_terminator,   STATE_start,            ACTION_ignore,
                MATCH_any,          STATE_comment,          ACTION_ignore,
            },
            new int[]{//STATE_key
                MATCH_end_of_input, STATE_finish,           ACTION_store_property,
                MATCH_terminator,   STATE_start,            ACTION_store_property,
                MATCH_whitespace,   STATE_before_separator, ACTION_ignore,
                '\\',               STATE_key_escape,       ACTION_escape,
                ':',                STATE_after_separator,  ACTION_ignore,
                '=',                STATE_after_separator,  ACTION_ignore,
                MATCH_any,          STATE_key,              ACTION_add_to_key,
            },
            new int[]{//STATE_key_escape
                MATCH_terminator,   STATE_key_ws,           ACTION_ignore,
                MATCH_any,          STATE_key,              ACTION_add_to_key,
            },
            new int[]{//STATE_key_ws
                MATCH_end_of_input, STATE_finish,           ACTION_store_property,
                MATCH_terminator,   STATE_start,            ACTION_store_property,
                MATCH_whitespace,   STATE_key_ws,           ACTION_ignore,
                '\\',               STATE_key_escape,       ACTION_escape,
                ':',                STATE_after_separator,  ACTION_ignore,
                '=',                STATE_after_separator,  ACTION_ignore,
                MATCH_any,          STATE_key,              ACTION_add_to_key,
            },
            new int[]{//STATE_before_separator
                MATCH_end_of_input, STATE_finish,           ACTION_store_property,
                MATCH_terminator,   STATE_start,            ACTION_store_property,
                MATCH_whitespace,   STATE_before_separator, ACTION_ignore,
                '\\',               STATE_value_escape,     ACTION_escape,
                ':',                STATE_after_separator,  ACTION_ignore,
                '=',                STATE_after_separator,  ACTION_ignore,
                MATCH_any,          STATE_value,            ACTION_add_to_value,
            },
            new int[]{//STATE_after_separator
                MATCH_end_of_input, STATE_finish,           ACTION_store_property,
                MATCH_terminator,   STATE_start,            ACTION_store_property,
                MATCH_whitespace,   STATE_after_separator,  ACTION_ignore,
                '\\',               STATE_value_escape,     ACTION_escape,
                MATCH_any,          STATE_value,            ACTION_add_to_value,
            },
            new int[]{//STATE_value
                MATCH_end_of_input, STATE_finish,           ACTION_store_property,
                MATCH_terminator,   STATE_start,            ACTION_store_property,
                '\\',               STATE_value_escape,     ACTION_escape,
                MATCH_any,          STATE_value,            ACTION_add_to_value,
            },
            new int[]{//STATE_value_escape
                MATCH_terminator,   STATE_value_ws,         ACTION_ignore,
                MATCH_any,          STATE_value,            ACTION_add_to_value
            },
            new int[]{//STATE_value_ws
                MATCH_end_of_input, STATE_finish,           ACTION_store_property,
                MATCH_terminator,   STATE_start,            ACTION_store_property,
                MATCH_whitespace,   STATE_value_ws,         ACTION_ignore,
                '\\',               STATE_value_escape,     ACTION_escape,
                MATCH_any,          STATE_value,            ACTION_add_to_value,
            }
        };

        private Dictionary<string, string> properties;

        private const int bufferSize = 1000;

        private bool escaped = false;
        private StringBuilder keyBuilder = new StringBuilder();
        private StringBuilder valueBuilder = new StringBuilder();
        
        public PropertiesReader(Dictionary<string, string> properties)
        {
            this.properties = properties;
        }
        
        public void Parse(Stream stream)
        {
            Parse(stream, null);
        }

        public void Parse(Stream stream, Encoding encoding)
        {
            var bufferedStream = new BufferedStream(stream, bufferSize);
            var parserEncoding = encoding ?? Encoding.GetEncoding(28592);
            reader = new BinaryReader(bufferedStream, parserEncoding);

            int state = STATE_start;
            do
            {
                int ch = nextChar();

                bool matched = false;

                for (int s = 0; s < states[state].Length; s += 3)
                {
                    if (matches(states[state][s], ch))
                    {
                        matched = true;
                        doAction(states[state][s + 2], ch);

                        state = states[state][s + 1];
                        break;
                    }
                }

                if (!matched)
                {
                    throw new ParseException("Unexpected character at " + 1 + ": <<<" + ch + ">>>");
                }
            } while (state != STATE_finish);
        }

        private bool matches(int match, int ch)
        {
            switch (match)
            {
                case MATCH_end_of_input:
                    return ch == -1;

                case MATCH_terminator:
                    if (ch == '\r')
                    {
                        if (peekChar() == '\n')
                        {
                            saved = false;
                        }
                        return true;
                    }
                    else if (ch == '\n')
                    {
                        return true;
                    }
                    return false;

                case MATCH_whitespace:
                    return ch == ' ' || ch == '\t' || ch == '\f';

                case MATCH_any:
                    return true;

                default:
                    return ch == match;
            }
        }

        private void doAction(int action, int ch)
        {
            switch (action)
            {
                case ACTION_add_to_key:
                    keyBuilder.Append(escapedChar(ch));
                    escaped = false;
                    break;

                case ACTION_add_to_value:
                    valueBuilder.Append(escapedChar(ch));
                    escaped = false;
                    break;

                case ACTION_store_property:
                    properties[keyBuilder.ToString()] = valueBuilder.ToString();
                    keyBuilder.Length = 0;
                    valueBuilder.Length = 0;
                    escaped = false;
                    break;

                case ACTION_escape:
                    escaped = true;
                    break;

                //case ACTION_ignore:
                default:
                    escaped = false;
                    break;
            }
        }

        private char escapedChar(int ch)
        {
            if (escaped)
            {
                switch (ch)
                {
                    case 't':
                        return '\t';
                    case 'r':
                        return '\r';
                    case 'n':
                        return '\n';
                    case 'f':
                        return '\f';
                    case 'u':
                        int uch = 0;
                        for (int i = 0; i < 4; i++)
                        {
                            ch = nextChar();
                            if (ch >= '0' && ch <= '9')
                            {
                                uch = (uch << 4) + ch - '0';
                            }
                            else if (ch >= 'a' && ch <= 'z')
                            {
                                uch = (uch << 4) + ch - 'a' + 10;
                            }
                            else if (ch >= 'A' && ch <= 'Z')
                            {
                                uch = (uch << 4) + ch - 'A' + 10;
                            }
                            else
                            {
                                throw new ParseException("Invalid Unicode character.");
                            }
                        }
                        return (char)uch;
                }
            }

            return (char)ch;
        }
        
        private BinaryReader reader = null;
        private int savedChar;
        private bool saved = false;

        private int nextChar()
        {
            if (saved)
            {
                saved = false;
                return savedChar;
            }

            return ReadCharSafe();
        }

        private int peekChar()
        {
            if (saved)
            {
                return savedChar;
            }

            saved = true;
            return savedChar = ReadCharSafe();
        }
        
        private int ReadCharSafe()
        {
            if (reader.BaseStream.Position == reader.BaseStream.Length)
            {
                return -1;
            }
            return reader.ReadChar();
        }
    }
}
