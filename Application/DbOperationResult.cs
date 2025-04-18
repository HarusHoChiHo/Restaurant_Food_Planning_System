using Newtonsoft.Json;

namespace Application;

public class DbOperationResult<T>
{
    public int      amount    { get; set; }
    public List<T>? resultDto { get; set; }


    public override string ToString()
    {
        return $"DbOperationResult<{typeof(T).Name}>: {JsonConvert.SerializeObject(this)}";
    }
}