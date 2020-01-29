using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace herad
{
    public static class Settings
    {
        public static string Folder { get; set; }

        private static string _ReadsPath;
        public static string ReadsPath {
            get { return Path.Combine(Folder, _ReadsPath); }
            set { _ReadsPath = value; }
        }

        private static string _ContigsPath;
        public static string ContigsPath
        {
            get { return Path.Combine(Folder, _ContigsPath); }
            set => _ContigsPath = value;
        }

        private static string _ReadToReadPath;
        public static string ReadToReadPath
        {
            get { return Path.Combine(Folder, _ReadToReadPath); }
            set { _ReadToReadPath = value; }
        }

        private static string _ReadToContigPath;
        public static string ReadToContigPath
        {
            get { return Path.Combine(Folder, _ReadToContigPath); }
            set { _ReadToContigPath = value; }
        }

        private static string _ResultingFileName;
        public static string ResultingFileName
        {
            get { return Path.Combine(Folder, _ResultingFileName); }
            set { _ResultingFileName = value; }
        }

        public static bool FileToSettings(string path)
        {
            var successfullyParsed = false;
            if (File.Exists(path))
            {
                var lines = File.ReadAllLines(path);

                
                if (TryParseSettingsFile(lines))
                {
                    successfullyParsed = true;
                }
            }

            return successfullyParsed;
        }

        private static bool TryParseSettingsFile(string[] lines)
        {
            try
            {
                // this.folder = ParseLine(lines[0]);
                Folder = lines[0];
                ReadsPath = lines[1];
                ContigsPath = lines[2];
                ReadToReadPath = lines[3];
                ReadToContigPath = lines[4];
                ResultingFileName = lines[5];

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Parse settings line if it has 'value name = value' format.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static string ParseLine(string line)
        {
            string parser = "=";
            try
            {
                var data = line.Split(parser).Select(p => p.Trim()).ToList();
                if (data.Count != 2)
                {
                    return "";
                }

                return data[1];
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                return "";
            }
        }
    }
}
