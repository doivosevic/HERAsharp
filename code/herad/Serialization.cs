using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace herad
{
    public static class Serialization
    {
        private static void Deserialize()
        {
            List<Seq> aSeqs;
            Dictionary<int, List<Overlap>> allOverlaps;

            string aSeqsJsonFileName = "aSeqsJson.txt";
            string allOverlapsFileName = "allOverlapsJson.txt";

            aSeqs = (List<Seq>)JsonConvert.DeserializeObject<List<Seq>>(File.ReadAllText(aSeqsJsonFileName));
            allOverlaps = JsonConvert.DeserializeObject<Dictionary<int, List<Overlap>>>(File.ReadAllText(allOverlapsFileName));

            //MainLogic(aSeqs, allOverlaps);
        }

        private static void SerializeSetupObjects()
        {
            (List<Seq> aSeqs, Dictionary<int, List<Overlap>> allOverlaps) = InitializationCode.SetupVariables();
            string aSeqsJsonFileName = "aSeqsJson.txt";
            string allOverlapsFileName = "allOverlapsJson.txt";

            SerializeToFile(aSeqs, aSeqsJsonFileName);
            SerializeToFile(allOverlaps, allOverlapsFileName);
        }

        private static void SerializeToFile(object objectToSerialize, string fileName)
        {
            JsonSerializer serializer = new JsonSerializer();
            //serializer.Converters.Add(new JavaScriptDateTimeConverter());
            //serializer.NullValueHandling = NullValueHandling.Ignore;

            using (StreamWriter sw = new StreamWriter(fileName))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, objectToSerialize);
                // {"ExpiryDate":new Date(1230375600000),"Price":0}
            }
        }
    }
}
