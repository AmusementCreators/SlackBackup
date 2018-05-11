using Newtonsoft.Json;

namespace SlackBackup
{
    class Message
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("subtype")]
        public string Subtype { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("file")]
        public File File { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("upload")]
        public bool Upload { get; set; }

        [JsonProperty("display_as_bot")]
        public bool DisplayAsBot { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("bot_id")]
        public string BotId { get; set; }

        [JsonProperty("ts")]
        public string Ts { get; set; }
    }

    class File
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("created")]
        public long Created { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("mimetype")]
        public string Mimetype { get; set; }

        [JsonProperty("filetype")]
        public string Filetype { get; set; }

        [JsonProperty("pretty_type")]
        public string PrettyType { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("editable")]
        public bool Editable { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("mode")]
        public string Mode { get; set; }

        [JsonProperty("is_external")]
        public bool IsExternal { get; set; }

        [JsonProperty("external_type")]
        public string ExternalType { get; set; }

        [JsonProperty("is_public")]
        public bool IsPublic { get; set; }

        [JsonProperty("public_url_shared")]
        public bool PublicUrlShared { get; set; }

        [JsonProperty("display_as_bot")]
        public bool DisplayAsBot { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("url_private")]
        public string UrlPrivate { get; set; }

        [JsonProperty("url_private_download")]
        public string UrlPrivateDownload { get; set; }

        [JsonProperty("thumb_64")]
        public string Thumb64 { get; set; }

        [JsonProperty("thumb_80")]
        public string Thumb80 { get; set; }

        [JsonProperty("thumb_360")]
        public string Thumb360 { get; set; }

        [JsonProperty("thumb_360_w")]
        public long Thumb360_W { get; set; }

        [JsonProperty("thumb_360_h")]
        public long Thumb360_H { get; set; }

        [JsonProperty("thumb_480")]
        public string Thumb480 { get; set; }

        [JsonProperty("thumb_480_w")]
        public long Thumb480_W { get; set; }

        [JsonProperty("thumb_480_h")]
        public long Thumb480_H { get; set; }

        [JsonProperty("thumb_160")]
        public string Thumb160 { get; set; }

        [JsonProperty("thumb_720")]
        public string Thumb720 { get; set; }

        [JsonProperty("thumb_720_w")]
        public long Thumb720_W { get; set; }

        [JsonProperty("thumb_720_h")]
        public long Thumb720_H { get; set; }

        [JsonProperty("thumb_960")]
        public string Thumb960 { get; set; }

        [JsonProperty("thumb_960_w")]
        public long Thumb960_W { get; set; }

        [JsonProperty("thumb_960_h")]
        public long Thumb960_H { get; set; }

        [JsonProperty("thumb_1024")]
        public string Thumb1024 { get; set; }

        [JsonProperty("thumb_1024_w")]
        public long Thumb1024_W { get; set; }

        [JsonProperty("thumb_1024_h")]
        public long Thumb1024_H { get; set; }

        [JsonProperty("image_exif_rotation")]
        public long ImageExifRotation { get; set; }

        [JsonProperty("original_w")]
        public long OriginalW { get; set; }

        [JsonProperty("original_h")]
        public long OriginalH { get; set; }

        [JsonProperty("permalink")]
        public string Permalink { get; set; }

        [JsonProperty("permalink_public")]
        public string PermalinkPublic { get; set; }

        [JsonProperty("channels")]
        public object[] Channels { get; set; }

        [JsonProperty("groups")]
        public object[] Groups { get; set; }

        [JsonProperty("ims")]
        public object[] Ims { get; set; }

        [JsonProperty("comments_count")]
        public long CommentsCount { get; set; }
    }
}
