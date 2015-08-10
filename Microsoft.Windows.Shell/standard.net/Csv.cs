namespace Standard
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Text;

    internal static class Csv
    {
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static string Escape(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }

            if (str.Contains("\""))
            {
                str = str.Replace("\"", "\"\"");
            }

            if (str.IndexOfAny(new char[] { ',', '"', '\n' }) > -1)
            {
                str = '\"' + str + '\"';
            }

            return str;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static string MakeLine(string[] data)
        {
            var sb = new StringBuilder();
            foreach (var cell in data)
            {
                sb.Append(Escape(cell));
                sb.Append(',');
            }

            return sb.ToString(0, sb.Length - 1);
        }

        // Read a logical line from the stream.  It may contain embedded newlines if it was CSV-style escaped.
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private static string _ReadLine(StreamReader reader)
        {
            int i = 0;
            bool inEscape = false;
            bool lastChance = false;

            var sb = new StringBuilder(reader.ReadLine());
            
            while (true)
            {
                for (; i < sb.Length; ++i)
                {
                    if (sb[i] == '\"')
                    {
                        inEscape = !inEscape; 
                    }
                }
                if (!inEscape)
                {
                    return sb.ToString();
                }
                sb.Append("\n");
                if (lastChance)
                {
                    throw new ArgumentException("Invalid CSV data.");
                }
                sb.Append(reader.ReadLine());
                lastChance = reader.EndOfStream;
            }
        }

        private static readonly string bellString = Encoding.ASCII.GetString(new byte[] { 7 });

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private static List<string> _ParseLine(string row)
        {
            if (!row.Contains('\"'))
            {
                return new List<string>(row.Split(','));
            }

            // Make it simpler to not have to look for escaped quotes.
            row = row.Replace("\"\"", bellString);

            var ret = new List<string>();
            int startIndex = 0;
            for (int i = 0; i < row.Length; ++i)
            {
                if (row[i] == '\"')
                {
                    // Skip the opening quote
                    startIndex = i + 1;
                    while (row[++i] != '\"')
                    { }

                    // Remove the trailing quote, and replace back any embedded quotes.
                    ret.Add(row.Substring(startIndex, i - startIndex).Replace((char)7, '\"'));

                    ++i;
                    if (i < row.Length && row[i] != ',')
                    {
                        throw new ArgumentException("Malformed data.");
                    }
                    startIndex = i + 1;
                }
                else
                {
                    if (i >= row.Length - 1 || row[i] == ',')
                    {
                        ret.Add("");
                        startIndex = i + 1;
                        continue;
                    }

                    while (i < row.Length-1 && row[++i] != ',')
                    { }

                    ret.Add(row.Substring(startIndex, i - startIndex));

                    if (i < row.Length-1 && row[i] != ',')
                    {
                        throw new ArgumentException("Malformed data.");
                    }
                    startIndex = i + 1;
                }
            }

            if (row.EndsWith(",", StringComparison.Ordinal))
            {
                ret.Add("");
            }

            return ret;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static List<Dictionary<string, string>> ReadDocument(StreamReader reader)
        { 
            List<string> headers = _ParseLine(_ReadLine(reader));
            var ret = new List<Dictionary<string, string>>();

            while (!reader.EndOfStream)
            {
                var rowData = new Dictionary<string,string>();
                List<string> cells = _ParseLine(_ReadLine(reader));
                if (cells.Count != headers.Count)
                {
                    throw new ArgumentException("Bad CSV file.");
                }

                for (int i = 0; i < headers.Count; ++i)
                {
                    rowData.Add(headers[i], cells[i]);
                }
                ret.Add(rowData);
            }

            return ret;
        }
    }
}
