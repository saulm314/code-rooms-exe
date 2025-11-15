using Microsoft.JSInterop;

namespace CREBlazorApp.Pages;

public static class JsUtils
{
    public static async Task SetLocal<T>(this IJSRuntime js, string key, T value) => await js.InvokeVoidAsync("localStorage.setItem", key, value);

    public static async Task<T> GetLocal<T>(this IJSRuntime js, string key) => await js.InvokeAsync<T>("localStorage.getItem", key);

    public static async Task RemoveLocal(this IJSRuntime js, string key) => await js.InvokeVoidAsync("localStorage.removeItem", key);

    public static async Task ClearLocal(this IJSRuntime js) => await js.InvokeVoidAsync("localStorage.clear");

    public static async Task SetSession<T>(this IJSRuntime js, string key, T value) => await js.InvokeVoidAsync("sessionStorage.setItem", key, value);

    public static async Task<T> GetSession<T>(this IJSRuntime js, string key) => await js.InvokeAsync<T>("sessionStorage.getItem", key);

    public static async Task RemoveSession(this IJSRuntime js, string key) => await js.InvokeVoidAsync("sessionStorage.removeItem", key);

    public static async Task ClearSession(this IJSRuntime js) => await js.InvokeVoidAsync("sessionStorage.clear");

    public static async Task ScrollToTop(this IJSRuntime js, string elementId) => await js.InvokeVoidAsync("scrollToTop", elementId);

    public static async Task ScrollToBottom(this IJSRuntime js, string elementId) => await js.InvokeVoidAsync("scrollToBottom", elementId);

    public static async Task AddNewlineIndentEventListener(this IJSRuntime js, string textEditorId)
        => await js.InvokeVoidAsync("addNewlineIndentEventListener", textEditorId);

    public static async Task AddTextEditorCurlyBraceEventListener(this IJSRuntime js, string textEditorId)
        => await js.InvokeVoidAsync("addTextEditorCurlyBraceEventListener", textEditorId);

    public static async Task AddTextEditorTabEventListener(this IJSRuntime js, string textEditorId)
        => await js.InvokeVoidAsync("addTextEditorTabEventListener", textEditorId);
}