namespace VaultForce.GridStack.Models;

public class BlazorGridStackWidgetListEventArgs : EventArgs
{
    public IEnumerable<BlazorGridStackWidgetData> Items { get; set; } = [];
}