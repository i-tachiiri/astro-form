namespace AstroForm.Infra;

public interface IDataRepository
{
    Task SaveAsync(string data);
}
