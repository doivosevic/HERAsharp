using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace herad
{
    class Settings
    {
        public string Folder { get; set; }

        private string _ReadsPath;
        public string ReadsPath {
            get { return Path.Combine(Folder, _ReadsPath); }
            set { _ReadsPath = value; }
        }

        private string _ContigsPath;
        public string ContigsPath
        {
            get { return Path.Combine(Folder, _ContigsPath); }
            set => _ContigsPath = value;
        }

        private string _ReadToReadPath;
        public string ReadToReadPath
        {
            get { return Path.Combine(Folder, _ReadToReadPath); }
            set { _ReadToReadPath = value; }
        }

        private string _ReadToContigPath;
        public string ReadToContigPath
        {
            get { return Path.Combine(Folder, _ReadToContigPath); }
            set { _ReadToContigPath = value; }
        }

        public void FileToSettings(string path)
        {
            var settingsFileExists = false;
            if (File.Exists(path))
            {
                var lines = File.ReadAllLines(path);

                
                if (TryParseSettingsFile(lines))
                {
                    settingsFileExists = true;
                }
            }
            
            if (!settingsFileExists)
            {
                throw new Exception($"Settings file could not be parsed. Check if file is correctly set.");
            }
        }

        private bool TryParseSettingsFile(string[] lines)
        {
            try
            {
                // this.folder = ParseLine(lines[0]);
                this.Folder = lines[0];
                this.ReadsPath = lines[1];
                this.ContigsPath = lines[1];
                this.ReadToReadPath = lines[1];
                this.ReadToContigPath = lines[1];

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
        private string ParseLine(string line)
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
