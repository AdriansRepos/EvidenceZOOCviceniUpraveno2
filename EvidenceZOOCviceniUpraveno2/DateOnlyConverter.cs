using System.Text.Json;
using System.Text.Json.Serialization;


namespace EvidenceZOOCviceniUpraveno2
{
    /// <summary>
    /// JSON převodník pro typ <see cref="DateOnly"/>.
    /// Umožňuje serializaci a deserializaci datumů, protože
    /// <c>System.Text.Json</c> tento typ nativně nepodporuje.
    /// </summary>
    public class DateOnlyConverter : JsonConverter<DateOnly>
    {
        /// <summary>
        /// Načte hodnotu typu <see cref="DateOnly"/> z JSON řetězce.
        /// </summary>
        /// <param name="reader">JSON reader, který poskytuje textovou hodnotu.</param>
        /// <param name="t">Typ, který se má načíst (ignorováno).</param>
        /// <param name="o">Možnosti serializace (ignorováno).</param>
        /// <returns>
        /// Hodnota <see cref="DateOnly"/> získaná pomocí <see cref="DateOnly.Parse(string)"/>.
        /// </returns>
        public override DateOnly Read(ref Utf8JsonReader reader, Type t, JsonSerializerOptions o)
        => DateOnly.Parse(reader.GetString()!);

        /// <summary>
        /// Zapíše hodnotu typu <see cref="DateOnly"/> do JSONu jako formátovaný řetězec.
        /// </summary>
        /// <param name="writer">JSON writer, který zapisuje hodnotu.</param>
        /// <param name="value">Datum, které se má zapsat.</param>
        /// <param name="o">Možnosti serializace (ignorováno).</param>
        /// <remarks>
        /// Datum je formátováno jako <c>dd.MM.yyyy</c>.
        /// </remarks>
        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions o)
            => writer.WriteStringValue(value.ToString("dd.MM.yyyy"));
    }
}