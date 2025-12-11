using ProcrastiPlate.Api.Models;

namespace ProcrastiPlate.Api.Repositories.Interfaces;

public interface IProcrastiPlateRepository
{
    Author GetAuthor(int id);
}
