using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bot
{
    public partial class WallPostJson
    {
        [JsonProperty("response")]
        public List<FluffyResponse> Response { get; set; }
    }

    public partial class PurpleResponse
    {
        [JsonProperty("attachment")]
        public Attachment Attachment { get; set; }

        [JsonProperty("attachments")]
        public List<Attachment> Attachments { get; set; }

        [JsonProperty("can_delete")]
        public long CanDelete { get; set; }

        [JsonProperty("can_pin")]
        public long CanPin { get; set; }

        [JsonProperty("comments")]
        public Comments Comments { get; set; }

        [JsonProperty("date")]
        public long Date { get; set; }

        [JsonProperty("from_id")]
        public long FromId { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("likes")]
        public Likes Likes { get; set; }

        [JsonProperty("marked_as_ads")]
        public long MarkedAsAds { get; set; }

        [JsonProperty("media")]
        public Media Media { get; set; }

        [JsonProperty("online")]
        public long Online { get; set; }

        [JsonProperty("post_source")]
        public PostSource PostSource { get; set; }

        [JsonProperty("post_type")]
        public string PostType { get; set; }

        [JsonProperty("reply_count")]
        public long ReplyCount { get; set; }

        [JsonProperty("reposts")]
        public Reposts Reposts { get; set; }

        [JsonProperty("signer_id")]
        public long SignerId { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("to_id")]
        public long ToId { get; set; }
    }

    public partial class Reposts
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("user_reposted")]
        public long UserReposted { get; set; }
    }

    public partial class PostSource
    {
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class Media
    {
        [JsonProperty("item_id")]
        public long ItemId { get; set; }

        [JsonProperty("owner_id")]
        public long OwnerId { get; set; }

        [JsonProperty("thumb_src")]
        public string ThumbSrc { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class Likes
    {
        [JsonProperty("can_like")]
        public long CanLike { get; set; }

        [JsonProperty("can_publish")]
        public long CanPublish { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("user_likes")]
        public long UserLikes { get; set; }
    }

    public partial class Comments
    {
        [JsonProperty("can_post")]
        public long CanPost { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }
    }

    public partial class Attachment
    {
        [JsonProperty("photo")]
        public Photo Photo { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class Photo
    {
        [JsonProperty("access_key")]
        public string AccessKey { get; set; }

        [JsonProperty("aid")]
        public long Aid { get; set; }

        [JsonProperty("created")]
        public long Created { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("owner_id")]
        public long OwnerId { get; set; }

        [JsonProperty("pid")]
        public long Pid { get; set; }

        [JsonProperty("src")]
        public string Src { get; set; }

        [JsonProperty("src_big")]
        public string SrcBig { get; set; }

        [JsonProperty("src_small")]
        public string SrcSmall { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("user_id")]
        public long UserId { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }
    }

    public partial struct FluffyResponse
    {
        public long? Integer;
        public PurpleResponse PurpleResponse;
    }

    public partial class WallPostJson
    {
        public static WallPostJson FromJson(string json) => JsonConvert.DeserializeObject<WallPostJson>(json, Converter.Settings);
    }

    public partial struct FluffyResponse
    {
        public FluffyResponse(JsonReader reader, JsonSerializer serializer)
        {
            Integer = null;
            PurpleResponse = null;

            switch (reader.TokenType)
            {
                case JsonToken.Integer:
                    Integer = serializer.Deserialize<long>(reader);
                    break;
                case JsonToken.StartObject:
                    PurpleResponse = serializer.Deserialize<PurpleResponse>(reader);
                    break;
                default: throw new Exception("Cannot convert FluffyResponse");
            }
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            if (Integer != null)
            {
                serializer.Serialize(writer, Integer);
                return;
            }
            if (PurpleResponse != null)
            {
                serializer.Serialize(writer, PurpleResponse);
                return;
            }
            throw new Exception("Union must not be null");
        }
    }

    public static class Serialize
    {
        public static string ToJson(this WallPostJson self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    public class Converter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(FluffyResponse);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (t == typeof(FluffyResponse))
                return new FluffyResponse(reader, serializer);
            throw new Exception("Unknown type");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var t = value.GetType();
            if (t == typeof(FluffyResponse))
            {
                ((FluffyResponse)value).WriteJson(writer, serializer);
                return;
            }
            throw new Exception("Unknown type");
        }

        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = { new Converter() },
        };
    }
}
