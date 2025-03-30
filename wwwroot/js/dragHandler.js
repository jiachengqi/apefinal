window.dragHandler = {
    startDrag: function(dotNetRef) {
        document.onmousemove = function(e) {
            dotNetRef.invokeMethodAsync('OnPointerMove', e.clientX, e.clientY);
        };
        document.onmouseup = function(e) {
            dotNetRef.invokeMethodAsync('EndDrag', e.clientX, e.clientY);
            document.onmousemove = null;
            document.onmouseup = null;
        };
    }
};