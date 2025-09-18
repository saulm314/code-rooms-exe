using CRECSharpInterpreter;

namespace CREBlazorApp.Pages;

public record VarInfo(string ImageSource, string Value, int FontSize, bool IsBold, bool IsBlack, int PaddingTop, int PaddingBottom, int PaddingLeft,
	int PaddingRight, string HorizontalAlignment, string VerticalAlignment)
{
	public static VarInfo New(Variable variable)
	{
		string varTypeSlug = variable._VarType!.Slug;
		string imageSource = $"Files/Types/{varTypeSlug}.png";
		string value = variable.ValueAsString;
		if (varTypeSlug == "double" && value.Length > 7)
			value = value[..7];
		int fontSize = GetFontSize(value.Length, varTypeSlug);
		bool isBold = GetIsBold(variable._VarType._Storage);
		bool isBlack = GetIsBlack(variable._VarType._Storage);
		int paddingTop = GetPaddingTop();
		int paddingBottom = GetPaddingBottom();
		int paddingLeft = GetPaddingLeft();
		int paddingRight = GetPaddingRight(varTypeSlug);
		string horizontalAlignment = GetHorizontalAlignment(varTypeSlug);
		string verticalAlignment = GetVerticalAlignment(value.Length, varTypeSlug);
		return new(imageSource, value, fontSize, isBold, isBlack, paddingTop, paddingBottom, paddingLeft, paddingRight, horizontalAlignment, verticalAlignment);
	}

	public static int GetFontSize(int length, string varTypeSlug)
		=> varTypeSlug switch
		{
			"bool" => 16,
			"char" => 16,
			"int" or "double" when length <= 4 => 16,
			"int" or "double" when length <= 5 => 12,
			"int" or "double" when length <= 7 => 10,
			"int" or "double" when length <= 10 => 8,
			"int" or "double" => 7,
			"string" when length <= 3 => 16,
			"string" when length <= 4 => 12,
			"string" when length <= 5 => 10,
			"string" => 8,
			_ when length <= 4 => 16,
			_ when length <= 6 => 12,
			_ => 8
		};

	public static bool GetIsBold(VarType.Storage varTypeStorage) => varTypeStorage == VarType.Storage.Value;

	public static bool GetIsBlack(VarType.Storage varTypeStorage) => varTypeStorage == VarType.Storage.Value;

	public static int GetPaddingTop() => 0;

	public static int GetPaddingBottom() => 0;
	
	public static int GetPaddingLeft() => 0;
	
	public static int GetPaddingRight(string varTypeSlug)
		=> varTypeSlug switch
		{
			"string" => 7,
			_ => 0
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