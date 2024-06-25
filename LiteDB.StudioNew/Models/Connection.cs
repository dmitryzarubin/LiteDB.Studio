using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LiteDB.StudioNew.Services;

namespace LiteDB.StudioNew.Models;

public sealed class Connection : INotifyPropertyChanged
{
    private string _name;
    private string _path;
    
    public required Guid Guid { get; init; }

    public required string Name
    {
        get => _name;
        set => SetField(ref _name, value);
    }

    public required string Path
    {
        get => _path;
        set => SetField(ref _path, value);
    }

    public string? Password { get; set; }
    public required ConnectionType ConnectionType { get; set; }
    public bool ReadOnly { get; set; }
    public bool Upgrade { get; set; }
    public int InitialSize { get; set; }
    public string? Culture { get; set; }
    public string? Sort { get; set; }
    public bool LoadOnStartup { get; set; }
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}