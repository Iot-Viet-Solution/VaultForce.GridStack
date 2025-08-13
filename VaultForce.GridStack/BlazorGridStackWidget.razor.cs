using System.ComponentModel;
using VaultForce.GridStack.Models;
using Microsoft.AspNetCore.Components;

namespace VaultForce.GridStack;

partial class BlazorGridStackWidget : ComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public BlazorGridStackWidgetOptions? WidgetOptions { get; set; }
    [Parameter]
    [Category("Common")]public string? Class { get; set; }
    
    [Parameter]
    [Category("Common")]
    public string? SubClass { get; set; }

    private string MergedClass => "grid-stack-item" + (string.IsNullOrEmpty(Class) ? string.Empty : $" {Class}");
    private string MergedSubClass => "grid-stack-item-content" + (string.IsNullOrEmpty(SubClass) ? string.Empty : $" {SubClass}");
}