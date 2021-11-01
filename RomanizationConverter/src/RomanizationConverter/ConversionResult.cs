namespace RomanizationConverter {
   public record ConversionResult {
      public string? FromPinyin { get; init; }
      public string? FromJyutping { get; init; }
   }
}