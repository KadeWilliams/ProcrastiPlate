using ProcrastiPlate.Core.Models;
using ProcrastiPlate.Api.Repositories.Interfaces;
using ProcrastiPlate.Api.Services.Interface;

namespace ProcrastiPlate.Api.Services;
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

