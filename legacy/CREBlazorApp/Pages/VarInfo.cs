using CRECSharpInterpreter;

namespace CREBlazorApp.Pages;

public readonly record struct VarInfo(string ImageSource, string Value, int FontSize, bool IsBold, bool IsBlack, int Width, int Height,
	string HorizontalAlignment, string VerticalAlignment)
{
	public static VarInfo New(Variable variable)
	{
		string varTypeSlug = variable._VarType!.Slug;
		string imageSource = $"Files/Types/{varTypeSlug}.png";
		string value = variable.ValueAsString;
		int fontSize = GetFontSize(value.Length, varTypeSlug);
		bool isBold = GetIsBold(variable._VarType._Storage);
		bool isBlack = GetIsBlack(variable._VarType._Storage);
		int width = GetWidth(varTypeSlug);
		int height = GetHeight(value.Length, varTypeSlug);
		string horizontalAlignment = GetHorizontalAlignment(varTypeSlug);
		string verticalAlignment = GetVerticalAlignment(value.Length, varTypeSlug);
		return new(imageSource, value, fontSize, isBold, isBlack, width, height, horizontalAlignment, verticalAlignment);
	}

	public static int GetFontSize(int length, string varTypeSlug)
		=> varTypeSlug switch
		{
			"bool" => 16,
			"char" => 16,
			"int" or "double" when length <= 4 => 16,
			"int" or "double" when length <= 5 => 12,
			"int" or "double" => 8,
			"string" when length <= 3 => 16,
			"string" when length <= 4 => 12,
			"string" => 8,
			_ when length <= 4 => 16,
			_ when length <= 6 => 12,
			_ => 8
		};

	public static bool GetIsBold(VarType.Storage varTypeStorage) => varTypeStorage == VarType.Storage.Value;

	public static bool GetIsBlack(VarType.Storage varTypeStorage) => varTypeStorage == VarType.Storage.Value;

	public static int GetWidth(string varTypeSlug)
		=> varTypeSlug switch
		{
			"string" => 45,
			_ => 50
		};

	public static int GetHeight(int length, string varTypeSlug)
		=> varTypeSlug switch
		{
			"int" or "double" when length <= 5 => 20,
			"int" or "double" => 15,
			_ => 20
		};

	public static string GetHorizontalAlignment(string varTypeSlug)
		=> varTypeSlug switch
		{
			"string" => "end",
			_ => "center"
		};

	public static string GetVerticalAlignment(int length, string varTypeSlug)
		=> varTypeSlug switch
		{
			"int" or "double" when length <= 2 => "center",
			"int" or "double" => "end",
			_ => "center"
		};
}