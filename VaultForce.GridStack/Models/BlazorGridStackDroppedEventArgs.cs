namespace VaultForce.GridStack.Models;

public class BlazorGridStackDroppedEventArgs
{
    public BlazorGridStackWidgetData? PreviousWidget { get; set; }
    public BlazorGridStackWidgetData? NewWidget { get; set; }
}