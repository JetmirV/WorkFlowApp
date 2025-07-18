using WorkFlowApp.Interfaces;
using WorkFlowApp.Models;

namespace WorkFlowApp.Services;

public class DataAggregationService : IDataAggregationService
{
	private readonly IDataRepo _dataRepo;

	public DataAggregationService(IDataRepo dataRepo)
	{
		this._dataRepo = dataRepo;
	}

	public ChartOption1 GetSalesDataForOption1()
	{
		var records = this._dataRepo.GetAllSalesRecords();

		var grouped = records
			.GroupBy(r => new { r.BrandName, r.CategoryName })
			.Select(g => new
			{
				Category = g.Key.CategoryName,
				Total = g.Sum(r => r.SalesIncVatActual)
			})
			.OrderBy(x => x.Category)
			.ToList();

		var option = new ChartOption1
		{
			XAxis = new Axis
			{
				Type = "category",
				Data = grouped.Select(x => x.Category).ToList()
			},
			YAxis = new Axis
			{
				Type = "value"
			},
			Series = new List<Series> {
				  new Series {
					Data = grouped.Select(x => x.Total).ToList(),
					Type = "line"
				  }
			}
		};

		return option;
	}

	public PieOption GetSalesDataForOption2()
	{
		var records = this._dataRepo.GetAllSalesRecords();

		var grouped = records
			.GroupBy(r => r.CategoryName)
			.Select(g => new
			{
				Category = g.Key,
				TotalVol = g.Sum(r => r.Volume),
				TotalSales = g.Sum(r => r.SalesIncVatActual)
			})
			// 2) Rank by sales descending, take top 4
			.OrderByDescending(x => x.TotalSales)
			.Take(4)
			.ToList();

		double sumVolume = grouped.Sum(x => x.TotalVol);

		const int R = 60, G = 185, B = 226;

		// 5) Build PieData items
		var data = grouped.Select(x =>
		{
			double pct = sumVolume > 0
				? x.TotalVol / sumVolume
				: 0;

			// map pct [0..1] → alpha [0.2..1]
			double alpha = 0.2 + pct * 0.8;
			double emphasisAlpha = Math.Min(alpha + 0.2, 1.0);

			string normalColor = $"rgb({R}, {G}, {B}, {alpha})";
			string emphasisColor = $"rgb({R}, {G}, {B}, {emphasisAlpha})";

			return new PieData
			{
				Value = x.TotalVol,
				Name = x.Category,
				ItemStyle = new ItemStyle
				{
					Normal = new ColorState { Color = normalColor },
					Emphasis = new ColorState { Color = emphasisColor }
				}
			};
		})
		.ToList();

		// 6) Wrap in the outer option
		var option = new PieOption
		{
			Series = new PieSeries
			{
				Type = "pie",
				Data = data
			}
		};

		return option;
	}

	public TreemapOption GetSalesDataForOption3()
	{
		var records = this._dataRepo.GetAllSalesRecords();

		var byBrand = records
		   .GroupBy(r => r.BrandName)
		   .Select(g => new {
			   Brand = g.Key,
			   TotalSales = g.Sum(r => r.SalesIncVatActual)
		   });

		// b) Bucket each brand-total
		var bucketed = byBrand
			.Select(b => new {
				Bucket = b.TotalSales <= 10
						 ? "0-10"
						 : b.TotalSales <= 100
						   ? "10-100"
						   : "100+",
				Value = b.TotalSales
			});

		// c) Aggregate per bucket
		var aggregated = bucketed
			.GroupBy(x => x.Bucket)
			.Select(g => new TreemapData
			{
				Name = g.Key,
				Value = g.Sum(x => x.Value)
			})
			// ensure the desired order:
			.OrderBy(x => x.Name switch {
				"0-10" => 0,
				"10-100" => 1,
				"100+" => 2,
				_ => 3
			})
			.ToList();

		// d) Wrap in the option
		var option = new TreemapOption
		{
			Series = new TreemapSeries
			{
				Type = "treemap",
				Data = aggregated
			}
		};

		return option;
	}
}
