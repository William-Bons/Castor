using Castor.gui.common;
using Castor.Properties;
using EfSchemaCompare;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Castor.database.tables
{

    [Serializable]
    public class CastorUpdater
    {
        private CompareEfSql comparer = new CompareEfSql();
        
        public CastorUpdater()
        {
        }

        public List<Update> updates = new List<Update>();

        public static CastorUpdater? Load()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CastorUpdater));
            using (FileStream fs = new FileStream("assets/updates.xml", FileMode.OpenOrCreate))
            {
                return xmlSerializer.Deserialize(fs) as CastorUpdater;
            }
        }

        public bool ProcessingUpdateDatabase()
        {
            if (comparer.CompareEfWithDb(new CastorContext()))
            {
                string Errors = comparer.GetAllErrors;
                List<string> ExpectedIdsForUpdate = Errors.Split("\r\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Where(l => Regex.IsMatch(l, @"^NOT IN DATABASE:"))
                    .Select(l => Regex.Match(l, @"(?<=Expected =\s+)\w+").Value)
                    .ToList();


                CastorUpdater? updater = Load();

                List<string> offsets = Settings.Default.UpdatesRun.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
                List<string> queries = new List<string>();

                try
                {
                    updater?.updates
                        .Where(u => offsets.Contains(u.Id) == false)
                        .ToList()
                        .ForEach(u =>
                        {
                            using CastorContext castor = new CastorContext();
                            castor.Database.ExecuteSqlRaw(u.Value);
                            queries.Add(u.Value);
                            offsets.Add($"{u.Id};");
                            Settings.Default.UpdatesRun = string.Join(';', offsets);
                            Settings.Default.Save();
                        });

                    Message.ShowPopup(string.Join('\n', queries));
                }
                catch (Exception ex)
                {
                    Message.ShowPopup(ex.Message);
                    return false;
                }
            }
            return true;
        }
    }

    public struct Update
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}
