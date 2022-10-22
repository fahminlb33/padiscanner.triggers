using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace PadiScanner.Triggers.Infra;

public class UlidConverter : ValueConverter<Ulid, string>
{
    public UlidConverter() : base(v => v.ToString(), v => Ulid.Parse(v)) { }
}

public class ProbabilitiesConverter : ValueConverter<Dictionary<string, double>, string>
{
    public ProbabilitiesConverter() : base(
        v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
        v => JsonSerializer.Deserialize<Dictionary<string, double>>(v, new JsonSerializerOptions()))
    {
    }
}
