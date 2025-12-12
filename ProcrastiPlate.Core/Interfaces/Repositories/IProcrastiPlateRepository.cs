using ProcrastiPlate.Core.Models;

namespace ProcrastiPlate.Core.Interfaces.Repositories;

public interface IProcrastiPlateRepository
{
    Author GetAuthor(int id);
}
