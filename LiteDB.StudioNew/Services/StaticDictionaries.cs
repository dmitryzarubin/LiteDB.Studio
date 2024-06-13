using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;

namespace LiteDB.StudioNew.Services;

public static class StaticDictionaries
{
    public static IReadOnlyList<string> CulturesList { get; } = CultureInfo.GetCultures(CultureTypes.AllCultures)
        .Select(x => x.LCID)
        .Distinct()
        .Where(x => x != 4096)
        .Select(x => CultureInfo.GetCultureInfo(x).Name)
        .ToImmutableList();


    public static IReadOnlyList<string> SortsList { get; } = Enumerable.Empty<string>()
        .Append(string.Empty)
        .Union(Enum.GetNames(typeof(CompareOptions)))
        .ToImmutableList();
}