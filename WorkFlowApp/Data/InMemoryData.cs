using Newtonsoft.Json;
using WorkFlowApp.Data.Entities;

namespace WorkFlowApp.Data;

#nullable disable  
public static class InMemoryData
{
    public static void Initialize()
    {
        LoadWorkflowdata();

		LoadWorkflowContent();

		LoadRoles();

		LoadSales();
    }	

	public static void Clear()
    {
        // This method can be used to clear in-memory data if needed.  
        // For example, you can reset the state of the in-memory database.  
    }

    public static List<User> Users { get; set; } = new List<User>();
    public static List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public static List<Workflow> Workflows { get; set; } = new List<Workflow>();
    public static List<WorkflowNote> WorkflowNotes { get; set; } = new List<WorkflowNote>();
    public static List<WorkflowTask> WorkflowTasks { get; set; } = new List<WorkflowTask>();
    public static List<WorkflowAttachment> WorkflowAttachments { get; set; } = new List<WorkflowAttachment>();
    public static List<Role> Roles { get; set; } = new List<Role>();


    public static List<SalesRecord> SalesRecords { get; set; } = new List<SalesRecord>();

    private static void LoadWorkflowdata()
    {
        var baseDir = AppContext.BaseDirectory;
        var jsonPath = Path.Combine(baseDir, "Data\\RawData", "Workflow.json");

        if (!File.Exists(jsonPath))
            throw new FileNotFoundException($"Could not find workflows file at '{jsonPath}'");

        // c) read and deserialize  
        var json = File.ReadAllText(jsonPath);

        var data = JsonConvert.DeserializeObject<List<Workflow>>(json)
               ?? new List<Workflow>();

		if (Workflows.Count == 0)
		{
			Workflows.AddRange(data);
		}
	}

	private static void LoadWorkflowContent()
	{
		// Tasks
		var baseDir = AppContext.BaseDirectory;
		var tasksJsonPath = Path.Combine(baseDir, "Data\\RawData", "WorkflowTasks.json");
		var notesJsonPath = Path.Combine(baseDir, "Data\\RawData", "WorkflowNotes.json");
		var attachmentsJsonPath = Path.Combine(baseDir, "Data\\RawData", "WorkflowAttachments.json");

		if (!File.Exists(tasksJsonPath) || !File.Exists(notesJsonPath) || !File.Exists(attachmentsJsonPath))
			throw new FileNotFoundException($"Could not find workflow content file'");

		// c) read and deserialize
		var tasksJson = File.ReadAllText(tasksJsonPath);
		var notesJson = File.ReadAllText(notesJsonPath);
		var attachmentsJson = File.ReadAllText(attachmentsJsonPath);

		var tasksData = JsonConvert.DeserializeObject<List<WorkflowTask>>(tasksJson)
			?? new List<WorkflowTask>();

		var notesData = JsonConvert.DeserializeObject<List<WorkflowNote>>(notesJson)
			?? new List<WorkflowNote>();

		var attachmentsData = JsonConvert.DeserializeObject<List<WorkflowAttachment>>(attachmentsJson)
			?? new List<WorkflowAttachment>();

		if (WorkflowTasks.Count == 0)
		{
			WorkflowTasks.AddRange(tasksData);
		}

		if (WorkflowNotes.Count == 0)
		{
			WorkflowNotes.AddRange(notesData);
		}

		if (WorkflowAttachments.Count == 0)
		{
			WorkflowAttachments.AddRange(attachmentsData);
		}
	}

	private static void LoadRoles()
	{
		var baseDir = AppContext.BaseDirectory;
		var jsonPath = Path.Combine(baseDir, "Data\\RawData", "Roles.json");

		if (!File.Exists(jsonPath))
			throw new FileNotFoundException($"Could not find workflows file at '{jsonPath}'");

		var json = File.ReadAllText(jsonPath);

		var data = JsonConvert.DeserializeObject<List<Role>>(json)
			   ?? new List<Role>();

		if (Roles.Count == 0)
		{
			Roles.AddRange(data);
		}
	}

	private static void LoadSales()
	{
		var baseDir = AppContext.BaseDirectory;
		var jsonPath = Path.Combine(baseDir, "Data\\RawData", "sales_dummy_data 1.json");
		
		if (!File.Exists(jsonPath))
			throw new FileNotFoundException($"Could not find sales file at '{jsonPath}'");
		
		var json = File.ReadAllText(jsonPath);
		var data = JsonConvert.DeserializeObject<List<SalesRecord>>(json)
			   ?? new List<SalesRecord>();

		if (SalesRecords.Count == 0)
		{
			SalesRecords.AddRange(data);
		}
	}

}
