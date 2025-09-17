function scrollToBottom(elementId) {
    const element = document.getElementById(elementId);
    element.scrollTop = element.scrollHeight;
}

function addTextEditorTabEventListener(textEditorId) {
    const textEditor = document.getElementById(textEditorId);
    textEditor.addEventListener('keydown', function(event) {
        if (event.key !== 'Tab')
            return;
        event.preventDefault();
        const start = this.selectionStart;
        const end = this.selectionEnd;
        let middle = this.value.substring(start, end);
        const initialLength = middle.length;
        middle = middle.replaceAll('\n', '\n    ');
        const offset = middle.length - initialLength;
        this.value = this.value.substring(0, start) + '    ' + middle + this.value.substring(end);
        if (start === end) {
            this.selectionStart = this.selectionEnd = start + 4;
        }
        else {
            this.selectionStart = start;
            this.selectionEnd = end + offset + 4;
        }
    });
}