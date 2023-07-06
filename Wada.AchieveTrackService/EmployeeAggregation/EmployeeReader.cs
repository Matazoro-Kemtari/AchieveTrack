using Wada.Data.OrderManagement.Models;

namespace Wada.AchieveTrackService.EmployeeAggregation;

public class EmployeeReader : IEmployeeReader
{
    IEmployeeRepository _employeeRepository;

    public EmployeeReader(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

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
