namespace Wada.AchieveTrackService.EmployeeAggregation;

public record class Employee
{
    private Employee(uint employeeNumber, string name, uint? departmentID)
    {
        EmployeeNumber = employeeNumber;
        Name = name;
        DepartmentId = departmentID;
    }

    /// <summary>
    /// インフラ層専用再構築メソッド
    /// </summary>
    /// <param name="employeeNumber"></param>
    /// <param name="departmentID"></param>
    /// <param name="processFlowId"></param>
    /// <returns></returns>
    public static Employee Reconstruct(uint employeeNumber, string name, uint? departmentID)
        => new(employeeNumber, name, departmentID);

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
}

public class TestEmployeeFactory
{
    public static Employee Create(
        uint employeeNumber = 4001u,
        string name = "本社　無人",
        uint? departmentID = 4)
        => Employee.Reconstruct(employeeNumber, name, departmentID);
}
