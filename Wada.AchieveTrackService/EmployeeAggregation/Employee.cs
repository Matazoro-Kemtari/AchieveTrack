namespace Wada.AchieveTrackService.EmployeeAggregation;

public record class Employee
{
    private Employee(uint employeeNumber, string name, uint? departmentID, uint? achievementClassificationId)
    {
        EmployeeNumber = employeeNumber;
        Name = name;
        DepartmentId = departmentID;
        AchievementClassificationId = achievementClassificationId;
    }

    /// <summary>
    /// インフラ層専用再構築メソッド
    /// </summary>
    /// <param name="employeeNumber"></param>
    /// <param name="departmentID"></param>
    /// <param name="achievementClassificationId"></param>
    /// <returns></returns>
    public static Employee Reconstruct(uint employeeNumber, string name, uint? departmentID, uint? achievementClassificationId)
        => new(employeeNumber, name, departmentID, achievementClassificationId);

    internal static Employee Parse(Data.OrderManagement.Models.EmployeeAggregation.Employee employee)
        => new(employee.EmployeeNumber, employee.Name, employee.DepartmentId, employee.AchievementClassificationId);

    /// <summary>
    /// 社員番号
    /// </summary>
    public uint EmployeeNumber { get; }

    /// <summary>
    /// 氏名
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 部署ID
    /// </summary>
    public uint? DepartmentId { get; }

    /// <summary>
    /// 実績工程ID
    /// </summary>
    public uint? AchievementClassificationId { get; }
}

public class TestEmployeeFactory
{
    public static Employee Create(
        uint employeeNumber = 4001u,
        string name = "無人",
        uint? departmentID = 4,
        uint? achievementClassificationId = 3u)
        => Employee.Reconstruct(employeeNumber, name, departmentID, achievementClassificationId);
}
