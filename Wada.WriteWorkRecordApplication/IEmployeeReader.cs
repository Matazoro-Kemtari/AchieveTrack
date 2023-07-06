using Wada.AchieveTrackService.EmployeeAggregation;

namespace Wada.WriteWorkRecordApplication;

public interface IEmployeeReader
{
    /// <summary>
    /// 社員番号で社員を検索する
    /// </summary>
    /// <param name="employeeNumber"></param>
    /// <returns></returns>
    /// <exception cref="EmployeeAggregationException"></exception>
    Task<Employee> FindByEmployeeNumberAsync(uint employeeNumber);
}
