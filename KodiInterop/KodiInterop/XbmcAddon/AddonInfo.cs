using Smx.KodiInterop;

namespace XbmcAddon
{
	public enum AddonInfo
	{
		[StringValue("author")]
		Author = 0,
		[StringValue("changelog")]
		Changelog,
		[StringValue("description")]
		Description,
		[StringValue("disclaimer")]
		Disclaimer,
		[StringValue("fanart")]
		Fanart,
		[StringValue("icon")]
		Icon,
		[StringValue("id")]
		Id,
		[StringValue("name")]
		Name,
		[StringValue("path")]
		Path,
		[StringValue("profile")]
		Profile,
		[StringValue("stars")]
		Stars,
		[StringValue("summary")]
		Summary,
		[StringValue("type")]
		Type,
		[StringValue("version")]
		Version
	}
}