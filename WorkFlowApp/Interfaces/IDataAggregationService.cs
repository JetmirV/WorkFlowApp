using WorkFlowApp.Models;

namespace WorkFlowApp.Interfaces;
public interface IDataAggregationService
{
	ChartOption1 GetSalesDataForOption1();
	PieOption GetSalesDataForOption2();
	TreemapOption GetSalesDataForOption3();
}