using ProcrastiPlate.API.Models;

namespace ProcrastiPlate.API.Services.Interface;

public interface IProcrastiPlateService
{
    Author GetAuthor(int id); 
}