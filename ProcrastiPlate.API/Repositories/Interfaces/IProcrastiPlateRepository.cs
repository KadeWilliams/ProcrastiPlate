using ProcrastiPlate.API.Models;

namespace ProcrastiPlate.API.Repositories.Interfaces;

public interface IProcrastiPlateRepository
{
    Author GetAuthor(int id); 
}
