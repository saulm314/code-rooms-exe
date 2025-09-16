using Microsoft.JSInterop;

namespace CREBlazorApp.Pages;

public static class JsUtils
{
    public static async Task ScrollToBottom(this IJSRuntime js, string elementId) => await js.InvokeVoidAsync("scrollToBottom", elementId);
}