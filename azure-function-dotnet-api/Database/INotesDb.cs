using System.Collections.Generic;
using System.Threading.Tasks;

namespace EvokeApi.Database
{
    public interface INotesDb
    {
        Task InsertAsync(Note note, string partitionKey);
        Task<Note> GetByIdAsync(string id, string partitionKey);
        Task<IEnumerable<Note>> GetAll(string userId);
    }
}
