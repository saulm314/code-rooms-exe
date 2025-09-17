function scrollToBottom(elementId) {
    const element = document.getElementById(elementId);
    element.scrollTop = element.scrollHeight;
}

function addTextEditorTabEventListener(textEditorId) {
    const textEditor = document.getElementById(textEditorId);
    textEditor.addEventListener('keydown', function(event) {
        if (event.key === 'Tab') {
            event.preventDefault();
            const start = this.selectionStart;
            const end = this.selectionEnd;
            this.value = this.value.substring(0, start) + '    ' + this.value.substring(end);
            this.selectionstart = this.selectionEnd = start + 4;
        }
    });
}