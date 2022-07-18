using FolderizerLib.Results;
using System.Threading.Tasks;

namespace FolderizerLib.Organizers
{
    public interface IOrganizer
    {
        /// <summary>
        /// Executes the organization according to the provided preferences.
        /// </summary>
        OrganizationResult Organize();

        /// <summary>
        /// Executes the organization asynchronously, according to the provided preferences.
        /// </summary>
        Task<OrganizationResult> OrganizeAsync();
    }
}
