using System.Diagnostics;
using VaultForce.GridStack.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace VaultForce.GridStack;

public partial class BlazorGridStackBody : ComponentBase, IAsyncDisposable
{
    [Parameter] public RenderFragment? ChildContent { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    [Parameter] public BlazorGridStackBodyOptions GridStackOptions { get; set; } = new();

    /// <summary>
    ///     Called when widgets are being added to a grid
    /// </summary>
    [Parameter]
    public EventCallback<BlazorGridStackWidgetListEventArgs> OnAdded { get; set; }

    /// <summary>
    ///     Occurs when widgets change their position/size due to constrain or direct changes
    /// </summary>
    [Parameter]
    public EventCallback<BlazorGridStackWidgetListEventArgs> OnChange { get; set; }

    [Parameter] public EventCallback<int> OnColumnMaxChanged { get; set; }

    [Parameter] public EventCallback OnDisabled { get; set; }

    /// <summary>
    ///     called while grid item is being dragged, for each new row/column value (not every pixel)
    /// </summary>
    [Parameter]
    public EventCallback<BlazorGridStackWidgetEventArgs> OnDrag { get; set; }

    /// <summary>
    ///     called when grid item is starting to be dragged
    /// </summary>
    [Parameter]
    public EventCallback<BlazorGridStackWidgetEventArgs> OnDragStart { get; set; }

    /// <summary>
    ///     called after the user is done moving the item, with updated DOM attributes.
    /// </summary>
    [Parameter]
    public EventCallback<BlazorGridStackWidgetEventArgs> OnDragStop { get; set; }

    /// <summary>
    ///     called when an item has been dropped and accepted over a grid. If the item came from another grid, the previous widget node info will also be sent (but dom item long gone).
    /// </summary>
    [Parameter]
    public EventCallback<BlazorGridStackDroppedEventArgs> OnDropped { get; set; }

    [Parameter] public EventCallback OnEnable { get; set; }

    /// <summary>
    ///     Called when items are being removed from the grid
    /// </summary>
    [Parameter]
    public EventCallback OnRemoved { get; set; }

    /// <summary>
    ///     called while grid item is being resized, for each new row/column value (not every pixel)
    /// </summary>
    [Parameter]
    public EventCallback<BlazorGridStackWidgetEventArgs> OnResize { get; set; }

    /// <summary>
    ///     called before the user starts resizing an item
    /// </summary>
    [Parameter]
    public EventCallback<BlazorGridStackWidgetEventArgs> OnResizeStart { get; set; }

    /// <summary>
    ///     called after the user is done resizing the item, with updated DOM attributes.
    /// </summary>
    [Parameter]
    public EventCallback<BlazorGridStackWidgetEventArgs> OnResizeStop { get; set; }

    [Parameter] public Func<Task>? OnGridStackReady { get; set; }

    private readonly string _elementId = Guid.NewGuid().ToString();
    private IJSObjectReference? _gridInstance;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _gridInstance = await GridJs.CreateInstance(GridStackOptions, _elementId);

            GridJs.OnAdded += GridJsOnOnAdded;
            GridJs.OnChange += GridJsOnOnChange;
            GridJs.OnColumnMaxChanged += GridJsOnOnColumnMaxChanged;
            GridJs.OnDisable += GridJsOnOnDisable;
            GridJs.OnDrag += GridJsOnOnDrag;
            GridJs.OnDragStart += GridJsOnOnDragStart;
            GridJs.OnDragStop += GridJsOnOnDragStop;
            GridJs.OnDropped += GridJsOnOnDropped;
            GridJs.OnEnable += GridJsOnOnEnable;
            GridJs.OnRemoved += GridJsOnOnRemoved;
            GridJs.OnResize += GridJsOnOnResize;
            GridJs.OnResizeStart += GridJsOnOnResizeStart;
            GridJs.OnResizeStop += GridJsOnOnResizeStop;
            await OnGridStackReadyHandler();
        }
    }

    private async Task OnGridStackReadyHandler()
    {
        if (OnGridStackReady != null)
        {
            await OnGridStackReady.Invoke();
        }
    }

    private void GridJsOnOnEnable(object? sender, EventArgs e)
    {
        OnEnable.InvokeAsync(e);
    }

    private void GridJsOnOnAdded(object? sender, BlazorGridStackWidgetListEventArgs e)
    {
        OnAdded.InvokeAsync(e);
    }

    private void GridJsOnOnChange(object? sender, BlazorGridStackWidgetListEventArgs args)
    {
        OnChange.InvokeAsync(args);
    }

    private void GridJsOnOnColumnMaxChanged(object? sender, int args)
    {
        OnColumnMaxChanged.InvokeAsync(args);
    }

    private void GridJsOnOnDisable(object? sender, EventArgs e)
    {
        OnDisabled.InvokeAsync(e);
    }

    private void GridJsOnOnDrag(object? sender, BlazorGridStackWidgetEventArgs args)
    {
        OnDrag.InvokeAsync(args);
    }

    private void GridJsOnOnDragStart(object? sender, BlazorGridStackWidgetEventArgs args)
    {
        OnDragStart.InvokeAsync(args);
    }

    private void GridJsOnOnDragStop(object? sender, BlazorGridStackWidgetEventArgs args)
    {
        OnDragStop.InvokeAsync(args);
    }

    private void GridJsOnOnDropped(object? sender, BlazorGridStackDroppedEventArgs args)
    {
        OnDropped.InvokeAsync(args);
    }

    private void GridJsOnOnRemoved(object? sender, BlazorGridStackWidgetListEventArgs args)
    {
        OnRemoved.InvokeAsync(args);
    }

    private void GridJsOnOnResize(object? sender, BlazorGridStackWidgetEventArgs args)
    {
        OnResize.InvokeAsync();
    }

    private void GridJsOnOnResizeStart(object? sender, BlazorGridStackWidgetEventArgs args)
    {
        OnResizeStart.InvokeAsync(args);
    }

    private void GridJsOnOnResizeStop(object? sender, BlazorGridStackWidgetEventArgs args)
    {
        OnResizeStop.InvokeAsync(args);
    }

    /// <summary>
    ///     Creates new widget and returns it. Options is an object containing the fields x,y,width,height,etc...
    ///     Parameters:
    ///     Widget will be always placed even if result height is more than actual grid height. You need to use willItFit method before calling addWidget for additional check.
    /// </summary>
    /// <param name="options">widget position/size options (optional, and ignore if first param is already option)</param>
    /// <returns></returns>
    public Task AddWidget(BlazorGridStackWidgetOptions options)
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.AddWidget(_gridInstance, options);
    }

    /// <summary>
    ///     Creates new widget and returns it. Options is an object containing the fields x,y,width,height,etc...
    ///     Parameters:
    ///     Widget will be always placed even if result height is more than actual grid height. You need to use willItFit method before calling addWidget for additional check.
    /// </summary>
    /// <returns></returns>
    public Task AddWidget(string id)
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.AddWidget(_gridInstance, id);
    }

    /// <summary>
    ///     use before calling a bunch of addWidget() to prevent un-necessary relayouts in between (more efficient) and get a single event callback. You will see no changes until batchUpdate(false) is called.
    /// </summary>
    /// <param name="flag"></param>
    /// <returns></returns>
    public Task BatchUpdate(bool? flag = null)
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.BatchUpdate(_gridInstance, flag);
    }

    /// <summary>
    ///     re-layout grid items to reclaim any empty space.
    /// </summary>
    /// <returns></returns>
    public Task Compact()
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.Compact(_gridInstance);
    }

    /// <summary>
    ///     Update current cell height (see - cellHeight options format). This method rebuilds an internal CSS stylesheet (unless optional update=false). Note: You can expect performance issues if call this method too often.
    /// </summary>
    /// <param name="val"></param>
    /// <param name="update"></param>
    /// <returns></returns>
    public Task CellHeight(int val, bool? update = null)
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.CellHeight(_gridInstance, val, update);
    }

    /// <summary>
    ///     Gets current cell width (grid width / # of columns).
    /// </summary>
    /// <returns></returns>
    public Task<int> CellWidth()
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.CellWidth(_gridInstance);
    }

    /// <summary>
    ///     set the number of columns in the grid. Will update existing widgets to conform to new number of columns, as well as cache the original layout so you can revert back to previous positions without loss. Requires gridstack-extra.css (or minimized version) for [2-11], else you will need to generate correct CSS (see https://github.com/gridstack/gridstack.js#change-grid-columns)
    /// </summary>
    /// <param name="column">Integer > 0 (default 12)</param>
    /// <param name="layout">specify the type of re-layout that will happen (position, size, etc...). Note: items will never be outside of the current column boundaries. default ('moveScale'). Ignored for 1 column. Possible values: 'moveScale' | 'move' | 'scale' | 'none' | (column: number, oldColumn: number, nodes: GridStackNode[], oldNodes: GridStackNode[]) => void. A custom function option takes new/old column count, and array of new/old positions. Note: new list may be partially already filled if we have a partial cache of the layout at that size (items were added later). If complete cache is present this won't get called at all.</param>
    /// <returns></returns>
    public Task Column(int column, string? layout = null)
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.Column(_gridInstance, column, layout);
    }

    /// <summary>
    ///     Destroys a grid instance.
    /// </summary>
    /// <param name="removeDom">if false nodes and grid will not be removed from the DOM (Optional. Default true).</param>
    /// <returns></returns>
    public Task Destroy(bool? removeDom = null)
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.Destroy(_gridInstance, removeDom);
    }

    /// <summary>
    ///     Disables widgets moving/resizing
    /// </summary>
    /// <returns></returns>
    public Task Disable()
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.Disable(_gridInstance);
    }

    /// <summary>
    ///     Enables widgets moving/resizing
    /// </summary>
    /// <returns></returns>
    public Task Enable()
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.Enable(_gridInstance);
    }

    /// <summary>
    ///     Enables/disables widget moving (default: true), and setting the disableDrag grid option
    /// </summary>
    /// <param name="doEnable"></param>
    /// <returns></returns>
    public Task EnableMove(bool doEnable)
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.EnableMove(_gridInstance, doEnable);
    }

    /// <summary>
    ///     Enables/disables widget sizing (default: true), and setting the disableResize grid option
    /// </summary>
    /// <param name="doEnable"></param>
    /// <returns></returns>
    public Task EnableResize(bool doEnable)
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.EnableResize(_gridInstance, doEnable);
    }

    /// <summary>
    ///     get floating widgets
    /// </summary>
    /// <returns></returns>
    public Task<bool> GetFloat()
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.GetFloat(_gridInstance);
    }

    /// <summary>
    ///     set floating widgets
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public Task SetFloat(bool? val = null)
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.SetFloat(_gridInstance, val);
    }

    /// <summary>
    ///     Gets current cell height.
    /// </summary>
    /// <returns></returns>
    public Task<int> GetCellHeight()
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.GetCellHeight(_gridInstance);
    }

    /// <summary>
    ///     Get the position of the cell under a pixel on screen.
    /// </summary>
    /// <param name="top"></param>
    /// <param name="left"></param>
    /// <param name="useOffset">if true, value will be based on offset vs position (Optional. Default false). Useful when grid is within position: relative element.</param>
    /// <returns>an object with properties x and y i.e. the column and row in the grid.</returns>
    public Task<BlazorGridCoordinates> GetCellFromPixel(int top, int left, bool? useOffset = null)
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.GetCellFromPixel(_gridInstance, top, left, useOffset);
    }

    /// <summary>
    ///     returns the number of columns in the grid.
    /// </summary>
    /// <returns></returns>
    public Task<int> GetColumn()
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.GetColumn(_gridInstance);
    }

    /// <summary>
    ///     Return list of GridItem HTML elements (excluding temporary placeholder) in DOM order, wether they are node items yet or not (looks by class)
    /// </summary>
    /// <returns></returns>
    public Task<IEnumerable<BlazorGridStackWidgetData>> GetGridItems()
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.GetGridItems(_gridInstance);
    }

    /// <summary>
    ///     returns current margin value (undefined if all 4 sides don't match).
    /// </summary>
    /// <returns></returns>
    public Task<int> GetMargin()
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.GetMargin(_gridInstance);
    }

    /// <summary>
    ///     Checks if specified area is empty.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public Task<bool> IsAreaEmpty(int x, int y, int width, int height)
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.IsAreaEmpty(_gridInstance, x, y, width, height);
    }

    /// <summary>
    ///     load the widgets from a list (see save()). This will call update() on each (matching by id) or add/remove widgets that are not there. used to restore a grid layout for a saved layout list (see save()).
    /// </summary>
    /// <param name="layout"></param>
    /// <param name="addAndRemove">Optional addAndRemove boolean (default true) or callback method can be passed to control if and how missing widgets can be added/removed, giving the user control of insertion.</param>
    /// <returns></returns>
    public Task Load(IEnumerable<BlazorGridStackWidgetOptions> layout, bool? addAndRemove = null)
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.Load(_gridInstance, layout, addAndRemove);
    }

    /// <summary>
    ///     If you add elements to your gridstack container by hand, you have to tell gridstack afterwards to make them widgets. If you want gridstack to add the elements for you, use addWidget instead. Makes the given element a widget and returns it.
    /// </summary>
    /// <param name="id">element to convert to a widget</param>
    /// <returns></returns>
    public Task MakeWidget(string id)
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.MakeWidget(_gridInstance, id);
    }

    /// <summary>
    ///     gap between grid item and content (default?: 10). This will set all 4 sides and support the CSS formats below
    /// </summary>
    /// <param name="value">an integer (px)</param>
    /// <returns></returns>
    public Task Margin(int value)
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.Margin(_gridInstance, value);
    }

    /// <summary>
    ///     gap between grid item and content (default?: 10). This will set all 4 sides and support the CSS formats below
    /// </summary>
    /// <param name="value">
    ///     - a string with possible units (ex: '5', '2em', '20px', '2rem')
    ///     - string with space separated values (ex: '5px 10px 0 20px' for all 4 sides, or '5em 10em' for top/bottom and left/right pairs like CSS).
    /// </param>
    /// <returns></returns>
    public Task Margin(string value)
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.Margin(_gridInstance, value);
    }

    /// <summary>
    ///     Enables/Disables dragging by the user of specific grid element. If you want all items, and have it affect future items, use enableMove() instead. No-op for static grids. IF you are looking to prevent an item from moving (due to being pushed around by another during collision) use locked property instead.
    /// </summary>
    /// <param name="id">widget to modify</param>
    /// <param name="val">if true widget will be draggable.</param>
    /// <returns></returns>
    public Task Movable(string id, bool val)
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.Movable(_gridInstance, id, val);
    }

    /// <summary>
    ///     Removes widget from the grid.
    /// </summary>
    /// <param name="id">widget to remove.</param>
    /// <param name="removeDom">if false node won't be removed from the DOM (Optional. Default true).</param>
    /// <param name="triggerEvent">if false (quiet mode) element will not be added to removed list and no 'removed' callbacks will be called (Default true).</param>
    /// <returns></returns>
    public Task RemoveWidget(string id, bool? removeDom = null, bool? triggerEvent = true)
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.RemoveWidget(_gridInstance, id, removeDom, triggerEvent);
    }

    /// <summary>
    ///     Removes all widgets from the grid.
    /// </summary>
    /// <param name="removeDom">if false nodes won't be removed from the DOM (Optional. Default true).</param>
    /// <returns></returns>
    public Task RemoveAll(bool? removeDom = null)
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.RemoveAll(_gridInstance, removeDom);
    }

    /// <summary>
    ///     Enables/Disables user resizing of specific grid element. If you want all items, and have it affect future items, use enableResize() instead. No-op for static grids.
    /// </summary>
    /// <param name="id">widget to modify</param>
    /// <param name="val">if true widget will be resizable.</param>
    /// <returns></returns>
    public Task Resizable(string id, bool val)
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.Resizable(_gridInstance, id, val);
    }

    /// <summary>
    ///     saves the current layout returning a list of widgets for serialization which might include any nested grids.
    /// </summary>
    /// <param name="saveContent">if true (default) the latest html inside .grid-stack-content will be saved to GridStackWidget.content field, else it will be removed.</param>
    /// <returns></returns>
    public Task Save(bool? saveContent)
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.Save(_gridInstance, saveContent);
    }

    /// <summary>
    ///     Toggle the grid animation state. Toggles the grid-stack-animate class.
    /// </summary>
    /// <param name="doAnimate">if true the grid will animate.</param>
    /// <returns></returns>
    public Task SetAnimation(bool doAnimate)
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.SetAnimation(_gridInstance, doAnimate);
    }

    /// <summary>
    ///     Toggle the grid static state. Also toggle the grid-stack-static class.
    /// </summary>
    /// <param name="staticValue">if true the grid becomes static.</param>
    /// <returns></returns>
    public Task SetStatic(bool staticValue)
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.SetStatic(_gridInstance, staticValue);
    }

    /// <summary>
    ///     Updates widget position/size and other info. Note: if you need to call this on all nodes, use load() instead which will update what changed and more.
    /// </summary>
    /// <param name="id">widget to move (element or class string)</param>
    /// <param name="opts">updates all the possible item attributes passed in the structure (x, y, h, w, etc..). Only those set will be updated.</param>
    /// <returns></returns>
    public Task Update(string id, BlazorGridStackWidgetOptions opts)
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.Update(_gridInstance, id, opts);
    }

    /// <summary>
    ///     Returns true if the height of the grid will be less the vertical constraint. Always returns true if grid doesn't have height constraint.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="autoPosition"></param>
    /// <returns></returns>
    public Task<bool> WillItFit(int x, int y, int width, int height, bool autoPosition)
    {
        Debug.Assert(_gridInstance != null, nameof(_gridInstance) + " != null");
        return GridJs.WillItFit(_gridInstance, x, y, width, height, autoPosition);
    }

    public async ValueTask DisposeComponent()
    {
        if (_gridInstance != null) await GridJs.Destroy(_gridInstance, true);
    }

    public async ValueTask DisposeAsync()
    {
        if (_gridInstance != null)
        {
            try
            {
                await GridJs.Destroy(_gridInstance, true);
                await _gridInstance.DisposeAsync();
                _gridInstance = null;
            }
            catch (JSDisconnectedException)
            {
                //
            }
        }
    }
}