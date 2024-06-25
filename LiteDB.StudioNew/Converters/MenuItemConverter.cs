using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using LiteDB.StudioNew.ViewModels;

namespace LiteDB.StudioNew.Converters;

public class MenuItemConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is MenuItemViewModel menuItemViewModel)
            return CreateMenuItem(menuItemViewModel);

        if (value is IEnumerable<MenuItemViewModel> menuItemViewModelList)
            return menuItemViewModelList.Select(CreateMenuItem);
        
        throw new InvalidOperationException("Unsupported view model type");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private static object? CreateMenuItem(MenuItemViewModel menuItemViewModel)
    {
        return new MenuItem
        {
            Header = menuItemViewModel.Title,
            Command = menuItemViewModel.ActionCommand
        };
    }
}