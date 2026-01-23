using System;
using System.Collections.Generic;

namespace Castor.database.tables;

public partial class DictPlanning
{
    public int Keyid { get; set; }

    public string Description { get; set; } = null!;

    public int? Docdepid { get; set; }

    public int Isprivate { get; set; }

    public int Period { get; set; }
}
