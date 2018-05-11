using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackBackup
{
    class Channel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("created")]
        public string Created { get; set; }

        [JsonProperty("creator")]
        public string Creator { get; set; }

        [JsonProperty("is_archived")]
        public bool IsArchived { get; set; }

        [JsonProperty("is_general")]
        public bool IsGeneral { get; set; }

        [JsonProperty("members")]
        public string[] Members { get; set; }

        [JsonProperty("topic")]
        public Purpose Topic { get; set; }

        [JsonProperty("purpose")]
        public Purpose Purpose { get; set; }

        [JsonProperty("pins")]
        public Pin[] Pins { get; set; }
    }

    class Pin
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("owner")]
        public string Owner { get; set; }

        [JsonProperty("created")]
        public long Created { get; set; }
    }

    class Purpose
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("creator")]
        public string Creator { get; set; }

        [JsonProperty("last_set")]
        public string LastSet { get; set; }
    }
}
