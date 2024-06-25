using System.Collections.Generic;
using LiteDB.StudioNew.Services;

namespace LiteDB.StudioNew.Models;

public class Collection
{
    public string Name { get; set; }
    public bool IsSystem { get; set; }
    
    public required Database Database { get; init; }
    public IEnumerable<Index> Indices { get; set; }
}