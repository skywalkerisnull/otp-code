window.addShortcutListener = (dotNetHelper) => {
    document.addEventListener('keydown', function (e) {
        if (e.ctrlKey && e.key === ' ') {
            dotNetHelper.invokeMethodAsync('ToggleHideInputs');
        }
    });
};