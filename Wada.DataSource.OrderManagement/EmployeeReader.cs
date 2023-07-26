using Wada.AchieveTrackService.EmployeeAggregation;
using Wada.AOP.Logging;
using Wada.Data.OrderManagement.Models;
using Wada.WriteWorkRecordApplication;

namespace Wada.DataSource.OrderManagement;

public class EmployeeReader : IEmployeeReader
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeReader(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    [Logging]
    public async Task<Employee> FindByEmployeeNumberAsync(uint employeeNumber)
    {
        try
        {
            var employee = await _employeeRepository.FindByEmployeeNumberAsync(employeeNumber);
            return Employee.Parse(employee);
        }
        catch (Data.OrderManagement.Models.EmployeeAggregation.EmployeeAggregationException ex)
        {
            throw new EmployeeAggregationException(ex.Message, ex);
        }
    }
}
