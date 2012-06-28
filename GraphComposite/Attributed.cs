using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Digraph
{
    public class Attributed
    {
        private Dictionary<string, string> _attribs = new Dictionary<string, string>();

        public Dictionary<string, string> attribs
        {
            get
            {
                return _attribs;
            }
            set
            {
                _attribs = value;
            }
        }

        public Attributed() { }

        public Attributed(string key, string value)
        {
            _attribs.Add(key, value);
        }

        public Attributed(string[] keyVals)
        {
            char[] seperator = { '=' };
            foreach (string s in keyVals)
            {
                string[] keyValPair = s.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
                if (keyValPair.Length == 2)
                    _attribs.Add(keyValPair[0], keyValPair[1]);
            }
        }

        public Attributed(Attributed a)
        {
            string val;
            foreach (string key in a._attribs.Keys)
            {
                if (!_attribs.TryGetValue(key, out val))
                {
                    throw new Exception("shouldn't get here if single-threaded");
                }
                _attribs.Add(key, val);
            }
        }

        // construct from a config file
        public Attributed(string fileName, char delimiter)
        {
            StreamReader reader = new StreamReader(fileName);

            char[] delims = { delimiter };
            string[] splitLine;
            string line = reader.ReadLine();
            while (line != null)
            {
                splitLine = line.Split(delims, StringSplitOptions.RemoveEmptyEntries);
                if (splitLine.Length >= 2)
                    _attribs.Add(splitLine[0], splitLine[1]);
                line = reader.ReadLine();
            }
            reader.Close();
        }

        public string this[string key]
        {
            get
            {
                return _attribs[key];
            }
            set
            {
                _attribs[key] = value;
            }
        }

        // append entries from src into this
        // on matching keys, overwrite with values from src
        public void graft(Attributed src)
        {
            foreach (KeyValuePair<string, string> kvp in src.attribs)
            {
                if (hasKey(kvp.Key))
                    attribs[kvp.Key] = kvp.Value;
                else
                    attribs.Add(kvp.Key, kvp.Value);
            }
        }

        public bool hasKey(string key)
        {
            string val;
            return _attribs.TryGetValue(key, out val);
        }
    }
}
