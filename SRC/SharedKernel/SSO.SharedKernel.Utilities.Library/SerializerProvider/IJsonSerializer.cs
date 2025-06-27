namespace SSO.SharedKernel.Utilities.Library.SerializerProvider;

public interface IJsonSerializer
{
    string Serialize<TInput>(TInput input);

    TOutput Deserialize<TOutput>(string input);

    object Deserialize(string input, Type type);
}
