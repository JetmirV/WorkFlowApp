using Newtonsoft.Json;

public class SalesRecord
{
	[JsonProperty("id")]
	public int Id { get; set; }

	[JsonProperty("dateTime")]
	public DateTimeOffset DateTime { get; set; }

	[JsonProperty("channelCode")]
	public int ChannelCode { get; set; }

	[JsonProperty("transactionId")]
	public string TransactionId { get; set; } = null!;

	[JsonProperty("lineItemId")]
	public int LineItemId { get; set; }

	[JsonProperty("volume")]
	public double Volume { get; set; }

	[JsonProperty("salesIncVatActual")]
	public double SalesIncVatActual { get; set; }

	[JsonProperty("priceIncVat")]
	public double PriceIncVat { get; set; }

	[JsonProperty("priceIncVatOriginal")]
	public double PriceIncVatOriginal { get; set; }

	[JsonProperty("upcCode")]
	public long UpcCode { get; set; }

	[JsonProperty("upcName")]
	public string UpcName { get; set; } = null!;

	[JsonProperty("categoryName")]
	public string CategoryName { get; set; } = null!;

	[JsonProperty("subCategoryName")]
	public string SubCategoryName { get; set; } = null!;

	[JsonProperty("brandName")]
	public string BrandName { get; set; } = null!;

	[JsonProperty("packageName")]
	public string PackageName { get; set; } = null!;

	[JsonProperty("supplierName")]
	public string SupplierName { get; set; } = null!;

	[JsonProperty("numberOfTransactions")]
	public int NumberOfTransactions { get; set; }

	[JsonProperty("gender")]
	public string Gender { get; set; } = null!;

	[JsonProperty("customerSegment")]
	public string CustomerSegment { get; set; } = null!;

	[JsonProperty("terminalCheckRegister")]
	public int TerminalCheckRegister { get; set; }

	[JsonProperty("zipCode")]
	public int ZipCode { get; set; }

	[JsonProperty("zipCodeExtend")]
	public string ZipCodeExtend { get; set; } = null!;

	[JsonProperty("zipCodeTotal")]
	public string ZipCodeTotal { get; set; } = null!;

	[JsonProperty("street")]
	public string Street { get; set; } = null!;

	[JsonProperty("numOfEmployees")]
	public int NumOfEmployees { get; set; }

	[JsonProperty("m2")]
	public int M2 { get; set; }

	[JsonProperty("storeSegment")]
	public string StoreSegment { get; set; } = null!;

	[JsonProperty("region")]
	public string Region { get; set; } = null!;

	[JsonProperty("marginOctober")]
	public double MarginOctober { get; set; }

	[JsonProperty("priceIncVatOctober")]
	public double PriceIncVatOctober { get; set; }

	[JsonProperty("discountOctober")]
	public double DiscountOctober { get; set; }
}
