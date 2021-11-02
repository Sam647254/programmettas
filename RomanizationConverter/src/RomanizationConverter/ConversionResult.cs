namespace RomanizationConverter {
   public record ConversionResult {
      public string? FromPinyin { get; set; }
      public string? FromJyutping { get; set; }
   }
}