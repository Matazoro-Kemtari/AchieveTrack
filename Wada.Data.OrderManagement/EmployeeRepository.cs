using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wada.AchieveTrackService;
using Wada.AchieveTrackService.EmployeeAggregation;
using Wada.AOP.Logging;
using Wada.DataBase.EFCore.OrderManagement;

namespace Wada.Data.OrderManagement;

public class EmployeeRepository(IConfiguration configuration) : IEmployeeRepository
{
    private readonly IConfiguration _configuration = configuration;

    [Logging]
    public async Task<Employee> FindByEmployeeNumberAsync(uint employeeNumber)
    {
        using var dbContext = new OrderManagementContext(_configuration);
        try
        {
            var employee = await dbContext.Employees.SingleAsync(x => x.EmployeeNumber == (int)employeeNumber);
            return Employee.Reconstruct((uint)employee.EmployeeNumber,
                                        employee.Name,
                                        (uint?)employee.DepartmentID,
                                        (uint?)employee.ProcessFlowId);
        }
        catch (InvalidOperationException ex)
        {
            throw new EmployeeNotFoundException(
                $"社員情報が見つかりません 社員番号: {employeeNumber}", ex);
        }
    }
}
