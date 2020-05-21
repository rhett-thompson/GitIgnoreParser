using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SourceryCode
{
    public class GitIgnoreParser
    {

        private IEnumerable<string> exceptLines;
        private IEnumerable<string> ignoreLines;

        private IEnumerable<Regex> exceptTests;
        private IEnumerable<Regex> ignoreTests;

        public void ParseFromFile(string path)
        {
            ParseFromLines(File.ReadAllLines(path));
        }

        public void ParseFromString(string str)
        {
            ParseFromLines(str.Split('\n'));
        }

        public void ParseFromLines(string[] lines)
        {
            var parsedLines = lines.Select(x => x.Trim()).Where(x => !x.StartsWith("#") && !string.IsNullOrEmpty(x)).Distinct();

            exceptLines = parsedLines.Where(x => x.StartsWith("!"));
            ignoreLines = parsedLines.Except(exceptLines);

            exceptTests = exceptLines.Select(x => ProcessRegex(x.Substring(1)));
            ignoreTests = ignoreLines.Select(x => ProcessRegex(x));

        }

        private Regex ProcessRegex(string line)
        {

            line = line.Replace("*", "(.+)");

            return new Regex(line, RegexOptions.Compiled | RegexOptions.Singleline);

        }

        public bool Allowed(string path)
        {
            var allowed = ignoreTests.Any(m => m.IsMatch(path));
            return allowed;
        }

    }
}
