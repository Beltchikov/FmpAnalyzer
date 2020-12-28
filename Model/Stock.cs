using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FmpAnalyzer.Model
{
    /// <summary>
    /// Stock
    /// </summary>
    public class Stock
    {
        [Key]
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("price")]
        public double Price { get; set; }

        [JsonPropertyName("exchange")]
        public string Exchange { get; set; }
    }
}

