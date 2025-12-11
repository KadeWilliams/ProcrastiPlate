using ProcrastiPlate.Api.Models;

namespace ProcrastiPlate.Api.Services.Interface;

public interface IProcrastiPlateService
{
    Author GetAuthor(int id); 
}