using Castor.gui.common;
using Castor.Properties;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Xml.Serialization;

namespace Castor.database.tables
{

    [Serializable]
    public class CastorUpdater
    {

        public CastorUpdater()
        {
        }

        public List<Update> updates { get; set; } = new List<Update>();

        public static CastorUpdater Load()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CastorUpdater));
            using (FileStream fs = new FileStream("assets/updates.xml", FileMode.OpenOrCreate))
            {
                return xmlSerializer.Deserialize(fs) as CastorUpdater;
            }
        }

        public void ProcessingUpdateDatabase()
        {
            CastorUpdater updater = Load();
            List<string> offsets = Settings.Default.UpdatesRun.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
            List<string> queries = new List<string>();

            try
            {
                updater.updates
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
            }
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
