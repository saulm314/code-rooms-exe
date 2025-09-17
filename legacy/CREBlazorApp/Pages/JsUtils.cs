using Microsoft.JSInterop;

namespace CREBlazorApp.Pages;

public static class JsUtils
{
    public static async Task ScrollToBottom(this IJSRuntime js, string elementId) => await js.InvokeVoidAsync("scrollToBottom", elementId);

    public static async Task AddNewlineIndentEventListener(this IJSRuntime js, string textEditorId)
        => await js.InvokeVoidAsync("addNewlineIndentEventListener", textEditorId);

    public static async Task AddTextEditorCurlyBraceEventListener(this IJSRuntime js, string textEditorId)
        => await js.InvokeVoidAsync("addTextEditorCurlyBraceEventListener", textEditorId);

    public static async Task AddTextEditorTabEventListener(this IJSRuntime js, string textEditorId)
        => await js.InvokeVoidAsync("addTextEditorTabEventListener", textEditorId);
}