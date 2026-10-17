using Newtonsoft.Json.Linq;

public interface IOptionsContainer
{
    JToken GetValuesToken();
    void LoadValues(JToken token);
}
