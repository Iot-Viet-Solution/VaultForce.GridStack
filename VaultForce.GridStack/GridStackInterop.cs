using VaultForce.GridStack.Extensions;
using VaultForce.GridStack.Models;
using Microsoft.JSInterop;

namespace VaultForce.GridStack;

public class GridStackInterop : IAsyncDisposable
{
    private IJSObjectReference? _moduleTask;
    private Func<int, IJSObjectReference, bool>? _acceptWidgetsDelegate;
    private readonly IJSRuntime _jsRuntime;
    private readonly DotNetObjectReference<GridStackInterop> _dotNetObjectReference;

    public GridStackInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        _dotNetObjectReference = DotNetObjectReference.Create(this);
    }

    public async Task InitializeAsync(string path = "./_content/VaultForce.GridStack/gridStackInterop.js")
    {
        if (_moduleTask != null)
            return;
        _moduleTask = await JSLoader.LoadAsync(_jsRuntime, path);
    }

    public async ValueTask DisposeAsync()
    {
        if (_moduleTask != null)
        {
            await _moduleTask.DisposeAsync();
        }

        _dotNetObjectReference.Dispose();
    }

    public event EventHandler<BlazorGridStackWidgetListEventArgs>? OnAdded;
    public event EventHandler<BlazorGridStackWidgetListEventArgs>? OnChange;
    public event EventHandler? OnDisable;
    public event EventHandler<BlazorGridStackWidgetEventArgs>? OnDragStart;
    public event EventHandler<BlazorGridStackWidgetEventArgs>? OnDrag;
    public event EventHandler<BlazorGridStackWidgetEventArgs>? OnDragStop;
    public event EventHandler<BlazorGridStackDroppedEventArgs>? OnDropped;
    public event EventHandler? OnEnable;
    public event EventHandler<BlazorGridStackWidgetListEventArgs>? OnRemoved;
    public event EventHandler<BlazorGridStackWidgetEventArgs>? OnResizeStart;
    public event EventHandler<BlazorGridStackWidgetEventArgs>? OnResize;
    public event EventHandler<BlazorGridStackWidgetEventArgs>? OnResizeStop;

    public event EventHandler<int>? OnColumnMaxChanged;


    public async Task<IJSObjectReference> CreateInstance(BlazorGridStackBodyOptions options, string elementId)
    {
        if (_moduleTask == null)
            await InitializeAsync();
        var gridReference = await (_moduleTask ?? throw new InvalidOperationException($"Please call {nameof(InitializeAsync)} before create a new instance")).InvokeAsync<IJSObjectReference>("init", SerializeModelToDictionary(options), _dotNetObjectReference, elementId);
        return gridReference;
    }

    public async Task<BlazorGridStackWidget> AddWidget(IJSObjectReference gridInstance, BlazorGridStackWidgetOptions widgetOptions)
    {
        return await gridInstance.InvokeAsync<BlazorGridStackWidget>("addWidgetForBlazor", widgetOptions);
    }

    public async Task<BlazorGridStackWidget> AddWidget(IJSObjectReference gridInstance, string id)
    {
        return await gridInstance.InvokeAsync<BlazorGridStackWidget>("addWidgetById", id);
    }

    public async Task BatchUpdate(IJSObjectReference gridInstance, bool? flag = null)
    {
        await gridInstance.InvokeVoidAsync("batchUpdate", flag);
    }

    public async Task Compact(IJSObjectReference gridInstance)
    {
        await gridInstance.InvokeVoidAsync("compact");
    }

    public async Task CellHeight(IJSObjectReference gridInstance, int val, bool? update = null)
    {
        await gridInstance.InvokeVoidAsync("cellHeight", val, update);
    }

    public async Task<int> CellWidth(IJSObjectReference gridInstance)
    {
        return await gridInstance.InvokeAsync<int>("cellWidth");
    }

    public async Task Column(IJSObjectReference gridInstance, int column, string? layout = null)
    {
        await gridInstance.InvokeVoidAsync("column", column, layout);
    }

    public async Task Destroy(IJSObjectReference gridInstance, bool? removeDom = null)
    {
        await gridInstance.InvokeVoidAsync("destroy", removeDom);
    }

    public async Task Disable(IJSObjectReference gridInstance)
    {
        await gridInstance.InvokeVoidAsync("disable");
    }

    public async Task Enable(IJSObjectReference gridInstance)
    {
        await gridInstance.InvokeVoidAsync("enable");
    }

    public async Task EnableMove(IJSObjectReference gridInstance, bool doEnable)
    {
        await gridInstance.InvokeVoidAsync("enableMove", doEnable);
    }

    public async Task EnableResize(IJSObjectReference gridInstance, bool doEnable)
    {
        await gridInstance.InvokeVoidAsync("enableResize", doEnable);
    }

    public async Task SetFloat(IJSObjectReference gridInstance, bool? val = null)
    {
        await gridInstance.InvokeVoidAsync("float", val);
    }

    public async Task<bool> GetFloat(IJSObjectReference gridInstance)
    {
        return await gridInstance.InvokeAsync<bool>("float");
    }

    public async Task<int> GetCellHeight(IJSObjectReference gridInstance)
    {
        return await gridInstance.InvokeAsync<int>("getCellHeight");
    }

    public async Task<BlazorGridCoordinates> GetCellFromPixel(IJSObjectReference gridInstance, int top, int left, bool? useOffset = null)
    {
        return await gridInstance.InvokeAsync<BlazorGridCoordinates>("getCellFromPixel", new { top, left }, useOffset);
    }

    public async Task<int> GetColumn(IJSObjectReference gridInstance)
    {
        return await gridInstance.InvokeAsync<int>("getColumn");
    }

    public async Task<IEnumerable<BlazorGridStackWidgetData>> GetGridItems(IJSObjectReference gridInstance)
    {
        return await gridInstance.InvokeAsync<IEnumerable<BlazorGridStackWidgetData>>("getGridItemsForBlazor");
    }

    public async Task<int> GetMargin(IJSObjectReference gridInstance)
    {
        return await gridInstance.InvokeAsync<int>("getMargin");
    }

    public async Task<bool> IsAreaEmpty(IJSObjectReference gridInstance, int x, int y, int width, int height)
    {
        return await gridInstance.InvokeAsync<bool>("isAreaEmpty", x, y, width, height);
    }

    public async Task Load(IJSObjectReference gridInstance, IEnumerable<BlazorGridStackWidgetOptions> layout, bool? addAndRemove = null)
    {
        await gridInstance.InvokeVoidAsync("load", layout, addAndRemove);
    }

    public async Task MakeWidget(IJSObjectReference gridInstance, string id)
    {
        await gridInstance.InvokeVoidAsync("makeWidgetById", id);
    }

    public async Task Margin(IJSObjectReference gridInstance, int value)
    {
        await gridInstance.InvokeVoidAsync("margin", value);
    }

    public async Task Margin(IJSObjectReference gridInstance, string value)
    {
        await gridInstance.InvokeVoidAsync("margin", value);
    }

    public async Task Movable(IJSObjectReference gridInstance, string id, bool val)
    {
        await gridInstance.InvokeVoidAsync("movabletById", id, val);
    }

    public async Task RemoveWidget(IJSObjectReference gridInstance, string id, bool? removeDom = null, bool? triggerEvent = true)
    {
        await gridInstance.InvokeVoidAsync("removeWidgetById", id, removeDom, triggerEvent);
    }

    public async Task RemoveAll(IJSObjectReference gridInstance, bool? removeDom = null)
    {
        await gridInstance.InvokeVoidAsync("removeAll", removeDom);
    }

    public async Task Resizable(IJSObjectReference gridInstance, string id, bool val)
    {
        await gridInstance.InvokeVoidAsync("resizableById", id, val);
    }

    public async Task Save(IJSObjectReference gridInstance, bool? saveContent)
    {
        await gridInstance.InvokeVoidAsync("save", saveContent);
    }

    public async Task SetAnimation(IJSObjectReference gridInstance, bool doAnimate)
    {
        await gridInstance.InvokeVoidAsync("setAnimation", doAnimate);
    }

    public async Task SetStatic(IJSObjectReference gridInstance, bool staticValue)
    {
        await gridInstance.InvokeVoidAsync("setStatic", staticValue);
    }

    public async Task Update(IJSObjectReference gridInstance, string id, BlazorGridStackWidgetOptions opts)
    {
        await gridInstance.InvokeVoidAsync("updateById", id, opts);
    }

    public async Task<bool> WillItFit(IJSObjectReference gridInstance, int x, int y, int width, int height, bool autoPosition)
    {
        return await gridInstance.InvokeAsync<bool>("willItFit", x, y, width, height, autoPosition);
    }

    private Dictionary<string, object> SerializeModelToDictionary(object? model)
    {
        var result = new Dictionary<string, object>();

        if (model != null)
            foreach (var property in model.GetType().GetProperties())
            {
                var value = property.GetValue(model);
                var name = property.Name.Substring(0, 1).ToLower() + property.Name.Substring(1);

                if (value is int or bool or string or float)
                {
                    result.Add(name, value);
                }

                else if (value is BlazorGridStackBodyAcceptWidgets acceptWidgets)
                {
                    if (acceptWidgets.BoolValue != null)
                    {
                        result.Add(name, acceptWidgets.BoolValue);
                    }

                    else if (acceptWidgets.StringValue != null)
                    {
                        result.Add(name, acceptWidgets.StringValue);
                    }

                    else if (acceptWidgets.FuncValue != null)
                    {
                        result.Add("acceptWidgetsEvent", true);
                        _acceptWidgetsDelegate = acceptWidgets.FuncValue;
                    }
                }

                else if (value is BlazorColumnOptions columnOptions)
                {
                    var key = "columnOpts";
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    if (columnOptions.BreakpointForWindow != null)
                        dict.Add("breakpointForWindow", columnOptions.BreakpointForWindow);
                    if (columnOptions.Breakpoints.Any()) dict.Add("breakpoints", columnOptions.Breakpoints.OrderByDescending(x => x.W).Select(x => new { w = x.W, c = x.C }).ToList());

                    if (columnOptions.ColumnMax != null) dict.Add("columnMax", columnOptions.ColumnMax);

                    if (columnOptions.ColumnWidth != null) dict.Add("columnWidth", columnOptions.ColumnWidth);

                    if (dict.Any())
                    {
                        dict.Add("layout", columnOptions.Layout.ToString());
                        result.Add(key, dict);
                    }
                }

                else if (value != null)
                {
                    result.Add(name, SerializeModelToDictionary(value));
                }
            }

        return result;
    }

    //EVENTS
    [JSInvokable]
    public bool AcceptWidgetsDelegateFired(int number, IJSObjectReference element)
    {
        return _acceptWidgetsDelegate?.Invoke(number, element) ?? false;
    }

    [JSInvokable]
    public void AddedFired(BlazorGridStackWidgetData[] widgets)
    {
        OnAdded?.Invoke(this, new BlazorGridStackWidgetListEventArgs { Items = widgets });
    }

    [JSInvokable]
    public void ChangeFired(BlazorGridStackWidgetData[] widgets)
    {
        OnChange?.Invoke(this, new BlazorGridStackWidgetListEventArgs { Items = widgets });
    }

    [JSInvokable]
    public void ColumnMaxChanged(int column)
    {
        OnColumnMaxChanged?.Invoke(this, column);
    }

    [JSInvokable]
    public void DisableFired()
    {
        OnDisable?.Invoke(this, EventArgs.Empty);
    }

    [JSInvokable]
    public void DragStartFired(BlazorGridStackWidgetData widget)
    {
        OnDragStart?.Invoke(this, new BlazorGridStackWidgetEventArgs { Item = widget });
    }

    [JSInvokable]
    public void DragFired(BlazorGridStackWidgetData widget)
    {
        OnDrag?.Invoke(this, new BlazorGridStackWidgetEventArgs { Item = widget });
    }

    [JSInvokable]
    public void DragStopFired(BlazorGridStackWidgetData widget)
    {
        OnDragStop?.Invoke(this, new BlazorGridStackWidgetEventArgs { Item = widget });
    }

    [JSInvokable]
    public void DroppedFired(BlazorGridStackWidgetData? previousWidget, BlazorGridStackWidgetData newWidget)
    {
        OnDropped?.Invoke(this, new BlazorGridStackDroppedEventArgs { NewWidget = newWidget, PreviousWidget = previousWidget });
    }

    [JSInvokable]
    public void EnableFired()
    {
        OnEnable?.Invoke(this, EventArgs.Empty);
    }

    [JSInvokable]
    public void RemovedFired(BlazorGridStackWidgetData[] widgets)
    {
        OnRemoved?.Invoke(this, new BlazorGridStackWidgetListEventArgs { Items = widgets });
    }

    [JSInvokable]
    public void ResizeStartFired(BlazorGridStackWidgetData widget)
    {
        OnResizeStart?.Invoke(this, new BlazorGridStackWidgetEventArgs { Item = widget });
    }

    [JSInvokable]
    public void ResizeFired(BlazorGridStackWidgetData widget)
    {
        OnResize?.Invoke(this, new BlazorGridStackWidgetEventArgs { Item = widget });
    }

    [JSInvokable]
    public void ResizeStopFired(BlazorGridStackWidgetData widget)
    {
        OnResizeStop?.Invoke(this, new BlazorGridStackWidgetEventArgs { Item = widget });
    }
}