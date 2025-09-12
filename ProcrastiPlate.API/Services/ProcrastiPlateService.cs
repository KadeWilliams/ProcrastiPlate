using ProcrastiPlate.API.Models;
using ProcrastiPlate.API.Repositories.Interfaces;
using ProcrastiPlate.API.Services.Interface;

namespace ProcrastiPlate.API.Services;
public class ProcrastiPlateService : IProcrastiPlateService
{
    private readonly IProcrastiPlateRepository _repo;
    public ProcrastiPlateService(IProcrastiPlateRepository repo)
    {
        _repo = repo;
    }
    public Author GetAuthor(int id)
    {
        return _repo.GetAuthor(id);
    }
}

