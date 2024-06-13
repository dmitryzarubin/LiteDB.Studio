using System;

namespace LiteDB.StudioNew.Models;

public class Connection
{
    public required Guid Guid { get; init; }
    public required string Name { get; set; }
    public required string Path { get; set; }
    public required string Password { get; set; }
    public required ConnectionType ConnectionType { get; set; }
    public bool ReadOnly { get; set; }
    public bool Upgrade { get; set; }
    public int InitialSize { get; set; }
    public string? Culture { get; set; }
    public string? Sort { get; set; }
    public bool LoadOnStartup { get; set; }
}