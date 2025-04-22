window.resizeHelper = {
    getSize: element => {
        if (!element || !(element instanceof Element)) {
            return { width: 0, height: 0 };
        }
        const rect = element.getBoundingClientRect();
        return {
            width:  Math.round(rect.width),
            height: Math.round(rect.height)
        };
    }
};
