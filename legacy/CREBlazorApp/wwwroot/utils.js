function scrollToTop(elementId) {
    const element = document.getElementById(elementId);
    element.scrollTop = 0;
}

function scrollToBottom(elementId) {
    const element = document.getElementById(elementId);
    element.scrollTop = element.scrollHeight;
}

function addNewlineIndentEventListener(textEditorId) {
    const textEditor = document.getElementById(textEditorId);

    textEditor.addEventListener('keydown', function(event) {
        if (event.key !== 'Enter')
            return;
        event.preventDefault();
        const start = this.selectionStart;
        const end = this.selectionEnd;
        const previousNewlineIndex = this.value.lastIndexOf('\n', start - 1);
        let spaceCount = 0;
        for (let i = previousNewlineIndex + 1; i < start; i++) {
            if (this.value[i] !== ' ')
                break;
            spaceCount++;
        }
        let curlyBraces = start > 0 && this.value[start - 1] === '{' && end < this.value.length && this.value[end] === '}';
        if (!curlyBraces) {
            this.value = this.value.substring(0, start) + '\n' + ' '.repeat(spaceCount) + this.value.substring(end);
            this.selectionStart = this.selectionEnd = start + spaceCount + 1;
        }
        else {
            this.value = this.value.substring(0, start) + '\n' + ' '.repeat(spaceCount + 4) + '\n' + ' '.repeat(spaceCount) + this.value.substring(end);
            this.selectionStart = this.selectionEnd = start + spaceCount + 5;
        }
        textEditor.scrollLeft = 0;
    });
}

function addTextEditorCurlyBraceEventListener(textEditorId) {
    const textEditor = document.getElementById(textEditorId);

    textEditor.addEventListener('keydown', function(event) {
        if (event.key !== '{')
            return;
        event.preventDefault();
        const start = this.selectionStart;
        const end = this.selectionEnd;
        this.value = this.value.substring(0, start) + '{}' + this.value.substring(end);
        this.selectionStart = this.selectionEnd = start + 1;
    });
}

function addTextEditorTabEventListener(textEditorId) {
    const textEditor = document.getElementById(textEditorId);

    let isShiftPressed = false;

    textEditor.addEventListener('keydown', function(event) {
        if (event.code !== 'ShiftLeft')
            return;
        isShiftPressed = true;
    });

    textEditor.addEventListener('keyup', function(event) {
        if (event.code !== 'ShiftLeft')
            return;
        isShiftPressed = false;
    });

    textEditor.addEventListener('keydown', function(event) {
        if (event.key !== 'Tab')
            return;
        event.preventDefault();
        const start = this.selectionStart;
        const end = this.selectionEnd;
        let middle = this.value.substring(start, end);

        const initialLength = middle.length;
        let spacesRemovedRight = 0;
        let spacesRemovedLeft = 0;
        let newStart = start;
        if (!isShiftPressed)
            middle = middle.replaceAll('\n', '\n    ');
        else {
            middle = middle.replaceAll('\n    ', '\n');
            for (let i = 0; i < 4; i++) {
                if (middle[i] !== ' ')
                    break;
                spacesRemovedRight++;
            }
            middle = middle.substring(spacesRemovedRight);
        }
        const offset = middle.length - initialLength;

        if (!isShiftPressed)
            this.value = this.value.substring(0, start) + '    ' + middle + this.value.substring(end);
        else {
            for (let i = start - 1; i >= start + spacesRemovedRight - 4 && i >= 0; i--) {
                if (this.value[i] !== ' ')
                    break;
                newStart = i;
            }
            spacesRemovedLeft = start - newStart;
            this.value = this.value.substring(0, newStart) + middle + this.value.substring(end);
        }

        if (!isShiftPressed) {
            if (start === end) {
                this.selectionStart = this.selectionEnd = start + 4;
            }
            else {
                this.selectionStart = start;
                this.selectionEnd = end + offset + 4;
            }
        }
        else {
            if (start === end) {
                this.selectionStart = this.selectionEnd = newStart;
            }
            else {
                this.selectionStart = newStart;
                this.selectionEnd = end + offset - spacesRemovedLeft;
            }
        }
    });
}