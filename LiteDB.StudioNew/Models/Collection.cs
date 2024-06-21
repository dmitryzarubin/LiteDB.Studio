using System.Collections.Generic;

namespace LiteDB.StudioNew.Models;

public class Collection
{
    public string Name { get; set; }
    public bool IsSystem { get; set; }
    public IEnumerable<Index> Indices { get; set; }
}