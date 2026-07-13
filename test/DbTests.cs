using Castor.database;
using Castor.database.tab_medis;
using Castor.database.tables;
using Castor.gui.common;
using Castor.Properties;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
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
                            .Where(m => fios.Contains(m.Patient?.fullname) && m.dat1 == null);

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

        public async Task TTestSelectTable2()
        {




        }

        public async Task TTestSelectTable3()
        {

            await Task.Run(() =>
            {
                string input = "DIFFERENT: Bandbook->PrimaryKey 'PK_Bandbooks', constraint name. Expected = PK_Bandbooks, found =\r\nDIFFERENT: Entity 'Bandbook', constraint name. Expected = PK_Bandbooks, found =\r\nNOT CHECKED: Entity 'Bandbook', constraint name. Expected = <null>\r\n\t, found = Bandbook\r\n\tDIFFERENT: Forced->PrimaryKey 'PK_Forced', constraint name. Expected = PK_Forced, found =\r\n\tDIFFERENT: Forced->Property 'Courtname', column type. Expected = TEXT, found = NUMERIC\r\n\tNOT IN DATABASE: Forced->Property 'Movebookid', column name. Expected = Movebookid\r\n\tDIFFERENT: Forced->Property 'Patientid', column type. Expected = INTEGER, found = NUMERIC\r\n\tNOT IN DATABASE: Forced->ForeignKey 'FK_Forced_Movebooks_Movebookid', constraint name. Expected = FK_Forced_Movebooks_Movebookid\r\n\tNOT IN DATABASE: Forced->Index 'Movebookid', index constraint name. Expected = IX_Forced_Movebookid\r\n\tDIFFERENT: Entity 'Forced', constraint name. Expected = PK_Forced, found =\r\n\tDIFFERENT: Fss->PrimaryKey 'PK_Fss', constraint name. Expected = PK_Fss, found =\r\n\tDIFFERENT: Entity 'Fss', constraint name. Expected = PK_Fss, found =\r\n\tDIFFERENT: PatientRecord->PrimaryKey 'PK_Movebooks', constraint name. Expected = PK_Movebooks, found =\r\n\tDIFFERENT: Entity 'PatientRecord', constraint name. Expected = PK_Movebooks, found =\r\n\tNOT CHECKED: Entity 'PatientRecord', constraint name. Expected = <null>, found = PatientRecord\r\nDIFFERENT: Reports->PrimaryKey 'PK_Reports', constraint name. Expected = PK_Reports, found = \r\nDIFFERENT: Entity 'Reports', constraint name. Expected = PK_Reports, found = \r\nDIFFERENT: Unvoluntary->PrimaryKey 'PK_Unvoluntaries', constraint name. Expected = PK_Unvoluntaries, found = \r\nDIFFERENT: Entity 'Unvoluntary', constraint name. Expected = PK_Unvoluntaries, found = ";

                List<string> lines = input.Split("\r\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Where(l => Regex.IsMatch(l, @"^NOT IN DATABASE:"))
                    .Select(l => Regex.Match(l, @"(?<=Expected =\s+)\w+").Value)
                    .ToList();


                string wordAfter = Regex.Match(input, @"(?<=Expected =\s+)\w+").Value;

                int t = 3 + 4;
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
