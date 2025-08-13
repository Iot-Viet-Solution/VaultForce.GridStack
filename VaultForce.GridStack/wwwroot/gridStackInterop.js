// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

import '/_content/VaultForce.GridStack/gridstack-12.1.2-all.min.js';

var gsCssId = "gs-blazor-styles";

export function init(options, interopReference, elementId) {
    initCss();

    if (options.acceptWidgetsEvent) {
        options.acceptWidgets =
            async (i, _) => await interopReference.invokeMethodAsync("AcceptWidgetsDelegateFired", i);
    }

    var grid = window.GridStack.init(options, document.getElementById(elementId));

    //events
    grid.on("added",
        async (event, items) => {
            await interopReference.invokeMethodAsync("AddedFired", items.map(i => {
                return generateGridWidgetObject(i)
            }));
        });

    grid.on("change",
        async (event, items) => {
            await interopReference.invokeMethodAsync("ChangeFired", items.map(i => {
                return generateGridWidgetObject(i)
            }));
            await interopReference.invokeMethodAsync("ColumnMaxChanged", grid.getColumn())
        });

    grid.on("disable",
        async (event) => {
            await interopReference.invokeMethodAsync("DisableFired");
        });

    grid.on("dragstart",
        async (event, el) => {
            await interopReference.invokeMethodAsync("DragStartFired", generateGridWidgetObjectFromElement(el));
        });

    grid.on("drag",
        async (event, el) => {
            await interopReference.invokeMethodAsync("DragFired", generateGridWidgetObjectFromElement(el));
        });

    grid.on("dragstop",
        async (event, el) => {
            await interopReference.invokeMethodAsync("DragStopFired", generateGridWidgetObjectFromElement(el));
        });

    grid.on("dropped",
        async (event, previousWidget, newWidget) => {
            await interopReference.invokeMethodAsync("DroppedFired", previousWidget, newWidget);
        });

    grid.on("enable",
        async (event) => {
            await interopReference.invokeMethodAsync("EnableFired");
        });

    grid.on("removed",
        async (event, items) => {
            await interopReference.invokeMethodAsync("RemovedFired", items.map(i => {
                return generateGridWidgetObject(i)
            }));
        });

    grid.on("resizestart",
        async (event, el) => {
            await interopReference.invokeMethodAsync("ResizeStartFired", generateGridWidgetObjectFromElement(el));
        });

    grid.on("resize",
        async (event, el) => {
            await interopReference.invokeMethodAsync("ResizeFired", generateGridWidgetObjectFromElement(el));
        });

    grid.on("resizestop",
        async (event, el) => {
            await interopReference.invokeMethodAsync("ResizeStopFired", generateGridWidgetObjectFromElement(el));
        });

    //methods
    grid.addWidgetForBlazor = (widgetOptions) => {
        return generateGridWidgetObjectFromElement(grid.addWidget(widgetOptions));
    }

    grid.addWidgetById = (id) => {
        return generateGridWidgetObjectFromElement(grid.addWidget(document.getElementById(id)));
    }

    grid.getGridItemsForBlazor = () => {
        return grid.getGridItems().map(i => {
            return generateGridWidgetObjectFromElement(i)
        });
    }

    grid.makeWidgetById = (id) => {
        grid.makeWidget(document.getElementById(id));
    }

    grid.movableById = (id, val) => {
        grid.movable(document.getElementById(id), val);
    }

    grid.removeWidgetById = (id, removeDOM, triggerEvent) => {
        var target = document.getElementById(id);
        if (!target) {
            return;
        }
        grid.removeWidget(target, removeDOM, triggerEvent);
    }

    grid.resizableById = (id, val) => {
        grid.resizable(document.getElementById(id), val);
    }

    grid.updateById = (id, opts) => {
        grid.update(document.getElementById(id), opts);
    }

    return grid;
}
 
function initCss() {
    //init css
    if (!document.getElementById(gsCssId)) {
        var head = document.getElementsByTagName('head')[0];
        var link = document.createElement('link');
        link.id = gsCssId;
        link.href = '_content/VaultForce.GridStack/gridstack.css';
        link.rel = "stylesheet";
        head.appendChild(link);
    }
}

function generateGridWidgetObject(widget) {
    return {
        x: widget.x,
        y: widget.y,
        h: widget.h,
        w: widget.w,
        content: null,
        className: widget.el ? widget.el.className : null,
        id: widget.el.id
    }
}

function generateGridWidgetObjectFromElement(element) {
    return {
        x: safeParseInt(element.getAttribute("gs-x")),
        y: safeParseInt(element.getAttribute("gs-y")),
        h: safeParseInt(element.getAttribute("gs-h")),
        w: safeParseInt(element.getAttribute("gs-w")),
        content: "",
        className: element.className,
        id: element.id
    }
}

const safeParseInt = (value, defaultValue = 0) => {
    const parsed = parseInt(value, 10);
    return isNaN(parsed) ? defaultValue : parsed;
};
