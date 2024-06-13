using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;

namespace LiteDB.StudioNew.ViewModels;

public class ViewModelBase : ReactiveObject, IValidatableViewModel
{
    public IValidationContext ValidationContext { get; } = new ValidationContext();
}