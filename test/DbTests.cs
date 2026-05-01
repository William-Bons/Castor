using Castor.database;
using Castor.database.tab_medis;
using Castor.database.tables;
using Castor.gui.common;
using Castor.gui.dialogs;
using Castor.Properties;
using EfSchemaCompare;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace Castor.test
{

    public class DbTests : IConsoleMessage, IRun
    {
        private object TestNo = null;
        public event ConsoleMessageHandler ConsoleMessage;


        public DbTests(object testNo)
        {
            TestNo = testNo;
        }

        public void Run()
        {
            MethodInfo methodInfo = typeof(DbTests).GetMethod(TestNo.ToString());
            methodInfo?.Invoke(this, null);
        }


        public async Task TTestSelectTable1()
        {
            // Выбирает несовершеннолетних пациентов за прошлый год с окончательным дз на Ф выписанных из отд
            Func<Task> GetNosoAnalysis = async () =>
            {

                try
                {
                    using (MedisContext medis = new MedisContext())
                    {
                        // select visits patients aged<18 in last year from LastSelectedDep
                        IEnumerable<visit> visits = medis.visit
                            .Where(v => v.depid == Settings.Default.LastSelectedDepId && v.dat >= (DateTime.Parse("01.01.2025").ToUniversalTime()) && v.dat <= DateTime.Parse("31.12.2025").ToUniversalTime() /*&& v.dat1 != null*/)
                            .Include(v => v.Patient)
                            .ThenInclude(p => p.Diagnoses)
                            .ThenInclude(d => d.Diagnos)
                            .ToList()
                            .Where(v => v.Age < 18);

                        var re =
                                 from v in visits
                                 from p in v.Patient.Diagnoses
                                 where p.Diagnos.code[0] == 'F' && p.diagform == 3
                                 select new
                                 {
                                     Visitid = v.keyid,
                                     Days = (v.dat1 - v.dat).Value.Days,
                                     Name = v.Patient.fullname,
                                     Ds = p.Diagnos.code,
                                     C = p.diagform

                                 };
                                 

                        re.Distinct().ToList(); // здесь точка останова для анализа и сохранения результата

                        int r = 3 + 4;
                    }
                }
                catch (Exception ex) { }

            };

            Func<Task> asyncLambda = async () =>
            {
                IEnumerable<Movebook> movebooks;
                IEnumerable<visit> visits;
                IEnumerable<string> fios;

                using (CastorContext castor = new CastorContext())
                {
                    // select Fio only from movebooks
                    movebooks = castor.Movebooks
                        .Where(m => m.Visitid == null);

                    fios = movebooks.Select(m => m.Fio);

                    using (MedisContext medis = new MedisContext())
                    {
                        // select all visits in DEPARTMENT (dep)
                        visits = medis.dep
                            .Where(d => d.keyid == Settings.Default.LastSelectedDepId)
                            .Include(d => d.Visits)
                            .ThenInclude(v => v.Patient)
                            .Select(d => d.Visits).First();

                        // select those visits where fio match movebook and date out empty
                        IEnumerable<visit> vv = visits
                            .Where(m => fios.Contains(m.Patient?.fullname) && m.dat1==null);

                        foreach (var mb in movebooks)
                        {
                            var q = vv.Where(v => v.Patient.fullname == mb.Fio);
                            mb.Visitid = vv.Where(v => v.Patient.fullname == mb.Fio).First().keyid;
                        }

                        int t = 0;
                    }
                }
            };


            MainWindow.Wait(true);
            //await asyncLambda();
            await GetNosoAnalysis();
            MainWindow.Wait();
        }

        public void TTestSelectTable2()
        {
            //string[] ds = { "F20", "F21","F32.2","F25","F43.2","F01.0","G20","F70.0","F23.1","20.0","21"  };
            //foreach (string s in ds)
            //{
            //    ConsoleMessage?.Invoke($"{s} == {Regex.IsMatch(s, @"^F(21|01|22|23|25|30|31|32)")}");
            //}

            //SETUP
            //var options = //... with connection to database to check

            CastorContext castorContext = new CastorContext();
            castorContext.DBHasErrors();

        }

        public async Task TTestSelectTable3()
        {
            // находятся в отделении но не внесены в базу
            
            await Task.Run(() =>
            {
                IEnumerable<visit> visitsWeek = new List<visit>();
                IEnumerable<string> fios = new List<string>();

                using (CastorContext castor = new CastorContext())
                {
                    using (MedisContext cc = new MedisContext())
                    {
                        // list of Fullnames unordered 
                        fios = castor.Movebooks
                            .Where(m => m.Dateout == null)
                            .Select(m => m.Fio).ToList();

                        // get visits for last 10 days
                        visitsWeek = cc.visit
                           .Where(v => v.depid == Settings.Default.LastSelectedDepId && (DateTime.Today.ToUniversalTime() - v.dat.Value).Days < 10 && v.dat1 == null)
                           .Include(v => v.Patient)
                           .ToList();

                        

                        


                    }
                }

                var a = visitsWeek
                           .Where(v => !fios.Contains(v.Fullname));
                int t = 3 + 3;

            });
        }

        public void TCreateHtml()
        {
            var html =
                @"<body>
                    <h1>This is <b>bold</b> heading</h1>
                    <P>This is <u>underlined</u> paragraph</P>
                </body>";

            var htmlDoc = new HtmlDocument();
            htmlDoc.OptionOutputOriginalCase = true;
            htmlDoc.OptionDefaultUseOriginalName = true;
            htmlDoc.OptionPreserveXmlNamespaces = true;
            htmlDoc.OptionOutputAsXml = true;
            htmlDoc.OptionOutputUpperCase = true;


            htmlDoc.LoadHtml(html);

            var htmlBody = htmlDoc.DocumentNode.SelectSingleNode("//body");
            HtmlNode newPara = HtmlNode.CreateNode("<P>This a new paragraph cap</P>");
            htmlBody.ChildNodes.Add(newPara);
            ;
        }

    }
}
